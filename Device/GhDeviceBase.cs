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

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    abstract class GhDeviceBase
    {
        public string Open(DeviceConfigurationInfo configInfo)
        {
            return OpenPort(configInfo.ComPorts);
        }

        public void Close()
        {
            if (port != null)
            {
                port.Close();
                port = null;
            }
        }

        protected SerialPort Port
        {
            get { return port; }
        }

        protected string ValidGlobalsatPort(SerialPort port)
        {
            port.ReadTimeout = 1000;
            port.Open();
            byte[] packet = GhPacketBase.GetWhoAmI();
            //Get the commandid, to match to returned packet
            byte commandId = GhPacketBase.SendPacketCommandId(packet);
            GhPacketBase.Response response = SendPacket(port, packet);
            string res = "";
            if (response.CommandId == commandId && response.PacketLength > 1)
            {
                byte[] data = response.PacketData;
                string devId = GhPacketBase.ByteArr2String(response.PacketData, 0, 8);
                if (!string.IsNullOrEmpty(devId))
                {
                    if (this.AllowedIds == null)
                    {
                        res = devId;
                    }
                    else
                    {
                        foreach (string aId in this.AllowedIds)
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
            return res;
        }

        protected static GhPacketBase.Response SendPacket(SerialPort port, byte[] packet)
        {
            byte sendCommandId = GhPacketBase.SendPacketCommandId(packet);
            if (sendCommandId == GlobalsatPacket.CommandGetScreenshot)
            {
                port.ReadTimeout = 3000;
            }
            try
            {
                port.Write(packet, 0, packet.Length);
            }
            catch (Exception e)
            {
                throw e;
            }

            GhPacketBase.Response received = new GhPacketBase.Response();

            received.CommandId = (byte)port.ReadByte();
            int hiPacketLen = port.ReadByte();
            int loPacketLen = port.ReadByte();
            received.PacketLength = (Int16)((hiPacketLen << 8) + loPacketLen);
            if (received.PacketLength > 2500)
            {
                //Max paxket length - can ite differ from device to device?
                throw new Exception(CommonResources.Text.Devices.ImportJob_Status_ImportError);
            }
            received.PacketData = new byte[received.PacketLength];
            for (Int16 b = 0; b < received.PacketLength; b++)
            {
                received.PacketData[b] = (byte)port.ReadByte();
            }
            received.Checksum = (byte)port.ReadByte();
            if (!GhPacketBase.ValidResponseCrc(received))
            {
                throw new Exception(CommonResources.Text.Devices.ImportJob_Status_ImportError);
            }
            if (received.CommandId != sendCommandId)
            {
                throw new Exception(CommonResources.Text.Devices.ImportJob_Status_ImportError);
            }
            return received;
        }

        protected virtual string OpenPort(IList<string> comPorts)
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
                    /* Linux */
                    for (int i = 0; i <= 30; i++)
                    {
                        comPorts.Add("/dev/ttyUSB" + i);
                        comPorts.Add("/dev/ttyACM" + i);
                    }
                    /* OSX */
                    for (int i = 0; i <= 30; i++)
                    {
                        comPorts.Add("/dev/tty.usbserial" + i);
                    }
                }
            }

            Exception lastException = new Exception();
            foreach (string comPort in comPorts)
            {
                foreach (int baudRate in BaudRates)
                {
                    port = null;
                    try
                    {
                        port = new SerialPort(comPort, baudRate);
                        string id = ValidGlobalsatPort(port);
                        if (!string.IsNullOrEmpty(id))
                        {
                            return id;
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
            string lastExceptionText = System.Environment.NewLine + System.Environment.NewLine + lastException.Message;
            throw new Exception(CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError+
            lastExceptionText);
        }

        protected virtual IList<int> BaudRates { get { return new List<int> { 115200 }; } }
        public virtual IList<string> AllowedIds { get { return null; } }
        private SerialPort port;
    }
}
