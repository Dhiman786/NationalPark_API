using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NationalPark_API.Data;
using NationalPark_API.Models;
using NationalPark_API.Repository.IRepository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NationalPark_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appsettings;
        public UserRepository (ApplicationDbContext context,IOptions<AppSettings>appsettings)
        {
            _context = context;
            _appsettings = appsettings.Value;
        }
        public User Authenticate(string username, string password)
        {
            var USerInDb = _context.Users.FirstOrDefault(u => u.UserName == username 
            && u.Password == password);
            if (USerInDb == null) return null;


            //JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim (ClaimTypes.Name,USerInDb.Id.ToString()),
                    new Claim(ClaimTypes.Role,USerInDb.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            USerInDb.Token = tokenHandler.WriteToken(token);


            //*************
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
