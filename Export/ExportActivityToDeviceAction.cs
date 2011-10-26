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
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.ComponentModel;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;
using ZoneFiveSoftware.Common.Visuals;

#if !ST_2_1
using ZoneFiveSoftware.Common.Visuals.Fitness;
using ZoneFiveSoftware.Common.Visuals.Util;
#endif

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class ExportActivityToDeviceAction : IAction
    {

#if ST_2_1
        public ExportActivityToDeviceAction(IGPSRoute gpsRoute)
        {
            _gpsRoute = gpsRoute;
            _running = false;

            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;

            _progressForm = new ProgressForm();
            _progressForm.Cancelled += new EventHandler(_progressForm_Cancelled);
        }
#else
		public ExportActivityToDeviceAction(IActivityReportsView view)
        {
			this.activetyreportview = view;
		}
		public ExportActivityToDeviceAction(IRouteView view)
        {
			this.routeview = view;
		}
		public ExportActivityToDeviceAction(IDailyActivityView view)
        {
 			this.dailyactivetyview = view;
		}		
#endif

        #region IAction Members

        public bool Enabled
        {
#if ST_2_1
			get { return !_running && _gpsRoute != null && _gpsRoute.Count > 0; }
#else
			get { return activities != null; }
#endif
        }

        public bool HasMenuArrow
        {
            get { return false; }
        }

        public Image Image
        {
            get { return Properties.Resources.Image_16_Keymaze; }
        }

        public void Refresh()
        {
        }

        public void Run(Rectangle rectButton)
        {
#if ST_2_1
			_running = true;
#else

            foreach (IActivity activity in activities)
            {
                //_gpsRoute = activity.GPSRoute;
                _activity = activity;

                _backgroundWorker = new BackgroundWorker();
                _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
                _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
                _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
                _backgroundWorker.WorkerReportsProgress = true;
                _backgroundWorker.WorkerSupportsCancellation = true;


#endif	

            _progressForm = new ProgressForm();
            _progressForm.Cancelled += new EventHandler(_progressForm_Cancelled);
            _progressForm.Text = CommonResources.Text.ActionExport;// Properties.Resources.UI_ExportActivityToDeviceForm_Title;
            _progressForm.Title = Properties.Resources.UI_ExportActivityToDeviceForm_Title;
            _progressForm.Progress = 0;


#if ST_2_1
            try
            {

                _backgroundWorker.RunWorkerAsync();
                _progressForm.ShowDialog();

            }
            finally
            {
                _running = false;
            }
#else
                _backgroundWorker.RunWorkerAsync();
                _progressForm.ShowDialog();
			}
#endif
        }


        public string Title
        {
            get { return Properties.Resources.Export_ExportActivityToDeviceAction_Text + "xxx"; }
        }

#if !ST_2_1
	    public bool Visible
        {
            get
            {
                if (activities.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }
		
		public IList<string> MenuPath 
		{ 
			get
			{
				return null;
			}
		}
#endif	
		
        #endregion

        #region INotifyPropertyChanged Members

#pragma warning disable 67
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        //private IGPSRoute _gpsRoute = null;
        private IActivity _activity = null;
        private BackgroundWorker _backgroundWorker;
        private ProgressForm _progressForm;
#if ST_2_1
        private bool _running = false;
#else	
		private IActivityReportsView activetyreportview = null;
		private IRouteView routeview = null;
		private IDailyActivityView dailyactivetyview = null;
	    private IList<IActivity> _activities = null;
        private IList<IActivity> activities
        {
            get
            {
                if (_activities == null)
                {
                    if (activetyreportview != null)
                    {
                        return CollectionUtils.GetAllContainedItemsOfType<IActivity>(activetyreportview.SelectionProvider.SelectedItems);
                    }
                    else if (routeview != null)
                    {
                        return CollectionUtils.GetAllContainedItemsOfType<IActivity>(routeview.SelectionProvider.SelectedItems);
                    }
                    else if (dailyactivetyview != null)
                    {
                        return CollectionUtils.GetAllContainedItemsOfType<IActivity>(dailyactivetyview.SelectionProvider.SelectedItems);
                    }
                    else
                    {
                        return new List<IActivity>();
                    }
                }

                return _activities;
            }
            set
            {
                _activities = value;
            }
        }	
#endif
        void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            IJobMonitor jobMonitor = new GlobalsatDevicePlugin.JobMonitor();
            GenericDevice device = new GenericDevice();
            GlobalsatProtocol device2 = device.Device(jobMonitor);
            if (device2 != null)
            {
                device2.SendTrack(_activity, worker, jobMonitor);
            }
            if (worker.CancellationPending)
            {
                e.Cancel = true;
            }
            e.Result = 1;
        }

        void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progressForm.Progress = e.ProgressPercentage;
        }


        void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            _progressForm.Hide();
            //_progressForm.Refresh();


            if (e.Error != null)
            {
                MessageDialog.Show(e.Error.Message, Properties.Resources.Device_SendTrack_Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (e.Cancelled)
            {
                return;
            }
            else
            {
                MessageDialog.Show(CommonResources.Text.MessageExportComplete, Properties.Resources.Export_ExportActivityToDeviceAction_Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        void _progressForm_Cancelled(object sender, EventArgs e)
        {
            try
            {
                _backgroundWorker.CancelAsync();
            }
            catch { }
        }
	}
}
