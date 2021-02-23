using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class RecipeDataService: IRecipeDataService
    {
        private readonly ApplicationContextFactory _factory;
        public RecipeDataService(ApplicationContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> CreateRecipe(Recipe recipe)
        {
            using (var connection = _factory.CreateConnection())
            {
                string query = "INSERT INTO Recipes (`ShortName`, `LongName`, `Brix`, `StorageCondition`, `TransportationCondition`, `Сarbohydrates`, `VitaminC`, `EnergyValue`, `DryContent`) VALUES " +
                    "(@ShortName,@LongName,@Brix,@StorageCondition,@TransportationCondition,@Сarbohydrates,@VitaminC,@EnergyValue,@DryContent)";

                var affectedRow = await connection.ExecuteAsync(query, recipe);

                return affectedRow == 1;
            }    
        }

        public async Task<IEnumerable<Recipe>> GetRecipes()
        {
            using (var connection = _factory.CreateConnection())
            {
                var recipes = await connection.GetAllAsync<Recipe>();
                return recipes;
            }
        }
    }
}
