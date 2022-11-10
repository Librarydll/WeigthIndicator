using System;
using System.Collections.Generic;
using System.Text;

namespace WeigthIndicator.Shared.Models
{
    public class AuthorizeResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string FIO { get; set; }
        public int Id { get; set; }
    }
}
