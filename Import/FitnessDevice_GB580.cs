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
    class FitnessDevice_GB580 : IFitnessDevice
    {
        public FitnessDevice_GB580()
        {
            this.id = new Guid("72de21ef-7810-46b8-a01c-d2449ac0bc76");
            this.image = Properties.Resources.Image_48_GB580;
            this.name = "Globalsat - GB-580";
        }

        public Guid Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public Image Image
        {
            get { return image; }
        }

        public string ConfiguredDescription(string configurationInfo)
        {
            return Name;
        }

        public string Configure(string configurationInfo)
        {
            DeviceConfigurationDlg dialog = new DeviceConfigurationDlg();
            DeviceConfigurationInfo configInfo = DeviceConfigurationInfo.Parse(configurationInfo);
            dialog.ConfigurationInfo = configInfo;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.ConfigurationInfo.ToString();
            }
            else
            {
                return null;
            }
        }

        public bool Import(string configurationInfo, IJobMonitor monitor, IImportResults importResults)
        {
            ImportJob_GB580 job = new ImportJob_GB580(ConfiguredDescription(configurationInfo), DeviceConfigurationInfo.Parse(configurationInfo), monitor, importResults);
            return job.Import();
        }

        private Guid id;
        private Image image;
        private string name;
    }
}
