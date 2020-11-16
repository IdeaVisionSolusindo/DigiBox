using digibox.data;
using digibox.services.Models;
using digibox.services.Repositories.baseclass;
using digibox.services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services
{
    public class PricingService: BaseClass, IPricingService
    {

        public PricingService(dbdigiboxEntities transaction) : base(transaction)
        {

        }

        public IQueryable<MaterialPricingModel> GetPriceByCollector(Guid id)
        {
            //ambil data material
            var material = db.tmmaterials.Where(x =>x.isdeleted==false);
            //ambil data material user by collector
            var usermaterial = db.tmmaterialusers.Where(x => x.isdeleted == false && x.userid == id);
            //distributor
            var distributor = db.tmdistributors.Where(x => x.isdeleted == false);

            var pricing = db.tmmaterialprices.Where(x => x.isdeleted == false && x.isactive==true);

            //query
            var dta = from um in usermaterial
                      join m in material on um.materialid equals m.id
                      join d in distributor on m.distributor equals d.id
                      join p in pricing on m.id equals p.materialid into prc
                      from price in prc.DefaultIfEmpty()
                      select new MaterialPricingModel()
                      {
                          currentprice = price.price,
                          distributor = d.name,
                          id = m.id,
                          name = m.name,
                          partno = m.partno
                          
                      };
            return dta;

        }

        /// <summary>
        /// Price List for Material
        /// </summary>
        /// <param name="id">Material ID</param>
        /// <returns></returns>
        public IQueryable<MaterialPriceListModel> GetMaterialPrices(Guid id)
        {
            var materialprice = db.tmmaterialprices.Where(x => x.materialid == id && x.isdeleted==false).OrderByDescending(x=>x.createdat);
            var dta = (from mp in materialprice 
                       select new MaterialPriceListModel()
                       {
                           id = mp.id,
                           dateend = mp.dateend,
                           datestart = mp.datestart,
                           isactive = mp.isactive,
                           materialid = mp.materialid,
                           price = mp.price,
                           status = mp.status
                       });
            return dta;
        }

        public bool ProposePrice(tmmaterialprice param)
        {
            var materialprice = db.tmmaterialprices.Find(param.id);
            materialprice.status = param.status;
            materialprice.updatedby = param.updatedby;
            materialprice.updatedat = DateTime.Now;
            db.SaveChanges();
            //send notification
            return true;
        }

        public IQueryable<MaterialPricingModel> GetPrices()
        {
            //ambil data material
            var material = db.tmmaterials.Where(x => x.isdeleted == false);
            //ambil data material user by collector
            var usermaterial = db.tmmaterialusers.Where(x => x.isdeleted == false);
            //distributor
            var distributor = db.tmdistributors.Where(x => x.isdeleted == false);

            var pricing = db.tmmaterialprices.Where(x => x.isdeleted == false && x.isactive == true);

            //query
            var dta = from um in usermaterial
                      join m in material on um.materialid equals m.id
                      join d in distributor on m.distributor equals d.id
                      join p in pricing on m.id equals p.materialid into prc
                      from price in prc.DefaultIfEmpty()
                      select new MaterialPricingModel()
                      {
                          currentprice = price.price,
                          distributor = d.name,
                          id = m.id,
                          name = m.name,
                          partno = m.partno
                      };
            return dta;
        }

        public IQueryable<MaterialPricingModel> GetProposedPrices(string[] proposeStatus, Guid createdBy)
        {
            //ambil data material
            var material = db.tmmaterials.Where(x => x.isdeleted == false);
            //ambil data material user by collector
            var usermaterial = db.tmmaterialusers.Where(x => x.isdeleted == false);
            //distributor
            var distributor = db.tmdistributors.Where(x => x.isdeleted == false);

            var pricing = db.tmmaterialprices.Where(x => x.isdeleted == false && x.isactive == false);

            //query
            var dta = from um in usermaterial
                      join m in material on um.materialid equals m.id
                      join d in distributor on m.distributor equals d.id
                      join p in pricing on m.id equals p.materialid into prc
                      from price in prc.DefaultIfEmpty()
                      join s in proposeStatus on price.status equals s
                      select new MaterialPricingModel()
                      {
                          id = um.materialid,
                          proposedprice = price.price,
                          status = price.status,
                          proposedtime = price.datestart,
                          myproposed = price.createdbyid == createdBy
                      };
            return dta;
        }

        public bool ApprovePrice(tmmaterialprice param, string status)
        {

            try
            {
                //check apakah ada harga yang masih berlaku.


                var dta = db.tmmaterialprices.Find(param.id);
                dta.status = status;
                dta.datestart = DateTime.Now;
                dta.approvedby = param.approvedby;
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public bool RejectPrice(tmmaterialprice param, string status)
        {

            try
            {
                var dta = db.tmmaterialprices.Find(param.id);
                dta.status = status;
                dta.datestart = DateTime.Now;
                dta.isactive = false;
                dta.approvedby = param.approvedby;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public IQueryable<PriceProposalListModel> GetProposalPrice(string[] proposalStatus, Guid proposedby)
        {
            var dta = from p in db.ttprices
                      join c in db.tmusers on p.collectorid equals c.id
                      join a in db.tmusers on p.approvedby equals a.id into app
                      from appv in app.DefaultIfEmpty()
                      join d in db.tmdistributors on p.distributorid equals d.id
                      join s in proposalStatus on p.status equals s
                      where p.isdeleted==false && c.isdeleted==false && d.isdeleted==false
                      select new PriceProposalListModel()
                      {
                          approvedby = appv.name,
                          collector = c.name,
                          ddate = p.ddate,
                          distributor = d.name,
                          id = p.id,
                          no = p.no,
                          status = p.status,
                          rejectedreason = p.rejectedreason
                      };
            return dta;
        }

        /// <summary>
        /// List of Curent Price of Material by Distributor
        /// </summary>
        /// <param name="distributorid"></param>
        /// <returns></returns>
        public IQueryable<MaterialPricingModel> GetMaterialPriceByDistributor(Guid distributorid)
        {
            var material = db.tmmaterials.Where(x => x.isdeleted == false && x.distributor == distributorid);
            var materialprice = db.tmmaterialprices.Where(x => x.isdeleted == false && x.isactive == true).Distinct();
            var dta = from m in material
                      join mp in materialprice on m.id equals mp.materialid into mat
                      from matp in mat.DefaultIfEmpty()
                      select new MaterialPricingModel()
                      {
                          id = m.id,
                          currentprice = matp.price,
                          name = m.name,
                          partno = m.partno
                      };
            return dta;
        }

        public bool CreateMultipleDetail(List<PriceProposalDetailModel> dta)
        {
            try
            {
                foreach (var itm in dta)
                {
                    tdprice price = new tdprice()
                    {
                        id = Guid.NewGuid(),
                        materialid = itm.materialid,
                        priceid = itm.priceid,
                        newprice = itm.newprice,
                        createdby = itm.createdby,
                        createdat = DateTime.Now,
                        isdeleted = false
                    };
                    db.tdprices.Add(price);
                }
                db.SaveChanges();
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }

        public bool UpdateMultipleDetail(List<PriceProposalDetailModel> oldDetail, List<PriceProposalDetailModel> newDetail)
        {
            try
            {
                foreach (var itm in oldDetail)
                {
                    var p = db.tdprices.Find(itm.id);
                    p.isdeleted = true;
                    p.deletedat = DateTime.Now;
                    p.isdeleted = true;
                }
                db.SaveChanges();
                foreach (var itm in newDetail)
                {
                    var p = new tdprice()
                    {
                        id = Guid.NewGuid(),
                        materialid = itm.materialid,
                        newprice = itm.newprice,
                        priceid = itm.priceid,
                        currentprice = itm.currentprice,
                        createdat = DateTime.Now,
                        isdeleted=false
                    };
                    db.tdprices.Add(p);
                }

                db.SaveChanges();
                return true;
            }catch(Exception e)
            {
                return false;
            }

        }
    }
}
