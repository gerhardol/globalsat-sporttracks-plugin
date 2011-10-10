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
