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
using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;
#if !ST_2_1
using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Visuals.Util;
#endif


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
#if ST_2_1	
	class ExtendExportActions : IExtendRouteExportActions, IExtendActivityExportActions 
#else
	class ExtendExportActions : IExtendDailyActivityViewActions, IExtendActivityReportsViewActions, IExtendRouteViewActions
#endif
    {
#if ST_2_1	
        #region IExtendRouteExportActions Members

        public IList<IAction> GetActions(IList<IRoute> routes)
        {
            return null;
        }

        public IList<IAction> GetActions(IRoute route)
        {
            if (route == null || route.GPSRoute == null || route.GPSRoute.Count == 0) return null;

            return new IAction[] { 
                new ExportActivityToDeviceAction(route.GPSRoute)
            };
        }

        #endregion


        #region IExtendActivityExportActions Members

        public IList<IAction> GetActions(IList<IActivity> activities)
        {
            return null;
        }

        public IList<IAction> GetActions(IActivity activity)
        {
            if (activity == null || activity.GPSRoute == null || activity.GPSRoute.Count == 0) return null;

            return new IAction[] { 
                new ExportActivityToDeviceAction(activity.GPSRoute),
                new ExportRouteToDeviceAction(new GlobalsatRoute(activity.StartTime.ToShortDateString(), activity.GPSRoute))
            };
        }

        #endregion

#else
        #region IExtendDailyActivityViewActions Members
        public IList<IAction> GetActions(IDailyActivityView view,
                                                 ExtendViewActions.Location location)
        {
            if (location == ExtendViewActions.Location.ExportMenu)
            {
		        return new IAction[] 
				    { 
					    new ExportActivityToDeviceAction(view)
				    };
            }
            else return new IAction[0];
        }
        public IList<IAction> GetActions(IActivityReportsView view,
                                         ExtendViewActions.Location location)
        {
            if (location == ExtendViewActions.Location.ExportMenu)
            {
		        return new IAction[] 
				    { 
					    new ExportActivityToDeviceAction(view)
				    };
            }
            else return new IAction[0];
        }
		
		public IList<IAction> GetActions(IRouteView view, ExtendViewActions.Location location)
        {
            if (location == ExtendViewActions.Location.ExportMenu)
            {
		        return new IAction[] 
				    { 
					    new ExportActivityToDeviceAction(view),
				    };
            }
            else return new IAction[0];
        }
		
		#endregion
		
#endif
    }
}
