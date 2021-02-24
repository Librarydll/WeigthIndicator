using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WeigthIndicator.ViewModels;
using System;
namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for RecipeView.xaml
    /// </summary>
    public partial class RecipeView : ReactiveUserControl<RecipeViewModel>
    {
        public RecipeView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                this.Bind(ViewModel, vm => vm.Recipe.ShortName, v => v.ShortName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.LongName, v => v.LongName.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.Brix, v => v.Brix.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.StorageCondition, v => v.StorageCondition.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.DryContent, v => v.DryContent.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.EnergyValue, v => v.EnergyValue.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.TransportationCondition, v => v.TransportationCondition.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.VitaminC, v => v.VitaminC.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.Сarbohydrates, v => v.Carbohydrates.Text)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.CreateRecipe, v => v.SaveButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.RecipesCollection, v => v.RecipesCollection.ItemsSource);

                this.WhenAnyValue(x => x.ViewModel)
                    .SelectMany(x => x.GetRecipesAsync())
                    .SubscribeOnDispatcher()
                    .Subscribe(x => ViewModel.InitializeCollection(x));
                    
            });
        }
    }
}
