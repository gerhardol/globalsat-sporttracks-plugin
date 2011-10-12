// Copyright (C) 2010 Zone Five Software
// Author: Aaron Averill
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class GhPacketBase
    {
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

        public class Response
        {
            public byte CommandId;
            public Int16 PacketLength;
            public byte[] PacketData;
            public byte Checksum;
        }

        public static byte SendPacketCommandId(byte[] packet)
        {
            return packet[3];
        }

        public static byte[] GetSystemConfiguration()
        {
            byte[] payload = new byte[1];
            payload[0] = 0x85;
            return ConstructPayload(payload);
        }

        public static byte[] GetTrackFileHeaders()
        {
            byte[] payload = new byte[1];
            payload[0] = 0x78;
            return ConstructPayload(payload);
        }

        public static byte[] GetNextSection()
        {
            byte[] payload = new byte[1];
            payload[0] = 0x81;
            return ConstructPayload(payload);
        }

        protected static byte[] ConstructPayload(byte[] payload)
        {
            Int16 payloadLen = (Int16)payload.Length;
            byte[] payloadLenBytes = BitConverter.GetBytes(payloadLen);
            byte hiPayloadLen = payloadLenBytes[1];
            byte loPayloadLen = payloadLenBytes[0];
            byte[] data = new byte[4 + payloadLen];
            data[0] = 0x02;
            data[1] = hiPayloadLen;
            data[2] = loPayloadLen;

            byte checksum = 0;
            checksum ^= hiPayloadLen;
            checksum ^= loPayloadLen;
            for (int i = 0; i < payloadLen; i++)
            {
                data[3 + i] = payload[i];
                checksum ^= payload[i];
            }
            data[payloadLen + 3] = checksum;
            return data;
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
        /// Read a two byte integer in big-endian format starting at the offset.
        /// </summary>
        protected static Int16 BigEndianReadInt16(byte[] data, int offset)
        {
            return (Int16)((data[offset] << 8) + data[offset + 1]);
        }

        /// <summary>
        /// Read a four byte integer in big-endian format starting at the offset.
        /// </summary>
        protected static Int32 BigEndianReadInt32(byte[] data, int offset)
        {
            return (data[offset] << 24) + (data[offset + 1] << 16) + (data[offset + 2] << 8) + data[offset + 3];
        }

        /// <summary>
        /// Write a two byte integer in big-endian format starting at the offset.
        /// </summary>
        protected static void BigEndianWrite(byte[] data, int offset, Int16 i)
        {
            byte[] b = BitConverter.GetBytes(i);
            data[offset + 0] = b[1];
            data[offset + 1] = b[0];
        }

        /// <summary>
        /// Write a four byte integer in big-endian format starting at the offset.
        /// </summary>
        //protected static void BigEndianWrite(byte[] data, int offset, Int32 i)
        //{
        //    byte[] b = BitConverter.GetBytes(i);
        //    data[offset + 0] = b[3];
        //    data[offset + 1] = b[2];
        //    data[offset + 2] = b[1];
        //    data[offset + 3] = b[0];
        //}

        /// <summary>
        /// Read a two byte integer in little-endian format starting at the offset.
        /// </summary>
        protected static Int16 ReadInt16(byte[] data, int offset)
        {
            return (Int16)((data[offset + 1] << 8) + data[offset]);
        }

        /// <summary>
        /// Read a four byte integer in little-endian format starting at the offset.
        /// </summary>
        protected static Int32 ReadInt32(byte[] data, int offset)
        {
            return (data[offset + 3] << 24) + (data[offset + 2] << 16) + (data[offset + 1] << 8) + data[offset];
        }

        /// <summary>
        /// Write a two byte integer in little-endian format starting at the offset.
        /// </summary>
        protected static void Write(byte[] data, int offset, Int16 i)
        {
            byte[] b = BitConverter.GetBytes(i);
            data[offset + 0] = b[0];
            data[offset + 1] = b[1];
        }

        /// <summary>
        /// Write a four byte integer in little-endian format starting at the offset.
        /// </summary>
        //protected static void Write(byte[] data, int offset, Int32 i)
        //{
        //    byte[] b = BitConverter.GetBytes(i);
        //    data[offset + 0] = b[0];
        //    data[offset + 1] = b[1];
        //    data[offset + 2] = b[2];
        //    data[offset + 3] = b[3];
        //}
    }
}
