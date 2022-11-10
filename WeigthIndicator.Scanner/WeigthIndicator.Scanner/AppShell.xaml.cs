using System;
using System.Collections.Generic;
using WeigthIndicator.Scanner.ViewModels;
using WeigthIndicator.Scanner.Views;
using Xamarin.Forms;

namespace WeigthIndicator.Scanner
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(OutcomePage), typeof(OutcomePage));
            Routing.RegisterRoute(nameof(IncomePage), typeof(IncomePage));
        }

    }
}
