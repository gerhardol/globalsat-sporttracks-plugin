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

    public class GlobalsatWaypointsImportExport
    {
        public static Stream Import(IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return null; }
            Stream result = KeymazePlugin.IO.ExportWaypoints.ExportGpxWaypointsStream(device2.GetWaypoints(jobMonitor));
            return result;
        }

        public static int Export(Stream waypoints, IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return 0; }
            return device2.SendWaypoints(KeymazePlugin.IO.ImportWaypoints.ImportStreamGpxWaypoints(waypoints), jobMonitor);
        }

        public static void Delete(Stream waypoints, IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return; }
            device2.DeleteWaypoints(KeymazePlugin.IO.ImportWaypoints.ImportStreamGpxWaypoints(waypoints), jobMonitor);
            jobMonitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_ImportComplete;
        }

        public static void DeleteAll(IJobMonitor jobMonitor)
        {
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 == null) { return; }
            device2.DeleteAllWaypoints(jobMonitor);
        }
    }
}
