using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Core.Abstractions.Models;
using API.Models;
using GradePortalAPI.Dtos;
using Project.Core.Abstractions.Services;

namespace API.Services.Interfaces
{
    public interface IUserService: IBaseRepository<User>
    {
        /// <summary>
        ///     Authenticate user with username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IResult<User>> Authenticate(string username, string password);

        /// <summary>
        ///     Get users with table params: page number and page size
        /// </summary>
        /// <param name="tableParams"></param>
        /// <returns></returns>
        Task<IResult<IList<User>>> GetUsersWithParams(TableParamsDto tableParams);

        /// <summary>
        ///     Create new user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<IResult> Create(User user, string password);

        /// <summary>
        ///     Update user personal data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        IResult Update(string id, User user);

        /// <summary>
        ///     Get all users in table for future async request
        /// </summary>
        /// <returns></returns>
        IQueryable<User> GetAll();

        /// <summary>
        ///     Return quantity of all users
        /// </summary>
        /// <returns></returns>
        int CountAllUsers();

        string CreateToken(string userId);
    }
}
