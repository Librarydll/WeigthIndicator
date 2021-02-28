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

namespace WeigthIndicator.Dialogs
{
    public class ReestrEditViewModel: DialogViewModelBase
    {
        private readonly ICustomerDataService _customerDataService;

        [Reactive] public Reestr Reestr { get; set; }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> CustomersCollection
        {
            get { return _customers; }
            set { this.RaiseAndSetIfChanged(ref _customers, value); }
        }
        [Reactive] public Customer SelectedCustomer { get; set; }


        public ReestrEditViewModel(ICustomerDataService customerDataService)
        {
            _customerDataService = customerDataService;
            Title = "Редактирования реестра";
        }

        public async Task<IEnumerable<Customer>> GetAsync()
        {
           return await _customerDataService.GetCustomers();
        }

        public void Initialize(IEnumerable<Customer> customers)
        {
            CustomersCollection = new ObservableCollection<Customer>(customers);
            SelectedCustomer = CustomersCollection.FirstOrDefault(x => x.Id == Reestr.CustomerId);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters != null)
            {
                Reestr = (Reestr)parameters.GetValue<Reestr>("model").Clone();
            }
        }

        protected override void CloseDialogOnOk(IDialogParameters parameters)
        {
            Reestr.Customer = SelectedCustomer;
            Reestr.CustomerId = SelectedCustomer.Id;
            Result = ButtonResult.OK;
            parameters = new DialogParameters();
            parameters.Add("model", Reestr);
            base.CloseDialogOnOk(parameters);
        }
    }
}
