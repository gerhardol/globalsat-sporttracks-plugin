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
using System.Xml;

using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Plugin : IPlugin
    {
        public Plugin()
        {
            instance = this;
        }

        #region IPlugin Members

        public Guid Id
        {
#if GLOBALSAT_DEVICE
            get { return new Guid("fb88d87e-5bea-4b70-892a-b97530108cfb"); }
#else
            get { return new Guid("18e55911-e8cb-4c32-aee1-e2cec2b3e132"); }
#endif

        }

        public IApplication Application
        {
            get { return application; }
            set { application = value; }
        }

        public string Name
        {
            get { 
#if GLOBALSAT_DEVICE && A_RIVAL_DEVICE
                return "Globalsat Compatible Plugin";
#elif GLOBALSAT_DEVICE
                return "Globalsat Device Plugin";
#elif A_RIVAL_DEVICE
                return "a-rival SpoQ Plugin";
#endif
            }
        }

        public string Version
        {
            get { return GetType().Assembly.GetName().Version.ToString(3); }
        }

        public void ReadOptions(XmlDocument xmlDoc, XmlNamespaceManager nsmgr, XmlElement pluginNode)
        {
            Settings.ReadOptions(xmlDoc, nsmgr, pluginNode);
        }

        public void WriteOptions(XmlDocument xmlDoc, XmlElement pluginNode)
        {
            Settings.WriteOptions(xmlDoc, pluginNode);
        }

        #endregion

        public static Plugin Instance
        {
            get { return instance; }
        }

        private static Plugin instance = null;

        private IApplication application;
    }
}
