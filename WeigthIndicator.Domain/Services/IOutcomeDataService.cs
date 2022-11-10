using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IOutcomeDataService
    {
        Task<PaginationObject<Outcome>> GetOutcomes(DateTime from, DateTime to, int currentPage, int pageSize, string searchingString);
        Task<Outcome> GetOutcome(int id);
        Task<Outcome> CreateOutcome(Outcome outcome);
        Task<Outcome> UpdateOutcome(Outcome outcome);
        Task<bool> DeleteOutcome(int outcomeId);

    }
}
