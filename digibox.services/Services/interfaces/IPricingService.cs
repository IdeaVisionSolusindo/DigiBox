using digibox.data;
using digibox.services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.services.Services.interfaces
{
    public interface IPricingService
    {
        IQueryable<MaterialPricingModel> GetPriceByCollector(Guid id);
        IQueryable<MaterialPriceListModel> GetMaterialPrices(Guid id);
        IQueryable<MaterialPricingModel> GetPrices();
        IQueryable<MaterialPricingModel> GetProposedPrices(string[] proposeStatus, Guid createdBy);
        bool ProposePrice(tmmaterialprice param);
        bool ApprovePrice(tmmaterialprice param, string status);
        bool RejectPrice(tmmaterialprice param, string status);

        IQueryable<PriceProposalListModel> GetProposalPrice(string[] proposeStatus, Guid proposedby);
        IQueryable<MaterialPricingModel> GetMaterialPriceByDistributor(Guid distributorid);

        bool CreateMultipleDetail(List<PriceProposalDetailModel> dta);
        bool UpdateMultipleDetail(List<PriceProposalDetailModel> oldDetail, List<PriceProposalDetailModel> newDetail);
    }
}
