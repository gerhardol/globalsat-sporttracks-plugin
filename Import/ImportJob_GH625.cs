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
    class ImportJob_GH625
    {
        public ImportJob_GH625(string sourceDescription, DeviceConfigurationInfo configInfo, IJobMonitor monitor, IImportResults importResults)
        {
            this.sourceDescription = sourceDescription.Replace(Environment.NewLine, " ");
            this.configInfo = configInfo;
            this.monitor = monitor;
            this.importResults = importResults;
        }

        public bool Import()
        {
            Gh625Device device = new Gh625Device();
            try
            {
                device.Open(this.configInfo);
                IList<Gh625Packet.TrackFileHeader> headers = device.ReadTrackHeaders(monitor);
                List<Gh625Packet.TrackFileHeader> fetch = new List<Gh625Packet.TrackFileHeader>();

                if (configInfo.ImportOnlyNew && Plugin.Instance.Application != null && Plugin.Instance.Application.Logbook != null)
                {
                    IDictionary<DateTime, List<Gh625Packet.TrackFileHeader>> headersByStart = new Dictionary<DateTime, List<Gh625Packet.TrackFileHeader>>();
                    foreach (Gh625Packet.TrackFileHeader header in headers)
                    {
                        DateTime start = header.StartTime.AddHours(configInfo.HoursAdjustment);
                        if (!headersByStart.ContainsKey(start))
                        {
                            headersByStart.Add(start, new List<Gh625Packet.TrackFileHeader>());
                        }
                        headersByStart[start].Add(header);
                    }
                    foreach (IActivity activity in Plugin.Instance.Application.Logbook.Activities)
                    {
                        DateTime findTime = activity.StartTime.ToLocalTime();
                        if (headersByStart.ContainsKey(findTime))
                        {
                            headersByStart.Remove(findTime);
                        }
                    }
                    foreach (List<Gh625Packet.TrackFileHeader> dateHeaders in headersByStart.Values)
                    {
                        fetch.AddRange(dateHeaders);
                    }
                }
                else
                {
                    fetch.AddRange(headers);
                }

                IList<Gh625Packet.TrackFileSection> sections = device.ReadTracks(fetch, monitor);
                AddActivities(importResults, sections);
                return true;
            }
            finally
            {
                device.Close();
            }
        }


        private void AddActivities(IImportResults importResults, IList<Gh625Packet.TrackFileSection> trackSections)
        {
            DateTime pointTime = DateTime.MinValue;
            IActivity activity = null;
            IList<IActivity> allActivities = new List<IActivity>();
            IList<IActivity> activitiesWithHeartRate = new List<IActivity>();
            IList<Gh625Packet.Lap> lastLaps = null;
            foreach (Gh625Packet.TrackFileSection section in trackSections)
            {
                if (section.LapCount > 0)
                {
                    lastLaps = section.Laps;
                }
                else
                {
                    if (section.StartPointIndex == 0)
                    {
                        pointTime = section.StartTime.ToUniversalTime().AddHours(configInfo.HoursAdjustment);
                        activity = importResults.AddActivity(pointTime);
                        activity.Metadata.Source = string.Format(CommonResources.Text.Devices.ImportJob_ActivityImportSource, sourceDescription);
                        activity.TotalTimeEntered = section.TotalTime;
                        activity.TotalDistanceMetersEntered = section.TotalDistanceMeters;
                        activity.TotalCalories = section.TotalCalories;
                        activity.AverageHeartRatePerMinuteEntered = section.AverageHeartRate;
                        activity.MaximumCadencePerMinuteEntered = section.MaximumHeartRate;
                        activity.GPSRoute = new GPSRoute();
                        activity.HeartRatePerMinuteTrack = new NumericTimeDataSeries();

                        if (lastLaps != null)
                        {
                            DateTime lapTime = activity.StartTime;
                            foreach (Gh625Packet.Lap lapPacket in lastLaps)
                            {
                                ILapInfo lap = activity.Laps.Add(lapTime, lapPacket.LapTime);
                                lap.TotalDistanceMeters = lapPacket.LapDistanceMeters;
                                lap.TotalCalories = lapPacket.LapCalories;
                                lap.AverageHeartRatePerMinute = lapPacket.AverageHeartRate;
                                lapTime = lapTime.Add(lapPacket.LapTime);
                            }
                            lastLaps = null;
                        }
                    }

                    if (activity != null)
                    {
                        bool foundHrPoint = false;
                        foreach (Gh625Packet.TrackPoint point in section.TrackPoints)
                        {
                            pointTime = pointTime.AddSeconds((double)point.IntervalTime / 10);

                            activity.HeartRatePerMinuteTrack.Add(pointTime, point.HeartRate);
                            if (point.HeartRate > 0)
                            {
                                foundHrPoint = true;
                            }
                            float latitude = (float)((double)point.Latitude / 1000000);
                            float longitude = (float)((double)point.Longitude / 1000000);
                            float elevation = point.Altitude;
                            activity.GPSRoute.Add(pointTime, new GPSPoint(latitude, longitude, elevation));
                        }
                        if (foundHrPoint && !activitiesWithHeartRate.Contains(activity))
                        {
                            activitiesWithHeartRate.Add(activity);
                        }
                    }
                }
            }
            foreach (IActivity hrActivity in allActivities)
            {
                if (!activitiesWithHeartRate.Contains(hrActivity))
                {
                    hrActivity.HeartRatePerMinuteTrack = null;
                }
            }
        }

        private string sourceDescription;
        private DeviceConfigurationInfo configInfo;
        private IJobMonitor monitor;
        private IImportResults importResults;
    }
}
