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

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public abstract class GlobalsatProtocol : GhDeviceBase
    {
        public GlobalsatProtocol(FitnessDevice_Globalsat fitnessDevice)
            : base(fitnessDevice)
        {
        }

        public class FeatureNotSupportedException : NotImplementedException
        {
            //This info should be handled by callers
            public FeatureNotSupportedException() : base(Properties.Resources.Device_Unsupported) { }
        }

        //Standard error text when a device is detected but "second protocol" times out
        public void NoCommunicationError(IJobMonitor jobMonitor)
        {
            if (!string.IsNullOrEmpty(this.devId))
            {
                jobMonitor.ErrorText = string.Format(Properties.Resources.Device_TurnOnNotConnected, this.devId);
            }
            else
            {
                jobMonitor.ErrorText = Properties.Resources.Device_OpenDevice_Error;
            }
        }

        //Import kept in separate structure, while most other protocols implemented here
        public virtual ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            throw new FeatureNotSupportedException();
        }

        public TimeSpan RemainingTime(IList<GlobalsatPacket.TrackFileHeader> headers)
        {
            //Find recording time in device
            int totalUsedPoints = 0;
            TimeSpan totalTime = TimeSpan.Zero;
            foreach (GlobalsatPacket.TrackFileHeader header in headers)
            {
                totalUsedPoints += header.TrackPointCount;
                totalTime += header.TotalTime;
            }
            int recordingInterval = 1; //Is in config, will not be correct when interval is changed
            if (totalUsedPoints > headers.Count * 2)
            {
                recordingInterval = (int)Math.Round(totalTime.TotalSeconds / totalUsedPoints);
            }

            TimeSpan remainTime = TimeSpan.FromSeconds((this.FitnessDevice.TotalPoints - totalUsedPoints) * recordingInterval);

            return remainTime;
        }

        public virtual int SendTrack(IList<GhPacketBase.Train> trains, IJobMonitor jobMonitor)
        {
            int result = 0;
            if (jobMonitor.Cancelled)
            {
                return result;
            }

            if (this.Open())
            {
                //Something to start the progress...
                jobMonitor.PercentComplete = 0.01F;
                try
                {
                    foreach (GhPacketBase.Train train in trains)
                    {
                        IList<GlobalsatPacket> packets = SendTrackPackets(train);

                        int i = 0;
                        foreach (GlobalsatPacket packet in packets)
                        {
                            GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                            if (response != null && response.CommandId != packet.CommandId)
                            {
                                //Generic codes handled in SendPacket
                                if (response.CommandId == GhPacketBase.ResponseSendTrackFinish)
                                {
                                    //Done, all sent
                                    break;
                                }
                                else if (response.CommandId == GhPacketBase.ResponseResendTrackSection)
                                {
                                    //Handled in SendPacket(), retry there or fail
                                    //throw new Exception(Properties.Resources.Device_SendTrack_Error);
                                }
                                //Console.WriteLine("------ send error 4");
                                throw new Exception(Properties.Resources.Device_SendTrack_Error + response.CommandId);
                            }

                            i++;
                            //Handle open packet as one, for each activity
                            float progress = (result + (1+1+i) / (float)(1+packets.Count))/(float)(trains.Count);

                            jobMonitor.PercentComplete = progress;

                            if (jobMonitor.Cancelled)
                            {
                                return result;
                            }
                        }
                        result++;

                        //Globalsat seem to be needing to wait in between activities....
                        if (trains.Count > 1)
                        {
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.DataRecieved)
                    {
                        jobMonitor.ErrorText = Properties.Resources.Device_SendTrack_Error + Environment.NewLine + ex.Message;
                        throw new Exception(Properties.Resources.Device_SendTrack_Error + ex);
                    }
                }
                finally
                {
                    this.Close();
                }
            }
            if (!this.DataRecieved)
            {
                NoCommunicationError(jobMonitor);
                result = 0;
            }
            return result;
        }

        public virtual IList<GlobalsatPacket> SendTrackPackets(GhPacketBase.Train trackFile)
        {
            IList<GlobalsatPacket> sendTrackPackets = new List<GlobalsatPacket>();

			//Console.WriteLine("------ SendTrackStart()");
            GlobalsatPacket startPacket = PacketFactory.SendTrackStart(trackFile);
            sendTrackPackets.Add(startPacket);

            //Some protocols send laps in separate header
            //Use common code instead of overiding this method
            GlobalsatPacket lapPacket = PacketFactory.SendTrackLaps(trackFile);
            if (lapPacket != null)
            {
                sendTrackPackets.Add(lapPacket);
            }

            for (int i = 0; i < trackFile.TrackPoints.Count; i += startPacket.TrackPointsPerSection)
            {
    			//Console.WriteLine("------ SendTrackSection()");
                GlobalsatPacket pointsPacket = PacketFactory.SendTrackSection(trackFile, i,
                    Math.Min(i + startPacket.TrackPointsPerSection - 1, trackFile.TrackPoints.Count - 1));
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
            //No need to check if device is connected
            GlobalsatDeviceConfiguration devConfig = new GlobalsatDeviceConfiguration();

            if (this.Open())
            {
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
                    throw new Exception(Properties.Resources.Device_GetInfo_Error + e);
                }
                finally
                {
                    this.Close();
                }
            }
            return null;
        }

        public virtual void SetSystemConfiguration2(GlobalsatDeviceConfiguration devConfig, IJobMonitor jobMonitor)
        {
            //No need to check if device is connected
            if (this.Open())
            {
                try
                {
                    GlobalsatPacket packet = PacketFactory.SetSystemConfiguration2(devConfig.SystemConfigData);
                    GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                    //No info in the response
                }
                catch (Exception e)
                {
                    throw new Exception(Properties.Resources.Device_GetInfo_Error + e);
                }
                finally
                {
                    this.Close();
                }
            }
        }

        public virtual IList<GlobalsatWaypoint> GetWaypoints(IJobMonitor jobMonitor)
        {
            IList<GlobalsatWaypoint> waypoints = null;
            if (this.Open())
            {
                try
                {
                    GlobalsatPacket packet = PacketFactory.GetWaypoints();
                    GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                    waypoints = response.ResponseGetWaypoints();

                }
                catch (Exception e)
                {
                    if (this.DataRecieved)
                    {
                        throw new Exception(Properties.Resources.Device_GetWaypoints_Error + e);
                    }
                }
                finally
                {
                    this.Close();
                }
            }
            if (!this.DataRecieved)
            {
                NoCommunicationError(jobMonitor);
                waypoints = null;
            }
            return waypoints;
        }

        public virtual int SendWaypoints(IList<GlobalsatWaypoint> waypoints, IJobMonitor jobMonitor)
        {
            int nrSentWaypoints = 0;
            if (this.Open())
            {
                try
                {
                    GlobalsatPacket packet = PacketFactory.SendWaypoints(this.FitnessDevice.configInfo.MaxNrWaypoints, waypoints);
                    GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                    // km500 no out of memory- waypoint overwritten
                    nrSentWaypoints = response.ResponseSendWaypoints();

                }
                catch (Exception e)
                {
                    if (this.DataRecieved)
                    {
                        throw new Exception(Properties.Resources.Device_SendWaypoints_Error + e);
                    }
                }
                finally
                {
                    this.Close();
                }
            }
            if (!this.DataRecieved)
            {
                NoCommunicationError(jobMonitor);
                nrSentWaypoints = 0;
            }
            return nrSentWaypoints;
        }

        public virtual bool DeleteWaypoints(IList<GlobalsatWaypoint> waypointNames, IJobMonitor jobMonitor)
        {
            if (this.Open())
            {
                try
                {
                    GlobalsatPacket packet = PacketFactory.DeleteWaypoints(this.FitnessDevice.configInfo.MaxNrWaypoints, waypointNames);
                    GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                }
                catch (Exception e)
                {
                    if (this.DataRecieved)
                    {
                        throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error + e);
                    }
                }
                finally
                {
                    this.Close();
                }
            }
            if (!this.DataRecieved)
            {
                NoCommunicationError(jobMonitor);
                return false;
            }
            return true;
        }


        public virtual bool DeleteAllWaypoints(IJobMonitor jobMonitor)
        {
            if (this.Open())
            {
                try
                {
                    GlobalsatPacket packet = PacketFactory.DeleteAllWaypoints();
                    GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                }
                catch (Exception e)
                {
                    if (this.DataRecieved)
                    {
                        throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error + e);
                    }
                }
                finally
                {
                    this.Close();
                }
            }
            if (!this.DataRecieved)
            {
                NoCommunicationError(jobMonitor);
                return false;
            }
            return true;
        }

        public virtual int SendRoute(IList<GlobalsatRoute> routes, IJobMonitor jobMonitor)
        {
            int res = 0;
            int totPackets = routes.Count;
            int extraPackets = 0; //Open, wpt etc
            if (this.Open())
            {
                try
                {
                    GlobalsatPacket packet;
                    GlobalsatPacket response;
                    if (this.FitnessDevice.RouteRequiresWaypoints)
                    {
                        packet = PacketFactory.GetWaypoints();
                        response = (GlobalsatPacket)this.SendPacket(packet);
                        IList<GlobalsatWaypoint> wptDev = response.ResponseGetWaypoints();
                        extraPackets++;
                        jobMonitor.PercentComplete = (float)(res + extraPackets) / (float)(totPackets + extraPackets);

                        //Routes need waypoints - find those missing
                        IList<GlobalsatWaypoint> wptSend = new List<GlobalsatWaypoint>();
                        foreach (GlobalsatRoute route in routes)
                        {
                            foreach (GlobalsatWaypoint wpt1 in route.wpts)
                            {
                                bool found = false;
                                foreach (GlobalsatWaypoint wpt2 in wptDev)
                                {
                                    if (GhPacketBase.ToGlobLatLon(wpt1.Latitude) == GhPacketBase.ToGlobLatLon(wpt2.Latitude) &&
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
                            extraPackets++;
                            jobMonitor.PercentComplete = (float)(res + extraPackets) / (float)(totPackets + extraPackets);
                            this.Open(); //Reopen
                            extraPackets++;
                            jobMonitor.PercentComplete = (float)(res + extraPackets) / (float)(totPackets + extraPackets);
                        }
                    }

                    //Finally the routes...
                    foreach (GlobalsatRoute route in routes)
                    {
                        packet = PacketFactory.SendRoute(route);
                        response = (GlobalsatPacket)this.SendPacket(packet);
                        res++;
                        jobMonitor.PercentComplete = (float)(res + extraPackets) / (float)(totPackets + extraPackets);
                    }
                }
                catch (Exception e)
                {
                    if (this.DataRecieved)
                    {
                        if (e is InsufficientMemoryException)
                        {
                            throw e;
                        }
                        throw new Exception(Properties.Resources.Device_SendRoute_Error + e);
                    }
                }
                finally
                {
                    this.Close();
                }
            }
            if (!this.DataRecieved)
            {
                NoCommunicationError(jobMonitor);
                return 0;
            }
            return res;
        }

        public virtual Bitmap GetScreenshot(IJobMonitor jobMonitor)
        {
            //Note: No check for connected here
            if (this.Open())
            {
                try
                {
                    GlobalsatPacket packet = PacketFactory.GetScreenshot();
                    GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                    System.Drawing.RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone;
                    if (this.FitnessDevice.CanRotateScreen)
                    {
                        //try getting screen orientaion
                        try
                        {
                            packet = PacketFactory.GetSystemConfiguration2();
                            GlobalsatPacket response2 = (GlobalsatPacket)this.SendPacket(packet);
                            GlobalsatSystemConfiguration2 systemInfo = response2.ResponseGetSystemConfiguration2();
                            if (systemInfo.ScreenOrientation == 1)
                            {
                                rotate = RotateFlipType.Rotate90FlipNone;
                            }
                        }
                        catch{}
                    }
                    return response.ResponseGetScreenshot(rotate);
                }
                catch (Exception e)
                {
                    throw new Exception(Properties.Resources.Device_GetInfo_Error + e);
                }
                finally
                {
                    this.Close();
                }
            }
            return null;
        }
    }


    public abstract class GlobalsatProtocol2 : GlobalsatProtocol
    {
        public GlobalsatProtocol2(FitnessDevice_Globalsat fitnessDevice) : base(fitnessDevice) { }

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

            GlobalsatPacket getFilesPacket = PacketFactory.GetTrackFileSections(trackIndexes);
            GlobalsatPacket2 response = (GlobalsatPacket2)SendPacket(getFilesPacket);

            monitor.PercentComplete = 0;

            IList<GlobalsatPacket.Train> trains = new List<GlobalsatPacket.Train>();
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
                                trainLapsToRead = train.LapCount;
                                pointsToRead = train.TrackPointCount;
                                trains.Add(train);
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
                //All requests are the same
                response = (GlobalsatPacket2)SendPacket(PacketFactory.GetNextTrackSection());
            }

            monitor.PercentComplete = 1;
            monitor.StatusText = CommonResources.Text.Devices.ImportJob_Status_ImportComplete;
            return trains;
        }

        public virtual void DeleteAllTracks()
        {
            try
            {
                IJobMonitor jobMonitor = new JobMonitor();
                if (this.Open())
                {
                    //IList<GlobalsatPacket.TrackFileHeader> headers = ((GlobalsatProtocol2)device).ReadTrackHeaders(jobMonitor);
                    //List<GlobalsatPacket.TrackFileHeader> fetch = new List<GlobalsatPacket.TrackFileHeader>();
                    GlobalsatPacket getDeleteAllTracks = PacketFactory.GetDeleteAllTracks();
                    GlobalsatPacket2 response = (GlobalsatPacket2)SendPacket(getDeleteAllTracks);
                }
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_OpenDevice_Error + e);
            }
            finally
            {
                this.Close();
            }
        }

        //not working, deletes all...
        public virtual int DeleteTracks(DateTime oldest)
        {
            int res = -1;
            try
            {
                IJobMonitor jobMonitor = new JobMonitor();
                if (this.Open())
                {
                    IList<GlobalsatPacket.TrackFileHeader> headers = this.ReadTrackHeaders(jobMonitor);
                    IList<GlobalsatPacket.TrackFileHeader> fetch = new List<GlobalsatPacket.TrackFileHeader>();
                    foreach (GlobalsatPacket.TrackFileHeader t in headers)
                    {
                        if (t.StartTime < oldest)
                        {
                            fetch.Add(t);
                        }
                    }
                    res = fetch.Count;
                    if (fetch.Count > 0)
                    {
                        GlobalsatPacket getDeleteTracks = PacketFactory.GetDeleteTracks(fetch);
                        GlobalsatPacket2 response = (GlobalsatPacket2)SendPacket(getDeleteTracks);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_OpenDevice_Error + e);
            }
            finally
            {
                this.Close();
            }
            return res;
        }

        public TimeSpan GetRemainingTime()
        {
            TimeSpan res = TimeSpan.MinValue;
            try
            {
                IJobMonitor jobMonitor = new JobMonitor();
                if (this.Open())
                {
                    IList<GlobalsatPacket.TrackFileHeader> headers = this.ReadTrackHeaders(jobMonitor);
                    res = this.RemainingTime(headers);
                }
            }
            catch (Exception e)
            {
                throw new Exception(Properties.Resources.Device_OpenDevice_Error + e);
            }
            finally
            {
                this.Close();
            }
            return res;
        }
    }
}
