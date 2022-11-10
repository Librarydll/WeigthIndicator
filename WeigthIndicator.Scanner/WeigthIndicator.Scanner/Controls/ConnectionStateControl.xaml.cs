using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WeigthIndicator.Scanner.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConnectionStateControl : ContentView
    {
        public ConnectionStateControl()
        {
            InitializeComponent();
        }

        public bool IsConnected
        {
            get { return (bool)GetValue(IsConnectedProperty); }
            set { SetValue(IsConnectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsConnected.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty IsConnectedProperty =
            BindableProperty.Create("IsConnected", typeof(bool), typeof(ConnectionStateControl),false,BindingMode.TwoWay);


        public ICommand ReconnectCommand
        {
            get { return (ICommand)GetValue(ReconnectCommandProperty); }
            set { SetValue(ReconnectCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReconnectCommand.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty ReconnectCommandProperty =
            BindableProperty.Create("ReconnectCommand", typeof(ICommand), typeof(ConnectionStateControl), null);


    }
}