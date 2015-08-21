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
using System.Drawing;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class FitnessDevice_GH625 : FitnessDevice_Globalsat
    {
        public FitnessDevice_GH625()
            : base()
        {
            this.id = new Guid("34625543-d453-4117-965a-866b47078c27");
            this.image = Properties.Resources.Image_48_GH625;
            this.name = "Globalsat - GH625";
            this.device = new Gh625Device(this);
            //TODO: Find valid Id for KeyMaze
            this.configInfo = new DeviceConfigurationInfo(new List<string> { "GH-625M", "GH-625B", "KM" }, new List<int> { 57600 });
            this.GetConfigurationString(); //Set configuration from Preferences
        }

        public override GlobalsatPacket PacketFactory { get { return new Gh625Packet(this); } }

        //Problems accessing some (ageing?) 625, increase
        public override int ReadTimeoutDetect { get { return 8000; } }
        public override int ReadTimeout { get { return 8000; } }
        public override System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(120, 80); } }
        public override int ScreenBpp { get { return 1; } }
    }
}
