/*
Copyright (C) 2012 Gerhard Olsson

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

using System;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
	static class Settings
    {
        //Device specific information, but plugin cannot update saved info spontaneous easily
        private static IDictionary<string, IList<string>> lastValidComPorts = null;
        //Set a easy to read name for preferences
        private const string GenericDeviceName = "Generic";

        public static IList<string> GetLastValidComPorts(string Name)
        {
            if (lastValidComPorts != null && lastValidComPorts.ContainsKey(Name) && lastValidComPorts[Name] != null && lastValidComPorts[Name].Count > 0)
            {
                return lastValidComPorts[Name];
            }
            return new List<string>();
        }

        public static void SetLastValidComPort(string Name, string val)
        {
            if (lastValidComPorts != null)
            {
                if (!lastValidComPorts.ContainsKey(Name) || lastValidComPorts[Name] == null)
                {
                    lastValidComPorts[Name] = new List<string>();
                }
                if (lastValidComPorts[Name].Contains(val))
                {
                    //Make sure the most recent is first only
                    lastValidComPorts[Name].Remove(val);
                }
                lastValidComPorts[Name].Insert(0, val);
                if (!Name.Equals(""))
                {
                    SetLastValidComPort("", val);
                }
            }
        }

        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            try
            {
                String attr;
                attr = pluginNode.GetAttribute(xmlTags.LastValidComPorts);
                lastValidComPorts = new Dictionary<string, IList<string>>();
                if (attr.Length > 0)
                {
                    String[] values = attr.Split(';');
                    foreach (String column in values)
                    {
                        String[] keypairs = column.Split('=');
                        if (keypairs.Length >= 2)
                        {
                            string name = keypairs[0];
                            if (GenericDeviceName.Equals(name))
                            {
                                //Use empty name in the plugin
                                name = "";
                            }
                            lastValidComPorts[name] = new List<string>();
                            String[] ports = keypairs[1].Split(',');
                            foreach (string port in ports)
                            {
                                if (!string.IsNullOrEmpty(port))
                                {
                                    lastValidComPorts[name].Add(port);
                                }
                            }
                        }
                    }
                }
            }
            catch{}
        }

        public static void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            string s = "";
            if (lastValidComPorts != null)
            {
                foreach (KeyValuePair<string, IList<string>> kv in lastValidComPorts)
                {
                    string name = kv.Key;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = GenericDeviceName;
                    }
                    s += string.Format("{0}=", name);
                    foreach (string port in kv.Value)
                    {
                        s += string.Format("{0},", port);
                    }
                    s += ';';
                }
            }
            pluginNode.SetAttribute(xmlTags.LastValidComPorts, s);
        }

        private class xmlTags
        {
            public const string LastValidComPorts = "LastValidComPort";
        }

	}
}
