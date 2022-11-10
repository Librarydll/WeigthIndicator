using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using WeigthIndicator.Scanner.Services;
using WeigthIndicator.Scanner.Store;
using WeigthIndicator.Shared.Models;
using Xamarin.Forms;

namespace WeigthIndicator.Scanner.ViewModels
{
    public class ReestrsListViewModel : BaseViewModel
    {
        public int ReestrsCount => ReestrsStore.ReestrsCount;
        public double TotalNet => ReestrsStore.TotalNet;

        public string Comment => ReestrsStore.Comment;
        public string Title => ReestrsStore.Title;
        public ReestrsStore ReestrsStore { get; }

        public ObservableCollection<ReestrQrObject> Reestrs { get;  }

        public Command<ReestrQrObject> DeleteCommand { get; set; }

        public ReestrsListViewModel()
        {
            ReestrsStore = DependencyService.Resolve<ReestrsStore>();
            DeleteCommand = new Command<ReestrQrObject>(ExecuteDeleteCommand);
            Reestrs = new ObservableCollection<ReestrQrObject>(ReestrsStore.Reestrs);
        }

        private void ExecuteDeleteCommand(ReestrQrObject reestrQrObject)
        {
            ReestrsStore.RemoveReestr(reestrQrObject);
            Reestrs.Remove(reestrQrObject);

            var model = PrepareModelForRemovedReestr(reestrQrObject);
            DependencyService.Resolve<IWebsocketMessageHandler>().SendMessage(model);
        }

        private WebsocketRootModel PrepareModelForRemovedReestr(ReestrQrObject reestrQrObject)
        {
            return new WebsocketRootModel
            {
                MessageType = MessageType.ReestrRemoved,
                Reestr = reestrQrObject
            };
        }
    }
}
