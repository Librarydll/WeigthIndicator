﻿using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeigthIndicator.Models
{
    public class ComPortConnector:ReactiveObject,IDisposable
    {
        private bool _disposed = false;

        private string _pattern = @"wn([0-9]+\.?[0-9]+)kg";
        private SerialPort _serialPort;

        public bool IsOpen => _serialPort.IsOpen;

        [Reactive] public string CurrentValue { get; set; }
        [Reactive] public double ParsedValue { get; set; }



        public ComPortConnector()
        {
            _serialPort = new SerialPort();
        }

        public void InitializeSerialPort(ComPortConnectorSetting setting)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }

            string[] ports = SerialPort.GetPortNames();
            var port = ports.FirstOrDefault(x => x == setting.ComPortName);
            if (setting != null && port != null)
            {
                _serialPort.PortName = port;
                _serialPort.BaudRate = setting.BaudRate;
                _serialPort.DataBits = setting.DataBits;
                _serialPort.StopBits = setting.StopBits;
                _serialPort.Parity = setting.Parity;
                _serialPort.Open();

                _serialPort.DataReceived += SerialPort_DataReceived;
                _serialPort.ErrorReceived += SerialPort_ErrorReceived;
            }

            //Task.Run(async () =>
            //{
            //    await Imitation(9);
            //    await Task.Delay(7000);
            //    await Imitation(10);
            //});
        }

        private async Task Imitation(int z)
        {
            for (int i = 1; i <= 10; i++)
            {
                await Task.Delay(500);
                ParsedValue = i*z;
            }
        }

        private void SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {

        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            CurrentValue = _serialPort.ReadLine();
            if (!string.IsNullOrWhiteSpace(CurrentValue))
            {
                ParsedValue = ParseSerialPortData(CurrentValue.Trim('\r'));
            }
        }

        public double ParseSerialPortData(string data)
        {
            if (Regex.IsMatch(data, _pattern))
            {
                var match = Regex.Match(data, _pattern);
                var group = match.Groups[1];
                return double.Parse(group.Value, System.Globalization.CultureInfo.InvariantCulture);
            }

            return -1;
        }

       

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }
        public void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Dispose managed state (managed objects).
                _serialPort?.Dispose();
            }
            _disposed = true;
        }
    }
}
