/*
Copyright (C) 2010 Zone Five Software

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

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
        {
            this.id = new Guid("0a243ef2-8565-4863-b34b-d2e016038ec2");
            this.image = Properties.Resources.Image_48_GH505;
            this.name = "Globalsat - GH505";
        }

        public override bool Import(string configurationInfo, IJobMonitor monitor, IImportResults importResults)
        {
            Gh505Device device = new Gh505Device(DeviceConfigurationInfo.Parse(DefaultConfig, configurationInfo));
            ImportJob job = device.ImportJob(ConfiguredDescription(configurationInfo), monitor, importResults);
            return job.Import();
        }

        public override DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                DeviceConfigurationInfo info = new DeviceConfigurationInfo();
                info.AllowedIds = new List<string> { "GH-50" };
                return info;
            }
        }
    }
}
