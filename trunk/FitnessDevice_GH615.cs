// Copyright (C) 2010 Zone Five Software
// Author: Aaron Averill
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class FitnessDevice_GH615 : IFitnessDevice
    {
        public FitnessDevice_GH615()
        {
            this.id = new Guid("507979fb-304e-48cf-8cae-d35b55138485");
            this.image = Properties.Resources.Image_48_GH615;
            this.name = "Globalsat - GH615";
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
            ImportJob_GH615 job = new ImportJob_GH615(ConfiguredDescription(configurationInfo), DeviceConfigurationInfo.Parse(configurationInfo), monitor, importResults);
            return job.Import();
        }

        #region Private members
        private Guid id;
        private Image image;
        private string name;
        #endregion
    }
}
