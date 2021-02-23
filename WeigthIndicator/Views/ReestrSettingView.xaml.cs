using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
using WeigthIndicator.Dialogs;
using WeigthIndicator.ViewModels;

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

                this.Bind(ViewModel, vm => vm.ReestrSetting.BuyerName, v => v.BuyerName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.ReestrSetting.BatchNumber, v => v.BatchNumber.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.ReestrSetting.CurrentRecipe, v => v.RecipesCmb.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.RecipesCollection, v => v.RecipesCmb.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedRecipe, v => v.RecipesCmb.SelectedItem)
                    .DisposeWith(disposables);
            });
        }
    }
}
