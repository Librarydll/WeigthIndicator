using Newtonsoft.Json;
using Prism.Services.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeigthIndicator.Dialogs.Common;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dialogs
{
    public class ReestrEditViewModel: DialogViewModelBase
    {
        private readonly ICustomerDataService _customerDataService;
        private readonly IRecipeDataService _recipeDataService;
        private Task _recipeTask;
        [Reactive] public Reestr Reestr { get; set; }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> CustomersCollection
        {
            get { return _customers; }
            set { this.RaiseAndSetIfChanged(ref _customers, value); }
        }
        [Reactive] public Customer SelectedCustomer { get; set; }
        [Reactive] public IEnumerable<Recipe> Recipes { get; set; }
        [Reactive] public Recipe SelectedRecipe { get; set; }
        [Reactive] public string BatchNumber { get; set; }


        public ReestrEditViewModel(
            ICustomerDataService customerDataService,
            IRecipeDataService recipeDataService)
        {
            _customerDataService = customerDataService;
            _recipeDataService = recipeDataService;
            Title = "Редактирования реестра";
            Recipes = new List<Recipe>();
            

        }

        public async Task<IEnumerable<Customer>> GetAsync()
        {
           return await _customerDataService.GetCustomers();
        }

        public void Initialize(IEnumerable<Customer> customers)
        {
            CustomersCollection = new ObservableCollection<Customer>(customers);
            SelectedCustomer = CustomersCollection.FirstOrDefault(x => x.Id == Reestr.CustomerId);
            BatchNumber = Reestr.BatchNumber;
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters != null)
            {
                Reestr = (Reestr)parameters.GetValue<Reestr>("model").Clone();
                _recipeTask = LoadRecipes();
            }
        }

        protected override  void CloseDialogOnOk(IDialogParameters parameters)
        {
            if (SelectedRecipe == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(BatchNumber))
            {
                return;
            }

            var oldReestr = JsonConvert.SerializeObject(Reestr);

            FileLogger.Log( $"Изменение реестра было {oldReestr}");

            Reestr.Customer = SelectedCustomer;
            Reestr.BatchNumber = BatchNumber;
            Reestr.CustomerId = SelectedCustomer.Id;
            Reestr.RecipeId = SelectedRecipe.Id;
            Reestr.Recipe = SelectedRecipe;
            Result = ButtonResult.OK;
            parameters = new DialogParameters();
            parameters.Add("model", Reestr);

            FileLogger.Log( $"Изменение реестра стало {JsonConvert.SerializeObject(Reestr)}");

            base.CloseDialogOnOk(parameters);
        }

        private async Task LoadRecipes()
        {
            var recipes = await _recipeDataService.GetRecipes();

            Application.Current.Dispatcher.Invoke(() =>
            {
                Recipes = recipes.OrderBy(t => t.ShortName);
                SelectedRecipe = Recipes.FirstOrDefault(t => t.Id == Reestr.RecipeId);
            });
        }


    }
    public static class FileLogger
    {
        private static readonly string LogFilePath;

        static FileLogger()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var logDirectory = Path.Combine(documentsPath, "WeigthIndicator");
            Directory.CreateDirectory(logDirectory); // Создаст, если нет

            LogFilePath = Path.Combine(logDirectory, "log.txt");
        }

        public static void Log(string message)
        {
            try
            {
                string logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";
                File.AppendAllText(LogFilePath, logLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Fails silently — можно расширить, если нужно
               // MessageBox.Show("Ошибка при записи лога: " + ex.Message);
            }
        }
    }
}
