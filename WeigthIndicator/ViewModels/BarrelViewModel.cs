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
using WeigthIndicator.Events;

namespace WeigthIndicator.ViewModels
{
    public class BarrelViewModel:ReactiveObject
    {
        private readonly IRecipeDataService _recipeDataService;
        private readonly IBarrelStorageDataService _barrelStorageDataService;
        private IEnumerable<BarrelStorage> _barrelStorages;
        [Reactive] public BarrelStorage BarrelStorage { get; set; } = new BarrelStorage();
        [Reactive] public Recipe SelectedRecipe { get; set; }
        [Reactive] public Recipe SelectedRecipeForFilter { get; set; }

        [Reactive] public ObservableCollection<BarrelStorage> BarrelStoragesCollection { get; set; }
        [Reactive] public ObservableCollection<Recipe> RecipesCollection { get; set; }
        public ReactiveCommand<Unit, BarrelStorage> CreateRecipe { get; }

        public BarrelViewModel(IRecipeDataService recipeDataService, IBarrelStorageDataService barrelStorageDataService)
        {
            var canExecute = this
                .WhenAnyValue(x=>x.SelectedRecipe,x=> x.BarrelStorage.TotalWeight,
                (selectedReice,total)=>selectedReice!=null && total>0);

            this.WhenAnyValue(x => x.SelectedRecipeForFilter)
                .Where(x => x != null)
                .Select(x => x)
                .Subscribe(FilterCollection);

             BarrelStorage.ProductionDate = DateTime.Now;
            _recipeDataService = recipeDataService;
            _barrelStorageDataService = barrelStorageDataService;
            CreateRecipe = ReactiveCommand.CreateFromTask(ExecuteCreateBarrelStorage, canExecute);
            CreateRecipe.Subscribe(x => AddBarrelStorage(x));
        }

        private void FilterCollection(Recipe recipe)
        {
            var filtered = _barrelStorages.Where(x => x.RecipeId == recipe.Id);
            BarrelStoragesCollection = new ObservableCollection<BarrelStorage>(filtered);
        }

        public async Task<(IEnumerable<Recipe>,IEnumerable<BarrelStorage>)> GetCollectionsAsync()
        {
            var recipes = await _recipeDataService.GetRecipes();
            _barrelStorages = await _barrelStorageDataService.GetBarrelStorages();
            return (recipes, _barrelStorages);
        }

        public void InitializeCollection((IEnumerable<Recipe>, IEnumerable<BarrelStorage>) collections)
        {
            RecipesCollection = new ObservableCollection<Recipe>(collections.Item1);
            BarrelStoragesCollection = new ObservableCollection<BarrelStorage>(collections.Item2);
        }


        private void AddBarrelStorage(BarrelStorage barrel)
        {
            BarrelStoragesCollection.Add((BarrelStorage)barrel.Clone());
            BarrelStorage = new BarrelStorage
            {
                ProductionDate = DateTime.Now
            };
            SelectedRecipe = null;
            MessageBus.Current.SendMessage(new ReestredAddedEvent { Reestr = new Models.ViewModels.ReestrObject(new Reestr() { Recipe = barrel.Recipe }) });

        }

        private async Task<BarrelStorage> ExecuteCreateBarrelStorage()
        {
            BarrelStorage.RecipeId = SelectedRecipe.Id;
            BarrelStorage.Recipe = SelectedRecipe;
            await _barrelStorageDataService.CreateBarrelStorage(BarrelStorage);

            return BarrelStorage;
        }
    }
}
