using System;
using System.Collections.Generic;
using System.Text;

namespace WeigthIndicator.Dialog
{
    public interface IModal
    {
    }
    public interface IModal<T> : IModal
    {
        event Action<ModelDialogParameter<T>> DialogClosed;
        void OnDialogOpen(T model);
    }
}
