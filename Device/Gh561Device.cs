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
// Author: Gerhard Olsson


using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh561Device : GlobalsatProtocol
    {
        public Gh561Device(FitnessDevice_GH561 fitnessDevice)
            : base(fitnessDevice)
        {
        }

        //The 561 does only support waypoints and routes
        public override ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            monitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return null;
        }

        public override int SendTrack(IList<GhPacketBase.Train> trains, IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return -1;
        }
        //Called in SendTrack(), not really needed to set to exception
        public override IList<GlobalsatPacket> SendTrackPackets(GhPacketBase.Train train) { throw new FeatureNotSupportedException(); }

        //Implementation do not work, only transfers waypoints in route, but route is not registered
        public override int SendRoute(IList<GlobalsatRoute> routes, IJobMonitor jobMonitor)
        {
            jobMonitor.ErrorText = ZoneFiveSoftware.SportTracks.Device.Globalsat.Properties.Resources.Device_Unsupported;
            return -1;
        }
    }
}
