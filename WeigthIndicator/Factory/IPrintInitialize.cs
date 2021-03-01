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
        FlowDocument InitializeFlow(Reestr reestr);
    }

    public enum PrintViewType
    {
        WithAddress,
        WithNutritionValue,
        BuyerInformation
    }
}
