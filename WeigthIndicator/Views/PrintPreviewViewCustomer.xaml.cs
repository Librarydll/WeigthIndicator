using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Factory;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for PrintPreviewViewCustomer.xaml
    /// </summary>
    public partial class PrintPreviewViewCustomer : Window, IPrintInitialize
    {
        public PrintPreviewViewCustomer()
        {
            InitializeComponent();
        }

        public FlowDocument InitializeFlow(Reestr reestr)
        {
            for (int i = 0; i < 5; i++)
            {
                var p = new Paragraph() { LineHeight = 1 };
                p.Inlines.Add(new Run { Text = "Импортер/Aлушы:" ,FontSize=12.5,FontWeight=FontWeight.FromOpenTypeWeight(750),FontFamily=new FontFamily("Arial") });
                p.Inlines.Add(new Run { Text = reestr.Customer.ShortName, FontSize = 12.5, FontWeight = FontWeight.FromOpenTypeWeight(750), FontFamily = new FontFamily("Arial") });

                var p2 = new Paragraph { LineHeight = 1 };
                p2.Inlines.Add(new Run { Text = reestr.Customer.AddressRu, FontSize = 12.5, FontWeight = FontWeight.FromOpenTypeWeight(750), FontFamily = new FontFamily("Arial") });
                var p3 = new Paragraph { LineHeight = 2 };
                p3.Inlines.Add(new Run { Text = reestr.Customer.AddressKz, FontSize = 12.5, FontWeight = FontWeight.FromOpenTypeWeight(750), FontFamily = new FontFamily("Arial") });
                FD.Blocks.Add(p);
                FD.Blocks.Add(p2);
                FD.Blocks.Add(p3);
                if (i != 5) 
                    FD.Blocks.Add(new Paragraph { LineHeight = 1 });
            }

            return FD;
        }


    }
}
