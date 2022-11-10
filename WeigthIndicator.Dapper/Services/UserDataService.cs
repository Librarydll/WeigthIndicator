using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain;
using WeigthIndicator.Domain.DTO;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;

namespace WeigthIndicator.Dapper.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly ApplicationContextFactory _factory;

        public UserDataService(ApplicationContextFactory factory)
        {
            _factory = factory;
        }
        public async Task<Result> ChangeCredential(int id, string login, string password)
        {
            using (var connection = _factory.CreateConnection())
            {
                var query = "Select *from users where login=@login";
                var existedUser = await connection.QueryFirstOrDefaultAsync<User>(query, new { login });
                if (existedUser != null)
                {
                    return Result.CreateError("Пользователь с таким логином уже существует");
                }

                var queryUpdate = "Update users set login=@login,password=@password where id =@id";

                var row = await connection.ExecuteAsync(queryUpdate, (id, password, login));
                return new Result
                {
                    IsSuccess = row > 0
                };
            }
        }

        public async Task<IEnumerable<User>> GetUsers(int currentPage, int pageSize, string searchingString)
        {
            using (var connection = _factory.CreateConnection())
            {
                return await connection.GetAllAsync<User>();
            }
        }

        public async Task<AuthenticationResult> Login(string login, string password)
        {
            using (var connection = _factory.CreateConnection())
            {
                var query = "Select *from users where login=@login and password =@password";
                var existedUser = await connection.QueryFirstOrDefaultAsync<User>(query, new { login, password });
                if(existedUser == null)
                {
                    return new AuthenticationResult
                    {
                        Error = "Неверно введен логин или пароль"
                    };
                }
                return new AuthenticationResult
                {
                    User = existedUser,
                    IsSuccess =true
                };
            }
        }

        public async Task<RegistrationResult> Register(User user)
        {
            using (var connection = _factory.CreateConnection())
            {
                var query = "Select *from users where login=@login";
                var existedUser = await connection.QueryFirstOrDefaultAsync<User>(query, new { user.Login });
                if (existedUser != null)
                {
                    return new RegistrationResult
                    {
                        Error = "Пользователь с таким логином уже существует"
                    };
                }
                user.Id = await connection.InsertAsync(user);
                return new RegistrationResult
                {
                    User = user,
                    IsSuccess =true
                };
            }
        }

        public async Task<Result> UpdateUser(int id, string fio, string phoneNumber)
        {
            using (var connection = _factory.CreateConnection())
            {
                var query = "Update users set fio=@fio,phoneNumber=@phoneNumber where id =@id";
                var row = await connection.ExecuteAsync(query, new { id, fio, phoneNumber });
                return new Result
                {
                    IsSuccess = row > 0
                };
            }
        }
    }
}
