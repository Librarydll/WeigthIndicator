using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Views;
using WeigthIndicator.Views.PrintView;

namespace WeigthIndicator.Factory
{
    public class PrintPreviewFactory
    {

        public static IPrintInitialize GetPrintView(PrintViewType printViewType)
        {
            switch (printViewType)
            {
                case PrintViewType.WithAddress:
                    return new PrintPreviewView();
                case PrintViewType.WithNutritionValue:
                    return new PrintPreviewViewComposition();
                case PrintViewType.BuyerInformation:
                    return new PrintPreviewViewCustomer();
                case PrintViewType.PepsiCo:
                    return new PrintViewPepsiCo();
                default:
                    throw new ArgumentException("printViewType");
            }
        }
    }
}
