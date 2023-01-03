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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Factory;

namespace WeigthIndicator.Views.PrintView
{
    /// <summary>
    /// Interaction logic for PrintViewPepsiCo.xaml
    /// </summary>
    public partial class PrintViewPepsiCo : UserControl , IPrintInitialize
    {
        public PrintViewPepsiCo()
        {
            InitializeComponent();
}
        public FlowDocument InitializeFlow(Reestr reestr,string group)
        {
            ProductName.Text = reestr.Recipe.LongNameRu;
            BatchNumber.Text = reestr.BatchNumber;
            Brix.Text = reestr.Recipe.Brix.ToString();
            BarrelNumber.Text = reestr.BarrelNumber.ToString();
            MaterialGroup.Text = group;
            StorangeAndTransportContidition.Text = "Условия хранения и\nсрок годности";
            Manufacture.Text = "СП ООО \"AGROMIR\"";
            ProductionDate.Text = reestr.BarrelStorage.ProductionDate.ToString("dd.MM.yyyy");
            // ManufactureAddress.Text = reestr.
            ImporterSub.Text = "ООО “ПепсиКо Холдингс”\n";
            ProductionDate.Text = reestr.BarrelStorage.ProductionDate.ToString("dd.MM.yyyy");
            ExpiredDate.Text = reestr.BarrelStorage.ProductionDate.AddYears(2).ToString("dd.MM.yyyy");
            Net.Text = reestr.Net.ToString() + " кг";
            Brutto.Text = (reestr.TareBarrelWithLid + reestr.Net).ToString() + " кг";
            ManufactureAddress.Text = "ООО \"Gazalkent Meva\" Адрес.Республика Узбекистан Ташкентская область г.Газалкент ул А.Темура 49";
            return this.FD;
        }
    }
}
