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

using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;



namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public class GlobalsatRoute
    {
        public const int MaxRouteNameLength = 15;

        public IGPSRoute GPSRoute;
        public string Name;

        public GlobalsatRoute()
        {
            this.Name = "";
            this.GPSRoute = new GPSRoute();
        }

        public GlobalsatRoute(string name, IGPSRoute route)
        {
            this.Name = name;
            this.GPSRoute = route;
        }


    }
}
