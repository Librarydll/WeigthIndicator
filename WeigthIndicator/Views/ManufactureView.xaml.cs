using ReactiveUI;
using System.Reactive.Disposables;
using WeigthIndicator.ViewModels;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for ManufactureView.xaml
    /// </summary>
    public partial class ManufactureView : ReactiveUserControl<ManufactureViewModel>
    {
        public ManufactureView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                this.Bind(ViewModel, vm => vm.Manufacture.Name, v => v.ManufactureName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Manufacture.AddressKz, v => v.KzAddress.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Manufacture.AddressRu, v => v.RuAddress.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Manufacture.Index, v => v.Index.Text)
                 .DisposeWith(disposables);
                this.BindCommand(ViewModel, vm => vm.CreateManufacture, v => v.SaveButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.ManufacturesCollection, v => v.ManufacturesCollection.ItemsSource);

               // this.BindCommand(ViewModel, vm => vm.PrintCommand, v => v.PrintCommand, x => x.SelectedManufacture);

                this.Bind(ViewModel, vm => vm.SelectedManufacture, v => v.ManufacturesCollection.SelectedItem);
            });
        }
    }
}
