using Prism.Ioc;
using Prism.Unity;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Windows;
using WeigthIndicator.Dapper;
using WeigthIndicator.Dapper.Services;
using WeigthIndicator.Dialog;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Services;
using WeigthIndicator.Store;
using WeigthIndicator.ViewModels;
using WeigthIndicator.Views;

namespace WeigthIndicator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        
        public App()
        {
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;

        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
           MessageBox.Show(e.Exception.Message);
           MessageBox.Show(e.Exception.InnerException?.Message);
        }

        protected override Window CreateShell()
        {
            var loginvm = Container.Resolve<NavigationViewModel>();
            var store = Container.Resolve<NavigationStore>();

            var v = Container.Resolve<MainView>();
            store.CurrentViewModel = loginvm;

            return v;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<IComPortProvider>().ComPortConnector?.Dispose();
            Container.Resolve<IWebsocketServer>()?.Dispose();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var connectionString  =  ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            containerRegistry.RegisterSingleton<ApplicationContextFactory>(_ =>
            {
                return new ApplicationContextFactory(connectionString);
            });

            containerRegistry.RegisterSingleton<IRecipeDataService, RecipeDataService>();
            containerRegistry.RegisterSingleton<IReestrDataService, ReestrDataService>();
            containerRegistry.RegisterSingleton<ICustomerDataService, CustomerDataService>();
            containerRegistry.RegisterSingleton<IBarrelStorageDataService, BarrelStorageDataService>();
            containerRegistry.RegisterSingleton<IReestrSettingDataService, ReestrSettingDataService>();
           // containerRegistry.RegisterSingleton<IReestrSettingProvider, ReestrSettingProvider>();
            containerRegistry.RegisterSingleton<IComPortProvider, ComPortProvider>();
            containerRegistry.RegisterSingleton<IUserDataService, UserDataService>();
            containerRegistry.RegisterSingleton<IOutcomeDataService, OutcomeDataService>();
            containerRegistry.RegisterSingleton<IDialogNavigationService, DialogNavigationService>();
            containerRegistry.RegisterSingleton<IHelperDataService, HelperDataService>();
            containerRegistry.RegisterSingleton<IManufactureDataService, ManufactureDataService>();
            containerRegistry.RegisterSingleton<IWebsocketServer>(c=>
            {
              var ipAddress =  Dns.GetHostEntry(Dns.GetHostName())
                    .AddressList
                    .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?
                    .ToString();

                return new WebsocketServer($"ws://{ipAddress}:7070", c.Resolve<IWebsocketClientService>(), c.Resolve<IWebsocketMessageHandler>());
            });
            containerRegistry.RegisterSingleton<IWebsocketClientService, WebsocketClientService>();
            containerRegistry.RegisterSingleton<IWebsocketMessageHandler, WebsocketMessageHandler>();
            containerRegistry.RegisterSingleton<UserStore>();
            containerRegistry.RegisterSingleton<NavigationStore>();
            containerRegistry.RegisterSingleton<ModalNavigationStore>();
            containerRegistry.RegisterSingleton<NavigationViewModel>();
            containerRegistry.RegisterSingleton<MainViewModel>();
            containerRegistry.RegisterSingleton<ShellViewModel>();
            containerRegistry.RegisterSingleton<ReestrViewModel>();
            containerRegistry.RegisterSingleton<GroupedReestrViewModel>();
            containerRegistry.RegisterSingleton<OutcomeWebsocketViewModel>();
            containerRegistry.RegisterSingleton<MainView>(c =>
            {
                return new MainView
                {
                    DataContext = c.Resolve<MainViewModel>()
                };
            });
            containerRegistry.Register<AllReestrsViewModel>();
            containerRegistry.Register<CreateUpdateOutcomeViewModel>();
            containerRegistry.Register<OutcomeViewModel>();
            containerRegistry.Register<ReestrEditViewModel>();
            containerRegistry.Register<ReestrSettingViewModel>();
            containerRegistry.Register<ManufactureViewModel>();

        }

    }
}
