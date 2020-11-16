using digibox.data;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class InventoryRepository : BaseClass, IInventoryRepository
    {
        public InventoryRepository(dbdigiboxEntities entities):base(entities)
        {

        }

        public ttinventory Create(ttinventory param)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ttinventory param)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ttinventory> Find(Expression<Func<ttinventory, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public ttinventory FindById(Guid id)
        {
            return db.ttinventories.Find(id);
        }

        public IQueryable<ttinventory> GetAll()
        {
            return db.ttinventories.Where(x => x.isdeleted == false);
        }

        public IQueryable<ttinventory> GetByMaterial(Guid materialid)
        {
            return db.ttinventories.Where(x => x.isdeleted == false && x.materialid==materialid).OrderByDescending(x=>x.createdat);
        }

        public IQueryable<ttinventory> GetByRFIDCode(string rfidCode)
        {
            return db.ttinventories.Where(x => x.isdeleted == false && x.rfidcode == rfidCode).OrderByDescending(x => x.createdat);

        }

        public bool Update(ttinventory param)
        {
            try
            {
                var iv = db.ttinventories.Find(param);
                iv.inout = iv.inout;
                iv.materialid = iv.materialid;
                iv.replstock = iv.replstock;
                iv.rfidcode = iv.rfidcode;
                iv.updatedat = DateTime.Now;
                db.SaveChanges();
                return true;
            } catch(Exception e)
            {
                return false;
            }
        }

    }
}
