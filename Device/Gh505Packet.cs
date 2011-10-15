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
    class Gh505Packet : GhPacketBase
    {
        public class Header
        {
            public DateTime StartTime;
            public Int16 TrackPointCount;
            public TimeSpan TotalTime;
            public Int32 TotalDistanceMeters;
            public short LapCount;
        }

        public class TrackFileHeader : Header
        {
            public Int16 TrackPointIndex;
        }

        public class Train : Header
        {
            public Int16 TotalCalories;
            public Int16 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
        //    public Int16 StartPointIndex;
        //    public Int16 EndPointIndex;
            public IList<TrackPoint2> TrackPoints = new List<TrackPoint2>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public class Lap
        {
            public TimeSpan EndTime;
            public TimeSpan LapTime;
            public Int32 LapDistanceMeters;
            public Int16 LapCalories;
            public Int16 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
        //    public Int16 StartPointIndex;
        //    public Int16 EndPointIndex;
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
            int numHeaders = payload.Length / 24;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * 24;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, payload, trackStart);
                header.TrackPointIndex = ReadInt16(endianFormat, payload, trackStart + 18);
                headers.Add(header);
            }
            return headers;
        }

        public static Train UnpackTrainHeader(byte[] payload)
        {
            if (payload.Length < 52) return null;

            Train train = new Train();
            ReadHeader(train, payload, 0);
            train.TotalCalories = ReadInt16(endianFormat, payload, 24);
            train.MaximumSpeed = ReadInt16(endianFormat, payload, 26);
            train.MaximumHeartRate = payload[28];
            train.AverageHeartRate = payload[29];
            return train;
        }

        public static IList<Lap> UnpackLaps(byte[] payload)
        {
            if (payload.Length < 24) return new List<Lap>();

            IList<Lap> laps = new List<Lap>();

            int offset = 24;
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
                laps.Add(lap);
                offset += 36;
            }
            return laps;
        }

        public static IList<TrackPoint2> UnpackTrackPoints(byte[] payload)
        {
            if (payload.Length < 24) return new List<TrackPoint2>();

            IList<TrackPoint2> points = new List<TrackPoint2>();

            int offset = 24;
            while (offset < payload.Length)
            {
                TrackPoint2 point = new TrackPoint2();
                point.Latitude = ReadInt32(endianFormat, payload, offset);
                point.Longitude = ReadInt32(endianFormat, payload, offset + 4);
                point.Altitude = ReadInt16(endianFormat, payload, offset + 8);
                point.Speed = ReadInt16(endianFormat, payload, offset + 10);
                point.HeartRate = payload[offset + 12];
                point.IntervalTime = ReadInt32(endianFormat, payload, offset + 16);
                point.Cadence = ReadInt16(endianFormat, payload, offset + 20);
                point.Power = ReadInt16(endianFormat, payload, offset + 24);
                points.Add(point);
                offset += 28;
            }
            return points;
        }

        private Gh505Packet()
        {
        }

        private static void ReadHeader(Header header, byte[] payload, int offset)
        {
            header.StartTime = ReadDateTime(payload, offset).ToUniversalTime();
            header.TrackPointCount = ReadInt16(endianFormat, payload, offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, payload, offset + 8)) / 10);
            header.TotalDistanceMeters = ReadInt32(endianFormat, payload, offset + 12);
            header.LapCount = ReadInt16(endianFormat, payload, offset + 16);
        }

        const bool endianFormat = false; //little endian
    }
}
