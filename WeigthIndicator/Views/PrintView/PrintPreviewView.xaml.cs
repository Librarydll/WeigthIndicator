using System;
using System.Collections.Generic;
using System.IO;
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
using WeigthIndicator.Models.ViewModels;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for PrintPreviewView.xaml
    /// </summary>
    public partial class PrintPreviewView : UserControl, IPrintInitialize
    {
        public PrintPreviewView()
        {
            InitializeComponent();
        }


        public FlowDocument InitializeFlow(ReestrObject reestr)
        {
            NameRu.Text = reestr.Recipe.LongNameRu;
            NameKz.Text = reestr.Recipe.LongNameKz;
            BatchNumber.Text = reestr.BatchNumber;
            Brix.Text = reestr.Recipe.Brix.ToString();
            BarrelNumber.Text = reestr.BarrelNumber.ToString();
            ProductionDate.Text = reestr.BarrelStorage.ProductionDate.ToString("dd.MM.yyyy");
            BeforeDate.Text = reestr.BarrelStorage.ProductionDate.AddYears(2).ToString("dd.MM.yyyy");
            StorageCondition.Text = reestr.Recipe.StorageCondition;
            TranportationCondition.Text = reestr.Recipe.TransportationCondition;
            Net.Text = reestr.Net.ToString();
            Brutto.Text = (reestr.TareBarrelWithLid + reestr.Net).ToString();
            CustomerShortName.Text = reestr.Customer.ShortName;
            AddressRu.Text = reestr.Customer.AddressRu;
            AddressKz.Text = reestr.Customer.AddressKz;
            ManufactureIndex.Text = $"Адрес производства/Өндipic мекенжайы: {reestr.Manufacture.Index}";
            ManufactureAddressKz.Text = reestr.Manufacture.AddressKz;
            ManufactureAddressRu.Text = reestr.Manufacture.AddressRu;


            //if(reestr.QrCode != null)
            //    Qr.Source = BitmapToImageSource(reestr.QrCode);
            return this.FD;
        }

        private BitmapImage BitmapToImageSource(System.Drawing.Image bitmap)
        {

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
    }
}
