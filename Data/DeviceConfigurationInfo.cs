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
        public DeviceConfigurationInfo(IList<string> allowedIds, IList<int> baudRates)
        {
            BaudRates = baudRates;
            AllowedIds = allowedIds;
        }

        public void Copy(DeviceConfigurationInfo c)
        {
            this.HoursAdjustment = c.HoursAdjustment;
            this.ImportOnlyNew = c.ImportOnlyNew;
            this.DetectPauses = c.DetectPauses;
        }

        //Parse string overlay default configuration
        public void Parse(string configurationInfo)
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
                                    this.ImportOnlyNew = (parts[1] == "1");
                                    break;
                                case xmlTags.HoursAdjustment:
                                    this.HoursAdjustment = float.Parse(parts[1]);
                                    break;
                                case xmlTags.SecondsAlwaysImport:
                                    this.SecondsAlwaysImport = int.Parse(parts[1]);
                                    break;
                                case xmlTags.ComPortsText:
                                    this.ComPortsText = parts[1];
                                    break;
                                case xmlTags.BaudRatesText:
                                    this.BaudRatesText = parts[1];
                                    break;
                                case xmlTags.AllowedIdsText:
                                    this.AllowedIdsText = parts[1];
                                    break;
                                case xmlTags.ImportSpeedDistanceTrack:
                                    this.ImportSpeedDistanceTrack = (parts[1] == "1");
                                    break;
                                case xmlTags.DetectPausesFromSpeedTrack:
                                    if (parts[1] == "1")
                                    {
                                        this.DetectPauses = 1;
                                    }
                                    break;
                                case xmlTags.DetectPauses:
                                    this.DetectPauses = int.Parse(parts[1]);
                                    break;
                                case xmlTags.LastValidComPorts:
                                    lastValidComPorts = new List<string>();
                                    String[] ports = parts[1].Split(',');
                                    foreach (string port in ports)
                                    {
                                        if (!string.IsNullOrEmpty(port))
                                        {
                                            lastValidComPorts.Add(port);
                                        }
                                    }
                                    break;
                                case xmlTags.ScanComPorts:
                                    this.ScanComPorts = (parts[1] != "0");
                                    break;
                                case xmlTags.ReadTimeout:
                                    this.ReadTimeout = int.Parse(parts[1]);
                                    break;
                                case xmlTags.Verbose:
                                    this.Verbose = int.Parse(parts[1]);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                catch {}
            }
        }

        public override string ToString()
        {
            string s = xmlTags.ImportOnlyNew + "=" + (ImportOnlyNew ? "1" : "0") +
                ";" + xmlTags.HoursAdjustment + "=" + HoursAdjustment.ToString() +
                ";" + xmlTags.SecondsAlwaysImport + "=" + SecondsAlwaysImport.ToString() +
                ";" + xmlTags.ComPortsText + "=" + this.ComPortsText +
                ";" + xmlTags.BaudRatesText + "=" + this.BaudRatesText +
                ";" + xmlTags.AllowedIdsText + "=" + this.AllowedIdsText +
                ";" + xmlTags.ImportSpeedDistanceTrack + "=" + (this.ImportSpeedDistanceTrack ? "1" : "0") +
                ";" + xmlTags.DetectPauses + "=" + this.DetectPauses +
                ";" + xmlTags.LastValidComPorts + "=" + this.LastComPortsText() +
                ";" + xmlTags.ScanComPorts + "=" + (this.ScanComPorts ? "1" : "0") +
                ";" + xmlTags.ReadTimeout + "=" + this.ReadTimeout.ToString() +
                ";" + xmlTags.Verbose + "=" + this.Verbose;
            return s;
        }

        private static class xmlTags
        {
            public const string ImportOnlyNew = "newonly";
            public const string HoursAdjustment = "hr";
            public const string SecondsAlwaysImport = "SecondsAlwaysImport";
            public const string ComPortsText = "comports";
            public const string BaudRatesText = "baudrates";
            public const string AllowedIdsText = "allowedids";
            public const string ImportSpeedDistanceTrack = "ImportSpeedDistanceTrack";
            public const string DetectPausesFromSpeedTrack = "DetectPausesFromSpeedTrack"; //legacy parameter for migration from configurations to new
            public const string DetectPauses = "DetectPauses";
            public const string LastValidComPorts = "LastValidComPorts";
            public const string ScanComPorts = "ScanComPorts";
            public const string ReadTimeout = "ReadTimeout";
            public const string Verbose = "Verbose";
        }
        public int MaxPacketPayload = 2500;
        public int MaxNrWaypoints = 100;
        public IList<int> BaudRates = new List<int>();
        //Also used for naming families - first should be readable (null is Globalsat)
        public IList<string> AllowedIds = new List<string>();
        public bool ImportOnlyNew = true;
        public bool ImportSpeedDistanceTrack = false;
        public int DetectPauses;
        public int SecondsAlwaysImport = 0;
        public float HoursAdjustment = 0;
        public IList<string> ComPorts = new List<string>();
        //Allow override scanning of com ports (just use lastValidComPorts)
        public bool ScanComPorts = true;
        //Allow override ReadTimeouts (0 is no override)
        public int ReadTimeout = 0;
        public int Verbose = 10;
        private IList<string> lastValidComPorts = null;

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
                if (this.AllowedIds == null)
                {
                    this.AllowedIds = new List<string>();
                }
                string[] ids = value.Split(',');
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

        private string LastComPortsText()
        {
            string s = "";
            if (this.lastValidComPorts != null)
            {
                foreach (string port in lastValidComPorts)
                {
                    s += string.Format("{0},", port);
                }
            }
            return s;
        }

        public IList<string> GetLastValidComPorts()
        {
            if (lastValidComPorts != null)
            {
                return lastValidComPorts;
            }
            return new List<string>();
        }

        public void SetLastValidComPort(string val)
        {
            if (lastValidComPorts == null)
            {
                lastValidComPorts = new List<string>();
            }
            if (lastValidComPorts.Contains(val))
            {
                //Make sure the most recent is first only
                lastValidComPorts.Remove(val);
            }
            lastValidComPorts.Insert(0, val);
        }

        public bool DynamicInfoChanged(string s)
        {
            bool res = false;
            DeviceConfigurationInfo d = new DeviceConfigurationInfo(new List<string>(), new List<int>());
            d.Parse(s);

            if (this.lastValidComPorts == null)
            {
                if (d.lastValidComPorts != null)
                {
                    res = true;
                }
            }
            else if (d.lastValidComPorts == null)
            {
                res = false;
            }
            else if (this.lastValidComPorts.Count != d.lastValidComPorts.Count)
            {
                res = true;
            }
            else
            {
                for (int i = 0; i < this.lastValidComPorts.Count; i++)
                {
                    if(!this.lastValidComPorts[i].Equals(d.lastValidComPorts[i]))
                    {
                        res = true;
                        break;
                    }
                }
            }
            return res;
        }
    }
}
