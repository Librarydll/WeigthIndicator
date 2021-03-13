using Prism.Services.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeigthIndicator.Dialogs.Common;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Models;
using WeigthIndicator.Services;

namespace WeigthIndicator.Dialogs
{
    public class ReestrSettingViewModel : DialogViewModelBase
    {
        private readonly string key = "vasya1999";

        private readonly IReestrSettingDataService _reestrSettingDataService;
        private readonly IRecipeDataService _recipeDataService;
        private readonly ICustomerDataService _customerDataService;

        private ObservableCollection<Recipe> _recipesCollection;
        public ObservableCollection<Recipe> RecipesCollection
        {
            get { return _recipesCollection; }
            set { this.RaiseAndSetIfChanged(ref _recipesCollection, value); }
        }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> CustomersCollection
        {
            get { return _customers; }
            set { this.RaiseAndSetIfChanged(ref _customers, value); }
        }

        [Reactive] public Recipe SelectedRecipe { get; set; }
        [Reactive] public Customer SelectedCustomer { get; set; }
        [Reactive] public string Password { get; set; }

        private Visibility _controlsVisibility = Visibility.Collapsed;
        public Visibility ControlsVisibility
        {
            get { return _controlsVisibility; }
            set { this.RaiseAndSetIfChanged(ref _controlsVisibility, value); }
        }


        [Reactive] public ReestrSetting ReestrSetting { get; set; } = new ReestrSetting();

        public ReestrSettingViewModel(
            IReestrSettingDataService reestrSettingDataService,
            IRecipeDataService recipeDataService, 
            ICustomerDataService customerDataService)
        {
            Title = "Настройки реестра";
            _reestrSettingDataService = reestrSettingDataService;
            _recipeDataService = recipeDataService;
            _customerDataService = customerDataService;

                 this.WhenAnyValue(x => x.Password)
                .Throttle(TimeSpan.FromSeconds(1))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ObserveOnDispatcher()
                .Subscribe(PasswordChecker);
        }

        public void PasswordChecker(string psw)
        {
            if (psw == key)
            {
                ControlsVisibility = Visibility.Visible;
            }
            else
            {
                ControlsVisibility = Visibility.Collapsed;
            }
            this.RaisePropertyChanged(nameof(ControlsVisibility));
        }

        public async Task<(IEnumerable<Recipe>,IEnumerable<Customer> ,ReestrSetting)> GetAsync()
        {
            var rs = await _reestrSettingDataService.GetReestrSetting();
            var collection = await _recipeDataService.GetRecipes();
            var customers = await _customerDataService.GetCustomers();
            return (collection, customers, rs);
        }

        public void Initialize((IEnumerable<Recipe>,IEnumerable<Customer> ,ReestrSetting) args)
        {
            RecipesCollection = new ObservableCollection<Recipe>(args.Item1);
            CustomersCollection = new ObservableCollection<Customer>(args.Item2);
            ReestrSetting = args.Item3 ?? new ReestrSetting();
            SelectedRecipe = RecipesCollection.FirstOrDefault(x => x.Id == ReestrSetting.RecipeId);
            SelectedCustomer = CustomersCollection.FirstOrDefault(x => x.Id == ReestrSetting.CustomerId);
        }

        protected override async void CloseDialogOnOk(IDialogParameters parameters)
        {
            Result = ButtonResult.OK;
            ReestrSetting.RecipeId = SelectedRecipe.Id;
            ReestrSetting.CurrentRecipe = SelectedRecipe;
            ReestrSetting.Customer = SelectedCustomer;
            ReestrSetting.CustomerId = SelectedCustomer.Id;
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
