using Autofac;
using Nop.Api.Factories;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Skle;

namespace Nop.Api.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //service
            builder.RegisterType<CommonFactory>().As<ICommonFactory>().InstancePerLifetimeScope();

            //skle
            builder.RegisterType<GroupService>().As<IGroupService>().InstancePerLifetimeScope();
            builder.RegisterType<MemberService>().As<IMemberService>().InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<PostService>().As<IPostService>().InstancePerLifetimeScope();
            builder.RegisterType<ProvinceService>().As<IProvinceService>().InstancePerLifetimeScope();
            builder.RegisterType<OtpService>().As<IOtpService>().InstancePerLifetimeScope();
        }

        public int Order => 3;
    }
}