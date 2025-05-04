using SCMS.Repos;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore.Query;

namespace SCMS.IRepo
{
    public interface IBasicRepo<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(params Func<IQueryable<T>?, IIncludableQueryable<T, object>>[]? includes);

        //Task<T> GetByIdAsync(int id);

        Task<T> GetByIdAsync(int id, params Func<IQueryable<T>?, IIncludableQueryable<T, object>>[]? includes);
        //Task<List<T>> GetByNameAsync(string name, params Func<IQueryable<T>?, IIncludableQueryable<T, object>>[]? includes)

        Task AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);
    }

}
