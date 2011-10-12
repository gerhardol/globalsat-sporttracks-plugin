using System;
using System.Collections.Generic;
using System.Text;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gb580Packet : GhPacketBase
    {
        public static byte CommandId_FINISH = 0x8A;

        public class Header
        {
            public DateTime StartTime;
            public Int16 TrackPointCount;
            public TimeSpan TotalTime;
            public Int32 TotalDistanceMeters;
            public Int16 LapCount;
        }

        //public class TrackFileHeader : Header
        //{
        //    public Int16 TrackPointIndex;
        //}

        public class Train : Header
        {
            public Int16 IndexStartPt;
            public Int16 LapIndexEndPt;
            public Int16 TotalCalories;
            public Int16 MaximumSpeed;
            public byte MaximumHeartRate;
            public byte AverageHeartRate;
        //    public Int16 StartPointIndex;
        //    public Int16 EndPointIndex;
            public IList<TrackPoint3> TrackPoints = new List<TrackPoint3>();
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
            payload[0] = 0x80;
            Write(payload, 1, (Int16)trackPointIndexes.Count);
            int offset = 3;
            foreach (Int16 index in trackPointIndexes)
            {
                Write(payload, offset, index);
                offset += 2;
            }
            return ConstructPayload(payload);
        }

        public static IList<Train> UnpackTrainHeaders(byte[] payload)
        {
            int numTrains = payload.Length / 24;
            IList<Train> trains = new List<Train>();
            for (int i = 0; i < numTrains; i++)
            {
                int trackStart = i * 24;
                Train train = new Train();
                ReadHeader(train, payload, trackStart);
                train.IndexStartPt = ReadInt16(payload, trackStart + 18);
                train.LapIndexEndPt = ReadInt16(payload, trackStart + 20);
                trains.Add(train);
            }
            return trains;
        }

        public static Train UnpackTrainHeader(byte[] payload)
        {
            if (payload.Length < 52) return null;

            Train train = new Train();
            ReadHeader(train, payload, 0);
            train.TotalCalories = ReadInt16(payload, 24);
            train.MaximumSpeed = ReadInt16(payload, 28);
            train.MaximumHeartRate = payload[32];
            train.AverageHeartRate = payload[33];
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

                lap.EndTime = TimeSpan.FromSeconds(((double)ReadInt32(payload, offset)) / 10);
                lap.LapTime = TimeSpan.FromSeconds(((double)ReadInt32(payload, offset + 4)) / 10);
                lap.LapDistanceMeters = ReadInt32(payload, offset + 8);
                lap.LapCalories = ReadInt16(payload, offset + 12);
                lap.MaximumSpeed = ReadInt16(payload, offset + 16);
                lap.MaximumHeartRate = payload[offset + 20];
                lap.AverageHeartRate = payload[offset + 21];
        //        //lap.StartPointIndex = ReadInt16(payload, 18);
        //        //lap.EndPointIndex = ReadInt16(payload, 20);
                laps.Add(lap);
                offset += 40;
            }
            return laps;
        }

        public static IList<TrackPoint3> UnpackTrackPoints(byte[] payload)
        {
            if (payload.Length < 24) return new List<TrackPoint3>();

            IList<TrackPoint3> points = new List<TrackPoint3>();

            int offset = 24;
            while (offset < payload.Length)
            {
                TrackPoint3 point = new TrackPoint3();
                point.Latitude = ReadInt32(payload, offset);
                point.Longitude = ReadInt32(payload, offset + 4);
                point.Altitude = ReadInt32(payload, offset + 8);
                point.Speed = ReadInt16(payload, offset + 12);
                point.HeartRate = payload[offset + 16];
                point.IntervalTime = ReadInt32(payload, offset + 20);
                point.Cadence = ReadInt16(payload, offset + 24);
                point.Power = ReadInt16(payload, offset + 28);
                points.Add(point);
                offset += 32;
            }
            return points;
        }

        private Gb580Packet()
        {
        }

        private static void ReadHeader(Header header, byte[] payload, int offset)
        {
            header.StartTime = ReadDateTime(payload, offset).ToUniversalTime();
            header.TrackPointCount = ReadInt16(payload, offset + 6);
            header.TotalTime = TimeSpan.FromSeconds(((double)ReadInt32(payload, offset + 8)) / 10);
            header.TotalDistanceMeters = ReadInt32(payload, offset + 12);
            header.LapCount = ReadInt16(payload, offset + 16);
        }
    }
}
