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
    class Gh615Packet : GhPacketBase
    {
        public class Header
        {
            public DateTime StartTime;
            public TimeSpan TotalTime;
            public Int32 TotalDistanceMeters;
            public Int16 TotalCalories;
            public Int16 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            public Int16 TrackPointCount;
        }

        public class TrackFileHeader : Header
        {
            public Int16 TrackPointIndex;
        }

        public class TrackFileSection : Header
        {
            public Int16 StartPointIndex;
            public Int16 EndPointIndex;
            public IList<TrackPoint> TrackPoints = new List<TrackPoint>();
        }

        public static byte[] GetTrackFileSections(IList<Int16> trackPointIndexes)
        {
            byte[] payload = new byte[3 + trackPointIndexes.Count * 2];
            payload[0] = 0x80;
            BigEndianWrite(payload, 1, (Int16)trackPointIndexes.Count);
            int offset = 3;
            foreach (Int16 index in trackPointIndexes)
            {
                BigEndianWrite(payload, offset, index);
                offset += 2;
            }
            return ConstructPayload(payload);
        }

        public static IList<TrackFileHeader> UnpackTrackHeaders(byte[] payload)
        {
            int numHeaders = payload.Length / 24;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i*24;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, payload, trackStart);
                header.TrackPointCount = BigEndianReadInt16(payload, trackStart + 20);
                header.TrackPointIndex = BigEndianReadInt16(payload, trackStart + 22);
                headers.Add(header);
            }
            return headers;
        }

        public static TrackFileSection UnpackTrackSection(byte[] payload)
        {
            if (payload.Length < 26) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, payload, 0);
            section.TrackPointCount = BigEndianReadInt16(payload, 20);
            section.StartPointIndex = BigEndianReadInt16(payload, 22);
            section.EndPointIndex = BigEndianReadInt16(payload, 24);
            int offset = 26;
            while (offset < payload.Length)
            {
                TrackPoint point = new TrackPoint();
                point.Latitude = BigEndianReadInt32(payload, offset);
                point.Longitude = BigEndianReadInt32(payload, offset + 4);
                point.Altitude = BigEndianReadInt16(payload, offset + 8);
                point.Speed = BigEndianReadInt16(payload, offset + 10);
                point.HeartRate = payload[offset + 12];
                point.IntervalTime = BigEndianReadInt16(payload, offset + 13);
                section.TrackPoints.Add(point);
                offset += 15;
            }
            return section;
        }

        private Gh615Packet()
        {
        }

        private static void ReadHeader(Header header, byte[] payload, int offset)
        {
            header.StartTime = ReadDateTime(payload, offset);
            header.TotalTime = TimeSpan.FromSeconds(((double)BigEndianReadInt32(payload, offset + 6)) / 10);
            header.TotalDistanceMeters = BigEndianReadInt32(payload, offset + 10);
            header.TotalCalories = BigEndianReadInt16(payload, offset + 14);
            header.MaximumSpeed = BigEndianReadInt16(payload, offset + 16);
            header.MaximumHeartRate = payload[offset + 18];
            header.AverageHeartRate = payload[offset + 19];
        }

    }
}
