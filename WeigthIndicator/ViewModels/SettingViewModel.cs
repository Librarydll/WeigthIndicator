using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Models;
using WeigthIndicator.Services;

namespace WeigthIndicator.ViewModels
{
    public class SettingViewModel:ReactiveObject
    {
        private readonly IComPortProvider _comPortProvider;

        public ObservableCollection<string> ComPorts { get; set; }
        public ObservableCollection<string> BaudRates { get; set; }
        public ObservableCollection<string> DataBits { get; set; }
        public ObservableCollection<string> StopBits { get; set; }
        public ObservableCollection<string> Parity { get; set; }

        [Reactive] public ComPortConnectorSetting ComPortSetting { get; set; }

        public ReactiveCommand<Unit,Unit> SaveSetting { get; set; }
        public SettingViewModel(IComPortProvider comPortProvider)
        {
            ComPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
            BaudRates = new ObservableCollection<string>(new string[] { "2400", "4800", "9600" });
            DataBits = new ObservableCollection<string>(new string[] { "5", "6", "7","8" });
            StopBits = new ObservableCollection<string>(new string[] { "One","Two" });
            Parity = new ObservableCollection<string>(new string[] { "None", "Odd", "Even" });

            ComPortSetting = ComPortConnectorSetting.GetComPortConnectorSetting() ?? new ComPortConnectorSetting();
            SaveSetting = ReactiveCommand.Create(ExecuteSaveSetting);
            _comPortProvider = comPortProvider;
        }

        private void ExecuteSaveSetting()
        {
            ComPortConnectorSetting.SaveComPortConnectorSetting(ComPortSetting);
            _comPortProvider.ComPortConnector.InitializeSerialPort(ComPortSetting);
        }
    }
}
