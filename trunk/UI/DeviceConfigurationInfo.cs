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
                            case "comport":
                                configInfo.ComPortsText = parts[1]; 
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
                ";hr=" + HoursAdjustment.ToString() +
                ";comport=" + this.ComPortsText;
        }

        public bool ImportOnlyNew = true;
        public int HoursAdjustment = 0;
        public IList<string> ComPorts = null;
        public string ComPortsText
        {
            get
            {
                string r = "";
                if (ComPorts != null)
                {
                    const string sep = ", ";
                    foreach (string s in ComPorts)
                    {
                        r += s + sep;
                    }
                    if (r.EndsWith(sep))
                    {
                        r = r.Remove(r.Length - sep.Length);
                    }
                }
                return r;
            }
            set
            {
                this.ComPorts = new List<string>();
                string[] ports = value.Split(',');
                foreach (string port in ports)
                {
                    string port2 = port.Trim();
                    if (!string.IsNullOrEmpty(port2))
                    {
                        ComPorts.Add(port2);
                    }
                }
            }
        }
    }
}
