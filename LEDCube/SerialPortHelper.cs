/*
 * Learning how to use SerialPort - Hari Wiguna, September 2010 * 
 * 
 * The FTDI serial converter is wired to Netduino like this:
 * Netduino     FTDI
 * GND          GND
 * -            CTS
 * -            3.3V
 * D0 (rx)      TXO (computer tx)
 * D1 (tx)      RXI (computer rx)
 * -            DTR
 * 
 */

using System;
using System.IO.Ports;
using System.Text;
using Microsoft.SPOT;
using SecretLabs.NETMF.Hardware.Netduino;
using System.Threading;

namespace LEDCube
{
    public class SerialPortHelper
    {
        static SerialPort serialPort;

        const int bufferMax = 4096;
        static byte[] buffer = new Byte[bufferMax];
        static int bufferLength = 0;
        public string lastError;

        public SerialPortHelper(string portName = "COM1", int baudRate = 250000, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            //serialPort.ReadTimeout = 1; // Set to 10ms. Default is -1?!
            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);
            serialPort.Flush();
            serialPort.Open();
        }

       

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            lock (buffer)
            {
                try
                {
                    int bytesReceived = serialPort.Read(buffer, bufferLength, bufferMax - bufferLength);
                    if (bytesReceived > 0)
                    {
                        //Debug.Print("Bytes Recieved: "+ bytesReceived);
                        bufferLength += bytesReceived;
                        //Debug.Print("Buffer Length: " + bufferLength);
                        if (bufferLength >= bufferMax)
                            throw new ApplicationException("Buffer Overflow.  Send shorter lines, or increase lineBufferMax.");
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex.Message;
                    PrintLine("error");
                }
               
            }
        }

        public string ReadLine()
        {
            string line = "";

            lock (buffer)
            {
                //-- Look for Return char in buffer --
                for (int i = 0; i < bufferLength; i++)
                {
                    //-- Consider EITHER CR or LF as end of line, so if both were received it would register as an extra blank line. --
                    if (buffer[i] == '\r' || buffer[i] == '\n')
                    {
                        buffer[i] = 0; // Turn NewLine into string terminator
                        line = "" + new string(Encoding.UTF8.GetChars(buffer)); // The "" ensures that if we end up copying zero characters, we'd end up with blank string instead of null string.
                        //Debug.Print("LINE: <" + line + ">");
                        bufferLength = bufferLength - i - 1;
                        Array.Copy(buffer, i + 1, buffer, 0, bufferLength); // Shift everything past NewLine to beginning of buffer
                        break;
                    }
                }
            }

            return line;
        }

        public bool ReadLayer(ref byte[] layer)
        {
            var newData = false;
            lock (buffer)
            {
                //-- Look for Return char in buffer --
                for (int i = 0; i < bufferLength; i++)
                {
                    //-- Consider EITHER CR or LF as end of line, so if both were received it would register as an extra blank line. --
                    if (buffer[i] == '\r' || buffer[i] == '\n')
                    {
                        newData = true;
                        buffer[i] = 0; // Turn NewLine into string terminator
                        Array.Copy(buffer,layer,i-1);
                         // The "" ensures that if we end up copying zero characters, we'd end up with blank string instead of null string.
                        //Debug.Print("LINE: <" + line + ">");
                        bufferLength = bufferLength - i - 1;
                        Array.Copy(buffer, i + 1, buffer, 0, bufferLength); // Shift everything past NewLine to beginning of buffer
                        break;
                    }
                }
            }
            return newData;

        }

        public void Print( string line )
        {
            UTF8Encoding encoder = new System.Text.UTF8Encoding();
            byte[] bytesToSend = encoder.GetBytes(line);
            serialPort.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public void PrintLine(string line)
        {
            Print(line + "\r");
        }

        public void PrintClear()
        {
            byte[] bytesToSend = new byte[2];
            bytesToSend[0] = 254;
            bytesToSend[1] = 1;
            serialPort.Write(bytesToSend, 0, 2);
            Thread.Sleep(500); // LCD is slow, pause for 500ms before sending more chars
        }
    }
}
