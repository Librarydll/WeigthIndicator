using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Scanner.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeigthIndicator.Scanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReestrsListPage : ContentPage
    {
        public ReestrsListPage()
        {
            InitializeComponent();
            BindingContext = new ReestrsListViewModel();
        }
    }
}