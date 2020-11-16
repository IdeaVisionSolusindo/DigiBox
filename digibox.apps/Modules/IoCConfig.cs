using Autofac;
using Autofac.Integration.Mvc;
using AutoMapper;
using digibox.data;
using digibox.services.Repositories;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services;
using digibox.services.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace digibox.apps.Modules
{
    public class IoCConfig:Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<dbdigiboxEntities>().AsSelf();
            builder.RegisterType<DistributorRepository>().As<IDistributorRepository>().SingleInstance();
            builder.RegisterType<AttributeRepository>().As<IAttributeRepository>().SingleInstance();
            builder.RegisterType<MaterialRepository>().As<IMaterialRepository>().SingleInstance();
            builder.RegisterType<MaterialSBURepository>().As<IMaterialSBURepository>().SingleInstance();
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<RoleRepository>().As<IRoleRepository>().SingleInstance();
            builder.RegisterType<RoleService>().As<IRoleService>().SingleInstance();
            builder.RegisterType<MaterialServices>().As<IMaterialServices>().SingleInstance();
            builder.RegisterType<MaterialPriceRepository>().As<IMaterialPriceRepository>().SingleInstance();
            builder.RegisterType<MessageRepository>().As<IMessageRepository>().SingleInstance();
            builder.RegisterType<PricingService>().As<IPricingService>().SingleInstance();
            builder.RegisterType<PriceRepository>().As<IPriceRepository>().SingleInstance();
            builder.RegisterType<PriceDetailRepository>().As<IPriceDetailRepository>().SingleInstance();
            builder.RegisterType<AttachmentRepository>().As<IAttachmentRepository>().SingleInstance();
            builder.RegisterType<MessageService>().As<IMessageService>().SingleInstance();
            builder.RegisterType<NotificationService>().As<INotificationService>().SingleInstance();
            builder.RegisterType<ReplenishRepository>().As<IReplenishRepository>().SingleInstance();
            builder.RegisterType<ReplenishDetailRepository>().As<IReplenishDetailRepository>().SingleInstance();
            builder.RegisterType<InventoryRepository>().As<IInventoryRepository>().SingleInstance();
            builder.RegisterType<RequestRepository>().As<IRequestRepository>().SingleInstance();
            builder.RegisterType<RequestDetailRepository>().As<IRequestDetailRepository>().SingleInstance();
            builder.RegisterType<OpnamRepository>().As<IOpnamRepository>().SingleInstance();
            builder.RegisterType<OpnameDetailRepository>().As<IOpnameDetailRepository>().SingleInstance();
            builder.RegisterType<FunctionRepository>().As<IFunctionRepository>().SingleInstance();
            
            //Builder automapper
            builder.Register(x => new MapperConfiguration(
                 config =>
                 {
                     config.AddProfile<MappingProfile>();
                 })).AsSelf().SingleInstance();

            base.Load(builder);
        }
    }


    public class iocInit
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new IoCConfig());
            builder.RegisterControllers(typeof(digibox.apps.MvcApplication).Assembly);
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}