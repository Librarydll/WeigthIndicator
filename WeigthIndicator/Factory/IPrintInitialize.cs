using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Factory
{
    public interface IPrintInitialize
    {
        FlowDocument InitializeFlow(Reestr reestr,string info = null, PolandData polandData = null);
    }
    public class PolandData
    {
        public string Month { get; set; }
        public string Year { get; set; }
    }

    public enum PrintViewType
    {
        WithAddress,
        WithNutritionValue,
        BuyerInformation,
        PepsiCo,
        Polad,
        NoPrint
    }
}
