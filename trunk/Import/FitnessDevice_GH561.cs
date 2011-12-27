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
    class FitnessDevice_GH561 : FitnessDevice_Globalsat
    {
        public FitnessDevice_GH561()
        {
            this.id = new Guid("99021a40-f9ba-11e0-be50-0800200c9a66");
            this.image = Properties.Resources.Image_48_GSSPORT;
            this.name = "Globalsat - GH561";
        }

        public override bool Import(string configurationInfo, IJobMonitor monitor, IImportResults importResults)
        {
            //Gh561Device device = new Gh561Device(DeviceConfigurationInfo.Parse(DefaultConfig, configurationInfo));
            //ImportJob job = device.ImportJob(ConfiguredDescription(configurationInfo), monitor, importResults);
            //return job.Import();
            //not implemented
            return false;
        }
    }
}
