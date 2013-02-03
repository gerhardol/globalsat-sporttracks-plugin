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
    class FitnessDevice_GB580 : FitnessDevice_Globalsat
    {
        public FitnessDevice_GB580()
            : base()
        {
            this.id = new Guid("72de21ef-7810-46b8-a01c-d2449ac0bc76");
            this.image = Properties.Resources.Image_48_GB580;
            this.name = "Globalsat - GB-580";
            this.device = new Gb580Device(this);
            this.configInfo = new DeviceConfigurationInfo(new List<string> { "GB-580", "GB-580P" }, new List<int> { 115200 });
            this.GetConfigurationString(); //Set configuration from Preferences
        }

        public override GlobalsatPacket PacketFactory { get { return new Gb580Packet(this); } }

        //Note: Device seem slower to respond than other, also 625M

        //The GB-580B has no barometer, unsure if 580F is ever reported
        //Do not use preferences here, it may have changed since recording
        public override bool HasElevationTrack { get { if (this.Device().devId == "GB-580P" || this.Device().devId == "GB-580F") { return true; } else { return false; } } }
        public override bool CanRotateScreen { get { return true; } }
        public override System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 128); } }
        public override int TotalPoints { get { return 52416; } }
        public override int PointsInBlock { get { return 126; } }
    }
}
