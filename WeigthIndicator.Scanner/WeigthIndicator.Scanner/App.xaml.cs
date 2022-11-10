using System;
using WeigthIndicator.Scanner.Helper;
using WeigthIndicator.Scanner.Services;
using WeigthIndicator.Scanner.Store;
using WeigthIndicator.Scanner.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeigthIndicator.Scanner
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<ReestrsStore>();
            DependencyService.RegisterSingleton<IWebsocketClientService>(new WebsocketClientService(AppSettingHelper.LoadApplicationProperty().CreateWebsocketAddress()));
            DependencyService.RegisterSingleton<IWebsocketMessageHandler>(new WebsocketMessageHandler(DependencyService.Resolve<IWebsocketClientService>()));
            DependencyService.RegisterSingleton(new UserStore());
            MainPage = new AppShell();
            Shell.Current.GoToAsync("//LoginPage");
        }

        protected override void OnStart()
        {
            var ws = DependencyService.Resolve<IWebsocketClientService>();
            ws.Connect();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
