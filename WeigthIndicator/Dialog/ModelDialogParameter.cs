using System;
using System.Collections.Generic;
using System.Text;

namespace WeigthIndicator.Dialog
{
    public class ModelDialogParameter
    {
        public bool IsSuccess { get; set; }
        public static ModelDialogParameter Cancel => new ModelDialogParameter();

    }
    public class ModelDialogParameter<T> : ModelDialogParameter
    {     
        public T Value { get; set; }
        public static ModelDialogParameter<T> Success(T entity) => new ModelDialogParameter<T> { Value = entity, IsSuccess = true };
    }
}
