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
    class FitnessDevice_SpoQ : FitnessDevice_GH625XT
    {
        public FitnessDevice_SpoQ()
            : base()
        {
            this.id = new Guid("604a8e62-337c-4425-9302-55f7be0b0ca0");
            this.image = Properties.Resources.Image_48_a_rival_spoq;
            this.name = "a-rival - SpoQ";
            this.device = new Gh625XTDevice(this);
            //The Spoq 100 has standard GH-625XT firmware
            this.configInfo = new DeviceConfigurationInfo(new List<string> { "SQ100", "GH-625XT" }, new List<int> { 115200 });
            this.GetConfigurationString(); //Set configuration from Preferences
        }
    }
}
