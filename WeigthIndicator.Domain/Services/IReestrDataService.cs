﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.DTO;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IReestrDataService
    {
        Task<Reestr> CreateReestr(Reestr reestr);
        Task<Reestr> CreateReestrAndUpdateBarrelStorage(Reestr recipe);
        Task<bool> UpdateReestr(Reestr reestr);
        Task<IEnumerable<Reestr>> GetReestrsByDate(DateTime dateTime,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack);

        Task<IEnumerable<Reestr>> GetReestrsByDateAndType(DateTime dateTime,int type,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack);
        Task<IEnumerable<Reestr>> GetReestrsByDates(DateTime fromDate,DateTime toDate, int type,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack);

        Task<IEnumerable<Reestr>> GetReestrsByBatchNumber(DateTime fromDate, DateTime toDate, string batchNumber, int type,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack);
        Task<IEnumerable<Reestr>> GetReestrsByBarrelNumbers(DateTime fromDate, DateTime toDate, int from,int to, int type,
            ReestrLocationState reestrLocationState = ReestrLocationState.InWarehouse | ReestrLocationState.Outcomed | ReestrLocationState.RetrunedBack);

        Task<IEnumerable<GroupedReestr>> GetGroupedReestrs(DateTime fromDate, DateTime toDate);
        Task<IEnumerable<GroupedReestr>> GetGroupedReestrs(DateTime fromDate);
        Task<Reestr> GetLastReestr(int recipeId, DateTime date);
        
        Task<IEnumerable<Reestr>> FilterReestr(
            DateTime packageDate, DateTime productionDate,
            string barrelNumber = null,
            int recipeId = 0,
            string batch = "Все",
            BarrelRecipeType? barrelRecipeType = null);

    }
}
