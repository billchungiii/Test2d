﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Test2d;

namespace TestSIM
{
    public class InverterSimulation : BoolSimulation
    {
        public override string Key
        {
            get { return "INVERTER"; }
        }

        public override Func<XGroup, BoolSimulation> Factory
        {
            get { return (group) => { return new InverterSimulation(null); }; }
        }

        public InverterSimulation()
            : base()
        {
        }

        public InverterSimulation(bool? state)
            : base()
        {
            base.State = state;
        }

        public override void Run(IClock clock)
        {
            int length = Inputs.Length;
            if (length == 0)
            {
                // Do nothing.
            }
            else if (length == 1)
            {
                var input = Inputs[0];
                base.State = input.IsInverted ? input.Simulation.State : !(input.Simulation.State);
            }
            else
            {
                throw new Exception("Inverter simulation can only have one input State.");
            }
        }
    }
}