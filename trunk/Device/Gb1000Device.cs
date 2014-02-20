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
using System.IO.Ports;
using System.Drawing;

using ZoneFiveSoftware.Common.Visuals;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gb1000Device : GlobalsatProtocol2
    {
        //The Gb1000 is basically a stripped down Gb580, most are not supported

        public Gb1000Device(FitnessDevice_GB1000 fitnessDevice)
            : base(fitnessDevice)
        {
        }
        public override int SendTrack(IList<GhPacketBase.Train> trains, IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return -1;
        }
        public override IList<GlobalsatPacket> SendTrackPackets(GhPacketBase.Train train) { throw new FeatureNotSupportedException(); }
        public override IList<GlobalsatWaypoint> GetWaypoints(IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return null;
        }
        public override int SendWaypoints(IList<GlobalsatWaypoint> waypoints, IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return -1;
        }
        public override bool DeleteWaypoints(IList<GlobalsatWaypoint> waypointNames, IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return false;
        }
        public override bool DeleteAllWaypoints(IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return false;
        }
        public override int SendRoute(IList<GlobalsatRoute> routes, IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return -1;
        }
        public override Bitmap GetScreenshot(IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return null;
        }

    }
}
