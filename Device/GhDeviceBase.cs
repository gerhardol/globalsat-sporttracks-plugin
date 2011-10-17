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

using System.IO.Ports;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Data.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public abstract class GhDeviceBase
    {
        public GhDeviceBase(DeviceConfigurationInfo configInfo)
        {
            this.configInfo = configInfo;
        }

        public GhDeviceBase(FitnessDevice_Globalsat fitDev)
        {
            this.configInfo = fitDev.DefaultConfig;
            foreach (IConfiguredDevice c in Plugin.Instance.Application.SystemPreferences.FitnessDevices)
            {
                if (c.Id == fitDev.Id)
                {
                    this.configInfo = DeviceConfigurationInfo.Parse(configInfo, c.Configuration);
                }
            }
        }

        public virtual GlobalsatPacket PacketFactory { get { return new GlobalsatPacket(); } }

        public string Open()
        {
            if (port == null)
            {
                OpenPort(configInfo.ComPorts);
            }
            return devId;
        }

        public void Close()
        {
            if (port != null)
            {
                port.Close();
                port = null;
            }
        }

        public SerialPort Port
        {
            get { return port; }
        }

        public void CopyPort(GhDeviceBase b)
        {
            this.port = b.port;
            this.devId = b.devId;
        }

        protected string ValidGlobalsatPort(SerialPort port)
        {
            GlobalsatPacket packet = new GlobalsatPacket(GlobalsatPacket.CommandWhoAmI);
            GhPacketBase response = SendPacket(packet);
            string res = "";
            if (response.CommandId == packet.CommandId && response.PacketLength > 1)
            {
                string devId = GhPacketBase.ByteArr2String(response.PacketData, 0, 8);
                if (!string.IsNullOrEmpty(devId))
                {
                    if (configInfo.AllowedIds == null)
                    {
                        res = devId;
                    }
                    else
                    {
                        foreach (string aId in configInfo.AllowedIds)
                        {
                            if (devId.StartsWith(aId))
                            {
                                res = devId;
                                break;
                            }
                        }
                    }
                }
            }
			/* TODO show "devId" in some sort of error message, if not null and "res" is still "" */
            return res;
        }

        public virtual GhPacketBase SendPacket(GlobalsatPacket packet)
        {
            if (!port.IsOpen)
            {
                port.Open();
            }
            if (packet.CommandId == GhPacketBase.CommandGetScreenshot)
            {
                port.ReadTimeout = 5000;
            }
            else
            {
                port.ReadTimeout = 1000;
            }
            try
            {
                byte[] sendPayload = packet.ConstructPayload();
                port.Write(sendPayload, 0, sendPayload.Length);
            }
            catch (Exception e)
            {
                throw e;
            }

            //Use packet factory, to make sure the packet matches the device
            GhPacketBase received = this.PacketFactory;

            received.CommandId = (byte)port.ReadByte();
            int hiPacketLen = port.ReadByte();
            int loPacketLen = port.ReadByte();
            received.PacketLength = (Int16)((hiPacketLen << 8) + loPacketLen);
            if (received.PacketLength > configInfo.MaxPacketPayload)
            {
                throw new Exception(CommonResources.Text.Devices.ImportJob_Status_ImportError);
            }
            received.PacketData = new byte[received.PacketLength];
            byte checksum;
            try
            {
                for (Int16 b = 0; b < received.PacketLength; b++)
                {
                    received.PacketData[b] = (byte)port.ReadByte();
                }
                checksum = (byte)port.ReadByte();
            }
            catch(Exception e)
            {
            //TODO: DEBUG timeout often occurs for GH-505
                throw e;
            }
            port.Close();
            if (!received.ValidResponseCrc(checksum))
            {
                throw new Exception(CommonResources.Text.Devices.ImportJob_Status_ImportError);
            }
            if (received.CommandId != packet.CommandId &&
                !((received.CommandId == GhPacketBase.CommandGetTrackFileSections ||
                received.CommandId == GhPacketBase.CommandId_FINISH) &&
                (packet.CommandId == GhPacketBase.CommandGetNextSection ||
                 packet.CommandId == GhPacketBase.CommandGetTrackFileSections)))
            {
                if (received.CommandId == GhPacketBase.ResponseInsuficientMemory)
                {
                    //TODO
                    //throw new Exception(Properties.Resources.Device_InsuficientMemory_Error);
                }
                throw new Exception(CommonResources.Text.Devices.ImportJob_Status_ImportError);
            }
            return received;
        }

        protected virtual void OpenPort(IList<string> comPorts)
        {
            if (comPorts == null || comPorts.Count == 0)
            {
                comPorts = new List<string>();

                if (Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    for (int i = 1; i <= 30; i++)
                    {
                        comPorts.Add("COM" + i);
                    }
                }
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    /* Linux & OSX */
					/* TODO Check if file exists, well maybe this is fast enough and a check is not needed atleast in Linux */
                    for (int i = 0; i <= 30; i++)
                    {
                        comPorts.Add("/dev/ttyUSB" + i); /* Linux: gh615/gh625/gh625xt */
                        comPorts.Add("/dev/ttyACM" + i); /* Linux: gh505/gh561/(gh580??) */
                        comPorts.Add("/dev/tty.usbserial" + i); /* OSX */
                    }
                }
            }

            Exception lastException = new Exception();
            foreach (int baudRate in configInfo.BaudRates)
            {
                foreach (string comPort in comPorts)
                {
                    port = null;
                    try
                    {
                        port = new SerialPort(comPort, baudRate);
                        string id = ValidGlobalsatPort(port);
                        if (!string.IsNullOrEmpty(id))
                        {
                            this.devId = id;
                            return;
                        }
                        else if (port != null)
                        {
                            port.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        if (port != null)
                        {
                            port.Close();
                        }
                        //info about the last exception only
                        lastException = e;
                    }
                }
            }
            //TODO: Filter out cannot open port, so not port30 always comes up?
            string lastExceptionText = System.Environment.NewLine + System.Environment.NewLine + lastException.Message;
            throw new Exception(CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError +
              lastExceptionText);
        }

        public DeviceConfigurationInfo configInfo;
        private SerialPort port = null;
        private string devId = "";
    }
}
