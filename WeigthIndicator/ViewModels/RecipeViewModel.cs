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
        }

        private async Task<Recipe> ExecuteCreateRecipe()
        {
            await _recipeDataService.CreateRecipe(Recipe);

            return Recipe;
        }

        public async Task<IEnumerable<Recipe>> GetRecipesAsync()
        {
            return await _recipeDataService.GetRecipes();
        }

        public void InitializeCollection(IEnumerable<Recipe> recipes)
        {
            RecipesCollection = new ObservableCollection<Recipe>(recipes);
        }
        public void AddRecipe(Recipe recipe)
        {
            recipe = (Recipe)Recipe.Clone();
            RecipesCollection.Add(recipe);
            Recipe = new Recipe();
        }
    }
}
