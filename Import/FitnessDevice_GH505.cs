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
    class FitnessDevice_GH505 : FitnessDevice_Globalsat
    {
        public FitnessDevice_GH505()
            : base()
        {
            this.id = new Guid("0a243ef2-8565-4863-b34b-d2e016038ec2");
            this.image = Properties.Resources.Image_48_GH505;
            this.name = "Globalsat - GH505";
            this.device = new Gh505Device(this);
            this.configInfo = new DeviceConfigurationInfo(new List<string> { "GH-505", "GH-50" }, new List<int> { 115200 });
            this.GetConfigurationString(); //Set configuration from Preferences
        }

        public override GlobalsatPacket PacketFactory { get { return new Gh505Packet(this); } }

        //Note: seem to be a little slow, used to have longer timeout than 625M

        public override int ScreenBpp { get { return 1; } }
        public override bool ScreenRowCol { get { return false; } }
        public override int TotalPoints { get { return 60736; } } //52416?
    }
}
