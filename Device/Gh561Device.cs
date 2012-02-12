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

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh561Device : GlobalsatProtocol
    {
        public Gh561Device() : base() { }
        public Gh561Device(string configInfo) : base(configInfo) { }

        public override DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                DeviceConfigurationInfo info = new DeviceConfigurationInfo(new List<string> { "GH-561" }, new List<int> { 115200 });
                return info;
            }
        }
        //Unknown protocol
        public override IList<GlobalsatPacket> SendTrackPackets(GhPacketBase.Train train) { throw new FeatureNotSupportedException(); }

        public override GlobalsatPacket PacketFactory { get { return new Gh561Packet(); } }
        public override bool BigEndianPacketLength { get { return false; } }
    }
}
