using System;
using System.Threading.Tasks;
using DatingApp.Api.Models;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DatingApp.Api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context=context;
        }
        public async Task<User> Login(String userName, string password)
        {
            var user =await _context.Users.FirstOrDefaultAsync(x=>x.UserName==userName);
            if(user==null )
                return null;

            if(!VerifiedPasswordHash(password,user.PassWordHash,user.PassWordSalt))
                return null;

            return user;
        }

        private bool VerifiedPasswordHash(string password, byte[] passWordHash, byte[] passWordSalt)
        {
           using( var hmac=  new System.Security.Cryptography.HMACSHA512(passWordSalt))
           {
               var computedHash =hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
               for (int i = 0; i < computedHash.Length; i++)
               {
                   if(computedHash[i]!=passWordHash[i])
                   return false;
                   
               }
           }
           return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passWordHash,passwordSalt;
            CreatePasswordHash(password,out  passWordHash,out passwordSalt);
            user.PassWordHash=passWordHash;
            user.PassWordSalt=passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
            
        }

        private void CreatePasswordHash(string password, out byte[] passWordHash, out byte[] passwordSalt)
        {
           using( var hmac=  new System.Security.Cryptography.HMACSHA512())
           {
               passwordSalt=hmac.Key;
               passWordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        
           }

        }

        public async Task<bool> UserExist(string userName)
        {
            if( await _context.Users.AnyAsync(x=>x.UserName==userName))
             return true;

            return false;
        }
    }
}