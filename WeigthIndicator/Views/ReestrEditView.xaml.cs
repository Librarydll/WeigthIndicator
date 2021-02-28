using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using WeigthIndicator.Dialogs;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for ReestrEditView.xaml
    /// </summary>
    public partial class ReestrEditView : ReactiveUserControl<ReestrEditViewModel>
    {
        public ReestrEditView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                   .BindTo(this, x => x.ViewModel);

                this.Bind(ViewModel, vm => vm.Reestr.Note, v => v.Note.Text);
               // this.Bind(ViewModel, vm => vm.Reestr.Buyer, v => v.Buyer.Text);
                this.Bind(ViewModel, vm => vm.Reestr.ReestrState, v => v.State.IsChecked);
                this.Bind(ViewModel, vm => vm.Reestr.Net, v => v.Net.Text);

                this.Bind(ViewModel,
                    vm => vm.Reestr.Net,
                    v => v.Net.Text,
                    x => x.ToString(),
                    x => ConvertToDouble(x))
                    .DisposeWith(disposables);


                this.OneWayBind(ViewModel, vm => vm.CustomersCollection, v => v.CustomerCmb.ItemsSource)
                   .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedCustomer, v => v.CustomerCmb.SelectedItem)
                    .DisposeWith(disposables);


                this.WhenAnyValue(x => x.ViewModel)
                    .SelectMany(x => x.GetAsync())
                    .ObserveOnDispatcher()
                    .Subscribe(x => ViewModel.Initialize(x));
            });
        }

        private double ConvertToDouble(string value)
        {
            if (double.TryParse(value.Replace(".", ","), out double result))
            {
                return result;
            }

            return 0;
        }
    }
}
