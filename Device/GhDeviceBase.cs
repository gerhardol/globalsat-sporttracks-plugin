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
    public abstract class GhDeviceBase : IDisposable
    {
        public GhDeviceBase(FitnessDevice_Globalsat fitnessDevice)
        {
            this.FitnessDevice = fitnessDevice;
        }

        public GlobalsatPacket PacketFactory { get { return this.FitnessDevice.PacketFactory; } }

        /// open port (if not open), return if open
        public bool Open()
        {
            if (port == null)
            {
                OpenPort(this.FitnessDevice.configInfo.ComPorts);
            }
            return (this.port != null);
        }

        public void Close()
        {
            ClosePort();
        }

        public void ClosePort()
        {
            //The actual port is closed after each packet, Close will require a new scan 
            if (this.port != null && this.port.IsOpen)
            {
                this.FitnessDevice.SetDynamicConfigurationString();
                this.port.Close();
            }
            this.port = null;
        }

        private SerialPort Port
        {
            get { return this.port; }
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
                    lastDevId = devId;
                    if (this.FitnessDevice.configInfo.AllowedIds == null || this.FitnessDevice.configInfo.AllowedIds.Count == 0)
                    {
                        this.devId = devId;
                        res = true;
                    }
                    else
                    {
                        foreach (string aId in this.FitnessDevice.configInfo.AllowedIds)
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
                if (MessageDialog.Show(s2, "", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation) != System.Windows.Forms.DialogResult.Yes)
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
                        port.ReadTimeout = this.FitnessDevice.ReadTimeoutDetect;
                    }
                    else if (packet.CommandId == GhPacketBase.CommandGetScreenshot)
                    {
                        port.ReadTimeout = 5000;
                    }
                    else
                    {
                        port.ReadTimeout = this.FitnessDevice.ReadTimeout;
                    }

                    //Override from device config?
                    if (this.FitnessDevice.configInfo.ReadTimeout > 0 &&
                        (packet.CommandId != GhPacketBase.CommandGetScreenshot ||
                        this.FitnessDevice.configInfo.ReadTimeout > port.ReadTimeout))
                    {
                        port.ReadTimeout = this.FitnessDevice.configInfo.ReadTimeout;
                    }

                    byte[] sendPayload = packet.ConstructPayload(this.FitnessDevice.BigEndianPacketLength);
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
                        throw;
                    }

                    try
                    {
                        int data = port.ReadByte();
                        received.CommandId = (byte)data;
                        if (data < 0 || data > 255)
                        {
                            //Special handling for first byte -1 is stream closed
                            throw new TimeoutException(string.Format("No data received for {0},{1}", packet.CommandId, data));
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
                        string s = string.Format("Error occurred, receiving {0} bytes ({1},{2}).",
                                received.PacketLength, packet.CommandId, received.CommandId);
                        ReportError(s, packet.CommandId == GhPacketBase.CommandWhoAmI, e);
                        port.Close();
                        throw;
                    }

                    if (packet.CommandId != GhPacketBase.CommandGetScreenshot && received.PacketLength > this.FitnessDevice.configInfo.MaxPacketPayload ||
                        received.PacketLength > 0x1000)
                    {
                        string s = string.Format("Error occurred, bad response receiving {0} bytes ({1},{2}).",
                                received.PacketLength, packet.CommandId, received.CommandId);
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
                        throw;

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

#pragma warning disable CS0168 // Variable is declared but never used
                catch (Exception e)
#pragma warning restore CS0168 // Variable is declared but never used
                {
                    remainingAttempts--;
                    if (packet.CommandId == GhPacketBase.CommandWhoAmI || remainingAttempts <= 0)
                    {
                        //No need retry
                        this.Close();
                        throw;
                    }
                }
            }
            return received;
        }

        private bool ComPortsAdd(IList<string> comPorts, string s)
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

                foreach (string port in this.FitnessDevice.configInfo.GetLastValidComPorts())
                {
                    this.ComPortsAdd(comPorts, port);
                }

                if (this.FitnessDevice.configInfo.ScanComPorts)
                {
                    if (Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT)
                    {
                        for (int i = 1; i <= 30; i++)
                        {
                            this.ComPortsAdd(comPorts, "COM" + i);
                        }
                    }
                    else if (Environment.OSVersion.Platform == PlatformID.Unix)
                    {
                        /* Linux & OSX */
                        /* TODO Check if file exists, well maybe this is fast enough and a check is not needed atleast in Linux */
                        for (int i = 0; i <= 30; i++)
                        {
                            this.ComPortsAdd(comPorts, "/dev/ttyUSB" + i); /* Linux: gh615/gh625/gh625xt */
                            this.ComPortsAdd(comPorts, "/dev/ttyACM" + i); /* Linux: gh505/gh561/(gb580??) */
                            this.ComPortsAdd(comPorts, "/dev/tty.usbserial" + i); /* OSX */
                        }
                    }
                }
            }

            foreach (int baudRate in this.FitnessDevice.configInfo.BaudRates)
            {
                foreach (string comPort in comPorts)
                {
                    this.ClosePort();
                    port = new SerialPort(comPort, baudRate)
                    {
                        WriteBufferSize = this.FitnessDevice.configInfo.MaxPacketPayload
                    };
                    if (ValidGlobalsatPort(port))
                    {
                        this.FitnessDevice.configInfo.SetLastValidComPort(comPort);
                        return;
                    }
                }
            }
        }

        //The identification reported from the device
        public string devId = "";
        public string lastDevId = "";
        //Data received other than identification packet (the fist packet)
        public bool DataRecieved = false;

        public FitnessDevice_Globalsat FitnessDevice;
        private SerialPort port = null;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
                this.port.Dispose();
            }
        }
    }
}
