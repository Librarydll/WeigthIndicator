using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;


namespace WeigthIndicator.Dialogs.Common
{
    public class DialogViewModelBase : BindableBase, IDialogAware
	{
		public event Action<IDialogResult> RequestClose;

		public ButtonResult Result { get; set; }
		private string _title;
		public string Title
		{
			get { return _title; }
			set { SetProperty(ref _title, value); }
		}
		public DelegateCommand<IDialogParameters> CloseDialogCancelCommand =>
			new DelegateCommand<IDialogParameters>(CloseDialogOnCancel);

		public DelegateCommand<IDialogParameters> CloseDialogOkCommand =>
			new DelegateCommand<IDialogParameters>(CloseDialogOnOk);

		protected virtual void CloseDialogOnCancel(IDialogParameters parameters)
		{
			Result = ButtonResult.Cancel;

			CloseDialog(parameters);
		}

		protected virtual void CloseDialogOnOk(IDialogParameters parameters)
		{
			Result = ButtonResult.OK;

			CloseDialog(parameters);
		}


		public bool CanCloseDialog()
		{
			return true;
		}

		public virtual void OnDialogClosed()
		{
		}

		public  void CloseDialog(IDialogParameters parameters)
		{
			RequestClose?.Invoke(new DialogResult(Result, parameters));
		}

		public virtual void OnDialogOpened(IDialogParameters parameters)
		{

		}
	}
}
