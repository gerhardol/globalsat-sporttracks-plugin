/*
Copyright (C) 2010 Zone Five Software

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

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
                //Gh625XTDevice device = (Gh625XTDevice)this.device;
                device.Open();
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
                AddActivities(importResults, trains);
                return true;
            }
            finally
            {
                device.Close();
            }
        }

        protected virtual void AddActivities(IImportResults importResults, IList<GlobalsatPacket.Train> trains)
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
                bool first = true;

                foreach (GhPacketBase.TrackPoint point in train.TrackPoints)
                {
                    double time = point.IntervalTime / 10.0;
                    float dist = (float)(point.Speed * time);
                    // TODO: How are GPS points indicated in indoor activities?
                    //It seems like all are the same
                    IGPSPoint gpsPoint = new GPSPoint((float)point.Latitude, (float)point.Longitude, point.Altitude);
                    //Bug in 625XT, incorrect last point
                    if (activity.GPSRoute.Count > 0 && point == train.TrackPoints[train.TrackPoints.Count - 1] &&
                        train == trains[trains.Count-1] && this.device is Gh625XTDevice)
                    {
                        gpsPoint = activity.GPSRoute[activity.GPSRoute.Count - 1].Value;
                    }
                    //There are no pause markers in the Globalsat protocol
                    //Insert pauses when estimated/listed distance differs "to much"
                    //Guess pauses - no info of real pause, but this can at least be marked in the track
                    bool isPause = false;
                    if (foundGPSPoint && activity.GPSRoute.Count > 0 ||
                        activity.HeartRatePerMinuteTrack.Count > 0)
                    {
                        double estimatedSec = 0;
                        if (activity.GPSRoute.Count > 0)
                        {
                            float gpsDist = gpsPoint.DistanceMetersToPoint(activity.GPSRoute[activity.GPSRoute.Count - 1].Value);
                            //Some limit on when to include pause
                            if (gpsDist > 50 && gpsDist > 3 * dist)
                            {
                                //Assume it is not a straight line, assume 2 times
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
                            }
                        }

                        if (false && estimatedSec == 0)
                        {
                            //TODO: from athlete? Should only filter jumps from pauses, but reconnect could give similar?
                            const int minHrStep = 20;
                            const int minHr = 70;
                            const int maxHr = 180;
                            if (activity.HeartRatePerMinuteTrack[activity.HeartRatePerMinuteTrack.Count - 1].Value > minHr &&
                            activity.HeartRatePerMinuteTrack[activity.HeartRatePerMinuteTrack.Count - 1].Value < maxHr &&
                            point.HeartRate > minHr &&
                            point.HeartRate < maxHr &&
                            Math.Abs(activity.HeartRatePerMinuteTrack[activity.HeartRatePerMinuteTrack.Count - 1].Value - point.HeartRate) > minHrStep)
                            {
                                estimatedSec = 4;
                            }
                        }
                        if (estimatedSec > 3)
                        {
                            DateTime pointTime2 = pointTime.AddSeconds(estimatedSec);
                            activity.TimerPauses.Add(new ValueRange<DateTime>(pointTime.AddSeconds(1), pointTime2.AddSeconds(-1)));
                            //TODO: Remove remark when stable
                            activity.Notes += string.Format("Added pause from {0} to {1} ({2}, {3}) ",
                                pointTime.ToLocalTime(), pointTime2.ToLocalTime(), dist, time) +
                                System.Environment.NewLine;
                            pointTime = pointTime2;
                            isPause = true;
                        }
                    }
                    //Note: There may be points witin the same second, the second point will then overwrite the first
                    if (!isPause)
                    {
                        if (!first)
                        {
                            pointTime = pointTime.AddSeconds(time);
                        }
                        else
                        {
                            first = false;
                        }
                        pointElapsed += time;
                        pointDist += dist;
                    }
                    activity.DistanceMetersTrack.Add(pointTime, pointDist);

                    // TODO: How are GPS points indicated in indoor activities?
                    if (point.Latitude != 0 || point.Longitude != 0)
                    {
                        activity.GPSRoute.Add(pointTime, gpsPoint);
                    }
                    else if (device.HasElevationTrack)
                    {
                        activity.ElevationMetersTrack.Add(pointTime, point.Altitude);
                    }
                    if (point.Latitude != train.TrackPoints[0].Latitude || point.Longitude != train.TrackPoints[0].Longitude || point.Altitude != train.TrackPoints[0].Altitude) foundGPSPoint = true;

                    activity.HeartRatePerMinuteTrack.Add(pointTime, point.HeartRate);
                    if (point.HeartRate > 0) foundHrPoint = true;

                    activity.CadencePerMinuteTrack.Add(pointTime, point.Cadence);
                    if (point.Cadence > 0) foundCadencePoint = true;

                    activity.PowerWattsTrack.Add(pointTime, point.Power);
                    if (point.Power > 0) foundPowerPoint = true;
                }

                TimeSpan lapElapsed = TimeSpan.Zero;
                foreach (GlobalsatPacket.Lap lapPacket in train.Laps)
                {
                    DateTime lapTime = ZoneFiveSoftware.Common.Data.Algorithm.DateTimeRangeSeries.AddTimeAndPauses(activity.StartTime, lapElapsed, activity.TimerPauses);
                    lapElapsed += lapPacket.LapTime;
                    ILapInfo lap = activity.Laps.Add(lapTime, lapPacket.LapTime);
                    lap.TotalDistanceMeters = lapPacket.LapDistanceMeters;
                    lap.TotalCalories = lapPacket.LapCalories;
                    lap.AverageHeartRatePerMinute = lapPacket.AverageHeartRate;
                    lap.AverageCadencePerMinute = lapPacket.AverageCadence;
                    lap.AveragePowerWatts = lapPacket.AveragePower;
                    //TODO: Localise outputs?
                    lap.Notes = string.Format("MaxSpeed:{0:0.##}m/s MaxHr:{1} MinAlt:{2}m MaxAlt:{3}m",
                        lapPacket.MaximumSpeed, lapPacket.MaximumHeartRate, lapPacket.MinimumAltitude, lapPacket.MaximumAltitude);
                    //Not adding Power/Cadence - not available
                    //lap.Notes = string.Format("MaxSpeed={0} MaxHr={1} MinAlt={2} MaxAlt={3} MaxCadence={4} MaxPower={5}",
                    //    lapPacket.MaximumSpeed, lapPacket.MaximumHeartRate, lapPacket.MinimumAltitude, lapPacket.MaximumAltitude, lapPacket.MaximumCadence, lapPacket.MaximumPower);
                }

                if (!foundGPSPoint) activity.GPSRoute = null;
                if (!device.HasElevationTrack || activity.ElevationMetersTrack.Count == 0) activity.ElevationMetersTrack = null; 
                if (!foundHrPoint) activity.HeartRatePerMinuteTrack = null;
                if (!foundCadencePoint) activity.CadencePerMinuteTrack = null;
                if (!foundPowerPoint) activity.PowerWattsTrack = null;
                if (pointDist == 0) activity.DistanceMetersTrack = null;
            }
        }
    }
}
