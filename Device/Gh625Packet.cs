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
    class Gh625Packet : GhPacketBase
    {
        public new class Header : GhPacketBase.Header
        {
            //public DateTime StartTime;
            //public byte LapCount;
            //public TimeSpan TotalTime;
            //public Int32 TotalDistanceMeters;
            public Int16 TotalCalories;
            public Int16 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            //public Int16 TrackPointCount;
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
            public IList<Lap> Laps = new List<Lap>();
        }

        public static byte[] GetTrackFileSections(IList<Int16> trackPointIndexes)
        {
            byte[] payload = new byte[3 + trackPointIndexes.Count * 2];
            payload[0] = CommandGetTrackFileSections;
            Write(endianFormat, payload, 1, (Int16)trackPointIndexes.Count);
            int offset = 3;
            foreach (Int16 index in trackPointIndexes)
            {
                Write(endianFormat, payload, offset, index);
                offset += 2;
            }
            return ConstructPayload(payload);
        }

        public static IList<TrackFileHeader> UnpackTrackHeaders(byte[] payload)
        {
            int numHeaders = payload.Length / 31;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i*31;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, payload, trackStart);
                header.TrackPointCount = ReadInt16(endianFormat, payload, trackStart + 25);
                header.TrackPointIndex = ReadInt16(endianFormat, payload, trackStart + 27);
                headers.Add(header);
            }
            return headers;
        }

        public static TrackFileSection UnpackTrackSectionLaps(byte[] payload)
        {
            if (payload.Length < 31) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, payload, 0);
            section.LapCount = payload[6];
            section.TrackPointCount = ReadInt16(endianFormat, payload, 25);
            section.StartPointIndex = ReadInt16(endianFormat, payload, 27);
            section.EndPointIndex = ReadInt16(endianFormat, payload, 29);

            int offset = 31;
            while (offset < payload.Length)
            {
                Lap lap = new Lap();
                lap.EndTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, payload, offset)) / 10);
                lap.LapTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, payload, offset + 4)) / 10);
                lap.LapDistanceMeters = ReadInt32(endianFormat, payload, offset + 8);
                lap.LapCalories = ReadInt16(endianFormat, payload, offset + 12);
                lap.MaximumSpeed = ReadInt16(endianFormat, payload, offset + 14);
                lap.MaximumHeartRate = payload[offset + 16];
                lap.AverageHeartRate = payload[offset + 17];
                //lap.StartPointIndex = ReadInt16(endianFormat, payload, 18);
                //lap.EndPointIndex = ReadInt16(endianFormat, payload, 20);
                section.Laps.Add(lap);
                offset += 22;
            }
            return section;
        }

        public static TrackFileSection UnpackTrackSection(byte[] payload)
        {
            if (payload.Length < 31) return null;

            TrackFileSection section = new TrackFileSection();
            ReadHeader(section, payload, 0);
            section.TrackPointCount = ReadInt16(endianFormat, payload, 25);
            section.StartPointIndex = ReadInt16(endianFormat, payload, 27);
            section.EndPointIndex = ReadInt16(endianFormat, payload, 29);

            int offset = 31;
            while (offset < payload.Length)
            {
                TrackPoint point = new TrackPoint();
                point.Latitude = ReadInt32(endianFormat, payload, offset);
                point.Longitude = ReadInt32(endianFormat, payload, offset + 4);
                point.Altitude = ReadInt16(endianFormat, payload, offset + 8);
                point.Speed = ReadInt16(endianFormat, payload, offset + 10);
                point.HeartRate = payload[offset + 12];
                point.IntervalTime = ReadInt16(endianFormat, payload, offset + 13);
                section.TrackPoints.Add(point);
                offset += 15;
            }
            return section;
        }

        private Gh625Packet()
        {
        }

        private static void ReadHeader(Header header, byte[] payload, int offset)
        {
            header.StartTime = ReadDateTime(payload, offset);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, payload, offset + 7)) / 10);
            header.TotalDistanceMeters = ReadInt32(endianFormat, payload, offset + 11);
            header.TotalCalories = ReadInt16(endianFormat, payload, offset + 15);
            header.MaximumSpeed = ReadInt16(endianFormat, payload, offset + 17);
            header.MaximumHeartRate = payload[offset + 19];
            header.AverageHeartRate = payload[offset + 20];
        }

        const bool endianFormat = true; //big endian
    }
}
