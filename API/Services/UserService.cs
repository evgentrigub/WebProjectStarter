﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Core.Abstractions.Models;
using API.Helpers;
using API.Models;
using API.Models.Base;
using API.Services.Interfaces;
using API.Services.Repositories;
using GradePortalAPI.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class UserService : BaseRepository<User>, IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly DataContext _context;

        public UserService(
            DataContext context,
            IOptions<AppSettings> appSettings
        ) : base(context)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        /// <inheritdoc />
        public async Task<IResult<User>> Authenticate(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return new Result<User>(message: "Email or password is empty", isSuccess: false, data: null);

            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return new Result<User>(message: "Email or password is incorrect. Try sign in again.",
                    isSuccess: false, data: null);

            return new Result<User>(message: "Authenticate successful!", isSuccess: true, data: user);
        }

        /// <inheritdoc />
        public async Task<IResult> Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return new Result("Password is empty!", false);

            if (_context.Users.Any(x => x.Email == user.Email))
                return new Result("Email has already registered!", false);

            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);
            if (passwordHash == null || passwordSalt == null)
                throw new AppException("Can't create password hash");
            user.Username = user.Email;

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            user.QuickSearch = user.Username.ToUpperInvariant() + user.Email.ToUpperInvariant() +
                                   user.Username.ToUpperInvariant();

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (AppException e)
            {
                throw new AppException("Add user Error" + e.Message);
            }

            return new Result("Created successful!", true);
        }

        /// <inheritdoc />
        public async Task<IResult<IList<User>>> GetUsersWithParams(TableParamsDto tableParams)
        {
            try
            {
                var users = await GetAll().Skip(tableParams.Skip()).Take(tableParams.Take()).ToListAsync();
                return new Result<IList<User>>(message: "Success", isSuccess: true, data: users);
            }
            catch (AppException e)
            {
                throw new AppException("Internal Error: can't get users with params: "
                                       + tableParams.Page + " " + tableParams.PageSize + " .Error" + e.Message);
            }
        }

        /// <inheritdoc />
        public IResult Update(string id, User user)
        {
            var currentUser = _context.Users.Find(id);

            if (currentUser == null)
                return new Result("User not found", false);

            if (user.Email!= currentUser.Email && _context.Users.Any(x => x.Email == user.Email))
                return new Result("Email has already registered.", false);

            if (user.Username != currentUser.Username && _context.Users.Any(x => x.Username == user.Username))
                return new Result("Username has already existed.", false);

            currentUser.Email = user.Email;
            currentUser.Username = user.Username;

            currentUser.QuickSearch =
                user.Username.ToUpperInvariant() + user.Email.ToUpperInvariant() +
                user.Username.ToUpperInvariant();

            //if (!string.IsNullOrWhiteSpace(password))
            //{
            //    byte[] passwordHash, passwordSalt;
            //    CreatePasswordHash(password, out passwordHash, out passwordSalt);

            //    currentUser.PasswordHash = passwordHash;
            //    currentUser.PasswordSalt = passwordSalt;
            //}

            _context.Users.Update(currentUser);
            _context.SaveChanges();
            return new Result("Update successful!", true);
        }

        /// <inheritdoc />
        public IQueryable<User> GetAll()
        {
            return _context.Users.AsQueryable();
        }

        public int CountAllUsers()
        {
            return _context.Users.Count();
        }

        /// <inheritdoc />
        public string CreateToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userId)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value is empty", "password");

            var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPasswordHash(string password, byte[] localHash, byte[] localSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value is empty", "password");
            if (localHash.Length != 64)
                throw new ArgumentException("Invalid length. Password hash expected 64 bytes.", "passwordHash");
            if (localSalt.Length != 128)
                throw new ArgumentException("Invalid length. Password salt expected 128 bytes.", "passwordHash");

            var hmac = new HMACSHA512(localSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (var i = 0; i < computedHash.Length; i++)
                if (computedHash[i] != localHash[i])
                    return false;

            return true;
        }
    }
}