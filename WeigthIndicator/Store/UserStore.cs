using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Store
{
    public class UserStore
    {

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set => _currentUser =value;
        }
    }
}
