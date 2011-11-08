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
    class FitnessDevice_GH615 : FitnessDevice_Globalsat
    {
        public FitnessDevice_GH615()
        {
            this.id = new Guid("507979fb-304e-48cf-8cae-d35b55138485");
            this.image = Properties.Resources.Image_48_GH615;
            this.name = "Globalsat - GH615";
        }

        public override bool Import(string configurationInfo, IJobMonitor monitor, IImportResults importResults)
        {
            Gh615Device device = new Gh615Device(configurationInfo);
            ImportJob job = device.ImportJob(ConfiguredDescription(configurationInfo), monitor, importResults);
            return job.Import();
        }
    }
}
