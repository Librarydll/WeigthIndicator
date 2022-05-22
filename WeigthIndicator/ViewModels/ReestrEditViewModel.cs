using Prism.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WeigthIndicator.Dialog;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Store;

namespace WeigthIndicator.ViewModels
{
    public class ReestrEditViewModel : ReactiveObject, IModal<Reestr>
    {
        private readonly ICustomerDataService _customerDataService;
        private readonly ModalNavigationStore _modalNavigationStore;

        [Reactive] public Reestr Reestr { get; set; }

        private ObservableCollection<Customer> _customers;

        public event Action<ModelDialogParameter<Reestr>> DialogClosed;

        public ObservableCollection<Customer> CustomersCollection
        {
            get { return _customers; }
            set { this.RaiseAndSetIfChanged(ref _customers, value); }
        }
        [Reactive] public Customer SelectedCustomer { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand SubmitCommand { get; set; }

        public ReestrEditViewModel(ICustomerDataService customerDataService,ModalNavigationStore modalNavigationStore)
        {
            _customerDataService = customerDataService;
            _modalNavigationStore = modalNavigationStore;
            SubmitCommand = new DelegateCommand(ExecuteSubmitCommand);
            CancelCommand = new DelegateCommand(() => modalNavigationStore.Close());
            Task.Run(Initialize);
        }


        private void ExecuteSubmitCommand()
        {
            CloseDialogSuccess();
        }

        private async Task Initialize()
        {
            var customers = await _customerDataService.GetCustomers();
            CustomersCollection = new ObservableCollection<Customer>(customers);
            SelectedCustomer = CustomersCollection.FirstOrDefault(x => x.Id == Reestr.CustomerId);
        }


        public void OnDialogOpen(Reestr model)
        {
            if (model != null)
                Reestr = (Reestr)model.Clone();
        }

        public void CloseDialogSuccess()
        {
            Reestr.Customer = SelectedCustomer;
            Reestr.CustomerId = SelectedCustomer.Id;

            var model = new ModelDialogParameter<Reestr>
            {
                IsSuccess = true,
                Value = Reestr
            };

            DialogClosed?.Invoke(model);
            _modalNavigationStore.Close();
        }
    }
}
