using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Models.ViewModels;

namespace WeigthIndicator.Factory
{
    public interface IPrintInitialize
    {
        FlowDocument InitializeFlow(ReestrObject reestr);
    }

    public enum PrintViewType
    {
        WithAddress,
        WithNutritionValue,
        BuyerInformation,
        NoPrint
    }
}
