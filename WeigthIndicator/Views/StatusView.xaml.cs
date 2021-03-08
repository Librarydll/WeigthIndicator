using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    /// Interaction logic for StatusView.xaml
    /// </summary>
    public partial class StatusView : ReactiveUserControl<StatusViewModel>
    {
        public  Brush _isCriticalBrush = (Brush)new BrushConverter().ConvertFromString("#FFB33434");
        public  Brush _isNormal = (Brush)new BrushConverter().ConvertFromString("#61b15a");

        public StatusView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                this.OneWayBind(ViewModel,
                   vm => vm.RecipeReminder.Remainder,
                   v => v.Reminder.Text,
                  x => x.ToString("N"));

                this.Bind(ViewModel, vm => vm.RecipeReminder.RecipeShortName, v => v.RecipeName.Text)
                    .DisposeWith(disposables);


                this.OneWayBind(ViewModel, vm => vm.RecipeReminder.IsCritical, v => v.Sb.Background,
                    v=> BoolToColor(v))
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.LastReestrValue.BarrelNumber, v => v.LastBarrelNumber.Text);
                this.OneWayBind(ViewModel, vm => vm.LastReestrValue.Net, v => v.LastNet.Text,x=>x.ToString("N"));
                this.OneWayBind(ViewModel, vm => vm.LastReestrValue.Brutto, v => v.LastBrutto.Text, x => x.ToString("N"));

            });

        }

        private Brush BoolToColor(bool b)
        {
            if (b)
                return _isCriticalBrush;

            return _isNormal;

        }
    }
}
