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

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class GhPacketBase
    {
        //Public properties - Mostly response, send handles PacketData
        public byte CommandId;
        public Int16 PacketLength;
        //PacketData contains the usefuldata, not everything to send
        public byte[] PacketData;
        //Only used when receiving - sending is only included in the sent data
        //Not stored now
        //public byte Checksum;

        public void InitPacket(byte CommandId, Int16 PacketLength)
        {
            this.CommandId = CommandId;
            this.PacketLength = PacketLength;
            this.PacketData = new byte[PacketLength];
        }

        //The Gloabalsat specs does not explicitly write that the format is same for all

        public const byte CommandWhoAmI = 0xBF;
        public const byte CommandGetSystemInformation = 0x85;
        public const byte CommandGetSystemConfiguration = 0x86;
        public const byte CommandSetSystemConfiguration = 0x96;
        public const byte CommandSetSystemInformation = 0x98;
        public const byte CommandGetScreenshot = 0x83;

        public const byte CommandGetWaypoints = 0x77;
        public const byte CommandSendWaypoint = 0x76;
        public const byte CommandDeleteWaypoints = 0x75;

        public const byte CommandSendRoute = 0x93;
        public const byte CommandDeleteAllRoutes = 0x97;

        public const byte CommandGetTrackFileHeaders = 0x78;
        public const byte CommandGetTrackFileSections = 0x80;
        public const byte CommandGetNextTrackSection = 0x81;
        //public const byte CommandReGetLastSection = 0x82;
        public const byte CommandId_FINISH = 0x8A;
        public const byte CommandSendTrackStart = 0x90;
        public const byte CommandSendTrackSection = 0x91;

        public const byte HeaderTypeLaps = 0xAA;
        public const byte HeaderTypeTrackPoints = 0x55;

        public const byte ResponseInsuficientMemory = 0x95;
        public const byte ResponseResendTrackSection = 0x92;
        public const byte ResponseSendTrackFinish = 0x9A;

        public class Header
        {
            public DateTime StartTime;
            public Int32 TrackPointCount; //Int16 in all but 625XT
            public TimeSpan TotalTime;
            public Int32 TotalDistanceMeters;
            public Int16 LapCount; //Not used in all
        }

        public class Train : Header
        {
            //public Int16 StartPointIndex=0;
            //public Int16 EndPointIndex=0;
            //public bool Multisport;
            public Int16 TotalCalories = 0;
            public double MaximumSpeed = 0;
            public byte MaximumHeartRate = 0;
            public byte AverageHeartRate = 0;
            public Int16 TotalAscend = 0;
            public Int16 TotalDescend = 0;
            //public Int16 MinimumAltitude;
            //public Int16 MaximumAltitude;
            public Int16 AverageCadence = 0;
            public Int16 MaximumCadence = 0;
            public Int16 AveragePower = 0;
            public Int16 MaximumPower = 0;
            //byte Sport1;
            //byte Sport2;
            //byte Sport3;
            //byte Sport4;
            //byte Sport5;

            public IList<TrackPoint> TrackPoints = new List<TrackPoint>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public class TrackFileHeader : Header
        {
            public Int32 TrackPointIndex;
        }

        public class Lap
        {
            public TimeSpan EndTime;
            public TimeSpan LapTime;
            public Int32 LapDistanceMeters;
            public Int16 LapCalories;
            public double MaximumSpeed; //Int16 for some devices
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            public Int16 MinimumAltitude;
            public Int16 MaximumAltitude;
            public Int16 AverageCadence;
            public Int16 MaximumCadence;
            public Int16 AveragePower;
            public Int16 MaximumPower;
            //public bool Multisport;
            //    public Int16 StartPointIndex;
            //    public Int16 EndPointIndex;
        }

        //Superset of all trackpoints
        public class TrackPoint
        {
            //Same in all
            public double Latitude; //4bit, Degrees * 1000000
            public double Longitude; //4bit, Degrees * 1000000
            //Int32 in GB580, Int16 in GH625XT, GH505, GH625, GH615
            public Int32 Altitude; // Meters
            //Int32 in GH625XT, Int16 in GB580, GH505, GH625, GH615
            public double Speed=0; //4bit, Kilometers per hour * 100
            public Byte HeartRate=0;
            //Int32 in GH625XT, GB580, GH505, Int16 in GH625, GH615
            public double IntervalTime=0; // Seconds * 10
            //Int16 in GH625XT, GB580, GH505, (but not available?)
            public Int16 Power=0; // Power, unknown units
            //Int16 in GH625XT, GB580 (but not available, unknown use)
            public Int16 PowerCadence=0; // unknown units
            //Int16 in GH625XT, GB580, GH505, (but not available?)
            public Int16 Cadence=0; // Cadence, unknown units
        }

        public byte[] ConstructPayload(bool bigEndianPacketLength)
        {
            byte[] data = new byte[5 + this.PacketLength];
            data[0] = 0x02;
            //Add CommandId to length, always big endian
            //Write(true, data, 1, (Int16)(this.PacketLength + 1));
            byte[] b = BitConverter.GetBytes((Int16)(this.PacketLength + 1));
            if (bigEndianPacketLength)
            {
                data[1] = b[1];
                data[2] = b[0];
            }
            else
            {
                //Only for the 561
                data[1] = b[0];
                data[2] = b[1];
            }
            data[3] = this.CommandId;

            for (int i = 0; i < this.PacketLength; i++)
            {
                data[4 + i] = this.PacketData[i];
            }
            data[this.PacketLength + 4] = GetCrc(false);
            return data;
        }

        private byte GetCrc(bool received)
        {
            byte checksum = 0;

            int len = this.PacketLength;
            if (!received)
            {
                //For sent packets, include the packetid
                len++;
            }
            byte[] b = BitConverter.GetBytes(len);
            checksum ^= b[0];
            checksum ^= b[1];
            if (!received)
            {
                checksum ^= this.CommandId;
            }
            for (int i = 0; i < this.PacketLength; i++)
            {
                checksum ^= this.PacketData[i];
            }
            return checksum;
        }

        public bool ValidResponseCrc(byte checksum)
        {
            return (GetCrc(true) == checksum);
        }

        /// <summary>
        /// Read a six byte representation of a date and time starting at the offset in the following format:
        /// Year = 2000 + byte[0]
        /// Month = byte[1]
        /// Day = byte[2]
        /// Hour = byte[3] 
        /// Minute = byte[4]
        /// Second = byte[5]
        /// </summary>
        protected DateTime ReadDateTime(int offset)
        {
            return new DateTime(PacketData[offset + 0] + 2000, PacketData[offset + 1], PacketData[offset + 2], PacketData[offset + 3], PacketData[offset + 4], PacketData[offset + 5]);
        }
        protected int Write(int offset, DateTime t)
        {
            int year = (int)Math.Max(0, t.Year - 2000);
            this.PacketData[offset+0] = (byte)year;
            this.PacketData[offset+1] = (byte)t.Month;
            this.PacketData[offset+2] = (byte)t.Day;
            this.PacketData[offset+3] = (byte)t.Hour;
            this.PacketData[offset+4] = (byte)t.Minute;
            this.PacketData[offset+5] = (byte)t.Second;

            return 6;
        }

        /// <summary>
        /// Read a string starting at the offset.
        /// </summary>
        public string ByteArr2String(int startIndex, int length)
        {
            for (int i = startIndex; i < Math.Min(startIndex + length, PacketData.Length); i++)
            {
                if (PacketData[i] == 0x0)
                {
                    length = i - startIndex;
                    break;
                }
            }
            string str = UTF8Encoding.UTF8.GetString(PacketData, startIndex, length);
            return str;
        }

        public int Write(int startIndex, int length, string s)
        {
            for (int j = 0; j < length; j++)
            {
                byte c = (byte)(j < s.Length ? s[j] : '\0');
                this.PacketData[startIndex + j] = c;
            }
            //Ensure null termination
            this.PacketData[startIndex + length - 1] = 0;
            return length;
        }

        /// <summary>
        /// Read a two byte integer starting at the offset.
        /// </summary>
        protected Int16 ReadInt16(int offset)
        {
            if (!IsLittleEndian)
            {
                return (Int16)((this.PacketData[offset] << 8) + this.PacketData[offset + 1]);
            }
            else
            {
                return BitConverter.ToInt16(this.PacketData, offset);
            }
        }

        /// <summary>
        /// Read a four byte integer starting at the offset.
        /// </summary>
        protected Int32 ReadInt32(int offset)
        {
            if (!IsLittleEndian)
            {
                return (this.PacketData[offset] << 24) + (this.PacketData[offset + 1] << 16) + (this.PacketData[offset + 2] << 8) + this.PacketData[offset + 3];
            }
            else
            {
                return BitConverter.ToInt32(this.PacketData, offset);
            }
        }

        protected double ReadLatLon(int offset)
        {
            return ReadInt32(offset) / 1000000.0;
        }

        //Some static methods for standard field formats
        static public Int32 ToGlobLatLon(double latlon)
        {
            return (int)Math.Round(latlon * 1000000);
        }
        static public Int16 ToGlobSpeed(double speed)
        {
            return (Int16)Math.Round(speed * 100 * 3.6);
        }
        static public double FromGlobSpeed(int speed)
        {
            return speed / 100.0 / 3.6;
        }
        static public Int32 ToGlobTime(double time)
        {
            return (Int32)Math.Round(time * 10);
        }
        static public Int16 ToGlobTime16(double time)
        {
            return (Int16)ToGlobTime(time);
        }
        static public double FromGlobTime(int time)
        {
            return time/10.0;
        }

        /// <summary>
        /// Write a two byte integer starting at the offset.
        /// </summary>
        protected int Write(int offset, Int16 i)
        {
            byte[] b = BitConverter.GetBytes(i);
            if (!IsLittleEndian)
            {
                this.PacketData[offset + 0] = b[1];
                this.PacketData[offset + 1] = b[0];
            }
            else
            {
                this.PacketData[offset + 0] = b[0];
                this.PacketData[offset + 1] = b[1];
            }
            return 2;
        }

        /// <summary>
        /// Write a four byte integer in little-endian format starting at the offset.
        /// </summary>
        //No overload to make sure correct
        protected int Write32(int offset, Int32 i)
        {
            byte[] b = BitConverter.GetBytes(i);
            if (!IsLittleEndian)
            {
                this.PacketData[offset + 0] = b[3];
                this.PacketData[offset + 1] = b[2];
                this.PacketData[offset + 2] = b[1];
                this.PacketData[offset + 3] = b[0];
            }
            else
            {
                this.PacketData[offset + 0] = b[0];
                this.PacketData[offset + 1] = b[1];
                this.PacketData[offset + 2] = b[2];
                this.PacketData[offset + 3] = b[3];
            }
            return 4;
        }

        //Note: The assumption is that the platform is LittleEndian
        protected virtual bool IsLittleEndian { get { return false; } }
    }
}
