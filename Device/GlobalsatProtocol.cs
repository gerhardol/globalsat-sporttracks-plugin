/*
 *  Globalsat/Keymaze SportTracks Plugin
 *  Copyright 2009 John Philip
 *  Copyright 2011 Gerhard Olsson
 * 
 *  This software may be used and distributed according to the terms of the
 *  GNU Lesser General Public License version 2 or any later version.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using System.IO.Ports;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public abstract class GlobalsatProtocol : GhDeviceBase
    {
        public GlobalsatProtocol() : base() { }
        public GlobalsatProtocol(string configInfo) : base(configInfo) { }

        public class FeatureNotSupportedException : NotImplementedException
        {
            //TODO: Popup with more information too?
        }
        //Import kept in separate structure, while most other protocols implemented here
        public virtual ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            throw new FeatureNotSupportedException();
        }

        public virtual int SendTrack(IList<IActivity> activities, IJobMonitor jobMonitor)
        {
            int result = 0;
            if (jobMonitor.Cancelled)
            {
                return result;
            }

            this.Open();
            try
            {
                foreach (IActivity activity in activities)
                {
                    IList<GlobalsatPacket> packets = SendTrackPackets(activity);

                    int i = 0;
                    foreach (GlobalsatPacket packet in packets)
                    {
                        GlobalsatPacket response =  (GlobalsatPacket)this.SendPacket(packet);
 
                        if (response != null && response.CommandId != packet.CommandId )
                        {
                            //Generic codes handled in SendPacket
                            if (response.CommandId == GhPacketBase.ResponseResendTrackSection)
                            {
                                // TODO resend
                                throw new Exception(Properties.Resources.Device_SendTrack_Error);
                            }
                            else if (response.CommandId == GhPacketBase.ResponseSendTrackFinish)
                            {
                                //Done, all sent
                                break;
                            }
							//	 Console.WriteLine("------ send error 4");
                            throw new Exception(Properties.Resources.Device_SendTrack_Error + response.CommandId);
                        }

                        i++;
                        float progress = (float)(packets.Count <= 1 ? 1 : (double)(i) / (double)(packets.Count - 1));

                        jobMonitor.PercentComplete = progress;

                        if (jobMonitor.Cancelled)
                        {
                            return result;
                        }
                    }
                    result++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Properties.Resources.Device_SendTrack_Error+ex);
            }
            finally
            {
                this.Close();
            }
            return result;
        }

        public virtual IList<GlobalsatPacket> SendTrackPackets(IActivity activity)
        {
            IList<GlobalsatPacket> sendTrackPackets = new List<GlobalsatPacket>();

            IGPSRoute gpsRoute = activity.GPSRoute;
            GhPacketBase.TrackFileBase trackFileStart = new GhPacketBase.TrackFileBase(
                activity.StartTime, TimeSpan.FromSeconds(gpsRoute.TotalElapsedSeconds), gpsRoute.TotalDistanceMeters);
            trackFileStart.TrackPointCount = (short)gpsRoute.Count;

			//Console.WriteLine("------ SendTrackStart()");
            GlobalsatPacket startPacket = PacketFactory.SendTrackStart(trackFileStart);
            sendTrackPackets.Add(startPacket);

            //Some protocols send laps in separate header
            //Use common code instead of overiding this method
            GlobalsatPacket lapPacket = PacketFactory.SendTrackLaps(trackFileStart);
            if (lapPacket != null)
            {
                sendTrackPackets.Add(lapPacket);
            }
            int nrPointsPerSection = startPacket.TrackPointsPerSection;

            for (int i = 0; i < gpsRoute.Count; i += nrPointsPerSection)
            {
                GhPacketBase.TrackFileSectionSend trackFileSection = new GhPacketBase.TrackFileSectionSend(trackFileStart);
                trackFileSection.StartPointIndex = (Int16)i;
                trackFileSection.EndPointIndex = (Int16)Math.Min(i + nrPointsPerSection - 1, gpsRoute.Count - 1);
                trackFileSection.TrackPoints = new List<GhPacketBase.TrackPointSend>();

                for (int j = trackFileSection.StartPointIndex; j <= trackFileSection.EndPointIndex; j++)
                {
                    IGPSPoint point = gpsRoute[j].Value;
                    GhPacketBase.TrackPointSend trackpoint = new GhPacketBase.TrackPointSend(point.LatitudeDegrees, point.LongitudeDegrees, 
                        (Int32)point.ElevationMeters);
                    uint intTime = 0;
                    float dist = 0;
                    if (i == 0)
                    {
                        trackpoint.IntervalTime = 0;
                    }
                    else
                    {
                        intTime = gpsRoute[j].ElapsedSeconds - gpsRoute[j - 1].ElapsedSeconds;
                        dist = gpsRoute[j].Value.DistanceMetersToPoint(gpsRoute[j-1].Value);
                    }
                    if (intTime > 0)
                    {
                        trackpoint.IntervalTime = intTime;
                        trackpoint.Speed = dist / intTime;
                    }
                    trackFileSection.TrackPoints.Add(trackpoint);
                }

    			//Console.WriteLine("------ SendTrackSection()");
                GlobalsatPacket pointsPacket = PacketFactory.SendTrackSection(trackFileSection);
                sendTrackPackets.Add(pointsPacket);
            }

            return sendTrackPackets;
        }

        //Not used as a separate protocol now
        //public virtual GlobalsatPacket.GlobalsatSystemInformation GetSystemConfiguration(IJobMonitor jobMonitor)
        //{
        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = PacketFactory.GetSystemConfiguration();
        //        GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

        //        GlobalsatPacket.GlobalsatSystemConfiguration systemInfo = response.ResponseGetSystemConfiguration();
        //        return systemInfo;
        //    }
        //    catch(Exception e)
        //    {
        //        throw new Exception(Properties.Resources.Device_GetInfo_Error+e);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}

        public virtual GlobalsatDeviceConfiguration GetSystemConfiguration2(IJobMonitor jobMonitor)
        {
            GlobalsatDeviceConfiguration devConfig = new GlobalsatDeviceConfiguration();

            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetSystemConfiguration();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                GlobalsatSystemConfiguration systemInfo = response.ResponseGetSystemConfiguration();
                devConfig.DeviceName = systemInfo.DeviceName;
                //devConfig.SystemInfoData = response.PacketData;

                packet = PacketFactory.GetSystemConfiguration2();
                response = (GlobalsatPacket)this.SendPacket(packet);

                devConfig.SystemConfigData = response.PacketData;
                return devConfig;
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_GetInfo_Error+e);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual void SetSystemConfiguration2(GlobalsatDeviceConfiguration devConfig, IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.SetSystemConfiguration2(devConfig.SystemConfigData);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                //No info in the response
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_GetInfo_Error+e);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual IList<GlobalsatWaypoint> GetWaypoints(IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetWaypoints();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                IList<GlobalsatWaypoint> waypoints = response.ResponseGetWaypoints();

                return waypoints;
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_GetWaypoints_Error+e);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual int SendWaypoints(IList<GlobalsatWaypoint> waypoints, IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.SendWaypoints(this.configInfo.MaxNrWaypoints, waypoints);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                // km500 no out of memory- waypoint overwritten
                int nrSentWaypoints = response.ResponseSendWaypoints();

                return nrSentWaypoints;
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_SendWaypoints_Error+e);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual void DeleteWaypoints(IList<GlobalsatWaypoint> waypointNames, IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.DeleteWaypoints(this.configInfo.MaxNrWaypoints, waypointNames);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error+e);
            }
            finally
            {
                this.Close();
            }
        }


        public virtual void DeleteAllWaypoints(IJobMonitor jobMonitor)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.DeleteAllWaypoints();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error+e);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual int SendRoute(IList<GlobalsatRoute> routes, IJobMonitor jobMonitor)
        {
            int res = 0;
            this.Open();
            try
            {
                GlobalsatPacket packet;
                GlobalsatPacket response;
                if (this.RouteRequiresWaypoints)
                {
                    packet = PacketFactory.GetWaypoints();
                    response = (GlobalsatPacket)this.SendPacket(packet);
                    IList<GlobalsatWaypoint> wptDev = response.ResponseGetWaypoints();

                    //Routes need waypoints - find those missing
                    IList<GlobalsatWaypoint> wptSend = new List<GlobalsatWaypoint>();
                    foreach (GlobalsatRoute route in routes)
                    {
                        foreach (GlobalsatWaypoint wpt1 in route.wpts)
                        {
                            bool found = false;
                            foreach (GlobalsatWaypoint wpt2 in wptDev)
                            {
                                if (GhPacketBase.ToGlobLatLon(wpt1.Latitude)  == GhPacketBase.ToGlobLatLon(wpt2.Latitude) &&
                                    GhPacketBase.ToGlobLatLon(wpt1.Longitude) == GhPacketBase.ToGlobLatLon(wpt2.Longitude))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                wptSend.Add(wpt1);
                            }
                        }
                    }

                    if (wptSend.Count > 0)
                    {
                        //Send with normal protocol, 625XT requires one by one
                        this.SendWaypoints(wptSend, jobMonitor);
                        this.Open();
                    }
                }

                //Finally the routes...
                foreach (GlobalsatRoute route in routes)
                {
                    packet = PacketFactory.SendRoute(route);
                    response = (GlobalsatPacket)this.SendPacket(packet);
                    res++;
                }
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_SendRoute_Error + e);
            }
            finally
            {
                this.Close();
            }
            return res;
        }

        public virtual Bitmap GetScreenshot(IJobMonitor jobMonitor)
        {

            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetScreenshot();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                return response.ResponseGetScreenshot();
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_GetInfo_Error+e); 
            }
            finally
            {
                this.Close();
            }
        }

        //Barometric devices
        public virtual bool HasElevationTrack { get { return false; } }
        public virtual bool RouteRequiresWaypoints { get { return true; } }
    }


    public abstract class GlobalsatProtocol2 : GlobalsatProtocol
    {
        public GlobalsatProtocol2() : base() { }
        public GlobalsatProtocol2(string configInfo) : base(configInfo) { }

        public override ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return new ImportJob2(this, sourceDescription, monitor, importResults);
        }

        public enum ReadMode
        {
            Header = 0x0,
            Laps = GlobalsatPacket.HeaderTypeLaps,
            Points = GlobalsatPacket.HeaderTypeTrackPoints
        }
        public virtual IList<GlobalsatPacket.TrackFileHeader> ReadTrackHeaders(IJobMonitor monitor)
        {
            //monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_OpeningDevice;

            GlobalsatPacket getHeadersPacket = PacketFactory.GetTrackFileHeaders();
            GlobalsatPacket2 response = (GlobalsatPacket2)SendPacket(getHeadersPacket);
            return response.UnpackTrackHeaders();
        }

        public virtual IList<GlobalsatPacket.Train> ReadTracks(IList<GlobalsatPacket.TrackFileHeader> tracks, IJobMonitor monitor)
        {
            if (tracks.Count == 0) return new GlobalsatPacket.Train[0];

            float totalPoints = 0;
            IList<Int16> trackIndexes = new List<Int16>();
            foreach (GlobalsatPacket.TrackFileHeader header in tracks)
            {
                totalPoints += header.TrackPointCount;
                //track number, less than 100
                trackIndexes.Add((Int16)header.TrackPointIndex);
            }
            float totalPointsRead = 0;

            IList<GlobalsatPacket.Train> trains = new List<GlobalsatPacket.Train>();
            GlobalsatPacket getFilesPacket = PacketFactory.GetTrackFileSections(trackIndexes);
            GlobalsatPacket getNextPacket = PacketFactory.GetNextTrackSection();
            GlobalsatPacket2 response = (GlobalsatPacket2)SendPacket(getFilesPacket);

            monitor.PercentComplete = 0;

            ReadMode readMode = ReadMode.Header;
            int trainLapsToRead = 0;
            int pointsToRead = 0;
            while (response.CommandId != GlobalsatPacket2.CommandId_FINISH && !monitor.Cancelled)
            {
                //Check that previous mode was finished, especially at corruptions there can be out of sync
                if (readMode != ReadMode.Header)
                {
                    byte readMode2 = response.GetTrainContent();
                    if (readMode2 == GlobalsatPacket.HeaderTypeTrackPoints)
                    {
                        if (readMode != ReadMode.Points)
                        {
                            //TODO: Handle error
                        }
                        readMode = ReadMode.Points;
                    }
                    else if (readMode2 == GlobalsatPacket.HeaderTypeLaps)
                    {
                        if (readMode != ReadMode.Laps)
                        {
                            //TODO: Handle error
                        }
                        readMode = ReadMode.Laps;
                    }
                    else
                    {
                        if (readMode != ReadMode.Header)
                        {
                            //TODO: Handle error
                            if (trains.Count > 0)
                            {
                                trains.RemoveAt(trains.Count - 1);
                            }
                        }
                        readMode = ReadMode.Header;
                    }
                }
                if (response.CommandId == GlobalsatPacket2.CommandId_FINISH)
                {
                    break;
                }
                switch (readMode)
                {
                    case ReadMode.Header:
                        {
                            GlobalsatPacket.Train train = response.UnpackTrainHeader();
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
                            GlobalsatPacket.Train currentTrain = trains[trains.Count - 1];
                            IList<GlobalsatPacket.Lap> laps = response.UnpackLaps();
                            foreach (GlobalsatPacket.Lap lap in laps) currentTrain.Laps.Add(lap);
                            trainLapsToRead -= laps.Count;
                            if (trainLapsToRead <= 0)
                            {
                                readMode = ReadMode.Points;
                            }
                            break;
                        }
                    case ReadMode.Points:
                        {
                            GlobalsatPacket.Train currentTrain = trains[trains.Count - 1];
                            IList<GlobalsatPacket.TrackPoint> points = response.UnpackTrackPoints();
                            foreach (GlobalsatPacket.TrackPoint point in points) currentTrain.TrackPoints.Add(point);
                            pointsToRead -= points.Count;
                            totalPointsRead += points.Count;
                            DateTime startTime = currentTrain.StartTime.ToLocalTime();
                            string statusProgress = startTime.ToShortDateString() + " " + startTime.ToShortTimeString();
                            monitor.StatusText = String.Format(CommonResources.Text.Devices.ImportJob_Status_Reading, statusProgress);
                            monitor.PercentComplete = totalPointsRead / totalPoints;
                            if (pointsToRead <= 0)
                            {
                                readMode = ReadMode.Header;
                            }
                            break;
                        }
                }
                response = (GlobalsatPacket2)SendPacket(getNextPacket);
            }

            monitor.PercentComplete = 1;
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_ImportComplete;
            return trains;
        }
    }
}
