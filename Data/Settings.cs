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
        private static IDictionary<string, string> lastValidComPorts = null;

        public static string GetLastValidComPorts(string Name)
        {
            if (lastValidComPorts != null && lastValidComPorts.ContainsKey(Name) && !string.IsNullOrEmpty(lastValidComPorts[Name]))
            {
                return lastValidComPorts[Name];
            }
            return "";
        }

        public static void SetLastValidComPorts(string Name, string val)
        {
            if (lastValidComPorts != null)
            {
                lastValidComPorts[Name] = val;
            }
        }

        public static void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            try
            {
                String attr;
                attr = pluginNode.GetAttribute(xmlTags.LastValidComPorts);
                lastValidComPorts = new Dictionary<string, string>();
                if (attr.Length > 0)
                {
                    String[] values = attr.Split(';');
                    foreach (String column in values)
                    {
                        String[] keypairs = column.Split('=');
                        if (keypairs.Length >= 2)
                        {
                            lastValidComPorts[keypairs[0]] = keypairs[1];
                        }
                    }
                }
            }
            catch{}
        }

        public static void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            string s = "";
            foreach (KeyValuePair<string, string> kv in lastValidComPorts)
            {
                s += string.Format("{0}={1};", kv.Key, kv.Value);
            }
            pluginNode.SetAttribute(xmlTags.LastValidComPorts, s);
        }

        private class xmlTags
        {
            public const string LastValidComPorts = "LastValidComPort";
        }

	}
}
