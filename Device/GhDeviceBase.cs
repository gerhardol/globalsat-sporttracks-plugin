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
        public void Open()
        {
            port = OpenPort();
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

        protected bool ValidGlobalsatPort(SerialPort port)
        {
            port.ReadTimeout = 1000;
            port.Open();
            byte[] packet = GhPacketBase.GetSystemConfiguration();
            //Get the commandid, to match to returned packet
            byte commandId = GhPacketBase.SendPacketCommandId(packet);
            GhPacketBase.Response responsePacket = SendPacket(port, packet);
            return responsePacket.CommandId == commandId && responsePacket.PacketLength > 1;
        }

        protected static GhPacketBase.Response SendPacket(SerialPort port, byte[] packet)
        {
            GhPacketBase.Response received = new GhPacketBase.Response();

            try
            {
                port.Write(packet, 0, packet.Length);
            }
            catch (Exception e)
            {
                throw e;
            }


            received.CommandId = (byte)port.ReadByte();
            int hiPacketLen = port.ReadByte();
            int loPacketLen = port.ReadByte();
            received.PacketLength = (Int16)((hiPacketLen << 8) + loPacketLen);
            received.PacketData = new byte[received.PacketLength];
            for (Int16 b = 0; b < received.PacketLength; b++)
            {
                received.PacketData[b] = (byte)port.ReadByte();
            }
            received.Checksum = (byte)port.ReadByte();
            return received;
        }

        protected virtual SerialPort OpenPort()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                for (int i = 1; i <= 30; i++)
                {
                    SerialPort port = null;
                    try
                    {
                        port = new SerialPort("COM" + i, this.baudRate);
                        if (ValidGlobalsatPort(port))
                        {
                            return port;
                        }
                        else if (port != null)
                        {
                            port.Close();
                        }
                    }
                    catch (Exception)
                    {
                        if (port != null)
                        {
                            port.Close();
                        }
                    }
                }
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                /* Linux */
                for (int i = 0; i <= 30; i++)
                {
                    SerialPort port = null;
                    try
                    {
                        port = new SerialPort("/dev/ttyUSB" + i, this.baudRate);
                        if (ValidGlobalsatPort(port))
                        {
                            return port;
                        }
                        else if (port != null)
                        {
                            port.Close();
                        }
                    }
                    catch (Exception)
                    {
                        if (port != null)
                        {
                            port.Close();
                        }
                    }
                }
				for (int i = 0; i <= 30; i++)
                {
                    SerialPort port = null;
                    try
                    {
                        port = new SerialPort("/dev/ttyACM" + i, this.baudRate);
                        if (ValidGlobalsatPort(port))
                        {
                            return port;
                        }
                        else if (port != null)
                        {
                            port.Close();
                        }
                    }
                    catch (Exception)
                    {
                        if (port != null)
                        {
                            port.Close();
                        }
                    }
                }
                /* OSX */
                {
                    SerialPort port = null;
                    try
                    {
                        port = new SerialPort("/dev/tty.usbserial", this.baudRate);
                        if (ValidGlobalsatPort(port))
                        {
                            return port;
                        }
                        else if (port != null)
                        {
                            port.Close();
                        }
                    }
                    catch (Exception)
                    {
                        if (port != null)
                        {
                            port.Close();
                        }
                    }
                }
            }
            throw new Exception(CommonResources.Text.Devices.ImportJob_Status_CouldNotOpenDeviceError);
        }

        protected virtual int baudRate { get { return 115200; } }
        private SerialPort port;
    }
}
