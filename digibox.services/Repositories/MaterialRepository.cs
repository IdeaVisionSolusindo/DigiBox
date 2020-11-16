using AutoMapper.Configuration.Annotations;
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
    public class MaterialRepository : BaseClass, IMaterialRepository
    {
        public MaterialRepository(dbdigiboxEntities db):base(db)
        {

        }
        public tmmaterial Create(tmmaterial param)
        {
            param.id = Guid.NewGuid();
            param.createdat = DateTime.Now;
            param.createdby = "WILL BE CHANGED";
            param.isdeleted = false;
            db.tmmaterials.Add(param);
            db.SaveChanges();
            return param;
        }

        public async Task<bool> CrateMultiple(tmmaterial[] param, tmmaterialsbu[] sbus)
        {
            try
            {
                foreach (var itm in param)
                {
                    itm.createdat = DateTime.Now;
                    itm.isdeleted = false;
                    db.tmmaterials.Add(itm);
                }

                foreach(var itm in sbus)
                {
                    itm.createdat = DateTime.Now;
                    itm.isdeleted = false;
                    db.tmmaterialsbus.Add(itm);
                }

                db.SaveChanges();
                return true;
            }catch (Exception e)
            {
                return false;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                var dt = db.tmmaterials.Find(id);
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

        public bool Delete(tmmaterial param)
        {
            return Delete(param.id);
        }

        public IQueryable<tmmaterial> Find(Expression<Func<tmmaterial, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public tmmaterial FindById(Guid id)
        {
            return db.tmmaterials.Find(id);
        }

        public IQueryable<tmmaterial> GetAll()
        {
            return db.tmmaterials.Where(x => x.isdeleted == false);
        }

        public IQueryable<MaterialListModel> GetMaterial()
        {
            var material = db.tmmaterials.Where(x => x.isdeleted == false);
            var materialuser = db.tmmaterialusers.Where(x => x.isdeleted == false);

            var movementype = db.tmattributes.Where(x=> x.isdeleted == false && x.attributename== "MOVEMENT-TYPE");
            var materialtype = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "MATERIAL-TYPE");
            var location = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "LOCATION");
            var binlocation = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "BIN-LOCATION");
            var distributor = db.tmdistributors.Where(x => x.isdeleted == false);
            var msbu = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "SBU");
            var result = (from m in material
//                          join mu in materialuser on m.id equals mu.materialid 
                          join mt in movementype on m.movementtype equals mt.id into mv
                          from mvt in mv.DefaultIfEmpty()
                          join mty in materialtype on m.materialtype equals mty.id into mtt
                          from mtty in mtt.DefaultIfEmpty()
                          join d in distributor on m.distributor equals d.id into dst
                          from dist in dst.DefaultIfEmpty()
                          join l in location on m.location equals l.id into lct 
                          from locs in lct.DefaultIfEmpty()
                          join bl in binlocation on m.binlocation equals bl.id into blo
                          from btc in blo.DefaultIfEmpty()
                          select new MaterialListModel()
                          {
                              id = m.id,
                              movementtype = mvt.attributevalue,
                              maxstock = m.maxstock,
                              minstock = m.minstock,
                              name = m.name,
                              partno = m.partno,
                              datecreate = m.datecreate,
                              description = m.description,
                              materialtype = mtty.attributevalue,
                              distributor = dist.name,
                              location = locs.attributevalue,
                              binlocation = btc.attributevalue,
                              unit = m.unit,
                              plant = m.plant,
                              calhorizon = m.calhorizon,
                              sloc = m.sloc,
                              currentstock = m.currentstock
//                              collectorid = mu.userid
                          });
            return result;

        }

        public IQueryable<MaterialListModel> GetMaterialByCollector(Guid collectorId)
        {

            var materialuser = db.tmmaterialusers.Where(x => x.isdeleted == false && x.userid == collectorId);
            var material = db.tmmaterials.Where(x => x.isdeleted == false);

            var movementype = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "MOVEMENT-TYPE");
            var materialtype = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "MATERIAL-TYPE");
            var location = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "LOCATION");
            var binlocation = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "BIN-LOCATION");
            var distributor = db.tmdistributors.Where(x => x.isdeleted == false);
            var msbu = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "SBU");
            var result = (from m in material
                          join mu in materialuser on m.id equals mu.materialid
                          join mt in movementype on m.movementtype equals mt.id
                          join mty in materialtype on m.materialtype equals mty.id
                          join d in distributor on m.distributor equals d.id
                          join l in location on m.location equals l.id
                          join bl in binlocation on m.binlocation equals bl.id
                          select new MaterialListModel()
                          {
                              id = m.id,
                              movementtype = mt.attributevalue,
                              maxstock = m.maxstock,
                              minstock = m.minstock,
                              name = m.name,
                              partno = m.partno,
                              datecreate = m.datecreate,
                              description = m.description,
                              materialtype = mty.attributevalue,
                              distributor = d.name,
                              location = l.attributevalue,
                              binlocation = bl.attributevalue,
                              unit = m.unit,
                              plant = m.plant,
                              calhorizon = m.calhorizon,
                              sloc = m.sloc,
                              currentstock = m.currentstock,
                              collectorid = mu.userid
                          });
            return result;

        }

        public IQueryable<MaterialListModel> MinimumMaterial(Guid? id)
        {
            IQueryable<tmmaterialuser> materialuser;
            if (id ==null)
            {
                materialuser = db.tmmaterialusers.Where(x => x.isdeleted == false);
            }
            else
            {
                materialuser = db.tmmaterialusers.Where(x => x.isdeleted == false && x.userid==id);
            }
            var material = db.tmmaterials.Where(x => x.isdeleted == false);
            var movementype = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "MOVEMENT-TYPE");
            var materialtype = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "MATERIAL-TYPE");
            var location = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "LOCATION");
            var binlocation = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "BIN-LOCATION");
            var distributor = db.tmdistributors.Where(x => x.isdeleted == false);
            var msbu = db.tmattributes.Where(x => x.isdeleted == false && x.attributename == "SBU");
            var result = (from m in material
                          join mu in materialuser on m.id equals mu.materialid
                          join mt in movementype on m.movementtype equals mt.id
                          join mty in materialtype on m.materialtype equals mty.id
                          join d in distributor on m.distributor equals d.id
                          join l in location on m.location equals l.id
                          join bl in binlocation on m.binlocation equals bl.id
                          select new MaterialListModel()
                          {
                              id = m.id,
                              movementtype = mt.attributevalue,
                              maxstock = m.maxstock,
                              minstock = m.minstock,
                              name = m.name,
                              partno = m.partno,
                              datecreate = m.datecreate,
                              description = m.description,
                              materialtype = mty.attributevalue,
                              distributor = d.name,
                              location = l.attributevalue,
                              binlocation = bl.attributevalue,
                              unit = m.unit,
                              plant = m.plant,
                              calhorizon = m.calhorizon,
                              sloc = m.sloc,
                              currentstock = m.currentstock,
                              collectorid = mu.userid
                          });
            return result;
        }

        public IQueryable<tmmaterial> GetMaterialByDistributor(Guid distributorid)
        {
            return db.tmmaterials.Where(x => x.isdeleted == false && x.distributor == distributorid);
        }

        public bool Update(tmmaterial param)
        {
            try
            {
                tmmaterial material = db.tmmaterials.Find(param.id);
                material.name = param.name;
                material.description = param.description;
                material.distributor = param.distributor;
                material.location = param.location;
                material.materialtype = param.materialtype;
                material.maxstock = param.maxstock;
                material.minstock = param.minstock;
                material.movementtype = param.movementtype;
                material.partno = param.partno;
                material.updatedat = DateTime.Now;
                material.updatedby = param.updatedby;
                material.binlocation = param.binlocation;
                material.calhorizon = param.calhorizon;
                material.sloc = param.sloc;
                material.plant = param.plant;
                material.currentstock = param.currentstock;
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
