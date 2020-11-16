using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories.Interfaces.baseInterfaces
{
    public interface ICrudRepository<T> where T : class
    {
        T FindById(Guid id);
        T Create(T param);
        bool Delete(Guid id);
        bool Delete(T param);
        bool Update(T param);
    }
}
