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
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public abstract class GhDeviceBase
    {
        public GhDeviceBase()
        {
            this.configInfo = DefaultConfig;
        }

        public GhDeviceBase(string configurationInfo)
        {
            this.configInfo = DeviceConfigurationInfo.Parse(DefaultConfig, configurationInfo);
        }

        public string LastValidComPort
        {
            get
            {
                return Settings.GetLastValidComPorts(Name);
            }
        }

        private string Name
        {
            get
            {
                string name = "Generic";
                if (this.configInfo != null && this.configInfo.AllowedIds != null && this.configInfo.AllowedIds.Count > 0)
                {
                    name = this.configInfo.AllowedIds[0];
                }
                return name;
            }
        }

        /// <summary>
        /// Timeout when communicating, in ms
        /// </summary>
        public virtual int ReadTimeout
        {
            get
            {
                return 1000;
            }
        }

        /// <summary>
        /// Timeout when detecting, in ms
        /// </summary>
        public virtual int ReadTimeoutDetect
        {
            get
            {
                return 1000;
            }
        }

        public virtual DeviceConfigurationInfo DefaultConfig
        {
            get
            {
                //Must handle all possible devices
                DeviceConfigurationInfo info = new DeviceConfigurationInfo(new List<string>(), new List<int> { 115200, 57600 });
                return info;
            }
        }

        public abstract GlobalsatPacket PacketFactory { get; }

        /// open port (if not open), return if open
        public bool Open()
        {
            if (port == null)
            {
                OpenPort(configInfo.ComPorts);
            }
            return (port != null);
        }

        public void Close()
        {
            //The actual port is closed after each packet, Close will require a new scan 
            if (port != null && port.IsOpen)
            {
                port.Close();
            }
            port = null;
        }

        private SerialPort Port
        {
            get { return port; }
        }

        public void CopyPort(GhDeviceBase b)
        {
            this.port = b.port;
            this.devId = b.devId;
        }

        protected bool ValidGlobalsatPort(SerialPort port)
        {
            GlobalsatPacket packet = PacketFactory.GetWhoAmI();
            bool res = false;
            //If "probe" packet fails, this is not a Globalsat port
            //If some distinction needed (other device?), set some flag here
            GhPacketBase response = null;
            try
            {
                response = SendPacket(packet);
            }
            catch (Exception)
            {
            }
            if (response != null && response.CommandId == packet.CommandId && response.PacketLength > 1)
            {
                string devId = response.ByteArr2String(0, 8);
                if (!string.IsNullOrEmpty(devId))
                {
                    if (configInfo.AllowedIds == null || configInfo.AllowedIds.Count == 0)
                    {
                        this.devId = devId;
                        res = true;
                    }
                    else
                    {
                        foreach (string aId in configInfo.AllowedIds)
                        {
                            if (devId.StartsWith(aId))
                            {
                                this.devId = devId;
                                res = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!res)
            {
                this.Close();
            }
            return res;
        }

        private static bool showPopup = false; //Do not show by default
        internal static bool ReportError(string s, bool initial, Exception e)
        {
            if (e != null)
            {
                s += " exc:" + e.ToString();
            }
            return ReportError(s, initial);
        }
        internal static bool ReportError(string s, bool initial)
        {
            if (showPopup && !initial)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                System.Diagnostics.StackFrame[] stFrames = st.GetFrames();
                string trace = "";
                for (int i = 1; i < stFrames.Length && i < 6; i++)
                {
                    //TODO: Nicer formatting
                    trace += stFrames[i].ToString() + System.Environment.NewLine;
                }

                string s2 = s + " " + trace + string.Format(" To show further popups, press {0}", System.Windows.Forms.DialogResult.Yes);
                if (System.Windows.Forms.MessageBox.Show(s2, "", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation) != System.Windows.Forms.DialogResult.Yes)
                {
                    showPopup = false;
                }
            }
            return showPopup;
        }

        //This routine has a lot of try-catch-rethrow to simplify debugging
        //Globalsat device communication is tricky...
        public virtual GhPacketBase SendPacket(GlobalsatPacket packet)
        {
            //Use packet factory, to make sure the packet matches the device
            GhPacketBase received = this.PacketFactory;

            //sending occasionally fails, retry
            int remainingAttempts = 3;
            while (remainingAttempts > 0)
            {
                try
                {
                    if (!port.IsOpen)
                    {
                        //Physical port should be closed
                        port.Open();
                    }

                    if (packet.CommandId == GhPacketBase.CommandWhoAmI)
                    {
                        //Speed-up device detection, keep this as short as possible. 
                        //625XT seem to work with 5ms, 505 needs more than 100
                        port.ReadTimeout = this.ReadTimeoutDetect;
                    }
                    else if (packet.CommandId == GhPacketBase.CommandGetScreenshot)
                    {
                        port.ReadTimeout = 5000;
                    }
                    else
                    {
                        port.ReadTimeout = this.ReadTimeout;
                    }

                    byte[] sendPayload = packet.ConstructPayload(BigEndianPacketLength);
                    try
                    {
                        /*
                                         * Console.Write("Write:");
                                        for(int i = 0; i < sendPayload.Length;i++)
                                        {
                                            Console.Write(" " + sendPayload[i].ToString() );
                                        }
                                        Console.WriteLine("");
                        */
                        port.Write(sendPayload, 0, sendPayload.Length);
                    }
                    catch (Exception e)
                    {
                        string s = string.Format("Error occurred, sending {0} bytes.",
                                sendPayload.Length);
                        ReportError(s, packet.CommandId == GhPacketBase.CommandWhoAmI, e);
                        port.Close();
                        throw e;
                    }

                    try
                    {
                        int data = port.ReadByte();
                        received.CommandId = (byte)data;
                        if (data < 0 || data > 255)
                        {
                            //Special handling for first byte -1 is stream closed
                            throw new TimeoutException(string.Format("No data received for {0},{1}", packet.CommandId,data));
                        }
                        if (packet.CommandId == GhPacketBase.CommandWhoAmI)
                        {
                            this.DataRecieved = false;
                        }
                        else
                        {
                            this.DataRecieved = true;
                        }
                        int hiPacketLen = port.ReadByte();
                        int loPacketLen = port.ReadByte();
                        //Note: The endian for size (except for 561) from the device always seem to be the same (not so when sending)
                        received.PacketLength = (Int16)((hiPacketLen << 8) + loPacketLen);
                    }
                    catch (Exception e)
                    {
                        string s = string.Format("Error occurred, receiving {0} bytes ({2},{3}).",
                                received.PacketLength, this.DataRecieved, packet.CommandId, received.CommandId);
                        ReportError(s, packet.CommandId == GhPacketBase.CommandWhoAmI, e);
                        port.Close();
                        throw e;
                    }

                    if (packet.CommandId != GhPacketBase.CommandGetScreenshot && received.PacketLength > configInfo.MaxPacketPayload ||
                        received.PacketLength > 0x1000)
                    {
                        string s = string.Format("Error occurred, bad response receiving {0} bytes ({2},{3}).",
                                received.PacketLength, this.DataRecieved, packet.CommandId, received.CommandId);
                        ReportError(s, packet.CommandId == GhPacketBase.CommandWhoAmI);
                        port.Close();
                        throw new Exception(Properties.Resources.Device_OpenDevice_Error);
                    }

                    received.PacketData = new byte[received.PacketLength];
                    byte checksum = 0;
                    int receivedBytes = 0; //debug timeouts

                    try
                    {
                        for (Int16 b = 0; b < received.PacketLength; b++)
                        {
                            int data = port.ReadByte();
                            if (data < 0 || data > 255)
                            {
                                //-1 is stream closed
                                throw new TimeoutException(string.Format("All data not received for {0}({4},{1}) bytes ({2},{3}).",
                                received.PacketLength, receivedBytes, packet.CommandId, received.CommandId, data));
                            }
                            received.PacketData[b] = (byte)data;
                            receivedBytes++;
                        }
                        checksum = (byte)port.ReadByte();
                        receivedBytes++;
                        /*				
                                    Console.Write("Read: id:" + received.CommandId + " length:" + received.PacketLength);
                                    for(int i = 0; i < received.PacketLength;i++)
                                    {
                                        Console.Write(" " + received.PacketData[i].ToString() );
                                    }
                                    Console.WriteLine(" checksum:" + checksum);
                        */

                    }
                    catch (Exception e)
                    {
                        string s = string.Format("Error occurred, receiving {0}({1}) bytes ({2},{3}).",
                                received.PacketLength, receivedBytes, packet.CommandId, received.CommandId);
                        ReportError(s, packet.CommandId == GhPacketBase.CommandWhoAmI, e);
                        port.Close();
                        throw e;

                        //Debug template, if the device is corrupted
                        //Should use resend
                        //Ignore the exception, just to get data from the device
                        //if (!(this is Gh505Device &&
                        //    (receivedBytes == 2005 && received.PacketLength == 2068 ||
                        //    receivedBytes == 913 && received.PacketLength == 976)))
                        //{
                        //    throw e;
                        //}
                        //received.PacketLength = (Int16)receivedBytes;
                        //checksum = 0;
                        //overrideException = true;
                    }

                    if (!received.ValidResponseCrc(checksum))
                    {
                        string s = string.Format("Error occurred, invalid checksum receiving {0}({1}) bytes ({2},{3}).",
                                         received.PacketLength, receivedBytes, packet.CommandId, received.CommandId);
                        ReportError(s, packet.CommandId == GhPacketBase.CommandWhoAmI);
                        port.Close();
                        throw new Exception(Properties.Resources.Device_OpenDevice_Error);
                    }
                    else if (received.CommandId == GhPacketBase.ResponseResendTrackSection && remainingAttempts > 0)
                    {
                        //retry sending
                        remainingAttempts--;
                    }
                    else if (received.CommandId != packet.CommandId &&
                        //TODO: Cleanup in allowed sender/response allowed (probably overload)
                        !((received.CommandId == GhPacketBase.CommandGetTrackFileSections ||
                           received.CommandId == GhPacketBase.CommandId_FINISH ||
                           received.CommandId == GhPacketBase.ResponseSendTrackFinish) &&
                          (packet.CommandId == GhPacketBase.CommandGetNextTrackSection ||
                           packet.CommandId == GhPacketBase.CommandGetTrackFileSections ||
                           packet.CommandId == GhPacketBase.CommandSendTrackSection))
                        )
                    {
                        string s = string.Format("Error occurred, invalid response {0}({1}) bytes ({2},{3}).",
                                         received.PacketLength, receivedBytes, packet.CommandId, received.CommandId);
                        ReportError(s, packet.CommandId == GhPacketBase.CommandWhoAmI);
                        if (received.CommandId == GhPacketBase.ResponseInsuficientMemory)
                        {
                            throw new InsufficientMemoryException(Properties.Resources.Device_InsuficientMemory_Error);
                        }
                        throw new Exception(Properties.Resources.Device_OpenDevice_Error);
                    }
                    else
                    {
                        //Assume OK response, no more tries
                        remainingAttempts = 0;
                    }
                }

                catch (Exception e)
                {
                    remainingAttempts--;
                    if (packet.CommandId == GhPacketBase.CommandWhoAmI || remainingAttempts <= 0)
                    {
                        //No need retry
                        this.Close();
                        throw e;
                    }
                }
            }
            return received;
        }

        private bool comPortsAdd(IList<string> comPorts, string s)
        {
            bool res = false;
            if (!string.IsNullOrEmpty(s))
            {
                if (!comPorts.Contains(s))
                {
                    comPorts.Add(s);
                    res = true;
                }
            }
            return res;
        }

        //Try open the port. Catch all exceptions, let the caller determine if this is an error
        protected virtual void OpenPort(IList<string> comPorts)
        {
            this.devId = "";

            if (comPorts == null || comPorts.Count == 0)
            {
                comPorts = new List<string>();

                this.comPortsAdd(comPorts, this.LastValidComPort);
                if (Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    for (int i = 1; i <= 30; i++)
                    {
                        this.comPortsAdd(comPorts, "COM" + i);
                    }
                }
                else if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    /* Linux & OSX */
					/* TODO Check if file exists, well maybe this is fast enough and a check is not needed atleast in Linux */
                    for (int i = 0; i <= 30; i++)
                    {
                        this.comPortsAdd(comPorts, "/dev/ttyUSB" + i); /* Linux: gh615/gh625/gh625xt */
                        this.comPortsAdd(comPorts, "/dev/ttyACM" + i); /* Linux: gh505/gh561/(gb580??) */
                        this.comPortsAdd(comPorts, "/dev/tty.usbserial" + i); /* OSX */
                    }
                }
            }

            foreach (int baudRate in configInfo.BaudRates)
            {
                foreach (string comPort in comPorts)
                {
                    this.Close();
                    port = new SerialPort(comPort, baudRate);
                    port.WriteBufferSize = configInfo.MaxPacketPayload;
                    if (ValidGlobalsatPort(port))
                    {
                        Settings.SetLastValidComPorts(Name, comPort);
                        return;
                    }
                }
            }
        }

        //Data received other than identification packet (the fist packet)
        public bool DataRecieved = false;
        //The identification reported from the device
        public string devId = "";
        //The 561 only(?) have little endian size... Set here as it is controlled from the device, when probing
        public virtual bool BigEndianPacketLength { get { return true; } }
        public DeviceConfigurationInfo configInfo;
        private SerialPort port = null;
    }
}
