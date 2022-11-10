using ReactiveUI;
using System.Reactive.Disposables;
using WeigthIndicator.ViewModels;

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
