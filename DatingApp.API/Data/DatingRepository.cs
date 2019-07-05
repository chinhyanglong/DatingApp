using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(i=>i.Photos).FirstOrDefaultAsync(u =>u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserPrams userPrams)
        {
            var users =  _context.Users.Include(i=>i.Photos).OrderByDescending(p => p.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userPrams.UserId);
             
            users = users.Where(u => u.Gender == userPrams.Gender);

            if (userPrams.MinAge != 18 || userPrams.MaxAge != 99) {
                var minDob = DateTime.Today.AddYears(-userPrams.MaxAge -1);
                var maxDob = DateTime.Today.AddYears(-userPrams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }
            if(!string.IsNullOrEmpty(userPrams.OrderBy)){
                switch(userPrams.OrderBy)
                {
                    case "created": 
                        users = users.OrderByDescending(p=>p.Created);
                        break;
                    default:
                        users = users.OrderByDescending(p => p.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users,userPrams.PageNumber, userPrams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}