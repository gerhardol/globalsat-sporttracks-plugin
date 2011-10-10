// Copyright (C) 2010 Zone Five Software
// Author: Aaron Averill
using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class Gh625XTDevice : GhDeviceBase
    {
        public IList<Gh625XTPacket.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            byte[] getHeadersPacket = Gh625XTPacket.GetTrackFileHeaders();
            byte[] data = SendPacket(Port, getHeadersPacket).PacketData;
            return Gh625XTPacket.UnpackTrackHeaders(data);
        }

        private enum ReadMode
        {
            Header,
            Laps,
            Points
        }

        public IList<Gh625XTPacket.Train> ReadTracks(IList<Gh625XTPacket.TrackFileHeader> tracks, IJobMonitor monitor)
        {
            if (tracks.Count == 0) return new Gh625XTPacket.Train[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (Gh625XTPacket.TrackFileHeader header in tracks)
            {
                totalPoints += header.TrackPointCount;
                //track number, less than 100
                trackIndexes.Add((Int16)header.TrackPointIndex);
            }
            float pointsRead = 0;

            IList<Gh625XTPacket.Train> trains = new List<Gh625XTPacket.Train>();
            byte[] getFilesPacket = Gh625XTPacket.GetTrackFileSections(trackIndexes);
            byte[] getNextPacket = Gh625XTPacket.GetNextSection();
            GhPacketBase.Response response = SendPacket(Port, getFilesPacket);

            monitor.PercentComplete = 0;

            ReadMode readMode = ReadMode.Header;
            int trainLapsToRead = 0;
            int pointsToRead = 0;
            while (response.CommandId != Gh625XTPacket.CommandId_FINISH)
            {
                byte[] data = response.PacketData;
                switch (readMode)
                {
                    case ReadMode.Header:
                        {
                            Gh625XTPacket.Train train = Gh625XTPacket.UnpackTrainHeader(data);
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
                            Gh625XTPacket.Train currentTrain = trains[trains.Count - 1];
                            IList<Gh625XTPacket.Lap> laps = Gh625XTPacket.UnpackLaps(data);
                            foreach (Gh625XTPacket.Lap lap in laps) currentTrain.Laps.Add(lap);
                            trainLapsToRead -= laps.Count;
                            if (trainLapsToRead == 0)
                            {
                                readMode = ReadMode.Points;
                            }
                            break;
                        }
                    case ReadMode.Points:
                        {
                            Gh625XTPacket.Train currentTrain = trains[trains.Count - 1];
                            IList<GhPacketBase.TrackPoint2> points = Gh625XTPacket.UnpackTrackPoints(data);
                            foreach (GhPacketBase.TrackPoint2 point in points) currentTrain.TrackPoints.Add(point);
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
