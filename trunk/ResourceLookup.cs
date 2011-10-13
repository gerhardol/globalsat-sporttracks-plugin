/*
Copyright (C) 2010 Zone Five Software

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 3 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library. If not, see <http://www.gnu.org/licenses/>.
 */
// Author: Aaron Averill


using System;
using System.Collections.Generic;
using System.Threading;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    internal class ResourceLookup
    {
        internal static string DeviceConfigurationDlg_chkImportOnlyNew_Text
        {
            get { return GetString("DeviceConfigurationDlg_chkImportOnlyNew_Text"); }
        }

        internal static string GetString(string resourceId)
        {
            string[] locale = Thread.CurrentThread.CurrentUICulture.Name.Split('-');
            string text = null;
            if (locale.Length > 1) text = Properties.Resources.ResourceManager.GetString(locale[0] + "_" + locale[1] + "_" + resourceId);
            if (text == null && locale.Length > 0) text = Properties.Resources.ResourceManager.GetString(locale[0] + "_" + resourceId);
            if (text == null) text = Properties.Resources.ResourceManager.GetString(resourceId);
            if (text == null) text = "[MISSING: " + resourceId + "]";
            return text;
        }
    }
}
