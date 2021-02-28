using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.ViewModels
{
    public class CustomerViewModel:ReactiveObject
    {

        private ICustomerDataService _customerDataService;

        [Reactive] public Customer Customer { get; set; } = new Customer();
        [Reactive] public ObservableCollection<Customer> CustomersCollection { get; set; }
        public ReactiveCommand<Unit, Customer> CreateCustomer { get; }

        public CustomerViewModel(ICustomerDataService customerDataService)
        {
            _customerDataService = customerDataService;

            CreateCustomer = ReactiveCommand.CreateFromTask(ExecuteCreateCreateCustomer);
            CreateCustomer.Subscribe(x => AddCustomer(x));
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
