using digibox.data;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace digibox.services.Repositories
{
    public class MaterialPriceRepository : BaseClass, IMaterialPriceRepository
    {
        public MaterialPriceRepository(dbdigiboxEntities transaction) : base(transaction)
        {

        }
        public tmmaterialprice Create(tmmaterialprice param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.tmmaterialprices.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool Delete(Guid id)
        {
            try
            {
                var dt = db.tmmaterialprices.Find(id);
                dt.isdeleted = true;
                dt.deletedat = DateTime.Now;
                dt.deletedby = "WILL BE CHANGED";
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(tmmaterialprice param)
        {
            return Delete(param.id);
        }

        public IQueryable<tmmaterialprice> Find(Expression<Func<tmmaterialprice, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tmmaterialprice FindById(Guid id)
        {
            return db.tmmaterialprices.Find(id);
        }

        public IQueryable<tmmaterialprice> GetAll()
        {
            return db.tmmaterialprices.Where(x => x.isdeleted == false);
        }

        public tmmaterialprice GetCurrentPrice(Guid materialId)
        {
            return db.tmmaterialprices.Where(x => x.id == materialId && x.isactive == true && x.status == "APPROVED").FirstOrDefault();
        }

        public IQueryable<tmmaterialprice> Overdue(int days, Guid? user)
        {
            var overdueindate = DateTime.Today.AddDays(-days);
            var dta = db.tmmaterialprices.Where(x => x.isdeleted == false && x.dateend >= overdueindate && x.status == "APPROVED" && x.isactive == true);
            return dta;

        }

        public bool Update(tmmaterialprice param)
        {
            try
            {
                tmmaterialprice materialprice = db.tmmaterialprices.Find(param.id);
                materialprice.datestart = param.datestart;
                materialprice.isactive = param.isactive;
                materialprice.materialid = param.materialid;
                materialprice.price = param.price;
                materialprice.updatedby= param.updatedby;
                materialprice.updatedat = DateTime.Now;
                materialprice.updatedby = param.updatedby;
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
