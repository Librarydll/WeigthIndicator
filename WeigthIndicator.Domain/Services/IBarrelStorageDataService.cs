using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IBarrelStorageDataService
    {
        Task<BarrelStorage> CreateBarrelStorage(BarrelStorage barrelStorage);
        Task<IEnumerable<BarrelStorage>> GetBarrelStorages();

        Task<double> GetBarrelStorageRemainderByRecipe(int recipeId);
        Task<int> GetLastBarrelNumber(Recipe recipe);
    }
}
