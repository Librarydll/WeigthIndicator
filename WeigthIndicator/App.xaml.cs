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

        //void ConfigureServices(IServiceCollection services)
        //{

        //    services.AddSingleton(x =>
        //    {
        //        return new ApplicationContextFactory("Data Source=127.0.0.1;port=3306;Database=indicatordb;Uid=root;Pwd=root");
        //    });

        //    services.AddSingleton<IRecipeDataService, RecipeDataService>();

        //    services.AddSingleton<AppBootstrapper>(); //Implements IScreen
        //    services.AddSingleton(s => new MainWindow(s.GetRequiredService<AppBootstrapper>()));
        //    services.AddSingleton<IScreen, AppBootstrapper>(x => x.GetRequiredService<AppBootstrapper>());

        //    //services.AddSingleton<IViewFor<ShellViewModel>, ShellView>();
        //    //services.AddSingleton<ShellViewModel>();
        //    //services.AddTransient<IViewFor<RecipeViewModel>, RecipeView>();
        //    //services.AddTransient<RecipeViewModel>();
        //    //services.AddSingleton<IViewFor<ReestrSettingViewModel>, ReestrSettingView>();
        //    //services.AddSingleton<ReestrSettingViewModel>();

        //}
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

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var connectionString  =  ConfigurationManager.ConnectionStrings[1].ConnectionString;
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
