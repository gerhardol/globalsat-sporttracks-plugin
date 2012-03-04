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

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gb580Device : GlobalsatProtocol2
    {
        public Gb580Device() : base() { }
        public Gb580Device(string configInfo) : base(configInfo) { }

        //Slow?
        public override int ReadTimeout { get { return 2000; } }
        public override int ReadTimeoutDetect { get { return 1000; } }

        public override DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                DeviceConfigurationInfo info = new DeviceConfigurationInfo(new List<string> { "GB-580" }, new List<int> { 115200 });
                return info;
            }
        }

        public override GlobalsatPacket PacketFactory { get { return new Gb580Packet(); } }
    }
}
