// Copyright (C) 2010 Zone Five Software
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
    class ImportJob_GH505
    {
        public ImportJob_GH505(string sourceDescription, DeviceConfigurationInfo configInfo, IJobMonitor monitor, IImportResults importResults)
        {
            this.sourceDescription = sourceDescription.Replace(Environment.NewLine, " ");
            this.configInfo = configInfo;
            this.monitor = monitor;
            this.importResults = importResults;
        }

        public bool Import()
        {
            Gh505Device device = new Gh505Device();
            try
            {
                device.Open(115200);
                IList<Gh505Packet.TrackFileHeader> headers = device.ReadTrackHeaders(monitor);
                List<Gh505Packet.TrackFileHeader> fetch = new List<Gh505Packet.TrackFileHeader>();

                if (configInfo.ImportOnlyNew && Plugin.Instance.Application != null && Plugin.Instance.Application.Logbook != null)
                {
                    IDictionary<DateTime, List<Gh505Packet.TrackFileHeader>> headersByStart = new Dictionary<DateTime, List<Gh505Packet.TrackFileHeader>>();
                    foreach (Gh505Packet.TrackFileHeader header in headers)
                    {
                        DateTime start = header.StartTime.AddHours(configInfo.HoursAdjustment);
                        if (!headersByStart.ContainsKey(start))
                        {
                            headersByStart.Add(start, new List<Gh505Packet.TrackFileHeader>());
                        }
                        headersByStart[start].Add(header);
                    }
                    foreach (IActivity activity in Plugin.Instance.Application.Logbook.Activities)
                    {
                        DateTime findTime = activity.StartTime;
                        if (headersByStart.ContainsKey(findTime))
                        {
                            headersByStart.Remove(findTime);
                        }
                    }
                    foreach (List<Gh505Packet.TrackFileHeader> dateHeaders in headersByStart.Values)
                    {
                        fetch.AddRange(dateHeaders);
                    }
                }
                else
                {
                    fetch.AddRange(headers);
                }

                IList<Gh505Packet.Train> trains = device.ReadTracks(fetch, monitor);
                AddActivities(importResults, trains);
                return true;
            }
            finally
            {
                device.Close();
            }
        }


        private void AddActivities(IImportResults importResults, IList<Gh505Packet.Train> trains)
        {
            foreach (Gh505Packet.Train train in trains)
            {
                IActivity activity = importResults.AddActivity(train.StartTime);
                activity.Metadata.Source = string.Format(CommonResources.Text.Devices.ImportJob_ActivityImportSource, sourceDescription);
                activity.TotalTimeEntered = train.TotalTime;
                activity.TotalDistanceMetersEntered = train.TotalDistanceMeters;
                activity.TotalCalories = train.TotalCalories;
                activity.AverageHeartRatePerMinuteEntered = train.AverageHeartRate;
                activity.MaximumCadencePerMinuteEntered = train.MaximumHeartRate;

                bool foundGPSPoint = false;
                bool foundHrPoint = false;
                bool foundCadencePoint = false;
                bool foundPowerPoint = false;

                activity.GPSRoute = new GPSRoute();
                activity.HeartRatePerMinuteTrack = new NumericTimeDataSeries();
                activity.CadencePerMinuteTrack = new NumericTimeDataSeries();
                activity.PowerWattsTrack = new NumericTimeDataSeries();

                DateTime pointTime = activity.StartTime;                
                foreach (GhPacketBase.TrackPoint2 point in train.TrackPoints)
                {
                    pointTime = pointTime.AddSeconds((double)point.IntervalTime / 10);

                    // TODO: How are GPS points indicated in indoor activities?
                    float latitude = (float)((double)point.Latitude / 1000000);
                    float longitude = (float)((double)point.Longitude / 1000000);
                    float elevation = point.Altitude;
                    activity.GPSRoute.Add(pointTime, new GPSPoint(latitude, longitude, elevation));

                    if (point.Latitude != train.TrackPoints[0].Latitude || point.Longitude != train.TrackPoints[0].Longitude || point.Altitude != train.TrackPoints[0].Altitude) foundGPSPoint = true;

                    activity.HeartRatePerMinuteTrack.Add(pointTime, point.HeartRate);
                    if (point.HeartRate > 0) foundHrPoint = true;

                    activity.CadencePerMinuteTrack.Add(pointTime, point.Cadence);
                    if (point.Cadence > 0) foundCadencePoint = true;

                    activity.PowerWattsTrack.Add(pointTime, point.Power);
                    if (point.Power > 0) foundPowerPoint = true;
                }

                DateTime lapTime = activity.StartTime;
                foreach (Gh505Packet.Lap lapPacket in train.Laps)
                {
                    ILapInfo lap = activity.Laps.Add(lapTime, lapPacket.LapTime);
                    lap.TotalDistanceMeters = lapPacket.LapDistanceMeters;
                    lap.TotalCalories = lapPacket.LapCalories;
                    lap.AverageHeartRatePerMinute = lapPacket.AverageHeartRate;
                    lapTime = lapTime.Add(lapPacket.LapTime);
                }

                if (!foundGPSPoint) activity.GPSRoute = null;
                if (!foundHrPoint) activity.HeartRatePerMinuteTrack = null;
                if (!foundCadencePoint) activity.CadencePerMinuteTrack = null;
                if (!foundPowerPoint) activity.PowerWattsTrack = null;
            }

        }

        private string sourceDescription;
        private DeviceConfigurationInfo configInfo;
        private IJobMonitor monitor;
        private IImportResults importResults;
    }
}
