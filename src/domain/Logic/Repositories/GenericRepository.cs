﻿using Logic.Models;
using Logic.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Logic.Repositories
{
    public interface IGenericRepository<T> where T : Entity
    {
        Task<IReadOnlyList<T>> GetAll();
        Task<T> Get(long id);
        Task Insert(T obj);
        Task Update(T obj);
        Task Delete(long id);
        Task Save();
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : Entity
    {
        protected readonly EfDbContext _context;

        public GenericRepository(EfDbContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> Get(long id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Insert(T obj)
        {
            await _context.Set<T>().AddAsync(obj);
        }

        public async Task Update(T obj)
        {
            _context.Set<T>().Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }

        public async Task Delete(long id)
        {
            T existing = await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
            _context.Set<T>().Remove(existing);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
