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
        public byte Checksum;

        public GhPacketBase(byte CommandId, Int16 PacketLength)
        {
            this.CommandId = CommandId;
            this.PacketLength = PacketLength;
            this.PacketData = new byte[PacketLength];
        }
        public GhPacketBase()
        {
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
        public const byte CommandGetNextSection = 0x81;
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
            public Int32 TrackPointCount; //Int16 in some devices
            public TimeSpan TotalTime;
            public Int32 TotalDistanceMeters;
            public Int16 LapCount; //Not used in all
        }

        public class Lap
        {
            public TimeSpan EndTime;
            public TimeSpan LapTime;
            public Int32 LapDistanceMeters;
            public Int16 LapCalories;
            public Int32 MaximumSpeed; //Int16 for some devices
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            //    public Int16 StartPointIndex;
            //    public Int16 EndPointIndex;
        }

        public class TrackPoint
        {
            public Int32 Latitude; // Degrees * 1000000
            public Int32 Longitude; // Degrees * 1000000
            public Int16 Altitude; // Meters
            public Int16 Speed; // Kilometers per hour * 100
            public Byte HeartRate;
            public Int16 IntervalTime; // Seconds * 10
        }

        public class TrackPoint2
        {
            public Int32 Latitude; // Degrees * 1000000
            public Int32 Longitude; // Degrees * 1000000
            public Int16 Altitude; // Meters
            public Int16 Speed; // Kilometers per hour * 100
            public Byte HeartRate;
            public Int32 IntervalTime; // Seconds * 10
            public Int16 Power; // Power, unknown units
            public Int16 Cadence; // Cadence, unknown units
        }

        public class TrackPoint3
        {
            public Int32 Latitude; // Degrees * 1000000
            public Int32 Longitude; // Degrees * 1000000
            public Int32 Altitude; // Meters
            public Int16 Speed; // Kilometers per hour * 100
            public Byte HeartRate;
            public Int32 IntervalTime; // Seconds * 10
            public Int16 Power; // Power, unknown units
            public Int16 Cadence; // Cadence, unknown units
        }

        public class TrackPoint4
        {
            public Int32 Latitude; // Degrees * 1000000
            public Int32 Longitude; // Degrees * 1000000
            public Int16 Altitude; // Meters
            public Int32 Speed; // Kilometers per hour * 100
            public Byte HeartRate;
            public Int32 IntervalTime; // Seconds * 10
            public Int16 Power; // Power, unknown units
            public Int16 Cadence; // Cadence, unknown units
        }

        public byte[] ConstructPayload()
        {
            byte[] data = new byte[5 + this.PacketLength];
            data[0] = 0x02;
            //Add CommandId to length
            Write(true, data, 1, (Int16)(this.PacketLength + 1));
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

        public bool ValidResponseCrc()
        {
            return (GetCrc(true) == this.Checksum);
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
        protected static DateTime ReadDateTime(byte[] data, int offset)
        {
            return new DateTime(data[offset + 0] + 2000, data[offset + 1], data[offset + 2], data[offset + 3], data[offset + 4], data[offset + 5]);
        }

        /// <summary>
        /// Read a string starting at the offset.
        /// </summary>
        public static string ByteArr2String(byte[] byteArr, int startIndex, int length)
        {
            for (int i = startIndex; i < Math.Min(startIndex + length, byteArr.Length); i++)
            {
                if (byteArr[i] == 0x0)
                {
                    length = i - startIndex;
                    break;
                }
            }
            string str = UTF8Encoding.UTF8.GetString(byteArr, startIndex, length);
            return str;
        }

        /// <summary>
        /// Read a two byte integer starting at the offset.
        /// </summary>
        protected static Int16 ReadInt16(bool bigEndian, byte[] data, int offset)
        {
            if (bigEndian)
            {
                return (Int16)((data[offset] << 8) + data[offset + 1]);
            }
            else
            {
                return (Int16)((data[offset + 1] << 8) + data[offset]);
            }
        }

        /// <summary>
        /// Read a four byte integer starting at the offset.
        /// </summary>
        protected static Int32 ReadInt32(bool bigEndian, byte[] data, int offset)
        {
            if (bigEndian)
            {
                return (data[offset] << 24) + (data[offset + 1] << 16) + (data[offset + 2] << 8) + data[offset + 3];
            }
            else
            {
                return (data[offset + 3] << 24) + (data[offset + 2] << 16) + (data[offset + 1] << 8) + data[offset];
            }
        }

        /// <summary>
        /// Write a two byte integer starting at the offset.
        /// </summary>
        protected static void Write(bool bigEndian, byte[] data, int offset, Int16 i)
        {
            if (bigEndian)
            {
                byte[] b = BitConverter.GetBytes(i);
                data[offset + 0] = b[1];
                data[offset + 1] = b[0];
            }
            else
            {
                byte[] b = BitConverter.GetBytes(i);
                data[offset + 0] = b[0];
                data[offset + 1] = b[1];
            }
        }

        /// <summary>
        /// Write a four byte integer in little-endian format starting at the offset.
        /// </summary>
        //protected static void Write(bigEndian, byte[] data, int offset, Int32 i)
        //{
        //    byte[] b = BitConverter.GetBytes(i);
        //    data[offset + 0] = b[0];
        //    data[offset + 1] = b[1];
        //    data[offset + 2] = b[2];
        //    data[offset + 3] = b[3];
        //}

        protected virtual bool endianFormat { get { return true; } }
    }
}
