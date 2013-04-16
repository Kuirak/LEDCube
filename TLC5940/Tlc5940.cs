// <copyright file="Tlc5940.cs" company="Dave Smith">
//
// Copyright (C) 2010 Dave Smith http://dotnetdave.net
//
// This file is part of the TLC5940 driver for Netduino.

// The TLC5940 driver for Netduino is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// The TLC5940 driver for Netduino is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with the TLC5940 driver for Netduino.  If not, see http://www.gnu.org/licenses/.
// </copyright>
// <summary>
// Driver for the TI TLC 5490 written for the Netduino
// 
// Email: TLC5490@dotnetdave.net
// </summary>
namespace SmudgeIT.Tlc5940
{
    using System;
    using System.Threading;
    using Microsoft.SPOT;
    using Microsoft.SPOT.Hardware;

    /// <summary>
    /// Driver for the TI TLC5940
    /// </summary>
    public class Tlc5940
    {
        /// <summary>
        /// Max Value for the pin
        /// </summary>
        private const int GlcCounterMax = 4096;

        /// <summary>
        /// The port for the SCLK pin
        /// </summary>
        private OutputPort sclk;

        /// <summary>
        /// The port for the SIN pin
        /// </summary>
        private OutputPort sin;

        /// <summary>
        /// The port for the BLANK pin
        /// </summary>
        private OutputPort blank;

        /// <summary>
        /// The port for the XLAT pin
        /// </summary>
        private OutputPort xlat;

        /// <summary>
        /// The port for the VPROG pin
        /// </summary>
        private OutputPort vprog;

        /// <summary>
        /// The port for the GCCLK pin
        /// </summary>
        private OutputPort gcclk;

        /// <summary>
        /// The port for the BLANK pin
        /// </summary>
        private int[] dotcorrection;

        /// <summary>
        /// Brightness data
        /// </summary>
        private int[] bright;

        /// <summary>
        /// Is this the first cycle
        /// </summary>
        private bool firstCycle = true;

        /// <summary>
        /// Counter for the GLCCLK pin
        /// </summary>
        private int glcCounter = 1;

        /// <summary>
        /// Gets or sets the dot correction values.
        /// </summary>
        /// <value>The dot correction values.</value>
        public int[] DotCorrectionValues
        {
            get { return this.dotcorrection; }
            set { this.dotcorrection = value; }
        }

        /// <summary>
        /// Gets or sets the grayscale color values.
        /// </summary>
        /// <value>The grayscale color values.</value>
        public int[] GrayscaleColorValues
        {
            get { return this.bright; }
            set { this.bright = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tlc5940"/> class.
        /// </summary>
        /// <param name="sclkPin">The SCLK pin.</param>
        /// <param name="sinPin">The SIN pin.</param>
        /// <param name="blankPin">The BLANK pin.</param>
        /// <param name="xlatPin">The XLAT pin.</param>
        /// <param name="vprogPin">The VPROG pin.</param>
        /// <param name="gcclkPin">The GCCLK pin.</param>
        public Tlc5940(
            Cpu.Pin sclkPin,
            Cpu.Pin sinPin,
            Cpu.Pin blankPin,
            Cpu.Pin xlatPin,
            Cpu.Pin vprogPin,
            Cpu.Pin gcclkPin)
        {
            this.sclk = new OutputPort(sclkPin, false);
            this.sin = new OutputPort(sinPin, false);
            this.blank = new OutputPort(blankPin, true);
            this.xlat = new OutputPort(xlatPin, false);
            this.vprog = new OutputPort(vprogPin, true);
            this.gcclk = new OutputPort(gcclkPin, false);
            this.bright = new int[16] { 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800, 0x800 };
            this.dotcorrection = new int[16] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 };
        }

        /// <summary>
        /// Inits this instance.
        /// </summary>
        public void Init()
        {
            this.LoadDCValues();
            this.vprog.Write(false);
        }

        /// <summary>
        /// Sets all the outputs to the brightness value.
        /// </summary>
        /// <param name="brightness">The brightness.</param>
        public void SetAll(int brightness)
        {
            if (brightness > 0 || brightness < 101)
            {
                for (int i = 0; i < 16; i++)
                {
                    this.Set(i + 1, brightness);
                }
            }
        }

        /// <summary>
        /// Clears all the outputs.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < 16; i++)
            {
                this.Set(i + 1, 0);
            }
        }

        /// <summary>
        /// Sets the specified channel.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="brightness">The brightness.</param>
        public void Set(int channel, int brightness)
        {
            if (channel < 1 || channel > 16 || brightness < 0 || brightness > 101)
            {
                throw new ApplicationException("Bad Boy");
            }

            this.bright[channel - 1] = (int)System.Math.Ceiling(brightness * 20.48);
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            this.LoadBrightValues(this.firstCycle);
        }

        /// <summary>
        /// Loads the DC values.
        /// </summary>
        private void LoadDCValues()
        {
            this.vprog.Write(true);
            for (int i = 15; i >= 0; i--)
            {
                for (int j = 0x20; j != 0; j >>= 1)
                {
                    if ((this.dotcorrection[i] & j) == 0)
                    {
                        this.sin.Write(false);
                    }
                    else
                    {
                        this.sin.Write(true);
                    }

                    this.PulsePort(this.sclk);
                }
            }

            this.PulsePort(this.xlat);
        }

        /// <summary>
        /// Loads the bright values.
        /// </summary>
        /// <param name="first">if set to <c>true</c> [first].</param>
        private void LoadBrightValues(bool first)
        {
            this.blank.Write(false);
            this.glcCounter = 1;
            for (int i = 15; i >= 0; i--)
            {
                for (int j = 0x800; j != 0; j >>= 1)
                {
                    if ((this.bright[i] & j) == 0)
                    {
                        this.sin.Write(false);
                    }
                    else
                    {
                        this.sin.Write(true);
                    }

                    this.PulsePort(this.sclk);

                    this.PulsePort(this.gcclk);
                    this.glcCounter++;
                }
            }

            while (this.glcCounter < Tlc5940.GlcCounterMax)
            {
                this.PulsePort(this.gcclk);
                this.glcCounter++;
            }

            this.blank.Write(true);
            this.PulsePort(this.xlat);
            if (first)
            {
                this.PulsePort(this.sclk);
                this.firstCycle = false;
            }
        }

        /// <summary>
        /// Pulses the port.
        /// </summary>
        /// <param name="port">The port to set.</param>
        private void PulsePort(OutputPort port)
        {
            port.Write(true);
            port.Write(false);
        }
    }
}
