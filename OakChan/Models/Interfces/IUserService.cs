using OakChan.Models.DB.Entities;

namespace OakChan.Models.Interfces
{
    public interface IUserService
    {
        public User CreateAnonymous();
    }
}
