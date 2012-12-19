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

using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class FitnessDevice_GsSport : FitnessDevice_Globalsat
    {
        public FitnessDevice_GsSport()
            : base()
        {
            this.id = new Guid("16a23ea0-f5cc-11e0-be50-0800200c9a66");
            this.image = Properties.Resources.Image_48_GSSPORT;
            this.name = "Globalsat";
            this.device = new GenericDevice(this);
            //Must handle all possible devices
            this.configInfo = new DeviceConfigurationInfo(new List<string>(), new List<int> { 115200, 57600 });
            this.GetConfigurationString(); //Set configuration from Preferences
            m_FitnessDevice = null;
        }

        public override GlobalsatPacket PacketFactory { get { return new GlobalsatPacket(this); } }

        //Detect the Globalsat (protocol) device
        public override GlobalsatProtocol Device()
        { 
            GlobalsatProtocol gdevice = null;
            FitnessDevice_Globalsat gfdev = this.FitnessDevice;
            if (gfdev != null)
            {
                gdevice = gfdev.Device();
            }
            return gdevice;
        }
        
        //Detect the actual Globalsat Fitness Device
        private FitnessDevice_Globalsat FitnessDevice
        {
            get
            {
                if (DetectionAttempted == false)
                {
                    DetectionAttempted = true;
                    IList<FitnessDevice_Globalsat> fds = new List<FitnessDevice_Globalsat> {
                    new FitnessDevice_GH625XT(), new FitnessDevice_GH625(), new FitnessDevice_GB580(), new FitnessDevice_GH505(), 
                      //Not in general devices - will add one...
                      new FitnessDevice_GH615(), 
                      //No support for import, but waypoints
                      new FitnessDevice_GH561() 
                    };

                    if (!this.device.Open())
                    {
                        this.device.Close();
                        FitnessDevice_GH561 Gh561Device = new FitnessDevice_GH561();
                        //Support GH-561 - skipped by default, not working
                        foreach (IConfiguredDevice d in Plugin.Instance.Application.SystemPreferences.FitnessDevices)
                        {
                            if (Gh561Device.Id.Equals(d.Id))
                            {
                                this.m_bigEndianPacketLength = false;
                                this.device.Open();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(this.device.devId))
                    {
                        foreach (FitnessDevice_Globalsat g in fds)
                        {
                            if (g.configInfo.AllowedIds != null)
                            {
                                foreach (string s in g.configInfo.AllowedIds)
                                {
                                    if (this.device.devId.StartsWith(s))
                                    {
                                        m_FitnessDevice = g;
                                        m_FitnessDevice.Device().CopyPort(this.device);
                                        //Copy settings from generic
                                        this.SetDynamicConfigurationString();
                                        m_FitnessDevice.SetDynamicConfigurationString();
                                        m_FitnessDevice.configInfo.Copy(this.configInfo);
                                        return m_FitnessDevice;
                                    }
                                }
                            }
                        }
                    }
                    this.device.Close();
                }
                return m_FitnessDevice;
            }
        }

        public override bool BigEndianPacketLength { get { return m_bigEndianPacketLength; } }

        public bool DetectionAttempted = false;

        #region Private members
        private bool m_bigEndianPacketLength = true;
        private FitnessDevice_Globalsat m_FitnessDevice;
        #endregion
    }
}
