using Prism.Ioc;
using Prism.Unity;
using ReactiveUI;
using System.Configuration;
using System.Windows;
using WeigthIndicator.Dapper;
using WeigthIndicator.Dapper.Services;
using WeigthIndicator.Dialogs;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Services;
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
            return Container.Resolve<MainView>();
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

            containerRegistry.RegisterDialog<ReestrSettingView, ReestrSettingViewModel>();
            containerRegistry.RegisterDialog<ReestrEditView, ReestrEditViewModel>();

        }
    }
}
