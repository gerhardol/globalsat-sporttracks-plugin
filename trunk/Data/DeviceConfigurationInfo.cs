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
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class DeviceConfigurationInfo
    {
        public static DeviceConfigurationInfo Parse(DeviceConfigurationInfo configInfo, string configurationInfo)
        {
            if (configurationInfo != null)
            {
                try
                {
                    string[] configurationParams = configurationInfo.Split(';');
                    foreach (string configurationParam in configurationParams)
                    {
                        string[] parts = configurationParam.Split('=');
                        if (parts.Length == 2)
                        {
                            switch (parts[0])
                            {
                                case xmlTags.ImportOnlyNew:
                                    configInfo.ImportOnlyNew = (parts[1] == "1");
                                    break;
                                case xmlTags.HoursAdjustment:
                                    configInfo.HoursAdjustment = float.Parse(parts[1]);
                                    break;
                                case xmlTags.SecondsAlwaysImport:
                                    configInfo.SecondsAlwaysImport = int.Parse(parts[1]);
                                    break;
                                case xmlTags.ComPortsText:
                                    configInfo.ComPortsText = parts[1];
                                    break;
                                case xmlTags.BaudRatesText:
                                    configInfo.BaudRatesText = parts[1];
                                    break;
                                case xmlTags.AllowedIdsText:
                                    configInfo.AllowedIdsText = parts[1];
                                    break;
                                case xmlTags.ImportSpeedTrack:
                                    configInfo.ImportSpeedTrack = (parts[1] == "1");
                                    break;
                                case xmlTags.Verbose:
                                    configInfo.Verbose = int.Parse(parts[1]);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch() {}
            }
            return configInfo;
        }

        public DeviceConfigurationInfo(IList<string> allowedIds, IList<int> baudRates)
        {
            BaudRates = baudRates;
            AllowedIds = allowedIds;
        }

        public override string ToString()
        {
            return xmlTags.ImportOnlyNew + "=" + (ImportOnlyNew ? "1" : "0") +
                ";" + xmlTags.HoursAdjustment + "=" + HoursAdjustment.ToString() +
                ";" + xmlTags.SecondsAlwaysImport + "=" + SecondsAlwaysImport.ToString() +
                ";" + xmlTags.ComPortsText + "=" + this.ComPortsText +
                ";" + xmlTags.BaudRatesText + "=" + this.BaudRatesText +
                ";" + xmlTags.AllowedIdsText + "=" + this.AllowedIdsText +
            ";" + xmlTags.ImportSpeedTrack + "=" + this.ImportSpeedTrack +
           ";" + xmlTags.Verbose + "=" + this.Verbose;
        }

        private static class xmlTags
        {
            public const string ImportOnlyNew = "newonly";
            public const string HoursAdjustment = "hr";
            public const string SecondsAlwaysImport = "SecondsAlwaysImport";
            public const string ComPortsText = "comports";
            public const string BaudRatesText = "baudrates";
            public const string AllowedIdsText = "allowedids";
            public const string ImportSpeedTrack = "ImportSpeedTrack";
            public const string Verbose = "Verbose";
        }
        public int MaxPacketPayload = 2500;
        public int MaxNrWaypoints = 100;
        public IList<int> BaudRates = new List<int>();
        //Also used for naming families - first should be readable (null is Globalsat)
        public IList<string> AllowedIds = new List<string>();
        public bool ImportOnlyNew = true;
        public bool ImportSpeedTrack = false;
        public int SecondsAlwaysImport = 0;
        public float HoursAdjustment = 0;
        public IList<string> ComPorts = new List<string>();
        public int Verbose = 1;

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
                string[] ports = value.Split(',');
                if (ports.Length > 0)
                {
                    this.ComPorts = new List<string>();
                }
                foreach (string port in ports)
                {
                    string port2 = port.Trim();
                    if (!string.IsNullOrEmpty(port2) && !ComPorts.Contains(port2))
                    {
                        ComPorts.Add(port2);
                    }
                }
            }
        }

        public string BaudRatesText
        {
            get
            {
                string r = "";
                if (BaudRates != null)
                {
                    const string sep = ", ";
                    foreach (int s in BaudRates)
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
                string[] ids = value.Split(',');
                if (ids.Length > 0)
                {
                    this.BaudRates = new List<int>();
                }
                foreach (string port in ids)
                {
                    int port2 = int.Parse(port);
                    if (port2 > 0 && !BaudRates.Contains(port2))
                    {
                       BaudRates.Add(port2);
                    }
                }
            }
        }

        public string AllowedIdsText
        {
            get
            {
                string r = "";
                if (AllowedIds != null)
                {
                    const string sep = ", ";
                    foreach (string s in AllowedIds)
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
                this.AllowedIds = new List<string>();
                string[] ids = value.Split(',');
                if (ids.Length > 0)
                {
                    this.AllowedIds = new List<string>();
                }
                foreach (string port in ids)
                {
                    string port2 = port.Trim();
                    if (!string.IsNullOrEmpty(port2) && !AllowedIds.Contains(port2))
                    {
                        AllowedIds.Add(port2);
                    }
                }
            }
        }
    }
}
