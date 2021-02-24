using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Models;

namespace WeigthIndicator.Services
{
    public class ComPortProvider : IComPortProvider
    {
        public ComPortConnector ComPortConnector { get; set; }

        public ComPortProvider()
        {
            ComPortConnector = new ComPortConnector();

        }

        public void Start()
        {
            ComPortConnector.InitializeSerialPort(ComPortConnectorSetting.GetComPortConnectorSetting());
        }
    }
}
