using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
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

        public Recipe Recipe { get; set; } = new Recipe();

        public ReactiveCommand<Unit,Unit> CreateRecipe { get; }

        public RecipeViewModel(IRecipeDataService recipeDataService)
        {
           _recipeDataService = recipeDataService;
            CreateRecipe = ReactiveCommand.CreateFromTask(ExecuteCreateRecipe);
        }

        private async Task<Unit> ExecuteCreateRecipe()
        {
            var isSuccess = await _recipeDataService.CreateRecipe(Recipe);
            return Unit.Default;
        }
    }
}
