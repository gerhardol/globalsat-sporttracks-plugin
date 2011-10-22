/*
Copyright (C) 2011 Gerhard Olsson

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

using System;
using System.IO;
using System.Collections.Generic;
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
    public class WaypointImportExport : IWaypointImportExport
    {
        public static WaypointImportExport Instance
        {
            get
            {
                //The object is not reused right now, allow the user to reconnect
                //The device itself is fully dynamic in each call
                return new WaypointImportExport();
            }
        }

        public Stream Import(IJobMonitor jobMonitor)
        {
            throw new NotImplementedException();
        }

        public bool Export(Stream gpxDataStream, IJobMonitor jobMonitor)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Stream gpxDataStream, IJobMonitor jobMonitor)
        {
            throw new NotImplementedException();
        }
    }
    public interface IGlobalsatWaypoint
    {
        short Altitude { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        string WaypointName { get; set; }
        int IconNr { get; set; }
    }
    public interface IGlobalsatWaypointsImportExport
    {
        IList<IGlobalsatWaypoint> GetWaypoints();
        int SendWaypoints(IList<IGlobalsatWaypoint> waypoints);
        void DeleteWaypoints(IList<IGlobalsatWaypoint> waypoints);
        void DeleteAllWaypoints();
    }
    public class GlobalsatWaypointsImportExport : IGlobalsatWaypointsImportExport
    {
        public static GlobalsatWaypointsImportExport Instance
        {
            get
            {
                //The object is not reused right now, allow the user to reconnect
                //The device itself is fully dynamic in each call
                return new GlobalsatWaypointsImportExport();
            }
        }

        public IList<IGlobalsatWaypoint> GetWaypoints()
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device();
            if (device2 == null) { return null; }
            IList<IGlobalsatWaypoint> result = GlobalsatWaypoint.GetIWaypoints(device2.GetWaypoints());
            return result;
        }

        public int SendWaypoints(IList<IGlobalsatWaypoint> waypoints)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device();
            if (device2 == null) { return 0; }
            return device2.SendWaypoints(GlobalsatWaypoint.GetWaypoints(waypoints));
        }

        public void DeleteWaypoints(IList<IGlobalsatWaypoint> waypoints)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device();
            if (device2 == null) { return; }
            device2.DeleteWaypoints(GlobalsatWaypoint.GetWaypoints(waypoints));
        }

        public void DeleteAllWaypoints()
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device();
            if (device2 == null) { return; }
            device2.DeleteAllWaypoints();
        }
    }
}
