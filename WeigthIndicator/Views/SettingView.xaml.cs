using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeigthIndicator.ViewModels;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : ReactiveUserControl<SettingViewModel>
    {
        public SettingView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                this.OneWayBind(ViewModel, vm => vm.ComPorts, v => v.ComportsCmb.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.BaudRates, v => v.BaudRateCmb.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.DataBits, v => v.DataBitsCmb.ItemsSource)
                   .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.StopBits, v => v.StopBitsCmb.ItemsSource)
                   .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.Parity, v => v.ParityBitsCmb.ItemsSource)
                   .DisposeWith(disposables);



                this.Bind(ViewModel, vm => vm.ComPortSetting.ComPortName,
                    v => v.ComportsCmb.SelectedItem)
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.ComPortSetting.BaudRate,
                    v => v.BaudRateCmb.SelectedItem,
                    x=> x.ToString(),
                    x=> x ==null ? 0 : int.Parse(x.ToString()))
                    .DisposeWith(disposables);
                this.Bind(ViewModel, vm => vm.ComPortSetting.DataBits,
                    v => v.DataBitsCmb.SelectedItem,
                    x => x.ToString(),
                    x => x == null ? 0 : int.Parse(x.ToString()))
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ComPortSetting.StopBits,
                    v => v.StopBitsCmb.SelectedItem,
                    x => x.ToString(),
                    x=> x == null ? StopBits.None : (StopBits)Enum.Parse(typeof(StopBits), x.ToString()))
                     .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ComPortSetting.Parity,
                    v => v.ParityBitsCmb.SelectedItem,
                    x => x.ToString(),
                    x => x == null ? Parity.None : (Parity)Enum.Parse(typeof(Parity), x.ToString()))
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.SaveSetting, v => v.SaveButton);
            });
        }
    }
}
