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

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;


namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class SettingsPage : ISettingsPage, IDisposable
    {
        #region ISettingsPage Members

        public Guid Id
        {
#if GLOBALSAT_DEVICE
            get { return new Guid("{700e8ce0-ffff-11e0-be50-0800200c9a66}"); }
#else
            get { return new Guid("{4ab72f68-f02e-4b02-a944-7bf1e177205b}"); }
#endif
        }

        public IList<ISettingsPage> SubPages
        {
            //moved to device dialog
            //get { return new ISettingsPage[] { new DeviceConfigurationSettingsPage(), new ScreenCapturePage() }; }
            get { return null; }
        }
        
        #endregion

        #region IDialogPage Members

        public System.Windows.Forms.Control CreatePageControl()
        {
            if (control == null)
            {
                control = new SettingsControl();
            }
            return control;
        }

        public bool HidePage()
        {
            return true;
        }

        public string PageName
        {
            get { return this.Title; }
        }

        public void ShowPage(string bookmark)
        {
            if (control != null)
            {
                control.ShowPage();
            }
        }

        public IPageStatus Status
        {
            get { return null; }
        }

        public void ThemeChanged(ITheme visualTheme)
        {
            if (control != null)
            {
                control.ThemeChanged(visualTheme);
            }
        }

        public string Title
        {
            get { return Plugin.Instance.Name; }
        }

        public void UICultureChanged(System.Globalization.CultureInfo culture)
        {
            if (control != null)
            {
                control.UICultureChanged(culture);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

#pragma warning disable 67
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private members
        private SettingsControl control = null;
        #endregion


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.control.Dispose();
            }
        }
    }

}
