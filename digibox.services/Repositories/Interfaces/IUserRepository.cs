using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.Interfaces.baseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces
{
    public interface IUserRepository : ICrudRepository<tmuser>, IGetRepository<tmuser>
    {
        tmuser Login(string email, string passsword);
        bool updateUserToken(Guid id, string token);
        tmuser getByToken(string token);
        IQueryable<tmuser> getByRole(string role);
        bool changePassword(Guid id, string password);
        IQueryable<UserListModel> GetUsers();


    }
}
