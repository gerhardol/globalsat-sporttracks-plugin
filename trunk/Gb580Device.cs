using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gb580Device : GhDeviceBase
    {
        public IList<Gb580Packet.Train> ReadTrainHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            byte[] getHeadersPacket = Gb580Packet.GetTrackFileHeaders();
            byte[] data = SendPacket(Port, getHeadersPacket).PacketData;
            return Gb580Packet.UnpackTrainHeaders(data);
        }

        private enum ReadMode
        {
            Header,
            Laps,
            Points
        }

        public IList<Gb580Packet.Train> ReadTracks(IList<Gb580Packet.Train> headers, IJobMonitor monitor)
        {
            if (headers.Count == 0) return new Gb580Packet.Train[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (Gb580Packet.Train header in headers)
            {
                totalPoints += header.TrackPointCount;
                trackIndexes.Add(header.IndexStartPt);
            }
            float pointsRead = 0;

            IList<Gb580Packet.Train> trains = new List<Gb580Packet.Train>();
            byte[] getFilesPacket = Gb580Packet.GetTrackFileSections(trackIndexes);
            byte[] getNextPacket = Gb580Packet.GetNextSection();
            GhPacketBase.Response response = SendPacket(Port, getFilesPacket);

            monitor.PercentComplete = 0;

            ReadMode readMode = ReadMode.Header;
            int trainLapsToRead = 0;
            int pointsToRead = 0;
            while (response.CommandId != Gb580Packet.CommandId_FINISH)
            {
                byte[] data = response.PacketData;
                switch (readMode)
                {
                    case ReadMode.Header:
                        {
                            Gb580Packet.Train train = Gb580Packet.UnpackTrainHeader(data);
                            if (train != null)
                            {
                                trains.Add(train);
                                trainLapsToRead = train.LapCount;
                                pointsToRead = train.TrackPointCount;
                            }
                            readMode = ReadMode.Laps;
                            break;
                        }
                    case ReadMode.Laps:
                        {
                            Gb580Packet.Train currentTrain = trains[trains.Count - 1];
                            IList<Gb580Packet.Lap> laps = Gb580Packet.UnpackLaps(data);
                            foreach (Gb580Packet.Lap lap in laps) currentTrain.Laps.Add(lap);
                            trainLapsToRead -= laps.Count;
                            if (trainLapsToRead == 0)
                            {
                                readMode = ReadMode.Points;
                            }
                            break;
                        }
                    case ReadMode.Points:
                        {
                            Gb580Packet.Train currentTrain = trains[trains.Count - 1];
                            IList<GhPacketBase.TrackPoint3> points = Gb580Packet.UnpackTrackPoints(data);
                            foreach (GhPacketBase.TrackPoint3 point in points) currentTrain.TrackPoints.Add(point);
                            pointsToRead -= points.Count;
                            pointsRead += points.Count;
                            DateTime startTime = currentTrain.StartTime.ToLocalTime();
                            string statusProgress = startTime.ToShortDateString() + " " + startTime.ToShortTimeString();
                            monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, statusProgress);
                            monitor.PercentComplete = pointsRead / totalPoints;
                            if (pointsToRead == 0)
                            {
                                readMode = ReadMode.Header;
                            }
                            break;
                        }

                }
                response = SendPacket(Port, getNextPacket);
            }

            monitor.PercentComplete = 1;
            return trains;
        }
    }
}
