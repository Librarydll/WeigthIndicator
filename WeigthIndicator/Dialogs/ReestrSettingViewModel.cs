using Prism.Services.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Dialogs.Common;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Models;
using WeigthIndicator.Services;

namespace WeigthIndicator.Dialogs
{
    public class ReestrSettingViewModel : DialogViewModelBase
    {
        private readonly IReestrSettingDataService _reestrSettingDataService;
        private readonly IRecipeDataService _recipeDataService;

        private ObservableCollection<Recipe> _recipesCollection;
        public ObservableCollection<Recipe> RecipesCollection
        {
            get { return _recipesCollection; }
            set { this.RaiseAndSetIfChanged(ref _recipesCollection, value); }
        }

        [Reactive] public Recipe SelectedRecipe { get; set; }

        [Reactive] public ReestrSetting ReestrSetting { get; set; } = new ReestrSetting();
        public ReestrSettingViewModel(
            IReestrSettingDataService reestrSettingDataService,
            IRecipeDataService recipeDataService)
        {
            Title = "Настройки реестра";
            _reestrSettingDataService = reestrSettingDataService;
            _recipeDataService = recipeDataService;

        }


        public async Task<(IEnumerable<Recipe>, ReestrSetting)> GetAsync()
        {
            var rs = await _reestrSettingDataService.GetReestrSetting();
            var collection = await _recipeDataService.GetRecipes();

            return (collection, rs);
        }

        public void Initialize((IEnumerable<Recipe>, ReestrSetting) args)
        {
            RecipesCollection = new ObservableCollection<Recipe>(args.Item1);
            ReestrSetting = args.Item2 ?? new ReestrSetting();
            SelectedRecipe = RecipesCollection.FirstOrDefault(x => x.Id == ReestrSetting.RecipeId);

        }

        protected override async void CloseDialogOnOk(IDialogParameters parameters)
        {
            Result = ButtonResult.OK;
            ReestrSetting.RecipeId = SelectedRecipe.Id;
            ReestrSetting.CurrentRecipe = SelectedRecipe;
            if (ReestrSetting.Id == 0)
            {
                await _reestrSettingDataService.CreateReestrSetting(ReestrSetting);
            }
            else
            {
                await _reestrSettingDataService.UpdateReestrSetting(ReestrSetting);
            }

            parameters = new DialogParameters
            {
                { "model", (ReestrSetting)ReestrSetting.Clone() }
            };

            base.CloseDialogOnOk(parameters);
        }

    }
}
