using Prism.Ioc;
using Prism.Unity;
using System.Configuration;
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
            var loginvm = Container.Resolve<LoginViewModel>();
            var store = Container.Resolve<NavigationStore>();

            var v = Container.Resolve<MainView>();
            store.CurrentViewModel = loginvm;

            return v;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<IComPortProvider>().ComPortConnector?.Dispose();
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
            containerRegistry.RegisterSingleton<IDialogNavigationService, DialogNavigationService>();
            containerRegistry.RegisterSingleton<UserStore>();
            containerRegistry.RegisterSingleton<NavigationStore>();
            containerRegistry.RegisterSingleton<ModalNavigationStore>();
            containerRegistry.RegisterSingleton<NavigationViewModel>();
            containerRegistry.RegisterSingleton<MainViewModel>();
            containerRegistry.RegisterSingleton<ReestrSettingViewModel>();
            containerRegistry.RegisterSingleton<ShellViewModel>();
            containerRegistry.RegisterSingleton<ReestrEditViewModel>();
            containerRegistry.RegisterSingleton<MainView>(c =>
            {
                return new MainView
                {
                    DataContext = c.Resolve<MainViewModel>()
                };
            });
        }
    }
}
