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
            try
            {
                if (device.Open())
                {
                    IList<GlobalsatPacket.TrackFileHeader> headers = ((GlobalsatProtocol2)device).ReadTrackHeaders(monitor);
                    List<GlobalsatPacket.TrackFileHeader> fetch = new List<GlobalsatPacket.TrackFileHeader>();

                    if (device.configInfo.ImportOnlyNew && Plugin.Instance.Application != null && Plugin.Instance.Application.Logbook != null)
                    {
                        IDictionary<DateTime, IList<GlobalsatPacket.TrackFileHeader>> headersByStart = new Dictionary<DateTime, IList<GlobalsatPacket.TrackFileHeader>>();
                        foreach (GlobalsatPacket.TrackFileHeader header in headers)
                        {
                            DateTime start = header.StartTime.AddHours(device.configInfo.HoursAdjustment);
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
                            if (headersByStart.ContainsKey(findTime) && (now - findTime).TotalSeconds > device.configInfo.SecondsAlwaysImport)
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

                    IList<GlobalsatPacket.Train> trains = ((GlobalsatProtocol2)device).ReadTracks(fetch, monitor);
                    AddActivities(importResults, trains, device.configInfo.ImportSpeedTrack, device.configInfo.Verbose);
                }
            }
            catch (Exception e)
            {
                if (device.DataRecieved)
                {
                    throw e;
                }
            }
            finally
            {
                device.Close();
            }
            if (!device.DataRecieved)
            {
                device.NoCommunicationError(monitor);
                return false;
            }
            return true;
        }

        protected void AddActivities(IImportResults importResults, IList<GlobalsatPacket.Train> trains, bool importSpeedTrackAsDistance, int verbose)
        {
            foreach (GlobalsatPacket.Train train in trains)
            {
                IActivity activity = importResults.AddActivity(train.StartTime);
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
                activity.TotalDescendMetersEntered = train.TotalDescend;

                bool foundGPSPoint = false;
                bool foundHrPoint = false;
                bool foundCadencePoint = false;
                bool foundPowerPoint = false;

                activity.GPSRoute = new GPSRoute();
                activity.HeartRatePerMinuteTrack = new NumericTimeDataSeries();
                activity.CadencePerMinuteTrack = new NumericTimeDataSeries();
                activity.PowerWattsTrack = new NumericTimeDataSeries();
                activity.ElevationMetersTrack = new NumericTimeDataSeries(); 
                activity.DistanceMetersTrack = new DistanceDataTrack();

                DateTime pointTime = activity.StartTime;
                double pointElapsed = 0;
                float pointDist = 0;

                foreach (GhPacketBase.TrackPoint point in train.TrackPoints)
                {
                    double time = point.IntervalTime;
                    float dist = (float)(point.Speed * time);
                    // TODO: How are GPS points indicated in indoor activities?
                    //It seems like all are the same
                    IGPSPoint gpsPoint = new GPSPoint((float)point.Latitude, (float)point.Longitude, point.Altitude);
                    //Note: There may be points witin the same second, the second point will then overwrite the first
                    pointTime = pointTime.AddSeconds(time);
                    pointElapsed += time;
                    pointDist += dist;

                    //There are no pause markers in the Globalsat protocol
                    //Insert pauses when estimated/listed distance differs "too much"
                    //Guess pauses - no info of real pause, but this can at least be marked in the track
                    //Share setting with global split
                    if (ZoneFiveSoftware.SportTracks.Device.Globalsat.Plugin.Instance.Application.SystemPreferences.ImportSettings.SplitActivity &&
                        (foundGPSPoint && activity.GPSRoute.Count > 0 ||
                        activity.HeartRatePerMinuteTrack.Count > 0))
                    {
                        //estimated time for the pause
                        double estimatedSec = 0;
                        //how far from the first point
                        double perc = 0;
                        string info = "";
                        if (activity.GPSRoute.Count > 0)
                        {
                            float gpsDist = gpsPoint.DistanceMetersToPoint(activity.GPSRoute[activity.GPSRoute.Count - 1].Value);
                            //Some limit on when to include pause
                            if (gpsDist > 50 && gpsDist > 3 * dist)
                            {
                                //Assume lost path is not a straight line, assume 2 times
                                //Info on activity is unreliable as distance when paused is included, but average speed at start is too
                                if (pointDist > 0 && gpsDist < 10000)
                                {
                                    estimatedSec = 2 * gpsDist * pointElapsed / pointDist;
                                    //Sudden jumps can create huge estimations, limit
                                    estimatedSec = Math.Min(estimatedSec, 3600);
                                }
                                else
                                {
                                    estimatedSec = 4;
                                }                                
                                //The true pause point is somewhere between last and this point
                                //Use first part of the interval, insert a new point
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
                        if (estimatedSec > 3)
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
                                //Add extra point and pause
                                activity.DistanceMetersTrack.Add(pointTime, pointDist);

                                if (point.Latitude != 0 || point.Longitude != 0)
                                {
                                    activity.GPSRoute.Add(pointTime, newPoint);
                                }
                                else if (device.HasElevationTrack && !float.IsNaN(newPoint.ElevationMeters))
                                {
                                    activity.ElevationMetersTrack.Add(pointTime, newPoint.ElevationMeters);
                                }

                                //Only add even pauses, ST only handles complete seconds
                                DateTime pointTime2 = pointTime.AddSeconds((int)estimatedSec);
                                activity.TimerPauses.Add(new ValueRange<DateTime>(
                                    pointTime.AddMilliseconds(-pointTime.Millisecond),
                                    pointTime2.AddMilliseconds(-pointTime2.Millisecond)));
                                if (verbose >= 10)
                                {
                                    //TODO: Remove remark when stable
                                    activity.Notes += string.Format("Added pause from {0} to {1} (dist:{2}, elapsedSec:{3}, per:{4} {5}) ",
                                        pointTime.ToLocalTime(), pointTime2.ToLocalTime(), dist, time, perc, info) +
                                        System.Environment.NewLine;
                                }
                                pointTime = pointTime2;
                            }
                        }
                    }
                    activity.DistanceMetersTrack.Add(pointTime, pointDist);

                    if (point.Latitude != 0 || point.Longitude != 0)
                    {
                        activity.GPSRoute.Add(pointTime, gpsPoint);
                    }
                    if (device.HasElevationTrack && !float.IsNaN(point.Altitude))
                    {
                        activity.ElevationMetersTrack.Add(pointTime, point.Altitude);
                    }
                    //Ignore altitude when checking if there is a GPS track, no need to check HasElevationTrack
                    if (point.Latitude != train.TrackPoints[0].Latitude || point.Longitude != train.TrackPoints[0].Longitude)
                    {
                        foundGPSPoint = true;
                    }

                    activity.HeartRatePerMinuteTrack.Add(pointTime, point.HeartRate);
                    if (point.HeartRate > 0) foundHrPoint = true;

                    activity.CadencePerMinuteTrack.Add(pointTime, point.Cadence);
                    if (point.Cadence > 0) foundCadencePoint = true;

                    activity.PowerWattsTrack.Add(pointTime, point.Power);
                    if (point.Power > 0) foundPowerPoint = true;
                }

                TimeSpan lapElapsed = TimeSpan.Zero;
                int totalDistance = 0;
                foreach (GlobalsatPacket.Lap lapPacket in train.Laps)
                {
                    DateTime lapTime = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(activity.StartTime, lapElapsed, activity.TimerPauses);
                    lapElapsed += lapPacket.LapTime;
                    ILapInfo lap = activity.Laps.Add(lapTime, lapPacket.LapTime);
                    //Adding Distance markers will make ST fail to add new laps, it is not needed (Markers added)
                    //if (activity.TotalDistanceMetersEntered > 0)
                    //{
                    //    lap.TotalDistanceMeters = lapPacket.LapDistanceMeters;
                    //}
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
                        activity.Notes += string.Format("No GPS. Last known latitude:{0}, longitude:{1}", activity.GPSRoute[0].Value.LatitudeDegrees, activity.GPSRoute[0].Value.LongitudeDegrees);
                    }
                    activity.GPSRoute = null;
                }
                //Keep elevation only if the device (may) record elevation separately from GPS
                //It may be used also if the user drops GPS if points have been recorded.
                //(ST may have partial use of elevation together with GPS on other parts in the future?)
                if (!device.HasElevationTrack || activity.ElevationMetersTrack.Count == 0) activity.ElevationMetersTrack = null; 
                if (!foundHrPoint) activity.HeartRatePerMinuteTrack = null;
                if (!foundCadencePoint) activity.CadencePerMinuteTrack = null;
                if (!foundPowerPoint) activity.PowerWattsTrack = null;
                if (pointDist == 0)
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
