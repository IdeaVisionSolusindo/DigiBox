using Autofac;
using AutoMapper;
using digibox.data;
using digibox.services.Repositories;
using digibox.services.Repositories.Interfaces;
using digibox.services.Services;
using digibox.services.Services.interfaces;
using digibox.wind.Models;
using digibox.wind.Services;
using digibox.wind.View;
using digibox.wind.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace digibox.wind.Modules
{
    public class IoCConfiguration : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            //building inteface
            builder.RegisterType<UserRepository>().As<IUserRepository>().SingleInstance();
            builder.RegisterType<RoleService>().As<IRoleService>().SingleInstance();
            builder.RegisterType<ReplenishRepository>().As<IReplenishRepository>().SingleInstance();
            builder.RegisterType<MaterialRepository>().As<IMaterialRepository>().SingleInstance();
            builder.RegisterType<MaterialServices>().As<IMaterialServices>().SingleInstance();
            builder.RegisterType<MaterialPriceRepository>().As<IMaterialPriceRepository>().SingleInstance();
            builder.RegisterType<ReplenishDetailRepository>().As<IReplenishDetailRepository>().SingleInstance();
            builder.RegisterType<DrawRepository>().As<IDrawRepository>().SingleInstance();
            builder.RegisterType<DrawDetailRepository>().As<IDrawDetailRepository>().SingleInstance();
            builder.RegisterType<InventoryRepository>().As<IInventoryRepository>().SingleInstance();
            builder.RegisterType<DistributorRepository>().As<IDistributorRepository>().SingleInstance();

            
            //view model
            builder.RegisterType<UserLoginViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<MainViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<ReplenishListViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<ReplenishMaterialViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<DrawListViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<DrawMaterialViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<ReplenishConfirmViewModel>().AsSelf().SingleInstance();
            builder.RegisterType<HomeView>().AsSelf().SingleInstance();


            //window
            builder.RegisterType<MainWindow>().AsSelf().SingleInstance();
            builder.RegisterType<LoginView>().AsSelf().SingleInstance();
            builder.RegisterType<ReplenishListView>().AsSelf().SingleInstance();
            builder.RegisterType<ReplenishMaterialView>().AsSelf().SingleInstance();
            builder.RegisterType<DrawView>().AsSelf().SingleInstance();
            builder.RegisterType<DrawMaterialView>().AsSelf().SingleInstance();
            builder.RegisterType<ReplenishConfirmView>().AsSelf().SingleInstance();

            //Services
            builder.RegisterType<PrintLabelService>().AsSelf().SingleInstance();

            //building type
            builder.RegisterType<dbdigiboxEntities>().AsSelf();
            builder.RegisterType<UserModel>().AsSelf();

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
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new IoCConfiguration());
            return builder.Build();
        }
    }
}
