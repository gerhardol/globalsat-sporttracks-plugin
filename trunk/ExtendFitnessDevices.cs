// Copyright (C) 2010 Zone Five Software
// Author: Aaron Averill
using System;
using System.Collections.Generic;
using System.Text;

using ZoneFiveSoftware.Common.Visuals.Fitness;

namespace ZoneFiveSoftware.SportTracks.Device.Globalsat
{
    class ExtendFitnessDevices : IExtendFitnessDevices
    {
        public IList<IFitnessDevice> FitnessDevices
        {
            get { return new IFitnessDevice[] { new FitnessDevice_GH615(), new FitnessDevice_GH625(), new FitnessDevice_GH505(), 
                new FitnessDevice_GH625XT(), new FitnessDevice_GB580() }; }
        }
    }
}
