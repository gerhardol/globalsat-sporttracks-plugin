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
using System.Text;

using ZoneFiveSoftware.Common.Data;
using ZoneFiveSoftware.Common.Data.Fitness;
using ZoneFiveSoftware.Common.Data.GPS;

using ZoneFiveSoftware.Common.Visuals;
using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    public abstract class ImportJob
    {
        public ImportJob(GhDeviceBase device, string sourceDescription, IJobMonitor monitor, IImportResults importResults)
        {
            this.device = device;
            this.sourceDescription = sourceDescription.Replace(Environment.NewLine, " ");
            this.monitor = monitor;
            this.importResults = importResults;
        }

        public abstract bool Import();

        protected GhDeviceBase device;
        protected string sourceDescription;
        protected IJobMonitor monitor;
        protected IImportResults importResults;
    }
}
