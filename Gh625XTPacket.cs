// Copyright (C) 2010 Zone Five Software
// Author: Aaron Averill
using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh625XTPacket : GhPacketBase
    {
        public static byte CommandId_FINISH = 0x8A;

        public class Header
        {
            public DateTime StartTime;
            public Int32 TrackPointCount;
            public TimeSpan TotalTime;
            public Int32 TotalDistanceMeters;
            public short LapCount;
        }

        public class TrackFileHeader : Header
        {
            public Int32 TrackPointIndex;
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
            public Int32 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
        //    public Int16 StartPointIndex;
        //    public Int16 EndPointIndex;
        }

        const int dbTrainHeaderLength = 29;
        const int dbTrainHeaderCType = 28;
        const int dbTrainHeadLength = 58;
        const int dbLapLength = 41;
        const int dbRecPointLength = 25;

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
            int numHeaders = payload.Length / dbTrainHeaderLength;
            IList<TrackFileHeader> headers = new List<TrackFileHeader>();
            for (int i = 0; i < numHeaders; i++)
            {
                int trackStart = i * dbTrainHeaderLength;
                TrackFileHeader header = new TrackFileHeader();
                ReadHeader(header, payload, trackStart);
                header.TrackPointIndex = ReadInt32(payload, trackStart + 20);
                headers.Add(header);
            }
            return headers;
        }

        public static Train UnpackTrainHeader(byte[] payload)
        {
            if (payload.Length < dbTrainHeadLength) return null;

            Train train = new Train();
            ReadHeader(train, payload, 0);
            train.TotalCalories = ReadInt16(payload, 30);
            train.MaximumSpeed = ReadInt16(payload, 32);
            train.MaximumHeartRate = payload[34];
            train.AverageHeartRate = payload[35];
            return train;
        }

        public static IList<Lap> UnpackLaps(byte[] payload)
        {
            if (payload.Length < dbTrainHeaderLength ||
                payload[dbTrainHeaderCType] != 0xaa)
            {
                return new List<Lap>();
            }
            IList<Lap> laps = new List<Lap>();

            int offset = dbTrainHeaderLength;
            while (offset < payload.Length)
            {
                Lap lap = new Lap();

                lap.EndTime = TimeSpan.FromSeconds(((double)ReadInt32(payload, offset)) / 10);
                lap.LapTime = TimeSpan.FromSeconds(((double)ReadInt32(payload, offset + 4)) / 10);
                lap.LapDistanceMeters = ReadInt32(payload, offset + 8);
                lap.LapCalories = ReadInt16(payload, offset + 12);
                lap.MaximumSpeed = ReadInt32(payload, offset + 14);
                lap.MaximumHeartRate = payload[offset + 18];
                lap.AverageHeartRate = payload[offset + 19];
                //lap.StartPointIndex = ReadInt16(payload, 18);
                //lap.EndPointIndex = ReadInt16(payload, 20);
                laps.Add(lap);
                offset += dbLapLength;
            }
            return laps;
        }

        public static IList<TrackPoint2> UnpackTrackPoints(byte[] payload)
        {
            if (payload.Length < dbTrainHeaderLength ||
                payload[dbTrainHeaderCType] != 0x55)
            {
                return new List<TrackPoint2>();
            }

            IList<TrackPoint2> points = new List<TrackPoint2>();

            int offset = dbTrainHeaderLength;
            while (offset < payload.Length)
            {
                TrackPoint2 point = new TrackPoint2();
                point.Latitude = ReadInt32(payload, offset);
                point.Longitude = ReadInt32(payload, offset + 4);
                point.Altitude = ReadInt16(payload, offset + 8);
                point.Speed = (short)ReadInt32(payload, offset + 10);
                point.HeartRate = payload[offset + 14];
                point.IntervalTime = ReadInt32(payload, offset + 15);
                point.Cadence = ReadInt16(payload, offset + 19);
                point.Power = ReadInt16(payload, offset + 23);
                points.Add(point);
                offset += dbRecPointLength;
            }
            return points;
        }

        private Gh625XTPacket()
        {
        }


        //Both for DB_TRAINHEADER and DB_TRAIN
        private static void ReadHeader(Header header, byte[] payload, int offset)
        {
            header.StartTime = ReadDateTime(payload, offset).ToUniversalTime();
            header.TrackPointCount = ReadInt32(payload, offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(payload, offset + 10)) / 10);
            header.TotalDistanceMeters = ReadInt32(payload, offset + 14);
            header.LapCount = ReadInt16(payload, offset + 18);
        }
    }
}
