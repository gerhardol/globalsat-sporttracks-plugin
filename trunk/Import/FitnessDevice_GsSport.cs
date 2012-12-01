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
            m_FitnessDevice = null;
        }

        public override GlobalsatPacket PacketFactory { get { return new GlobalsatPacket(this); } }

        public override GlobalsatProtocol Device()
        { 
            GlobalsatProtocol gdevice = null;
            FitnessDevice_Globalsat gfdev = this.SpecificDevice;
            if (gfdev != null)
            {
                gdevice = gfdev.Device();
            }
            return gdevice;
        }
        
        public GenericDevice GetGenericDevice() { return (GenericDevice)this.device; }

        //Detect the actual Globalsat Device
        public FitnessDevice_Globalsat SpecificDevice
        {
            get
            {
                if (DetectionAttempted == false)
                {
                    DetectionAttempted = true;
                    if (!this.device.Open())
                    {
                        this.device.Close();
                        if (this.TryLittleEndian)
                        {
                            this.device.Open();
                        }
                    }

                    if (!string.IsNullOrEmpty(this.device.devId))
                    {
                        foreach (FitnessDevice_Globalsat g in new List<FitnessDevice_Globalsat> {
                    new FitnessDevice_GH625XT(), new FitnessDevice_GH625(), new FitnessDevice_GB580(), new FitnessDevice_GH505(), new FitnessDevice_GH615()/*, new Gh561Device()*/ })
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
                                        m_FitnessDevice.configInfo.Copy(this.configInfo);
                                        return m_FitnessDevice;
                                    }
                                }
                            }
                        }
                    }
                }
                return m_FitnessDevice;
            }
        }

        private bool TryLittleEndian
        {
            get
            {
                //561 - skipped by default, not working
                if (this.configInfo.AllowedIds != null)
                {
                    foreach (string s in this.configInfo.AllowedIds)
                    {
                        if (s.StartsWith("GH-561"))
                        {
                            this.m_bigEndianPacketLength = false;
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public override bool BigEndianPacketLength { get { return m_bigEndianPacketLength; } }

        public bool m_bigEndianPacketLength = true;

        #region Private members
        private bool DetectionAttempted = false;
        private FitnessDevice_Globalsat m_FitnessDevice;
        #endregion
    }
}
