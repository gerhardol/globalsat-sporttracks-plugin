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
    class SettingsPage : ISettingsPage
    {
        #region ISettingsPage Members

        public Guid Id
        {
            get { return new Guid("{700e8ce0-ffff-11e0-be50-0800200c9a66}"); }
        }

        public IList<ISettingsPage> SubPages
        {
            get { return new ISettingsPage[] { new DeviceConfigurationSettingsPage(), new ScreenCapturePage() }; }
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

    }


    class DeviceConfigurationSettingsPage : ISettingsPage
    {
        #region ISettingsPage Members

        public Guid Id
        {
            get { return new Guid("{700e8ce1-ffff-11e0-be50-0800200c9a66}"); }
        }

        public IList<ISettingsPage> SubPages
        {
            get { return null; }
        }

        #endregion

        #region IDialogPage Members

        public System.Windows.Forms.Control CreatePageControl()
        {
            if (control == null)
            {
                control = new DeviceConfigurationSettingsControl();
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
            get { return Properties.Resources.UI_Settings_DeviceConfiguration_Title; }
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

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Private members
        private DeviceConfigurationSettingsControl control = null;
        #endregion

    }


    class ScreenCapturePage : ISettingsPage
    {
        #region ISettingsPage Members

        public Guid Id
        {
            get { return new Guid("{700e8ce2-ffff-11e0-be50-0800200c9a66}"); }
        }

        public IList<ISettingsPage> SubPages
        {
            get { return null; }
        }

        #endregion

        #region IDialogPage Members

        public System.Windows.Forms.Control CreatePageControl()
        {
            if (control == null)
            {
                control = new ScreenCaptureControl();
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
            get { return Properties.Resources.UI_Settings_ScreenCapture_Title; }
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
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Private members
        private ScreenCaptureControl control = null;
        #endregion

    }






}
