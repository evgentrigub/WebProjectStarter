using System;
using System.Threading.Tasks;
using API.Core.Abstractions.Models;
using API.Helpers;
using API.Models.Base;
using Microsoft.EntityFrameworkCore;
using Project.Core.Abstractions.Services;

namespace API.Services.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class
    {
        private readonly DataContext _context;

        public BaseRepository(DataContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Set = _context.Set<TEntity>();
        }

        protected virtual DbSet<TEntity> Set { get; }

        public async Task<IResult<TEntity>> Create(TEntity entity)
        {
            try
            {
                var createdEntity = await Set.AddAsync(entity);
                if (createdEntity.Entity != null)
                {
                    await _context.SaveChangesAsync();
                    return new Result<TEntity>(message: "Success!", isSuccess: true, data: entity);
                }

                return new Result<TEntity>(message: "Base Repository Create Error", isSuccess:false, data:null);
            }
            catch (AppException e)
            {
                throw new AppException("BaseRepository Create Entity Error:" + e.Message);
            }
        }

        public async Task<IResult<TEntity>> FindById(string id)
        {
            try
            {
                var entity = await Set.FindAsync(id);
                if(entity == null) 
                    return new Result<TEntity>(message:"Can not find entity with id:"+id, isSuccess:false, data:null);

                return new Result<TEntity>(message:"Success!", isSuccess:true, data:entity);
            }
            catch (AppException e)
            {
                throw new AppException("BaseRepository Find Entity Error:" + e.Message);
            }
        }

        public async Task<IResult> Delete(string id)
        {
            try
            {
                var result = await FindById(id);
                if(!result.IsSuccess)
                    return new Result(result.Message, isSuccess:false);

                Set.Remove(result.Data);
                return new Result("Delete successful", true);
            }
            catch (AppException e)
            {
                throw new AppException("BaseRepository Delete Entity Error:" + e.Message);
            }
        }
    }
}