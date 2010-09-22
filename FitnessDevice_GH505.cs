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
    class FitnessDevice_GH505 : IFitnessDevice
    {
        public FitnessDevice_GH505()
        {
            this.id = new Guid("0a243ef2-8565-4863-b34b-d2e016038ec2");
            this.image = Properties.Resources.Image_48_GH505;
            this.name = "Globalsat - GH505";
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
            ImportJob_GH505 job = new ImportJob_GH505(ConfiguredDescription(configurationInfo), DeviceConfigurationInfo.Parse(configurationInfo), monitor, importResults);
            return job.Import();
        }

        #region Private members
        private Guid id;
        private Image image;
        private string name;
        #endregion
    }
}
