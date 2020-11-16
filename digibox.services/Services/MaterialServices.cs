using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.Razor.Generator;

namespace digibox.services.Services
{
    public class MaterialServices : BaseClass, IMaterialServices
    {

        public MaterialServices(dbdigiboxEntities transaction) : base(transaction)
        {

        }

        /// <summary>
        /// Assign user material
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="guids"> material id</param>
        /// <returns></returns>
        public bool AssignUserMaterial(Guid id, List<Guid> guids)
        {
            try
            {
                var usermat = db.tmmaterialusers.Where(x => x.userid == id);
                //ambil data id yang ga ada dalam usr material.
                var umid = (from um in usermat
                            select um.materialid).ToList();
                var ids = guids.Except(umid);

                //insert to user material
                foreach (var matid in ids)
                {
                    tmmaterialuser matus = new tmmaterialuser()
                    {
                        isdeleted = false,
                        id = Guid.NewGuid(),
                        materialid = matid,
                        userid = id
                    };
                    db.tmmaterialusers.Add(matus);
                }

                //remove material user yang tidak terpilih lagi.
                umid.Except(ids);
                foreach (var matid in umid)
                {
                    var umr = usermat.Where(x => x.materialid == matid).FirstOrDefault();
                    tmmaterialuser matus = db.tmmaterialusers.Find(umr.id);
                    matus.deletedat = DateTime.Now;
                    matus.isdeleted = true;
                }

                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public IQueryable<CurrentMaterialStatus> GetMaterialCurrentPrice()
        {
            var material = db.tmmaterials.Where(x => x.isdeleted == false);
            var materialPrice = db.tmmaterialprices.Where(x => x.isdeleted == false && x.isactive == true);
            var dta = (from m in material
                       join mp in materialPrice on m.id equals mp.materialid
                       select new CurrentMaterialStatus()
                       {
                           id = m.id,
                           price = mp.price,
                           name = m.name,
                           maxstock = m.maxstock,
                           minstock = m.minstock,
                           currentstock = m.currentstock,
                           partno = m.partno,
                           unit = m.unit
                       });
            return dta;
        }

        public IQueryable<MaterialAssignmentModel> MaterialAssignmentList(Guid id)
        {
            //get selected usermaterial.
            var materialuser = db.tmmaterialusers.Where(x => x.userid == id && x.isdeleted==false);
            //material
            var material = db.tmmaterials.Where(x => x.isdeleted == false);

            //distributor 
            var distributor = db.tmdistributors.Where(x => x.isdeleted == false);

            
            //assigned to user id
            var usm = from m in material
                      join u in materialuser on m.id equals u.materialid
                      join d in distributor on m.distributor equals d.id
                      select new MaterialAssignmentModel()
                      {
                          id=m.id,
                          Assigned = true,
                          name = m.name,
                          partno = m.partno,
                          distributor = d.name
                      };

            //unassigned 
            materialuser = db.tmmaterialusers.Where(x => x.isdeleted == false); //get all material user

            var unsm = from m in material
                       join d in distributor on m.distributor equals d.id
                       where !(from o in materialuser
                               select o.materialid)
                               .Contains(m.id)
                       select new MaterialAssignmentModel()
                       {
                           id=m.id,
                           Assigned = false,
                           name = m.name,
                           partno = m.partno,
                           distributor = d.name,
                       };
            usm = usm.Concat(unsm);
            return usm;
        }
    }
}
