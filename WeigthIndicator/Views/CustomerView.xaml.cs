using ReactiveUI;
using System.Reactive.Disposables;
using WeigthIndicator.ViewModels;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for CustomerView.xaml
    /// </summary>
    public partial class CustomerView : ReactiveUserControl<CustomerViewModel>
    {
        public CustomerView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                this.Bind(ViewModel, vm => vm.Customer.ShortName, v => v.ShortName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Customer.AddressKz, v => v.KzAddress.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Customer.AddressRu, v => v.RuAddress.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.CreateCustomer, v => v.SaveButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.CustomersCollection, v => v.CustomersCollection.ItemsSource);

                this.BindCommand(ViewModel, vm => vm.PrintCommand, v => v.PrintCommand, x => x.SelectedCustomer);

                this.Bind(ViewModel, vm => vm.SelectedCustomer, v => v.CustomersCollection.SelectedItem);
            });
        }
    }
}
