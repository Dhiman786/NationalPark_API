using NationalPark_API.Models;

namespace NationalPark_API.Repository.IRepository
{
    public interface IUserRepository
    {
        bool  IsUniqueUser(string username);
        User Authenticate(string username,string password);
        User Register(string username,string password);
    }
}
