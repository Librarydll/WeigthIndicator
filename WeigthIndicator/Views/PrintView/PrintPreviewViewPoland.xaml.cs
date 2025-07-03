using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Factory;
using WeigthIndicator.Models;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for PrintPreviewView.xaml
    /// </summary>
    public partial class PrintPreviewViewPoland : UserControl, IPrintInitialize
    {
        public PrintPreviewViewPoland()
        {
            InitializeComponent();
        }


        public FlowDocument InitializeFlow(Reestr reestr,string data =null ,PolandData polandData = null)
        {
            var manufacture = ManufactureProvider.GetManufacture();

            var months = CultureInfo.GetCultureInfo("en-US").DateTimeFormat.MonthNames;
            int monthNumber = Array.IndexOf(months, polandData.Month) + 1;


            NameRu.Text = reestr.Recipe.LongNameRu;
            NameKz.Text = reestr.Recipe.LongNameKz;
            BatchNumber.Text = reestr.BatchNumber;
            Brix.Text = reestr.Recipe.Brix.ToString();
            BarrelNumber.Text = reestr.BarrelNumber.ToString();

            ProductionDate.Text = polandData.Month +" " + polandData.Year;

            BeforeDate.Text = polandData.Month + " " + (int.Parse(polandData.Year) + 2);

            StorageCondition.Text = reestr.Recipe.StorageCondition;
            TranportationCondition.Text = reestr.Recipe.TransportationCondition;
            Net.Text = reestr.Net.ToString() + " kg";
            Brutto.Text = (reestr.TareBarrelWithLid + reestr.Net).ToString() +" kg";
            Index.Text = manufacture.Index;
            polandAddressName.Text = "140300, Republic of Uzbekistan, Samarkand district, Samarkand region, Village \"Gulobod\"/140300";
            ManufactureName.Text = manufacture.ManufactureName;
            return this.FD;
        }
      
    }
}
