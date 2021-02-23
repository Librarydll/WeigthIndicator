using Prism.Services.Dialogs;
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
    public class ReestrSettingViewModel: DialogViewModelBase
    {
        private readonly IReestrSettingProvider _reestrSettingProvider;
        private readonly IRecipeDataService _recipeDataService;
        [Reactive] public ObservableCollection<Recipe> RecipesCollection { get; set; }
        [Reactive] public Recipe SelectedRecipe { get; set; }

        public ReestrSetting ReestrSetting { get; set; } = new ReestrSetting();
        public ReestrSettingViewModel(
            IReestrSettingProvider reestrSettingProvider,
            IRecipeDataService recipeDataService)
        {
            Title = "Настройки реестра";
            _reestrSettingProvider = reestrSettingProvider;
            _recipeDataService = recipeDataService;

            InitializeCollection();
        }


        public async Task InitializeCollection()
        {
            var recipes = await _recipeDataService.GetRecipes();
            RecipesCollection = new ObservableCollection<Recipe>(recipes);
            SelectedRecipe = RecipesCollection.FirstOrDefault();
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            ReestrSetting =(ReestrSetting)_reestrSettingProvider.ReestrSetting.Clone();
        }

        protected override void CloseDialogOnOk(IDialogParameters parameters)
        {
            Result = ButtonResult.OK;
            _reestrSettingProvider.ReestrSetting = ReestrSetting;
            base.CloseDialogOnOk(parameters);
        }
    }
}
