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
// Author: Gerhard Olsson


using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh625XTPacket : GhPacketBase
    {

        const int dbTrainHeaderLength = 29;
        const int dbTrainHeaderCType = 28;
        const int dbTrainHeadLength = 58;
        const int dbLapLength = 41;
        const int dbTrackPoint4Length = 25;

        public class Train : Header
        {
            public Int16 TotalCalories;
            public Int16 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
            //    public Int16 StartPointIndex;
            //    public Int16 EndPointIndex;
            public IList<TrackPoint4> TrackPoints = new List<TrackPoint4>();
            public IList<Lap> Laps = new List<Lap>();
        }

        public class TrackFileHeader : Header
        {
            public Int32 TrackPointIndex;
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
            int numHeaders = payload.Length / dbTrainHeaderLength;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * dbTrainHeaderLength;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, payload, trackStart);
                header.TrackPointIndex = ReadInt32(endianFormat, payload, trackStart + 20);
                headers.Add(header);
            }
            return headers;
        }

        public static Train UnpackTrainHeader(byte[] payload)
        {
            if (payload.Length < dbTrainHeadLength) return null;

            Train train = new Train();
            ReadHeader(train, payload, 0);
            train.TotalCalories = ReadInt16(endianFormat, payload, 30);
            train.MaximumSpeed = ReadInt16(endianFormat, payload, 32);
            train.MaximumHeartRate = payload[34];
            train.AverageHeartRate = payload[35];
            return train;
        }

        public static IList<Lap> UnpackLaps(byte[] payload)
        {
            if (payload.Length < dbTrainHeaderLength ||
                payload[dbTrainHeaderCType] != HeaderTypeLaps)
            {
                return new List<Lap>();
            }
            IList<Lap> laps = new List<Lap>();

            int offset = dbTrainHeaderLength;
            while (offset < payload.Length)
            {
                Lap lap = new Lap();

                lap.EndTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, payload, offset)) / 10);
                lap.LapTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, payload, offset + 4)) / 10);
                lap.LapDistanceMeters = ReadInt32(endianFormat, payload, offset + 8);
                lap.LapCalories = ReadInt16(endianFormat, payload, offset + 12);
                lap.MaximumSpeed = ReadInt32(endianFormat, payload, offset + 14);
                lap.MaximumHeartRate = payload[offset + 18];
                lap.AverageHeartRate = payload[offset + 19];
                //lap.StartPointIndex = ReadInt16(endianFormat, payload, 18);
                //lap.EndPointIndex = ReadInt16(endianFormat, payload, 20);
                laps.Add(lap);
                offset += dbLapLength;
            }
            return laps;
        }

        public static IList<TrackPoint4> UnpackTrackPoints(byte[] payload)
        {
            if (payload.Length < dbTrainHeaderLength ||
                payload[dbTrainHeaderCType] != HeaderTypeTrackPoints)
            {
                return new List<TrackPoint4>();
            }

            IList<TrackPoint4> points = new List<TrackPoint4>();

            int offset = dbTrainHeaderLength;
            while (offset < payload.Length)
            {
                TrackPoint4 point = new TrackPoint4();
                point.Latitude = ReadInt32(endianFormat, payload, offset);
                point.Longitude = ReadInt32(endianFormat, payload, offset + 4);
                point.Altitude = ReadInt16(endianFormat, payload, offset + 8);
                point.Speed = ReadInt32(endianFormat, payload, offset + 10);
                point.HeartRate = payload[offset + 14];
                point.IntervalTime = ReadInt32(endianFormat, payload, offset + 15);
                point.Cadence = ReadInt16(endianFormat, payload, offset + 19);
                point.Power = ReadInt16(endianFormat, payload, offset + 23);
                points.Add(point);
                offset += dbTrackPoint4Length;
            }
            return points;
        }

        //Both for DB_TRAINHEADER and DB_TRAIN
        private static void ReadHeader(Header header, byte[] payload, int offset)
        {
            header.StartTime = ReadDateTime(payload, offset).ToUniversalTime();
            header.TrackPointCount = ReadInt32(endianFormat, payload, offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(endianFormat, payload, offset + 10)) / 10);
            header.TotalDistanceMeters = ReadInt32(endianFormat, payload, offset + 14);
            header.LapCount = ReadInt16(endianFormat, payload, offset + 18);
        }

        const bool endianFormat = true; //bigEndian
    }
}
