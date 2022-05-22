using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.DTO
{
    public class AuthBase
    {
        public User User { get; set; }
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }
}
