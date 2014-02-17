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
    class FitnessDevice_GB1000 : FitnessDevice_Globalsat
    {
        public FitnessDevice_GB1000()
            : base()
        {
            this.id = new Guid("a594d434-12c0-4ef1-a893-f6705c9e23be");
            this.image = Properties.Resources.Image_48_GB1000;
            this.name = "Globalsat - GB-1000";
            this.device = new Gb1000Device(this);
            this.configInfo = new DeviceConfigurationInfo(new List<string> { "GB-1000" }, new List<int> { 115200 });
            this.GetConfigurationString(); //Set configuration from Preferences
        }

        //Assume packet format is the same as GB580 for now
        public override GlobalsatPacket PacketFactory { get { return new Gb580Packet(this); } }

        //public override bool HasElevationTrack { get { if (this.Device().devId.EndsWith("580P") || this.Device().devId.EndsWith("580F")) { return true; } else { return false; } } }
        public override bool CanRotateScreen { get { return true; } }
        public override System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 128); } }
        public override int TotalPoints { get { return 52416; } }
        public override int PointsInBlock { get { return 126; } }
    }
}
