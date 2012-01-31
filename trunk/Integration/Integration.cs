/*
Copyright (C) 2011 Gerhard Olsson

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

using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

using ZoneFiveSoftware.SportTracks.Device.Globalsat;

namespace GlobalsatDevicePlugin
{
    public interface IWaypointImportExport
    {
        /** returns a stream with GPX XML data, null in case of errors; progress reporting, status updates and error reporting use the jobMonitor */
        Stream Import(IJobMonitor jobMonitor);
        /** returns true when successful, false when an error has occured */
        bool Export(Stream gpxDataStream, IJobMonitor jobMonitor);
        bool Delete(Stream gpxDataStream, IJobMonitor jobMonitor);
    }

    public class GlobalsatDeviceImportExport
    {
        public static Stream ImportWpt(IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return null; }
            Stream result = WaypointsPlugin.IO.ExportWaypoints.ExportGpxWaypointsStream(device2.GetWaypoints(jobMonitor));
            return result;
        }

        public static int ExportWpt(Stream waypoints, IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return 0; }
            return device2.SendWaypoints(WaypointsPlugin.IO.ImportWaypoints.ImportStreamGpxWaypoints(waypoints), jobMonitor);
        }

        public static void DeleteWpt(Stream waypoints, IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return; }
            device2.DeleteWaypoints(WaypointsPlugin.IO.ImportWaypoints.ImportStreamGpxWaypoints(waypoints), jobMonitor);
            jobMonitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_ImportComplete;
        }

        public static void DeleteAllWpt(IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return; }
            device2.DeleteAllWaypoints(jobMonitor);
        }

        public static Stream ImportRte(IJobMonitor jobMonitor)
        {
            throw new Exception();
            //GenericDevice device = new GenericDevice();
            //GlobalsatProtocol device2 = device.Device(jobMonitor);
            //if (device2 == null) { return null; }
            //Stream result = KeymazePlugin.IO.ExportRoutes.ExportGpxRoutesStream(device2.GetRoutes(jobMonitor));
            //return result;
        }

        public static int ExportRte(Stream routes, IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return 0; }
            return device2.SendRoute(WaypointsPlugin.IO.ImportRoutes.ImportStreamGpxRoutes(routes), jobMonitor);
        }

        public static void DeleteRte(Stream waypoints, IJobMonitor jobMonitor)
        {
            throw new Exception();
        }

        //Get the data in a generic Globalsat format, to separate ST
        public static IList<GhPacketBase.Train> ToGlobTrack(IList<IActivity> activities)
        {
            IList<GhPacketBase.Train> result = new List<GhPacketBase.Train>();

            foreach (IActivity activity in activities)
            {
                IGPSRoute gpsRoute = activity.GPSRoute;
                GhPacketBase.Train train = new GhPacketBase.Train();
                train.StartTime = activity.StartTime;
                train.TotalTime = TimeSpan.FromSeconds(gpsRoute.TotalElapsedSeconds);
                train.TotalDistanceMeters = (Int32)Math.Round(gpsRoute.TotalDistanceMeters);
                train.LapCount = 1;

                train.TotalCalories = (Int16)activity.TotalCalories;
                if (train.MaximumSpeed == 0 && train.TotalTime.TotalSeconds >= 1)
                {
                    //Better than 0(?) - Info() could be used
                    train.MaximumSpeed = train.TotalDistanceMeters / train.TotalTime.TotalSeconds;
                }
                train.MaximumHeartRate = (byte)activity.MaximumHeartRatePerMinuteEntered;
                train.AverageHeartRate = (byte)activity.AverageHeartRatePerMinuteEntered;
                train.TotalAscend = (Int16)activity.TotalAscendMetersEntered;
                train.TotalDescend = (Int16)activity.TotalDescendMetersEntered;
                train.AverageCadence = (Int16)activity.AverageCadencePerMinuteEntered;
                train.MaximumCadence = (Int16)activity.MaximumCadencePerMinuteEntered;
                train.AveragePower = (Int16)activity.AveragePowerWattsEntered;
                train.MaximumPower = (Int16)activity.MaximumPowerWattsEntered;
                //Some protocols send laps in separate header
                //Use common code instead of overiding this method

                //Laps are not implemented when sending, one lap hardcoded

                for (int j = 0; j < gpsRoute.Count; j++)
                {
                    IGPSPoint point = gpsRoute[j].Value;
                    GhPacketBase.TrackPoint trackpoint = new GhPacketBase.TrackPoint();
                    trackpoint.Latitude = point.LatitudeDegrees;
                    trackpoint.Longitude = point.LongitudeDegrees;
                    trackpoint.Altitude = (Int32)point.ElevationMeters;
                    if (j == 0)
                    {
                        trackpoint.IntervalTime = 0;
                    }
                    else
                    {
                        uint intTime = gpsRoute[j].ElapsedSeconds - gpsRoute[j - 1].ElapsedSeconds;
                        float dist = gpsRoute[j].Value.DistanceMetersToPoint(gpsRoute[j - 1].Value);
                        if (intTime > 0)
                        {
                            trackpoint.IntervalTime = intTime;
                            trackpoint.Speed = dist / intTime;
                        }
                        else
                        {
                            //Time is not really used - could probably be empty
                            //The alternative would be to drop the points
                            trackpoint.IntervalTime = 1;
                        }
                    }
                    train.TrackPoints.Add(trackpoint);
                }
                train.TrackPointCount = (short)train.TrackPoints.Count;

                result.Add(train);
            }

            return result;
        }

        public static int ExportAct(IList<IActivity> activities, IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return 0; }
            return device2.SendTrack(ToGlobTrack(activities), jobMonitor);
        }
    }
}
