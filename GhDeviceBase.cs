// Copyright (C) 2010 Zone Five Software
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
                        port = new SerialPort("COM" + i, 115200);
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
                        port = new SerialPort("/dev/ttyUSB" + i, 57600);
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
                        port = new SerialPort("/dev/tty.usbserial", 57600);
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

        private SerialPort port;
    }
}
