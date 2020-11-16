using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Repositories
{
    public class MaterialSBURepository : BaseClass, IMaterialSBURepository
    {
        public MaterialSBURepository(dbdigiboxEntities db):base(db)
        {

        }
        public tmmaterialsbu Create(tmmaterialsbu param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = param.createdby;
            param.isdeleted = false;
            db.tmmaterialsbus.Add(param);
            db.SaveChanges();
            return param;
        }

        public bool CreateMultiple(Guid materialid, Guid[] sbuids)
        {
            try
            {

                foreach(var sbu in sbuids)
                {
                    tmmaterialsbu mbu = new tmmaterialsbu()
                    {
                        id = Guid.NewGuid(),
                        materialid = materialid,
                        sbuid = sbu,
                        isdeleted=false
                    };
                    db.tmmaterialsbus.Add(mbu);
                }

                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                var dt = db.tmmaterialsbus.Find(id);
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

        public bool Delete(tmmaterialsbu param)
        {
            return Delete(param.id);
        }

        public bool DeleteMultiple(Guid[] ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var dt = db.tmmaterialsbus.Find(id);
                    dt.isdeleted = true;
                    dt.deletedat = DateTime.Now;
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IQueryable<tmmaterialsbu> Find(Expression<Func<tmmaterialsbu, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tmmaterialsbu FindById(Guid id)
        {
            return db.tmmaterialsbus.Find(id);
        }

        public IQueryable<tmmaterialsbu> GetAll()
        {
            return db.tmmaterialsbus.Where(x => x.isdeleted == false);
        }

        public IQueryable<tmmaterialsbu> GetMaterialSBUByMaterialID(Guid materialid)
        {
            return db.tmmaterialsbus.Where(x => x.materialid == materialid && x.isdeleted==false);

        }

        public List<MaterialSBUModel> GetTransposeAllMaterialSBU()
        {
            var msbu = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "SBU");
            var sbu = db.tmmaterialsbus.Where(x => x.isdeleted == false);
            var qmaterialsbu = (from s in sbu 
                  join ms in msbu on s.sbuid equals ms.id
                  select new MaterialSBUModel
                  {
                      id = s.id,
                      materialid=s.materialid,
                      sbu = ms.attributevalue
                  }).ToList();

            var materialsbu = qmaterialsbu.GroupBy(cc => cc.materialid).Select(dd => new MaterialSBUModel() { materialid = dd.Key, sbu = string.Join(", ", dd.Select(ee => ee.sbu).ToList())}).ToList();
            return materialsbu;
        }

        public bool Update(tmmaterialsbu param)
        {
            try
            {
                tmmaterialsbu material = db.tmmaterialsbus.Find(param.id);
                material.materialid= param.materialid;
                material.sbuid = param.sbuid;
                material.updatedat = DateTime.Now;
                material.updatedby = param.updatedby;
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
