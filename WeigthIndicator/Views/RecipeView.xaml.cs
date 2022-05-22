using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WeigthIndicator.ViewModels;
using System;
using WeigthIndicator.Domain.Models;
using System.Windows;

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
                this.Bind(ViewModel, vm => vm.Recipe.LongNameRu, v => v.LongNameRu.Text)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.Recipe.LongNameKz, v => v.LongNameKz.Text)
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
                this.Bind(ViewModel, vm => vm.Recipe.Carbohydrates, v => v.Carbohydrates.Text)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.Recipe.BarrelRecipeType, v => v.BarrelType.SelectedIndex,
                    x=> (int)x,
                    x=>(BarrelRecipeType)Enum.Parse(typeof(BarrelRecipeType),x.ToString()))
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.CreateRecipe, v => v.SaveButton)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.RecipesCollection, v => v.RecipesCollection.ItemsSource);


            });
      
        }
   
    }
}
