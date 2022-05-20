using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeigthIndicator.Core.Print;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Factory;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class CustomerViewModel:ReactiveObject
    {

        private ICustomerDataService _customerDataService;

        [Reactive] public Customer Customer { get; set; } = new Customer();
        [Reactive] public ObservableCollection<Customer> CustomersCollection { get; set; }
        public ReactiveCommand<Unit, Customer> CreateCustomer { get; }
        public ReactiveCommand<Customer, Unit> PrintCommand { get; set; }
        [Reactive] public Customer SelectedCustomer { get; set; }

        public CustomerViewModel(ICustomerDataService customerDataService)
        {
            _customerDataService = customerDataService;

            CreateCustomer = ReactiveCommand.CreateFromTask(ExecuteCreateCreateCustomer);
            CreateCustomer.Subscribe(x => AddCustomer(x));

            var canPrint = this
                .WhenAnyValue(x => x.SelectedCustomer)
                .Select(x => x != null);

            PrintCommand = ReactiveCommand.Create<Customer>(ExecutePrintCommand, canPrint);
        }

        private void ExecutePrintCommand(Customer customer)
        {
            var printInitialize = PrintPreviewFactory.GetPrintView(PrintViewType.BuyerInformation);
            var flowDoc = printInitialize.InitializeFlow(new Models.ViewModels.ReestrObject(new Reestr() { Customer = customer }));
            PrintHelper.Prints(flowDoc, customer.ShortName);
        }

        public async Task<IEnumerable<Customer>> GetCollectionsAsync()
        {
           return await _customerDataService.GetCustomers();
        }

        public void InitializeCollection(IEnumerable<Customer> collections)
        {
            CustomersCollection = new ObservableCollection<Customer>(collections);
        }

        private void AddCustomer(Customer customer)
        {
            CustomersCollection.Add((Customer)customer.Clone());
            Customer = new Customer();
        }

        private async Task<Customer> ExecuteCreateCreateCustomer()
        {
            await _customerDataService.CreateCustomer(Customer);

            return Customer;
        }
    }
}
