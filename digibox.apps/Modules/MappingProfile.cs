using AutoMapper;
using digibox.data;
using digibox.services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace digibox.apps.Modules
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<tmmaterial, MaterialModel>().ReverseMap();
            CreateMap<tmdistributor, DistributorModel>().ReverseMap();
            CreateMap<tmattribute, AttributeModel>().ReverseMap();
            CreateMap<tmuser, UserModel>().ReverseMap();
            CreateMap<tmmaterialprice, MaterialPriceListModel>().ReverseMap();
            CreateMap<ttprice, PriceProposalModel>().ReverseMap();
            CreateMap<tdprice, PriceProposalDetailModel>().ReverseMap();
            CreateMap<ttattachment, AttachmentModel>().ReverseMap();
            CreateMap<ttreplenish, ReplenishModel>().ReverseMap();
            CreateMap<tdreplenish, ReplenishDetailModel>().ReverseMap();
            CreateMap<ttinventory, InventoryModel>().ReverseMap();
            CreateMap<ttrequest, RequestModel>().ReverseMap();
            CreateMap<tdrequest, RequestDetailModel>().ReverseMap();
            CreateMap<tmrole, RoleModel>().ReverseMap();
            CreateMap<tdrole, RoleDetailModel>().ReverseMap();
        }
    }

}
