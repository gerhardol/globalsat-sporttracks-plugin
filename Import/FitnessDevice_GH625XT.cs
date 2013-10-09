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
    class FitnessDevice_GH625XT : FitnessDevice_Globalsat
    {
        public FitnessDevice_GH625XT()
            : base()
        {
            this.id = new Guid("53b374a0-f35e-11e0-be50-0800200c9a66");
            this.image = Properties.Resources.Image_48_GH625XT;
            this.name = "Globalsat - GH625XT";
            this.device = new Gh625XTDevice(this);
            this.configInfo = new DeviceConfigurationInfo(new List<string> { "GH-625XT" }, new List<int> { 115200 });
            this.GetConfigurationString(); //Set configuration from Preferences
        }

        public override GlobalsatPacket PacketFactory { get { return new Gh625XTPacket(this); } }

        //Timeout when detecting - 625XT seem to be faster than other models (used to be 100ms)
        public override int ReadTimeoutDetect { get { return 1000; } }
        public override int TotalPoints { get { return 121472; } }
        public override int MaxNoRoutePoints { get { return 200; } }
    }
}
