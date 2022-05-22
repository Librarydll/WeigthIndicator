using Prism.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using WeigthIndicator.Dialog;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Store;

namespace WeigthIndicator.ViewModels
{
    public class ReestrSettingViewModel : ReactiveObject ,IModal<ReestrSetting>
    {
        private readonly string key = "lenovo2021";
        private readonly ModalNavigationStore _modalNavigationStore;
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

        [Reactive] public Recipe SelectedRecipe { get; set; }
        [Reactive] public Customer SelectedCustomer { get; set; }
        [Reactive] public string Password { get; set; }

        private bool _controlsVisibility = false;

        public event Action<ModelDialogParameter<ReestrSetting>> DialogClosed;

        public bool ControlsEnabled
        {
            get { return _controlsVisibility; }
            set { this.RaiseAndSetIfChanged(ref _controlsVisibility, value); }
        }


        [Reactive] public ReestrSetting ReestrSetting { get; set; } = new ReestrSetting();
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SubmitCommand { get; set; }
        public ReestrSettingViewModel(
            ModalNavigationStore modalNavigationStore,
            IBarrelStorageDataService barrelStorageDataService,
            IReestrSettingDataService reestrSettingDataService,
            IRecipeDataService recipeDataService,
            ICustomerDataService customerDataService)
        {
            _modalNavigationStore = modalNavigationStore;
            _barrelStorageDataService = barrelStorageDataService;
            _reestrSettingDataService = reestrSettingDataService;
            _recipeDataService = recipeDataService;
            _customerDataService = customerDataService;

            this.WhenAnyValue(x => x.Password)
                .Throttle(TimeSpan.FromSeconds(1))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ObserveOnDispatcher()
                .Subscribe(PasswordChecker);

            this.WhenAnyValue(x => x.SelectedRecipe)
                .Skip(1)
                .SelectMany(async (x) => await _barrelStorageDataService.GetLastBarrelNumber(x))
                .ObserveOnDispatcher()
                .Subscribe(number => ReestrSetting.InitialBarrelNumber = number+1);

            Task.Run(Initialize);
            SubmitCommand = new DelegateCommand(ExecuteSubmitCommand);
            CancelCommand = new DelegateCommand(() => modalNavigationStore.Close());
        }

        private async void ExecuteSubmitCommand()
        {
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

            var result = new ModelDialogParameter<ReestrSetting>
            {
                IsSuccess = true,
                Value = (ReestrSetting)ReestrSetting.Clone()
            };
            _modalNavigationStore.Close();
            DialogClosed?.Invoke(result);
        }

        private async Task Initialize()
        {
            var rs = await _reestrSettingDataService.GetReestrSetting();
            var collection = await _recipeDataService.GetRecipes();
            var customers = await _customerDataService.GetCustomers();
            var barrelNumber = await _barrelStorageDataService.GetLastBarrelNumber(rs.CurrentRecipe);

            RecipesCollection = new ObservableCollection<Recipe>(collection);
            CustomersCollection = new ObservableCollection<Customer>(customers);
            ReestrSetting = rs ?? new ReestrSetting();
            SelectedRecipe = RecipesCollection.FirstOrDefault(x => x.Id == ReestrSetting.RecipeId);
            SelectedCustomer = CustomersCollection.FirstOrDefault(x => x.Id == ReestrSetting.CustomerId);
            ReestrSetting.InitialBarrelNumber = barrelNumber + 1;
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
  
        public void OnDialogOpen(ReestrSetting model)
        {
           
        }
    }
}
