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
    class ImportJob_GH625 : ImportJob
    {
        public ImportJob_GH625(GlobalsatProtocol device, string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        : base(device, sourceDescription, monitor, importResults)
        {
        }

        public override bool Import()
        {
            bool result = false;
            try
            {
                Gh625Device device = (Gh625Device)this.device;
                if (device.Open())
                {
                    IList<Gh625Packet.TrackFileHeader625M> headers = device.ReadTrackHeaders(monitor);
                    List<Gh625Packet.TrackFileHeader625M> fetch = new List<Gh625Packet.TrackFileHeader625M>();

                    if (device.FitnessDevice.configInfo.ImportOnlyNew && Plugin.Instance.Application != null && Plugin.Instance.Application.Logbook != null)
                    {
                        IDictionary<DateTime, IList<Gh625Packet.TrackFileHeader625M>> headersByStart = new Dictionary<DateTime, IList<Gh625Packet.TrackFileHeader625M>>();
                        foreach (Gh625Packet.TrackFileHeader625M header in headers)
                        {
                            DateTime start = header.StartTime;
                            if (!headersByStart.ContainsKey(start))
                            {
                                headersByStart.Add(start, new List<Gh625Packet.TrackFileHeader625M>());
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
                        foreach (IList<Gh625Packet.TrackFileHeader625M> dateHeaders in headersByStart.Values)
                        {
                            fetch.AddRange(dateHeaders);
                        }
                    }
                    else
                    {
                        fetch.AddRange(headers);
                    }

                    IList<Gh625Packet.TrackFileSection625M> sections = device.ReadTracks(fetch, monitor);
                    AddActivities(importResults, sections, device.FitnessDevice.configInfo.ImportSpeedDistanceTrack);
                }
                result = true;
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
                device.NoCommunicationError(monitor);
            }
            return result;
        }


        private void AddActivities(IImportResults importResults, IList<Gh625Packet.TrackFileSection625M> trackSections, bool importSpeedTrackAsDistance)
        {
            DateTime pointTime = DateTime.MinValue;
            IActivity activity = null;
            IList<IActivity> allActivities = new List<IActivity>();
            IList<IActivity> activitiesWithHeartRate = new List<IActivity>();
            IList<Gh625Packet.Lap> lastLaps = null;
            float pointDist = 0;
						
            foreach (Gh625Packet.TrackFileSection625M section in trackSections)
            {
                if (section.LapCount > 0)
                {
                    lastLaps = section.Laps;
                }
                else
                {
                    if (section.StartPointIndex == 0)
                    {
                        pointTime = section.StartTime;
                        activity = importResults.AddActivity(pointTime);
                        allActivities.Add(activity);
                        activity.Metadata.Source = string.Format(CommonResources.Text.Devices.ImportJob_ActivityImportSource, sourceDescription);
                        activity.TotalTimeEntered = section.TotalTime;
                        activity.TotalDistanceMetersEntered = section.TotalDistanceMeters;
                        activity.TotalCalories = section.TotalCalories;
                        activity.AverageHeartRatePerMinuteEntered = section.AverageHeartRate;
                        activity.MaximumCadencePerMinuteEntered = section.MaximumHeartRate;
                        activity.GPSRoute = new GPSRoute();
                        activity.HeartRatePerMinuteTrack = new NumericTimeDataSeries();
                        activity.DistanceMetersTrack = new DistanceDataTrack();
                        pointDist = 0;

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
                            double time = point.IntervalTime;
                            float dist = (float)(point.Speed * time);
                            pointDist += dist;
                            pointTime = pointTime.AddSeconds(time);

                            activity.HeartRatePerMinuteTrack.Add(pointTime, point.HeartRate);
                            if (point.HeartRate > 0)
                            {
								foundHrPoint = true;
                            }

							activity.DistanceMetersTrack.Add(pointTime, pointDist);
                            activity.GPSRoute.Add(pointTime, new GPSPoint((float)point.Latitude, (float)point.Longitude, point.Altitude));
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
                if (hrActivity.DistanceMetersTrack.Max <= 0 || !importSpeedTrackAsDistance)
                {
                    hrActivity.DistanceMetersTrack = null;
                }
                if (!activitiesWithHeartRate.Contains(hrActivity))
                {
                    hrActivity.HeartRatePerMinuteTrack = null;
                }
            }
        }
    }


}
