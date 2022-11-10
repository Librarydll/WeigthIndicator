using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.ViewModels
{
    public class RecipeViewModel : ReactiveObject
    {
        private readonly IRecipeDataService _recipeDataService;

        [Reactive] public Recipe Recipe { get; set; } = new Recipe();

        public ReactiveCommand<Unit,Recipe> CreateRecipe { get; }

        [Reactive] public ObservableCollection<Recipe> RecipesCollection { get; set; }

        public RecipeViewModel(IRecipeDataService recipeDataService)
        {
           _recipeDataService = recipeDataService;
            CreateRecipe = ReactiveCommand.CreateFromTask(ExecuteCreateRecipe);
            CreateRecipe.Subscribe(x => AddRecipe(x));
            Task.Run(Initialize);
        }

        private async Task Initialize()
        {
            var recipes = await _recipeDataService.GetRecipes();
            RecipesCollection = new ObservableCollection<Recipe>(recipes);

        }

        private async Task<Recipe> ExecuteCreateRecipe()
        {
            await _recipeDataService.CreateRecipe(Recipe);

            return Recipe;
        }
        public void AddRecipe(Recipe recipe)
        {
            RecipesCollection.Add((Recipe)recipe.Clone());
            Recipe = new Recipe();
        }
    }
}
