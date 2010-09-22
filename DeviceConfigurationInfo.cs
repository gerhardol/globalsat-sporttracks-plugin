// Copyright (C) 2010 Zone Five Software
// Author: Aaron Averill
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class DeviceConfigurationInfo
    {
        public static DeviceConfigurationInfo Parse(string configurationInfo)
        {
            DeviceConfigurationInfo configInfo = new DeviceConfigurationInfo();
            if (configurationInfo != null)
            {
                string[] configurationParams = configurationInfo.Split(';');
                foreach (string configurationParam in configurationParams)
                {
                    string[] parts = configurationParam.Split('=');
                    if (parts.Length == 2)
                    {
                        switch (parts[0])
                        {
                            case "newonly":
                                configInfo.ImportOnlyNew = parts[1] == "1";
                                break;
                            case "hr":
                                configInfo.HoursAdjustment = int.Parse(parts[1]);
                                break;
                        }
                    }
                }
            }
            return configInfo;
        }

        private DeviceConfigurationInfo()
        {
        }

        public override string ToString()
        {
            return "newonly=" + (ImportOnlyNew ? "1" : "0") +
                ";hr=" + HoursAdjustment.ToString();
        }

        public bool ImportOnlyNew = true;
        public int HoursAdjustment = 0;
    }
}
