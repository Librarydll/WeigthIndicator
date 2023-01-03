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
    /// Interaction logic for PrintPreviewViewComposition.xaml
    /// </summary>
    public partial class PrintPreviewViewComposition : UserControl, IPrintInitialize
    {
        public PrintPreviewViewComposition()
        {
            InitializeComponent();
        }

        public FlowDocument InitializeFlow(Reestr reestr,string data =null)
        {
            NameRu.Text = reestr.Recipe.LongNameRu;
            NameKz.Text = reestr.Recipe.LongNameKz;
            BatchNumber.Text = reestr.BatchNumber;
            Brix.Text = reestr.Recipe.Brix.ToString();
            BarrelNumber.Text = reestr.BarrelNumber.ToString();
            ProductionDate.Text = reestr.BarrelStorage.ProductionDate.ToString("dd.MM.yyyy");
            BeforeDate.Text = reestr.BarrelStorage.ProductionDate.AddYears(2).ToString("dd.MM.yyyy");
          //  PackingDate.Text = reestr.PackingDate.ToString("HH:mm:ss dd.MM.yyyy");
            StorageCondition.Text = reestr.Recipe.StorageCondition;
            TranportationCondition.Text = reestr.Recipe.TransportationCondition;
            Net.Text = reestr.Net.ToString();
            Brutto.Text = (reestr.TareBarrelWithLid + reestr.Net).ToString();
            Carbo.Text = reestr.Recipe.Carbohydrates.ToString();
            VitaminC.Text = reestr.Recipe.VitaminC.ToString();
            EnergyBalue.Text = reestr.Recipe.EnergyValue.ToString();
            DryContent.Text = reestr.Recipe.DryContent.ToString();
            return this.FD;
        }
    }
}
