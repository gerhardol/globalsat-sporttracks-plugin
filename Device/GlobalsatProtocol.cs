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

        public override GlobalsatPacket PacketFactory { get { return new GlobalsatPacket(); } }

        //Import kept in separate structure
        public virtual ImportJob ImportJob(string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            return null;
        }

        //public void SendTrack(IGPSRoute gpsRoute, BackgroundWorker worker)
        //{
        //    if (worker.CancellationPending)
        //    {
        //        return;
        //    }

        //    this.Open();
        //    try
        //    {
        //        List<GlobalsatPacket> packets = _packetFactory.SendTrack(gpsRoute);

        //        int i = 0;
        //        foreach (GlobalsatPacket packet in packets)
        //        {
        //            GlobalsatPacket response = null;
        //            int nrAttempts = 0;

        //            do
        //            {
                        
        //                try
        //                {
        //                    response = this.SendPacket(packet);
        //                    break;
        //                }
        //                catch(Exception ex1)
        //                {
        //                    nrAttempts++;
        //                    if (nrAttempts >= 3)
        //                    {
        //                        throw ex1;
        //                    }
        //                }

        //            }
        //            while (nrAttempts < 3);

        //            if (response != null && response.CommandId != packet.PacketCommandId)
        //            {
        //                if (response.CommandId == GhPacketBase.ResponseInsuficientMemory)
        //                {
        //                    throw new Exception(Properties.Resources.Device_InsuficientMemory_Error);
        //                }
        //                if (response.CommandId == GhPacketBase.ResponseResendTrackSection)
        //                {
        //                    // TODO resend
        //                    throw new Exception(Properties.Resources.Device_SendTrack_Error);
        //                }
        //                else if (response.CommandId == GhPacketBase.ResponseSendTrackFinish)
        //                {
        //                    return;
        //                }
        //                else
        //                {
        //                    throw new Exception(Properties.Resources.Device_SendTrack_Error);
        //                }
        //            }

        //            device.Port.Close();
        //            device.Port.Open();

                    
        //            i++;
        //            double progress = packets.Count <= 1 ? 100 : (double)(i * 100) / (double)(packets.Count - 1);

        //            worker.ReportProgress((int)progress);


        //            if (worker.CancellationPending)
        //            {
        //                return;
        //            }

        //        }
                

        //        // should not reach here if finish ack was received
        //        //throw new Exception(Properties.Resources.Device_SendTrack_Error);
        //    }
        //    catch(Exception ex)
        //    {
        //        //TODO: popup instead of exception
        //        throw ex;
        //        //throw new Exception(Properties.Resources.Device_SendTrack_Error);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}


        //public void SendRoute(GlobalsatRoute route)
        //{
        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = _packetFactory.SendRoute(route);
        //        GlobalsatPacket response = device.SendPacket(packet);

        //    }
        //    catch
        //    {
        //        //throw new Exception(Properties.Resources.Device_SendRoute_Error);
        //        throw;
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}

        //public GlobalsatSystemInformation GetSystemInformation()
        //{
        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = GhPacketBase.GetSystemInformation();
        //        GlobalsatPacket response = device.SendPacket(packet);

        //        GlobalsatSystemInformation systemInfo = response.GetSystemInformation();
        //        return systemInfo;
        //    }
        //    catch
        //    {
        //        throw;
        //        throw new Exception(Properties.Resources.Device_GetInfo_Error);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}

        public IList<GlobalsatWaypoint> GetWaypoints()
        {

            this.Open();
            try
            {
                GlobalsatPacket packet = new GlobalsatPacket(GhPacketBase.CommandGetWaypoints);
                GlobalsatPacket response = (GlobalsatPacket)this.SendPacket(packet);
                IList<GlobalsatWaypoint> waypoints = response.ResponseWaypoints();

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
        public int SendWaypoints(List<GlobalsatWaypoint> waypoints)
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

        public void DeleteWaypoints(IList<string> waypointNames)
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


        public void DeleteAllWaypoints()
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

        //public void GetSystemConfiguration()
        //{
        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = _packetFactory.GetSystemConfiguration();
        //        GlobalsatPacket response = this.SendPacket(packet);

        //        GlobalsatSystemConfiguration systemConfig = response.GetSystemConfiguration();

        //        //return systemInfo;
        //    }
        //    catch
        //    {
        //        throw;
        //        throw new Exception("Error getting device config");
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}

        //public GlobalsatDeviceConfiguration GetDeviceConfigurationData()
        //{
        //    GlobalsatDeviceConfiguration devConfig = new GlobalsatDeviceConfiguration();

        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = _packetFactory.GetSystemInformation();
        //        GlobalsatPacket response = this.SendPacket(packet);

        //        GlobalsatSystemInformation systemInfo = response.GetSystemInformation();
        //        devConfig.DeviceName = systemInfo.DeviceName;
        //        //devConfig.SystemInfoData = response.PacketData;


        //        packet = _packetFactory.GetSystemConfiguration();
        //        response = this.SendPacket(packet);

        //        devConfig.SystemConfigData = response.PacketData;


        //        return devConfig;
        //    }
        //    catch
        //    {
        //        throw;
        //        throw new Exception(Properties.Resources.Device_GetInfo_Error);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}



        //public void SetDeviceConfigurationData(GlobalsatDeviceConfiguration devConfig)
        //{
            
        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = _packetFactory.SetSystemConfiguration(devConfig.SystemConfigData);
        //        this.SendPacket(packet);
               
        //    }
        //    catch
        //    {
        //        throw;
        //        throw new Exception(Properties.Resources.Device_GetInfo_Error);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}



        //public Bitmap GetScreenshot()
        //{

        //    this.Open();
        //    try
        //    {
        //        GlobalsatPacket packet = _packetFactory.GetScreenshot();
        //        GlobalsatPacket response = this.SendPacket(packet);
        //        return response.GetScreenshot();
        //    }
        //    catch
        //    {
        //        throw;
        //        throw new Exception(""); // TODO create message  Properties.Resources.Device_GetInfo_Error);
        //    }
        //    finally
        //    {
        //        this.Close();
        //    }
        //}
    }
}
