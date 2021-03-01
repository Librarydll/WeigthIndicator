using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Views;

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
                default:
                    throw new ArgumentException("printViewType");
            }
        }
    }
}
