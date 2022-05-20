using Prism.Events;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Events;
using WeigthIndicator.Models;
using WeigthIndicator.Models.ViewModels;

namespace WeigthIndicator.ViewModels
{
    public class StatusViewModel : ReactiveObject
    {
        private bool _isInitial = true;

        private readonly IBarrelStorageDataService _barrelStorageDataService;
        private readonly IReestrDataService _reestrDataService;
        private Recipe _currentRecipe;

        [Reactive] public RecipeReminder RecipeReminder { get; set; }
        [Reactive] public ReestrObject LastReestrValue { get; set; }

        public StatusViewModel(IBarrelStorageDataService barrelStorageDataService,IReestrDataService reestrDataService)
        {
            _barrelStorageDataService = barrelStorageDataService;
            _reestrDataService = reestrDataService;
            RecipeReminder = new RecipeReminder();
            _currentRecipe = new Recipe();

            MessageBus.Current.Listen<ReestredAddedEvent>()
                 .SelectMany(x => UpdateStatus(x.Reestr)) 
                 .Subscribe();

            MessageBus.Current.Listen<Recipe>()
                .Where(x => x.Id != _currentRecipe.Id)
                .SelectMany(CalculateReminder)
                .Subscribe();

        }

        public async Task<Unit> UpdateStatus(ReestrObject reestr)
        {
            LastReestrValue = reestr;
            await CalculateReminder(reestr.Recipe);
            return Unit.Default;
        }

        public async Task<Unit> CalculateReminder(Recipe recipe)
        {
            if (_isInitial)
            {
                 var last = await _reestrDataService.GetLastReestr(recipe.Id, DateTime.Now);
                LastReestrValue = new ReestrObject(last);
                _isInitial = false;
            }
            var reminder = await _barrelStorageDataService.GetBarrelStorageRemainderByRecipe(recipe.Id);
            RecipeReminder.Remainder = reminder;
            RecipeReminder.RecipeShortName = recipe.ShortName;
            if (reminder < 10000)
            {
                RecipeReminder.IsCritical = true;
            }
            else
            {
                RecipeReminder.IsCritical = false;
            }
            return Unit.Default;
        }
    }
}
