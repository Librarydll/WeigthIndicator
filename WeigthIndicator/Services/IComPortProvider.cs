using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Models;

namespace WeigthIndicator.Services
{
    public interface IComPortProvider
    {
        ComPortConnector ComPortConnector { get; set; }
        void Start();
    }
}
