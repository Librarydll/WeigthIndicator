using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.ViewModels
{
    public class BarellViewModel:ReactiveObject
    {
        private readonly IRecipeDataService _recipeDataService;
        private readonly IBarellStorageDataService _barellStorageDataService;
        private IEnumerable<BarellStorage> _barellStorages;
        [Reactive] public BarellStorage BarellStorage { get; set; } = new BarellStorage();
        [Reactive] public Recipe SelectedRecipe { get; set; }
        [Reactive] public Recipe SelectedRecipeForFilter { get; set; }

        [Reactive] public ObservableCollection<BarellStorage> BarellStoragesCollection { get; set; }
        [Reactive] public ObservableCollection<Recipe> RecipesCollection { get; set; }
        public ReactiveCommand<Unit, BarellStorage> CreateRecipe { get; }

        public BarellViewModel(IRecipeDataService recipeDataService, IBarellStorageDataService  barellStorageDataService)
        {
            var canExecute = this
                .WhenAnyValue(x=>x.SelectedRecipe,x=> x.BarellStorage.TotalWeight,
                (selectedReice,total)=>selectedReice!=null && total>0);

            this.WhenAnyValue(x => x.SelectedRecipeForFilter)
                .Where(x => x != null)
                .Select(x => x)
                .Subscribe(FilterCollection);

             BarellStorage.ProductionDate = DateTime.Now;
            _recipeDataService = recipeDataService;
            _barellStorageDataService = barellStorageDataService;
            CreateRecipe = ReactiveCommand.CreateFromTask(ExecuteCreateBarellStorage, canExecute);
            CreateRecipe.Subscribe(x => AddBarellStorage(x));
        }

        private void FilterCollection(Recipe recipe)
        {
            var filtered = _barellStorages.Where(x => x.RecipeId == recipe.Id);
            BarellStoragesCollection = new ObservableCollection<BarellStorage>(filtered);
        }

        public async Task<(IEnumerable<Recipe>,IEnumerable<BarellStorage>)> GetCollectionsAsync()
        {
            var recipes = await _recipeDataService.GetRecipes();
            _barellStorages = await _barellStorageDataService.GetBarellStorages();
            return (recipes, _barellStorages);
        }

        public void InitializeCollection((IEnumerable<Recipe>, IEnumerable<BarellStorage>) collections)
        {
            RecipesCollection = new ObservableCollection<Recipe>(collections.Item1);
            BarellStoragesCollection = new ObservableCollection<BarellStorage>(collections.Item2);
        }


        private void AddBarellStorage(BarellStorage barell)
        {
            BarellStoragesCollection.Add((BarellStorage)barell.Clone());
            BarellStorage = new BarellStorage
            {
                ProductionDate = DateTime.Now
            };
            SelectedRecipe = null;
        }

        private async Task<BarellStorage> ExecuteCreateBarellStorage()
        {
            BarellStorage.RecipeId = SelectedRecipe.Id;
            BarellStorage.Recipe = SelectedRecipe;
            await _barellStorageDataService.CreateBarellStorage(BarellStorage);

            return BarellStorage;
        }
    }
}
