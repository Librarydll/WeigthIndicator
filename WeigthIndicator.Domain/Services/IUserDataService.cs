using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.DTO;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Domain.Services
{
    public interface IUserDataService
    {
        Task<IEnumerable<User>> GetUsers(int currentPage, int pageSize, string searchingString);
        Task<AuthenticationResult> Login(string login, string password);
        Task<RegistrationResult> Register(User user);
        Task<Result> ChangeCredential(int id ,string login,string password);
        Task<Result> UpdateUser(int id ,string fio,string phoneNumber);
    }
}
