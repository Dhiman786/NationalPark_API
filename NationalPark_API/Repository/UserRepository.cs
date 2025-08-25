using NationalPark_API.Data;
using NationalPark_API.Models;
using NationalPark_API.Repository.IRepository;

namespace NationalPark_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository (ApplicationDbContext context)
        {
            _context = context;
        }
        public User Authenticate(string username, string password)
        {
            var USerInDb = _context.Users.FirstOrDefault(u => u.UserName == username 
            && u.Password == password);
            if (USerInDb == null) return null;
            //JWT Token


            //*******
            USerInDb.Password = "";
            return USerInDb;
        }

        public bool IsUniqueUser(string username)
        {
            var UserInDb = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (UserInDb == null) return true;return false;
        }

        public User Register(string username, string password)
        {
            User user = new User
            {
                UserName = username,
                Password = password,
                Role = "Admin"
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }
    }
}
