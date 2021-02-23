using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IRecipeDataService
    {
        Task<bool> CreateRecipe(Recipe recipe);

        Task<IEnumerable<Recipe>> GetRecipes();
    }
}
