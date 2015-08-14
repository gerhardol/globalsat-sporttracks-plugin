/*
Copyright (C) 2010 Zone Five Software

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */
// Author: Aaron Averill


using System;
using System.Collections.Generic;
using System.Text;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public abstract class ImportJob
    {
        public ImportJob(GlobalsatProtocol device, string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            this.device = device;
            this.sourceDescription = sourceDescription.Replace(Environment.NewLine, " ");
            this.monitor = monitor;
            this.monitor.PercentComplete = 0;
            this.importResults = importResults;
        }

        public abstract bool Import();
        protected GlobalsatProtocol device;
        protected string sourceDescription;
        protected IJobMonitor monitor;
        protected IImportResults importResults;
    }

    //Common for newer fitness devices
    public class ImportJob2 : ImportJob
    {
        public ImportJob2(GlobalsatProtocol2 device, string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        : base(device, sourceDescription, monitor, importResults)
        {
        }

        public override bool Import()
        {
            bool result = false;
            try
            {
                if (device.Open())
                {
                    IList<GlobalsatPacket.TrackFileHeader> headers = ((GlobalsatProtocol2)device).ReadTrackHeaders(monitor);
                    List<GlobalsatPacket.TrackFileHeader> fetch = new List<GlobalsatPacket.TrackFileHeader>();

                    if (device.FitnessDevice.configInfo.ImportOnlyNew && Plugin.Instance.Application != null && Plugin.Instance.Application.Logbook != null)
                    {
                        IDictionary<DateTime, IList<GlobalsatPacket.TrackFileHeader>> headersByStart = new Dictionary<DateTime, IList<GlobalsatPacket.TrackFileHeader>>();
                        foreach (GlobalsatPacket.TrackFileHeader header in headers)
                        {
                            //Adjust time in headers
                            DateTime start = header.StartTime;
                            if (!headersByStart.ContainsKey(start))
                            {
                                headersByStart.Add(start, new List<GlobalsatPacket.TrackFileHeader>());
                            }
                            headersByStart[start].Add(header);
                        }
                        DateTime now = DateTime.UtcNow;
                        foreach (IActivity activity in Plugin.Instance.Application.Logbook.Activities)
                        {
                            DateTime findTime = activity.StartTime;
                            if (headersByStart.ContainsKey(findTime) && (now - findTime).TotalSeconds > device.FitnessDevice.configInfo.SecondsAlwaysImport &&
                                //always import "bad" data
                                findTime - device.FitnessDevice.NoGpsDate < TimeSpan.FromDays(14))
                            {
                                headersByStart.Remove(findTime);
                            }
                        }
                        foreach (IList<GlobalsatPacket.TrackFileHeader> dateHeaders in headersByStart.Values)
                        {
                            fetch.AddRange(dateHeaders);
                        }
                    }
                    else
                    {
                        fetch.AddRange(headers);
                    }

                    //Read the complete activities from the device
                    IList<GlobalsatPacket.Train> trains = ((GlobalsatProtocol2)device).ReadTracks(fetch, monitor);

                    //popup if short remaining time
                    GlobalsatSystemConfiguration2 systemInfo = ((GlobalsatProtocol2)device).GetGlobalsatSystemConfiguration2();
                    TimeSpan remainTime = device.RemainingTime(headers, systemInfo);
                    if (remainTime < TimeSpan.FromHours(5))
                    {
                        if (remainTime < TimeSpan.Zero)
                        {
                            remainTime = TimeSpan.Zero;
                        }
                        string msg = string.Format("Remaining recording time about {0}", remainTime.ToString());
                        System.Windows.Forms.MessageBox.Show(msg, "", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    }

                    for (int i = 0; i < trains.Count; i++)
                    {
                        //Adjust "no gps fix" time so they are easier to find...
                        //Guess assumes no-GPS matches a real activity and that activities are in order
                        GlobalsatPacket.Train train = trains[i];
                        if (i > 20)
                        { }
                        IActivity logActivity = this.getMatchingLogActivity(i, trains.Count);
                        if (train.StartTime - device.FitnessDevice.NoGpsDate < TimeSpan.FromDays(14) && logActivity != null)
                        {
                            train.Comment += "Original activity date: " + train.StartTime + ", index: "+ i;
                            if (DateTime.UtcNow - logActivity.StartTime < TimeSpan.FromDays(7))
                            {
                                //fairly recent, assume this is a duplicate
                                train.StartTime = logActivity.StartTime;
                            }
                            else
                            {
                                //older, set older date
                                train.StartTime = DateTime.UtcNow - TimeSpan.FromDays(7 + trains.Count - i);
                            }
                        }
                    }
                    AddActivities(importResults, trains, device.FitnessDevice.configInfo.ImportSpeedDistanceTrack, device.FitnessDevice.configInfo.DetectPausesFromSpeedTrack, device.FitnessDevice.configInfo.Verbose);
                    result = true;
                }
            }
            catch (Exception e)
            {
                //if (device.DataRecieved)
                {
                    monitor.ErrorText = e.Message;
                    //throw e;
                }
            }
            finally
            {
                device.Close();
            }
            if (!device.DataRecieved)
            {
                //override other possible errors
                device.NoCommunicationError(monitor);
            }
            return result;
        }

        //A device without time set has bad start dates, try to find a "matching" activity in tha latest logbook entries
        private System.Collections.SortedList activities = null;
        IActivity getMatchingLogActivity(int index, int count)
        {
            //Note: the activity list may not be in perfect order, sort the tail
            if (activities == null || activities.Count != count)
            {
                activities = new System.Collections.SortedList(count);
                int logIndex = Plugin.Instance.Application.Logbook.Activities.Count - 1;
                for (int i = 0; i < count; i++)
                {
                    //TBD: Only use My Activities and ignore My Friends Activities?
                    while (logIndex >= 0 && activities.ContainsKey(Plugin.Instance.Application.Logbook.Activities[logIndex].StartTime))
                    {
                        logIndex--;
                    }
                    if (logIndex > 0)
                    {
                        IActivity activity = Plugin.Instance.Application.Logbook.Activities[logIndex];
                        activities.Add(activity.StartTime, activity);
                    }
                    else
                    {
                        activities.Add(DateTime.MinValue, null);
                    }
                }
            }
            return (IActivity)activities.GetByIndex(index);
        }

        protected void AddActivities(IImportResults importResults, IList<GlobalsatPacket.Train> trains, bool importSpeedTrackAsDistance, bool detectPausesFromSpeed, int verbose)
        {
            foreach (GlobalsatPacket.Train train in trains)
            {
                DateTime pointTime = train.StartTime;
                IActivity activity = importResults.AddActivity(pointTime);
                activity.Metadata.Source = string.Format(CommonResources.Text.Devices.ImportJob_ActivityImportSource, sourceDescription);
                activity.TotalTimeEntered = train.TotalTime;
                activity.TotalDistanceMetersEntered = train.TotalDistanceMeters;
                activity.TotalCalories = train.TotalCalories;
                activity.MaximumHeartRatePerMinuteEntered = train.MaximumHeartRate;
                activity.AverageHeartRatePerMinuteEntered = train.AverageHeartRate;
                activity.MaximumCadencePerMinuteEntered = train.MaximumCadence;
                activity.AverageCadencePerMinuteEntered = train.AverageCadence;
                activity.MaximumPowerWattsEntered = train.MaximumPower;
                activity.AveragePowerWattsEntered = train.AveragePower;
                activity.TotalAscendMetersEntered = train.TotalAscend;
                activity.TotalDescendMetersEntered = -train.TotalDescend;
                activity.Notes += train.Comment;

                bool foundGPSPoint = false;
                bool foundCadencePoint = false;
                bool foundPowerPoint = false;

                activity.GPSRoute = new GPSRoute();
                activity.HeartRatePerMinuteTrack = new NumericTimeDataSeries();
                activity.CadencePerMinuteTrack = new NumericTimeDataSeries();
                activity.PowerWattsTrack = new NumericTimeDataSeries();
                activity.TemperatureCelsiusTrack = new NumericTimeDataSeries();
                activity.ElevationMetersTrack = new NumericTimeDataSeries(); 
                activity.DistanceMetersTrack = new DistanceDataTrack();

                float pointDist = 0;
                double pointElapsed = 0;
                //As interval to first is not zero, add point
                activity.DistanceMetersTrack.Add(pointTime, pointDist);

                //Fix for (GB-580 only?) recording problem with interval 10 or larger (in fw before 2012-09)
                double? fixInterval = null;
                if (train.TrackPoints.Count > 1)
                {
                    //All points except last has 0.1s interval
                    double testIntervall = (train.TotalTime.TotalSeconds - train.TrackPoints[train.TrackPoints.Count - 1].IntervalTime) / (train.TrackPointCount - 1 - 1);
                    if (testIntervall > 9.6)
                    {
                        fixInterval = testIntervall;
                    }
                }

                foreach (GhPacketBase.TrackPoint point in train.TrackPoints)
                {
                    double time = point.IntervalTime;
                    if (time < 0.11 && fixInterval != null)
                    {
                        time = (double)fixInterval;
                    }
                    pointElapsed += time;
                    //Note: There is an intervall time to the first point, also if TGP says it is 0
                    pointTime = pointTime.AddSeconds(time);

                    float dist = (float)(point.Speed * time);
                    pointDist += dist;

                    // TODO: How are GPS points indicated in indoor activities?
                    //It seems like all are the same
                    IGPSPoint gpsPoint = new GPSPoint((float)point.Latitude, (float)point.Longitude, point.Altitude);

                    //There are no pause markers in the Globalsat protocol
                    //Insert pauses when estimated/listed distance differs "too much"
                    //Guess pauses - no info of real pause, but this can at least be marked in the track
                    //Share setting with global split
                    if (detectPausesFromSpeed &&
                        (foundGPSPoint && activity.GPSRoute.Count > 0 ||
                        activity.HeartRatePerMinuteTrack.Count > 0))
                    {
                        //estimated time for the pause
                        double estimatedSec = 0;
                        //how far from the first point
                        double perc = 0;
                        string info = "";
                        bool insertPause = false;
                        if (activity.GPSRoute.Count > 0)
                        {
                            float gpsDist = gpsPoint.DistanceMetersToPoint(activity.GPSRoute[activity.GPSRoute.Count - 1].Value);
                            //Some limit on when to include pause
                            //gpsDist must be higher than (all) GPS errors
                            if (gpsDist > 100 && gpsDist > 3 * dist)
                            {
                                insertPause = true;
                                //We cannot know the true time for the expected pause, just set time between as pause to show on map
                                estimatedSec = time;

                                //Old guess
                                ////Info on activity is unreliable as distance when paused is included, but average speed at start is too
                                //if (pointDist > 0 && gpsDist < 10000)
                                //{
                                //    //Avarage speed since start, not the last points
                                //    estimatedSec = 2 * pointElapsed * gpsDist / pointDist;
                                //    //Sudden jumps can create huge estimations, limit
                                //    estimatedSec = Math.Min(estimatedSec, 3600);
                                //}
                                //else
                                //{
                                //    estimatedSec = 4;
                                //}                                
                                //if (estimatedSec > 3) { insertPause = true; }
                                ////The true pause point is somewhere between last and this point
                                ////Use first part of the interval, insert a new point
                                if (gpsDist > 0)
                                {
                                    perc = dist / gpsDist;
                                }
                                info += "gps: " + gpsDist;
                            }
                        }
                        //Deactivated for now
                        //if (estimatedSec == 0)
                        //{
                        //    //TODO: from athlete? Should only filter jumps from pauses, but reconnect could give similar?
                        //    const int minHrStep = 20;
                        //    const int minHr = 70;
                        //    const int maxHr = 180;
                        //    if (activity.HeartRatePerMinuteTrack[activity.HeartRatePerMinuteTrack.Count - 1].Value > minHr &&
                        //    activity.HeartRatePerMinuteTrack[activity.HeartRatePerMinuteTrack.Count - 1].Value < maxHr &&
                        //    point.HeartRate > minHr &&
                        //    point.HeartRate < maxHr &&
                        //    Math.Abs(activity.HeartRatePerMinuteTrack[activity.HeartRatePerMinuteTrack.Count - 1].Value - point.HeartRate) > minHrStep)
                        //    {
                        //        estimatedSec = 4;
                        //    }
                        //}
                        if (insertPause)
                        {
                            //Use complete seconds only - pause is estimated, ST handles sec internally and this must be synced to laps
                            estimatedSec = Math.Round(estimatedSec);
                            IGPSPoint newPoint = (new GPSPoint.ValueInterpolator()).Interpolate(
                                activity.GPSRoute[activity.GPSRoute.Count - 1].Value, gpsPoint, perc);

                            if (point == train.TrackPoints[train.TrackPoints.Count - 1])
                            {
                                //Last point is incorrect, adjust (added normally)
                                gpsPoint = newPoint;
                            }
                            else
                            {
                                DateTime pointTimePrev;
                                DateTime pointTimeNext;
                                if (estimatedSec <= time+1)
                                {
                                    pointTimePrev = pointTime.AddSeconds(-time);
                                    pointTimeNext = pointTime;
                                }
                                else
                                {
                                    //Add extra point
                                    activity.DistanceMetersTrack.Add(pointTime, pointDist);

                                    if (point.Latitude != 0 || point.Longitude != 0)
                                    {
                                        activity.GPSRoute.Add(pointTime, newPoint);
                                    }
                                    else if (device.FitnessDevice.HasElevationTrack && !float.IsNaN(newPoint.ElevationMeters))
                                    {
                                        activity.ElevationMetersTrack.Add(pointTime, newPoint.ElevationMeters);
                                    }
                                    pointTimePrev = pointTime;
                                    pointTimeNext = pointTime.AddSeconds((int)(estimatedSec - time));
                                    pointTime = pointTimeNext;
                                }
                                //Only add rounded pauses, ST only handles complete seconds
                                activity.TimerPauses.Add(new ValueRange<DateTime>(
                                    pointTimePrev.AddMilliseconds(-pointTimePrev.Millisecond+1000),
                                    pointTimeNext.AddMilliseconds(-pointTimeNext.Millisecond-10000)));
                                if (verbose >= 10)
                                {
                                    //TODO: Remove remark when stable
                                    activity.Notes += string.Format("Added pause from {0} to {1} (dist:{2}, elapsedSec:{3}, per:{4} {5}) ",
                                        pointTimePrev.ToLocalTime(), pointTimeNext.ToLocalTime(), dist, time, perc, info) +
                                        System.Environment.NewLine;
                                }
                            }
                        }
                    }
                    activity.DistanceMetersTrack.Add(pointTime, pointDist);

                    //lat/lon is 0 if a device has never had a fix
                    if (point.Latitude != 0 || point.Longitude != 0)
                    {
                        activity.GPSRoute.Add(pointTime, gpsPoint);

                        //Check if lat/lon ever change (ignore altitude), GlobalSat reports last known location without a fix
                        if (point.Latitude != train.TrackPoints[0].Latitude || point.Longitude != train.TrackPoints[0].Longitude)
                        {
                            foundGPSPoint = true;
                        }
                    }
                    if (device.FitnessDevice.HasElevationTrack && !float.IsNaN(point.Altitude))
                    {
                        activity.ElevationMetersTrack.Add(pointTime, point.Altitude);
                    }

                    //zero HR is invalid reading - drop
                    if (point.HeartRate > 0)
                    {
                        activity.HeartRatePerMinuteTrack.Add(pointTime, point.HeartRate);
                    }

                    //Zero Cadence/Power may be valid values, if there are any values (no way to detect lost communication)
                    activity.CadencePerMinuteTrack.Add(pointTime, point.Cadence);
                    if (point.Cadence > 0) foundCadencePoint = true;

                    activity.PowerWattsTrack.Add(pointTime, point.Power);
                    if (point.Power > 0) foundPowerPoint = true;

                    if (point.Temperature != 0x7fff)
                    {
                        activity.TemperatureCelsiusTrack.Add(pointTime, point.Temperature / 10.0F);
                    }
                }

                TimeSpan lapElapsed = TimeSpan.Zero;
                int totalDistance = 0;
                foreach (GlobalsatPacket.Lap lapPacket in train.Laps)
                {
                    DateTime lapTime = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(activity.StartTime, lapElapsed, activity.TimerPauses);
                    lapElapsed += lapPacket.LapTime; //Same as lapPacket.EndTime for unpaused
                    ILapInfo lap = activity.Laps.Add(lapTime, lapPacket.LapTime);
                    //Adding Distance would previously make ST fail to add new laps. The distance is needed only when there is no GPS (Markers added)
                    if (activity.TotalDistanceMetersEntered > 0)
                    {
                        lap.TotalDistanceMeters = lapPacket.LapDistanceMeters;
                    }
                    if (lapPacket.LapCalories > 0)
                    {
                        lap.TotalCalories = lapPacket.LapCalories;
                    }
                    if (lapPacket.AverageHeartRate > 0)
                    {
                        lap.AverageHeartRatePerMinute = lapPacket.AverageHeartRate;
                    }
                    if (lapPacket.AverageCadence > 0)
                    {
                        lap.AverageCadencePerMinute = lapPacket.AverageCadence;
                    }
                    if (lapPacket.AveragePower > 0)
                    {
                        lap.AveragePowerWatts = lapPacket.AveragePower;
                    }
                    if (!foundGPSPoint && activity.ElevationMetersTrack != null && activity.ElevationMetersTrack.Count > 1)
                    {
                        //Limitation in ST: lap elevation not auto calc without GPS, lap preferred to elevation
                        DateTime lapEnd = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(activity.StartTime, lapElapsed, activity.TimerPauses);
                        ITimeValueEntry<float> p1 = activity.ElevationMetersTrack.GetInterpolatedValue(lapTime);
                        ITimeValueEntry<float> p2 = activity.ElevationMetersTrack.GetInterpolatedValue(lapEnd);
                        if (p1 != null && p2 != null)
                        {
                            lap.ElevationChangeMeters = p2.Value - p1.Value;
                        }
                    }
                    if (verbose >= 5)
                    {
                        //TODO: Localise outputs?
                        lap.Notes = string.Format("MaxSpeed:{0:0.##}m/s MaxHr:{1} MinAlt:{2}m MaxAlt:{3}m",
                            lapPacket.MaximumSpeed, lapPacket.MaximumHeartRate, lapPacket.MinimumAltitude, lapPacket.MaximumAltitude);
                        //Not adding Power/Cadence - not available
                        //lap.Notes = string.Format("MaxSpeed={0} MaxHr={1} MinAlt={2} MaxAlt={3} MaxCadence={4} MaxPower={5}",
                        //    lapPacket.MaximumSpeed, lapPacket.MaximumHeartRate, lapPacket.MinimumAltitude, lapPacket.MaximumAltitude, lapPacket.MaximumCadence, lapPacket.MaximumPower);
                    }
                    //Add distance markers from Globalsat. Will for sure be incorrect after pause insertion
                    totalDistance += lapPacket.LapDistanceMeters;
                    activity.DistanceMarkersMeters.Add(totalDistance);
                }
                
                if (!foundGPSPoint)
                {
                    if (activity.GPSRoute.Count > 0)
                    {
                        activity.Notes += string.Format("No GPS. Last known latitude:{0}, longitude:{1}", 
                            activity.GPSRoute[0].Value.LatitudeDegrees, activity.GPSRoute[0].Value.LongitudeDegrees);
                    }
                    activity.GPSRoute = null;
                }
                //Keep elevation only if the device (may) record elevation separately from GPS
                //It may be used also if the user drops GPS if points have been recorded.
                //(ST may have partial use of elevation together with GPS on other parts in the future?)
                if (!device.FitnessDevice.HasElevationTrack || activity.ElevationMetersTrack.Count == 0) activity.ElevationMetersTrack = null;

                //Barometric devices occasionally have bad points last
                if (activity.ElevationMetersTrack != null && activity.ElevationMetersTrack.Count > 1 &&
                    Math.Abs(activity.ElevationMetersTrack[activity.ElevationMetersTrack.Count - 1].Value - 
                      activity.ElevationMetersTrack[activity.ElevationMetersTrack.Count - 2].Value) > 1)
                {
                    if (activity.GPSRoute != null &&
                        activity.ElevationMetersTrack.StartTime.AddSeconds(activity.ElevationMetersTrack.TotalElapsedSeconds) ==
                        activity.GPSRoute.StartTime.AddSeconds(activity.GPSRoute.TotalElapsedSeconds))
                    {
                        IGPSPoint g = activity.GPSRoute[activity.GPSRoute.Count - 1].Value;
                        activity.GPSRoute.SetValueAt(activity.GPSRoute.Count - 1, new GPSPoint(g.LatitudeDegrees, g.LongitudeDegrees, float.NaN));
                    }
                    activity.ElevationMetersTrack.RemoveAt(activity.ElevationMetersTrack.Count - 1);
                }
                if (activity.HeartRatePerMinuteTrack.Count == 0) activity.HeartRatePerMinuteTrack = null;
                if (!foundCadencePoint) activity.CadencePerMinuteTrack = null;
                if (!foundPowerPoint) activity.PowerWattsTrack = null;
                if (activity.TemperatureCelsiusTrack.Count == 0) activity.TemperatureCelsiusTrack = null;
                if (pointDist == 0 || !importSpeedTrackAsDistance && foundGPSPoint)
                {
                    activity.DistanceMetersTrack = null;
                }
#if DISTANCETRACK_FIX
                //attempt to fix import of distance track in 3.1.4515 - to be updated when rereleased
                //for now import distance track, to keep compatibility(?)
                try
                {
                    activity.CalcSpeedFromDistanceTrack = importSpeedTrackAsDistance;
                }
                catch
                {
                    //older than 3.1.4515, disable
                    if (!importSpeedTrackAsDistance && !foundGPSPoint)
                    {
                        activity.DistanceMetersTrack = null;
                    }
                }
#endif
            }
        }
    }
}
