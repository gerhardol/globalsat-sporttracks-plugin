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

using ZoneFiveSoftware.Common.Visuals;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gb580Device : GlobalsatProtocol2
    {
        public Gb580Device() : base() { }
        public Gb580Device(string configInfo) : base(configInfo) { }

        //Note: Device seem slower to respond than other, also 625M

        public override DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                DeviceConfigurationInfo info = new DeviceConfigurationInfo(new List<string> { "GB-580", "GB-580P" }, new List<int> { 115200 });
                return info;
            }
        }

        public override GlobalsatPacket PacketFactory { get { return new Gb580Packet(this); } }

        public override int SendRoute(IList<GlobalsatRoute> routes, IJobMonitor jobMonitor)
        {
            throw new FeatureNotSupportedException();
        }
        //The GB-580B has no barometer, unsure if 580F is ever reported
        //Do not use preferences here, it may have changed since recording
        public override bool HasElevationTrack { get { if (this.devId == "GB-580P" || this.devId == "GB-580F") { return true; } else { return false; } } }
        public override bool CanRotateScreen { get { return true; } }
        public override System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 128); } }
    }
}
