using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using digibox.data;
using digibox.services;

namespace digibox.services.Repositories.baseclass
{
    public abstract class BaseClass
    {
        protected dbdigiboxEntities db;
        public BaseClass(dbdigiboxEntities transaction)
        {
            db = transaction;
        }

    }
}
