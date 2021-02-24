using Newtonsoft.Json;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Models
{
    public class ComPortConnectorSetting
    {
        private static string comportSettingJson = "comport.json";

        [Reactive] public string ComPortName { get; set; }
        [Reactive] public int BaudRate { get; set; }
        [Reactive] public int DataBits { get; set; }
        [Reactive] public StopBits StopBits { get; set; }
        [Reactive] public Parity Parity { get; set; }


        public static ComPortConnectorSetting GetComPortConnectorSetting()
        {
            if (!File.Exists(comportSettingJson))
            {
                return null;      
            }
            using (var reader = new StreamReader(comportSettingJson))
            {
                var lines = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<ComPortConnectorSetting>(lines);
            }
        }

        public static void SaveComPortConnectorSetting(ComPortConnectorSetting setting)
        {
            using (var writer = new StreamWriter(comportSettingJson))
            {
                var json = JsonConvert.SerializeObject(setting);
                writer.Write(json);
            }
        }
    }
}
