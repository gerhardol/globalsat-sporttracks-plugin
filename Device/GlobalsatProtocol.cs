/*
 *  Globalsat/Keymaze SportTracks Plugin
 *  Copyright 2009 John Philip 
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
    public class GlobalsatProtocol : GhDeviceBase
    {
        public GlobalsatProtocol(DeviceConfigurationInfo configInfo) : base(configInfo) { }
        public GlobalsatProtocol(FitnessDevice_Globalsat fitDev) : base(fitDev) { }

        //The device creating the packet, used by Send()
        public override GlobalsatPacket PacketFactory { get { return new GlobalsatPacket(); } }

        //Import kept in separate structure
        public virtual ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return null;
        }

        public virtual void SendTrack(IGPSRoute gpsRoute, BackgroundWorker worker)
        {
            if (worker.CancellationPending)
            {
                return;
            }

            this.Open();
            try
            {
                IList<GlobalsatPacket> packets = PacketFactory.SendTrack(gpsRoute);

                int i = 0;
                foreach (GlobalsatPacket packet in packets)
                {
                    GlobalsatPacket response = null;
                    int nrAttempts = 0;

                    do
                    {

                        try
                        {
                            response = (GlobalsatPacket)this.SendPacket(packet);
                            break;
                        }
                        catch (Exception ex1)
                        {
                            nrAttempts++;
                            if (nrAttempts >= 3)
                            {
                                throw ex1;
                            }
                        }

                    }
                    while (nrAttempts < 3);

                    if (response != null)
                    {
                        if (response.CommandId == GhPacketBase.ResponseInsuficientMemory)
                        {
                            //throw new Exception(Properties.Resources.Device_InsuficientMemory_Error);
                        }
                        else if (response.CommandId == GhPacketBase.ResponseResendTrackSection)
                        {
                            // TODO resend
                            //throw new Exception(Properties.Resources.Device_SendTrack_Error);
                        }
                        else if (response.CommandId == GhPacketBase.ResponseSendTrackFinish)
                        {
                            return;
                        }
                        else
                        {
                            //throw new Exception(Properties.Resources.Device_SendTrack_Error);
                        }
                        //TODO:
                        throw new Exception("Send track error" + response.CommandId);
                    }

                    this.Port.Close();
                    this.Port.Open();

                    i++;
                    double progress = packets.Count <= 1 ? 100 : (double)(i * 100) / (double)(packets.Count - 1);

                    worker.ReportProgress((int)progress);

                    if (worker.CancellationPending)
                    {
                        return;
                    }
                }
                // should not reach here if finish ack was received
                //throw new Exception(Properties.Resources.Device_SendTrack_Error);
            }
            catch (Exception ex)
            {
                //TODO: popup instead of exception
                throw ex;
                //throw new Exception(Properties.Resources.Device_SendTrack_Error);
            }
            finally
            {
                this.Close();
            }
        }


        public virtual void SendRoute(GlobalsatRoute route)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.SendRoute(route);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
            }
            catch
            {
                //throw new Exception(Properties.Resources.Device_SendRoute_Error);
                throw;
            }
            finally
            {
                this.Close();
            }
        }

        public virtual GlobalsatPacket.GlobalsatSystemInformation GetSystemInformation()
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetSystemInformation();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                GlobalsatPacket.GlobalsatSystemInformation systemInfo = response.ResponseGetSystemInformation();
                return systemInfo;
            }
            catch
            {
                throw;
                //throw new Exception(Properties.Resources.Device_GetInfo_Error);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual GlobalsatDeviceConfiguration GetDeviceConfigurationData()
        {
            GlobalsatDeviceConfiguration devConfig = new GlobalsatDeviceConfiguration();

            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetSystemInformation();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                GlobalsatPacket.GlobalsatSystemInformation systemInfo = response.ResponseGetSystemInformation();
                devConfig.DeviceName = systemInfo.DeviceName;
                //devConfig.SystemInfoData = response.PacketData;

                packet = PacketFactory.GetSystemConfiguration();
                response = (GlobalsatPacket)this.SendPacket(packet);

                devConfig.SystemConfigData = response.PacketData;
                return devConfig;
            }
            catch
            {
                throw;
                //throw new Exception(Properties.Resources.Device_GetInfo_Error);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual void SetDeviceConfigurationData(GlobalsatDeviceConfiguration devConfig)
        {

            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.SetSystemConfiguration(devConfig.SystemConfigData);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
            }
            catch
            {
                throw;
                //throw new Exception(Properties.Resources.Device_GetInfo_Error);
            }
            finally
            {
                this.Close();
            }
        }

        public virtual IList<GlobalsatWaypoint> GetWaypoints()
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = new GlobalsatPacket(GhPacketBase.CommandGetWaypoints);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                IList<GlobalsatWaypoint> waypoints = response.ResponseGetWaypoints();

                return waypoints;
            }
            catch
            {
                //throw new Exception(Properties.Resources.Device_GetWaypoints_Error);
                throw;
            }
            finally
            {
                this.Close();
            }
        }

        public virtual int SendWaypoints(IList<GlobalsatWaypoint> waypoints)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.SendWaypoints(waypoints);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);

                // km500 no out of memory- waypoint overwritten
                int nrSentWaypoints = response.GetSentWaypoints();

                return nrSentWaypoints;
            }
            catch
            {
                //throw new Exception(Properties.Resources.Device_SendWaypoints_Error);
                throw;
            }
        }

        public virtual void DeleteWaypoints(IList<GlobalsatWaypoint> waypointNames)
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.DeleteWaypoints(this.configInfo.MaxNrWaypoints, waypointNames);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
            }
            catch
            {
                //throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error);
                throw;
            }
            finally
            {
                this.Close();
            }
        }


        public virtual void DeleteAllWaypoints()
        {
            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.DeleteAllWaypoints();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
            }
            catch
            {
                //throw new Exception(Properties.Resources.Device_DeleteWaypoints_Error);
                throw;
            }
            finally
            {
                this.Close();
            }
        }

        public virtual Bitmap GetScreenshot()
        {

            this.Open();
            try
            {
                GlobalsatPacket packet = PacketFactory.GetScreenshot();
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                return response.ResponseScreenshot();
            }
            catch
            {
                throw;
                throw new Exception(""); // TODO create message  Properties.Resources.Device_GetInfo_Error);
            }
            finally
            {
                this.Close();
            }
        }

        //public class GlobalsatSystemInformation
        //{

        //    public string UserName;
        //    public bool IsFemale;
        //    public int Age;
        //    public int WeightPounds;
        //    public int WeightKg;
        //    public int HeightInches;
        //    public int HeightCm;
        //    public DateTime BirthDate;

        //    public string DeviceName;
        //    public double Version;
        //    public string Firmware;
        //    public int WaypointCount;
        //    public int TrackpointCount;
        //    public int ManualRouteCount;
        //    public int PcRouteCount;



        //    public GlobalsatSystemInformation(string deviceName, double version, string firmware,
        //        string userName, bool isFemale, int age, int weightPounds, int weightKg, int heightInches, int heightCm, DateTime birthDate,
        //        int waypointCount, int trackpointCount, int manualRouteCount, int pcRouteCount)
        //    {
        //        this.UserName = userName;
        //        this.IsFemale = isFemale;
        //        this.Age = age;
        //        this.WeightPounds = weightPounds;
        //        this.WeightKg = weightKg;
        //        this.HeightInches = heightInches;
        //        this.HeightCm = heightCm;
        //        this.BirthDate = birthDate;
        //        this.DeviceName = deviceName;
        //        this.Version = version;
        //        this.Firmware = firmware;
        //        this.WaypointCount = waypointCount;
        //        this.TrackpointCount = trackpointCount;
        //        this.ManualRouteCount = manualRouteCount;
        //        this.PcRouteCount = pcRouteCount;
        //    }



        //    public GlobalsatSystemInformation(string deviceName, string firmware,
        //        int waypointCount, int pcRouteCount)
        //    {
        //        this.DeviceName = deviceName;
        //        this.Firmware = firmware;
        //        this.WaypointCount = waypointCount;
        //        this.PcRouteCount = pcRouteCount;
        //    }
        //}

        //xxx
        public class GlobalsatDeviceConfiguration {
            public string DeviceName;
            public byte[] SystemConfigData;
        }
    }
}
