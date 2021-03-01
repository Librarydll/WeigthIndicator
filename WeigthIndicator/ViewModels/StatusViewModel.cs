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

namespace WeigthIndicator.ViewModels
{
    public class StatusViewModel:ReactiveObject
    {
        IObservable<long> timer = Observable.Interval(TimeSpan.FromMilliseconds(200.0));

        private readonly IBarrelStorageDataService _barrelStorageDataService;
        private Recipe _currentRecipe;

        [Reactive] public RecipeReminder RecipeReminder { get; set; }

        public StatusViewModel(IBarrelStorageDataService barrelStorageDataService)
        {
            _barrelStorageDataService = barrelStorageDataService;
            RecipeReminder = new RecipeReminder();
            _currentRecipe = new Recipe();


            MessageBus.Current.Listen<Recipe>()
                .Where(x => x.Id != _currentRecipe.Id)
                .SelectMany(CalculateReminder)
                .Subscribe();

        }

        public async Task<Unit> CalculateReminder(Recipe recipe)
        {
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
