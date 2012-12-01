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
using System.Drawing;
using System.Windows.Forms;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class GhConfig
    {
        public GhConfig()
        {
        }

        #region Configuration
        public string LastValidComPort
        {
            get
            {
                //xxx IList<string> ports = Settings.GetLastValidComPorts(fitnessDeviceName);
                string s = "";
                //if (ports.Count > 0)
                {
                    //s = ports[0];
                }
                return s;
            }
        }

        /// <summary>
        /// Timeout when communicating, in ms
        /// </summary>
        public virtual int ReadTimeout
        {
            get
            {
                return 4000;
            }
        }

        /// <summary>
        /// Timeout when detecting, in ms
        /// </summary>
        public virtual int ReadTimeoutDetect
        {
            get
            {
                return 4000;
            }
        }

        //The 561 only(?) have little endian size... Set here as it is controlled from the device, when probing
        public virtual bool BigEndianPacketLength { get { return true; } }
        public DeviceConfigurationInfo configInfo;

        //Barometric devices
        public virtual bool HasElevationTrack { get { return false; } }
        public virtual bool CanRotateScreen { get { return false; } }
        //625XT (but other?) do not require waypoints in routes
        public virtual bool RouteRequiresWaypoints { get { return true; } }
        //Some device related settings - affecting packets
        public virtual System.Drawing.Size ScreenSize { get { return new System.Drawing.Size(128, 96); } }
        public virtual int ScreenBpp { get { return 2; } }
        public virtual bool ScreenRowCol { get { return true; } } //Screenshot row over columns
        public virtual int TotalPoints { get { return 60000; } }

        #endregion
    }
}
