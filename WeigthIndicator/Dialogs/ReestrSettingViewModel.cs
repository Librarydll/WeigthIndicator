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
    public class ReestrSettingViewModel : DialogViewModelBase
    {
        private readonly IReestrSettingProvider _reestrSettingProvider;
        private readonly IReestrSettingDataService _reestrSettingDataService;
        private readonly IRecipeDataService _recipeDataService;

        private ObservableCollection<Recipe> _recipesCollection;
        public ObservableCollection<Recipe> RecipesCollection
        {
            get { return _recipesCollection; }
            set {  _recipesCollection = value; RaisePropertyChanged(); }
        }

        [Reactive] public Recipe SelectedRecipe { get; set; }

        [Reactive] public ReestrSetting ReestrSetting { get; set; } = new ReestrSetting();
        public ReestrSettingViewModel(
            IReestrSettingProvider reestrSettingProvider,
            IReestrSettingDataService reestrSettingDataService,
            IRecipeDataService recipeDataService)
        {
            Title = "Настройки реестра";
            _reestrSettingProvider = reestrSettingProvider;
            _reestrSettingDataService = reestrSettingDataService;
            _recipeDataService = recipeDataService;
            Initialize();
        }

        public async Task Initialize()
        {
            var recipes = await _recipeDataService.GetRecipes();
            RecipesCollection = new ObservableCollection<Recipe>(recipes);
            ReestrSetting = await _reestrSettingDataService.GetReestrSetting() ?? new ReestrSetting();
            _reestrSettingProvider.ReestrSetting = (ReestrSetting)ReestrSetting.Clone();
            SelectedRecipe = RecipesCollection.FirstOrDefault(x => x.Id == ReestrSetting.RecipeId);

        }

        public async Task<IEnumerable<Recipe>> GetRecipesAsync()
        {

            return await _recipeDataService.GetRecipes();
        }

        public void InitializeCollection(IEnumerable<Recipe> recipes)
        {


        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
           
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
            _reestrSettingProvider.ReestrSetting = (ReestrSetting)ReestrSetting.Clone();
            base.CloseDialogOnOk(parameters);
        }
      
    }
}
