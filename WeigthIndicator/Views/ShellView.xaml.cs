﻿using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using WeigthIndicator.ViewModels;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : ReactiveUserControl<ShellViewModel>
    {
        public ShellView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                this.OneWayBind(ViewModel,
                    vm => vm.ReestrsCollection,
                    v => v.ReestrsCollection.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ItemWeigth, v => v.ItemWeigth.Text);
                this.Bind(ViewModel, vm => vm.ProgressValue, v => v.ProgressBar.Value);
                this.Bind(ViewModel, vm => vm.MaxProgressValue, v => v.ProgressBar.Maximum);
                this.Bind(ViewModel, vm => vm.IsAutoMode, v => v.@switch.IsOn);
                this.BindCommand(ViewModel, vm => vm.OpenReestrSettingCommand, v => v.ReestrSetting);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveBtn);
                this.BindCommand(ViewModel, vm => vm.EditCommand, v => v.EditCommand,x=>x.SelectedReestr);
                this.BindCommand(ViewModel, vm => vm.PrintCommand, v => v.PrintCommand,x=>x.SelectedReestr);

                this.Bind(ViewModel, vm => vm.SelectedReestr, v => v.ReestrsCollection.SelectedItem);

                this.WhenAnyValue(x => x.ViewModel)
                    .SelectMany(x => x.Initialize())
                    .ObserveOnDispatcher()
                    .Subscribe(x => ViewModel.FillCollection(x))
                    .DisposeWith(disposables);
            });

        }

        private void ReestrsCollection_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
