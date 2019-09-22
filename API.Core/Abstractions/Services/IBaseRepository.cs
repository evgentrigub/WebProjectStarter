using System.Threading.Tasks;
using API.Core.Abstractions.Models;

namespace Project.Core.Abstractions.Services
{
    public interface IBaseRepository<TEnity>
        where TEnity : class
    {
        Task<IResult<TEnity>> Create(TEnity entity);
        Task<IResult<TEnity>> FindById(string id);
        Task<IResult> Delete(string id);
    }
}