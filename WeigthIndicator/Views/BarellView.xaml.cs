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
using WeigthIndicator.ViewModels;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for BarellView.xaml
    /// </summary>
    public partial class BarellView : ReactiveUserControl<BarellViewModel>
    {
        public BarellView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                  .BindTo(this, x => x.ViewModel);

                this.Bind(ViewModel, vm => vm.BarellStorage.TotalWeight, v => v.TotalWeigth.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.BarellStorage.ProductionDate, v => v.ProductionDate.SelectedDate)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.SelectedRecipe, v => v.RecipesCmb.SelectedItem)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedRecipeForFilter, v => v.RecipeCmbForFilter.SelectedItem)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.CreateRecipe, v => v.SaveButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.BarellStoragesCollection, v => v.BarellStoragesCollection.ItemsSource);
                this.OneWayBind(ViewModel, vm => vm.RecipesCollection, v => v.RecipeCmbForFilter.ItemsSource);

                this.OneWayBind(ViewModel, vm => vm.RecipesCollection, v => v.RecipesCmb.ItemsSource)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel)
                    .SelectMany(x => x.GetCollectionsAsync())
                    .SubscribeOnDispatcher()
                    .Subscribe(x => ViewModel.InitializeCollection(x));
            });
        }
    }
}
