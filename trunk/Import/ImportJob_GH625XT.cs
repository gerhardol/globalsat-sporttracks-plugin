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
    class ImportJob_GH625XT : ImportJob
    {
        public ImportJob_GH625XT(Gh625XTDevice device, string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        : base(device, sourceDescription, monitor, importResults)
        {
        }

        public override bool Import()
        {
            try
            {
                Gh625XTDevice device = (Gh625XTDevice)this.device;
                device.Open();
                IList<Gh625XTPacket.TrackFileHeader> headers = device.ReadTrackHeaders(monitor);
                List<Gh625XTPacket.TrackFileHeader> fetch = new List<Gh625XTPacket.TrackFileHeader>();

                if (device.configInfo.ImportOnlyNew && Plugin.Instance.Application != null && Plugin.Instance.Application.Logbook != null)
                {
                    IDictionary<DateTime, IList<Gh625XTPacket.TrackFileHeader>> headersByStart = new Dictionary<DateTime, IList<Gh625XTPacket.TrackFileHeader>>();
                    foreach (Gh625XTPacket.TrackFileHeader header in headers)
                    {
                        DateTime start = header.StartTime.AddHours(device.configInfo.HoursAdjustment);
                        if (!headersByStart.ContainsKey(start))
                        {
                            headersByStart.Add(start, new List<Gh625XTPacket.TrackFileHeader>());
                        }
                        headersByStart[start].Add(header);
                    }
                    DateTime now = DateTime.UtcNow;
                    foreach (IActivity activity in Plugin.Instance.Application.Logbook.Activities)
                    {
                        DateTime findTime = activity.StartTime;
                        if (headersByStart.ContainsKey(findTime) && (now-findTime).TotalSeconds > device.configInfo.SecondsAlwaysImport)
                        {
                            headersByStart.Remove(findTime);
                        }
                    }
                    foreach (IList<Gh625XTPacket.TrackFileHeader> dateHeaders in headersByStart.Values)
                    {
                        fetch.AddRange(dateHeaders);
                    }
                }
                else
                {
                    fetch.AddRange(headers);
                }

                IList<Gh625XTPacket.Train> trains = device.ReadTracks(fetch, monitor);
                AddActivities(importResults, trains);
                return true;
            }
            finally
            {
                device.Close();
            }
        }


        private void AddActivities(IImportResults importResults, IList<Gh625XTPacket.Train> trains)
        {
            foreach (Gh625XTPacket.Train train in trains)
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
                //TODO: Distance track from speed

                DateTime pointTime = activity.StartTime;                
                foreach (GhPacketBase.TrackPoint point in train.TrackPoints)
                {
                    //TODO: There are no pause markers in the Globalsat protocol
                    //Insert pauses when estimated/listed distance differs "to much"
                    
                    //Note: There may be points witin the same second, the second point will then overwrite the first
                    pointTime = pointTime.AddSeconds((double)point.IntervalTime / 10);

                    // TODO: How are GPS points indicated in indoor activities?
                    //It seems like all are the same
                    activity.GPSRoute.Add(pointTime, new GPSPoint((float)point.Latitude, (float)point.Longitude, point.Altitude));
                    
                    if (point.Latitude != train.TrackPoints[0].Latitude || point.Longitude != train.TrackPoints[0].Longitude || point.Altitude != train.TrackPoints[0].Altitude) foundGPSPoint = true;

                    activity.HeartRatePerMinuteTrack.Add(pointTime, point.HeartRate);
                    if (point.HeartRate > 0) foundHrPoint = true;

                    activity.CadencePerMinuteTrack.Add(pointTime, point.Cadence);
                    if (point.Cadence > 0) foundCadencePoint = true;

                    activity.PowerWattsTrack.Add(pointTime, point.Power);
                    if (point.Power > 0) foundPowerPoint = true;
                }

                DateTime lapTime = activity.StartTime;
                foreach (Gh625XTPacket.Lap lapPacket in train.Laps)
                {
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
                    lapTime = lapTime.Add(lapPacket.LapTime);
                }

                if (!foundGPSPoint) activity.GPSRoute = null;
                if (!foundHrPoint) activity.HeartRatePerMinuteTrack = null;
                if (!foundCadencePoint) activity.CadencePerMinuteTrack = null;
                if (!foundPowerPoint) activity.PowerWattsTrack = null;
            }
        }
    }
}
