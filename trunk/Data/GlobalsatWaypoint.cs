/*
 *  Globalsat/Keymaze SportTracks Plugin
 *  Copyright 2009 John Philip 
 * 
 *  This software may be used and distributed according to the terms of the
 *  GNU Lesser General Public License version 2 or any later version.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    //Sample implementation, not used right now
    public class JobMonitor : ZoneFiveSoftware.Common.Visuals.IJobMonitor
    {
        public bool Cancelled { get { return false; } }
        public string ErrorText { set { } }
        public float PercentComplete { set { } }
        public string StatusText { set { } }
    }

    public class GlobalsatWaypoint : IComparable
    {
        private string _waypointName;
        private int _iconNr;
        private short _altitude;
        private double _latitude;
        private double _longitude;

        public GlobalsatWaypoint(string waypointName, int iconNr, short altitude, double latitude, double longitude)
        {
//			    Console.WriteLine("GlobalsatWaypoint::GlobalsatWaypoint(name:" + waypointName + ",iconnr:"+iconNr+",altitude:"+altitude+",lat:"+latitude+", lon:" + longitude + ")");
            this.WaypointName = waypointName;
            this.IconNr = iconNr;
            this.Altitude = altitude;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        // tree list don't acept objects with the same id - not needed anymore?
        //public GlobalsatWaypoint ChangeNameRef()
        //{
        //    //this.WaypointName = this.WaypointName + "";
        //    return this;
        //}

        public GlobalsatWaypoint Clone()
        {
            GlobalsatWaypoint waypoint = new GlobalsatWaypoint(this.WaypointName + "", this.IconNr, this.Altitude, this.Latitude, this.Longitude);
            return waypoint;
        }

        public string WaypointName
        {
            get { return _waypointName; }
            set { _waypointName = value; }
        }

        public int IconNr
        {
            get { return _iconNr; }
            set
            {
                if (value < 0) value = 0;
                if (value > 35) value = 35;
                _iconNr = value;
            }
        }
        public short Altitude
        {
            get
            {
                return this._altitude;
            }
            set
            {
                this._altitude = value;
            }
        }
        public double Latitude
        {
            get
            {
                return this._latitude;
            }
            set
            {
                this._latitude = value;
            }
        }
        public double Longitude
        {
            get
            {
                return this._longitude;
            }
            set
            {
                this._longitude = value;
            }
        }

        public Bitmap GetIconByIndex()
        {
            return GetIconByIndex(this.IconNr + 1);
        }

        public static Bitmap GetIconByIndex(int iconNr)
        {
            return Properties.Resources.ResourceManager.GetObject("WaypointIcon" + iconNr) as Bitmap;
        }

        public override string ToString()
        {
            return this.WaypointName + " " + this.IconNr + " " + this.Latitude + " " + this.Longitude + " " + this.Altitude;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is GlobalsatWaypoint))
            {
                return -1;
            }
                GlobalsatWaypoint g = obj as GlobalsatWaypoint;
                if (this._waypointName.CompareTo(g._waypointName) < 0 ||
                    this._iconNr < g._iconNr ||
                    this.Latitude < g.Latitude ||
                    this.Longitude < g.Longitude ||
                    this.Altitude < g.Altitude)
                {
                    return -1;
                }
                else if (this._waypointName.CompareTo(g._waypointName) > 0 ||
                    this._iconNr > g._iconNr ||
                    this.Latitude > g.Latitude ||
                    this.Longitude > g.Longitude ||
                    this.Altitude > g.Altitude)
                {
                    return 1;
                }
                return 0;
        }
    }
}
