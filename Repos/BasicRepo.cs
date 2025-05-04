using System.Collections.Generic;
using System;
using SCMS.IRepo;
using SCMS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace SCMS.Repos
{
    public class BasicRepo<T> : IBasicRepo<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BasicRepo(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Func<IQueryable<T>?, IIncludableQueryable<T, object>>[]? includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => include(current));
            }

            return await query.ToListAsync();
        }

        //public async Task<T> GetByIdAsync(int id)
        //{
        //    return await _dbSet.FindAsync(id);
        //}

        public async Task<T> GetByIdAsync(int id, params Func<IQueryable<T>?, IIncludableQueryable<T, object>>[]? includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => include(current));
            }

            return await query.FirstOrDefaultAsync(entity => EF.Property<int>(entity, "Id") == id);
        }
        //public async Task<List<T>> GetByNameAsync(string name, params Func<IQueryable<T>?, IIncludableQueryable<T, object>>[]? includes)
        //{
        //    IQueryable<T> query = _dbSet;

        //    if (includes != null)
        //    {
        //        query = includes.Aggregate(query, (current, include) => include(current));
        //    }

        //    return await query.Where(entity => EF.Property<string>(entity, "Name") == name || EF.Property<string>(entity, "Title") == name).ToListAsync();
        //}

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }

}
