using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WeigthIndicator.Dialogs;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for ReestrSettingView.xaml
    /// </summary>
    public partial class ReestrSettingView : ReactiveUserControl<ReestrSettingViewModel>
    {
        public ReestrSettingView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                this.Bind(ViewModel, vm => vm.ReestrSetting.BatchNumber, v => v.BatchNumber.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ReestrSetting.InitialBarrelNumber, v => v.InitialBarrelNumber.Text)
                    .DisposeWith(disposables);


                this.Bind(ViewModel, vm => vm.Password, v => v.Password.Password)
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    vm => vm.ReestrSetting.MinDefaultWeigth,
                    v => v.DefaultWeight.Text,
                    x => x.ToString(),
                    x => ConvertToDouble(x))
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    vm => vm.ReestrSetting.MinWeight,
                    v => v.MinWeight.Text,
                    x => x.ToString(),
                    x => ConvertToDouble(x))
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    vm => vm.ReestrSetting.MaxWeight,
                    v => v.MaxWeight.Text,
                    x => x.ToString(),
                    x => ConvertToDouble(x))
                   .DisposeWith(disposables);

                this.Bind(ViewModel,
                    vm => vm.ReestrSetting.TaraBarrel,
                    v => v.TarraBarrel.Text,
                    x => x.ToString(),
                    x => ConvertToDouble(x))
                    .DisposeWith(disposables);

                this.Bind(ViewModel,
                    vm => vm.ReestrSetting.TaraBarrelWithLid,
                    v => v.TarraBarrelWithLid.Text,
                    x => x.ToString(),
                    x => ConvertToDouble(x))
                    .DisposeWith(disposables);


                this.OneWayBind(ViewModel, vm => vm.RecipesCollection, v => v.RecipesCmb.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedRecipe, v => v.RecipesCmb.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.CustomersCollection, v => v.CustomerCmb.ItemsSource)
                   .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedCustomer, v => v.CustomerCmb.SelectedItem)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel)
                    .SelectMany(x => x.GetAsync())
                    .ObserveOnDispatcher()
                    .Subscribe(x => ViewModel.Initialize(x));

                this.Bind(ViewModel, vm => vm.ControlsEnabled, v => v.TarraBarrelWithLid.IsEnabled);
                this.Bind(ViewModel, vm => vm.ControlsEnabled, v => v.TarraBarrelWithLidTb.IsEnabled);
                this.Bind(ViewModel, vm => vm.ControlsEnabled, v => v.TarraBarrelWithLidTb2.IsEnabled);
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
