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
        private readonly string key = "lenovo2021";
        private readonly IBarrelStorageDataService _barrelStorageDataService;
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

        private ObservableCollection<ManufactureTitle> _manufactures;
        public ObservableCollection<ManufactureTitle> Manufactures
        {
            get => _manufactures;
            set { this.RaiseAndSetIfChanged(ref _manufactures, value); }
        }
        [Reactive] public ManufactureTitle SelectedManufacture { get; set; }
        [Reactive] public Recipe SelectedRecipe { get; set; }
        [Reactive] public Customer SelectedCustomer { get; set; }
        [Reactive] public string Password { get; set; }

        private bool _controlsVisibility = false;
        public bool ControlsEnabled
        {
            get { return _controlsVisibility; }
            set { this.RaiseAndSetIfChanged(ref _controlsVisibility, value); }
        }


        [Reactive] public ReestrSetting ReestrSetting { get; set; } = new ReestrSetting();

        public ReestrSettingViewModel(
            IBarrelStorageDataService barrelStorageDataService,
            IReestrSettingDataService reestrSettingDataService,
            IRecipeDataService recipeDataService,
            ICustomerDataService customerDataService)
        {
            Title = "Настройки реестра";
            _barrelStorageDataService = barrelStorageDataService;
            _reestrSettingDataService = reestrSettingDataService;
            _recipeDataService = recipeDataService;
            _customerDataService = customerDataService;

            this.WhenAnyValue(x => x.Password)
                .Throttle(TimeSpan.FromSeconds(0.5))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ObserveOnDispatcher()
                .Subscribe(PasswordChecker);

            this.WhenAnyValue(x => x.SelectedRecipe)
                .Skip(1)
                .SelectMany(async (x) => await _barrelStorageDataService.GetLastBarrelNumber(x))
                .ObserveOnDispatcher()
                .Subscribe(number => ReestrSetting.InitialBarrelNumber = number + 1);
        
            Manufactures = new ObservableCollection<ManufactureTitle>
            {
                new ManufactureTitle("Agromir"),
                new ManufactureTitle("Gazalkent")
            };

            SelectedManufacture = Manufactures.FirstOrDefault(x => x.Title == ManufactureProvider.ManufactureType);

        
        }


        public void PasswordChecker(string psw)
        {
            if (psw == key)
            {
                ControlsEnabled = true;
            }
            else
            {
                ControlsEnabled = false;
            }
            this.RaisePropertyChanged(nameof(ControlsEnabled));
        }

        public async Task<(IEnumerable<Recipe>, IEnumerable<Customer>, ReestrSetting, int)> GetAsync()
        {
            var rs = await _reestrSettingDataService.GetReestrSetting();
            var collection = await _recipeDataService.GetRecipes();
            var customers = await _customerDataService.GetCustomers();
            var barrelNumber = await _barrelStorageDataService.GetLastBarrelNumber(rs.CurrentRecipe);
            return (collection, customers, rs, barrelNumber);
        }

        public void Initialize((IEnumerable<Recipe>, IEnumerable<Customer>, ReestrSetting, int) args)
        {
            RecipesCollection = new ObservableCollection<Recipe>(args.Item1);
            CustomersCollection = new ObservableCollection<Customer>(args.Item2);
            ReestrSetting = args.Item3 ?? new ReestrSetting();
            SelectedRecipe = RecipesCollection.FirstOrDefault(x => x.Id == ReestrSetting.RecipeId);
            SelectedCustomer = CustomersCollection.FirstOrDefault(x => x.Id == ReestrSetting.CustomerId);
            ReestrSetting.InitialBarrelNumber = args.Item4 + 1;
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
            ManufactureProvider.ManufactureType = SelectedManufacture.Title;
            parameters = new DialogParameters
            {
                { "model", (ReestrSetting)ReestrSetting.Clone() }
            };

            base.CloseDialogOnOk(parameters);
        }

    }
}
