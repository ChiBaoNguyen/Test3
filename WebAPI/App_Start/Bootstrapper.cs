using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Root.Data.Infrastructure;
using Root.Data.Repository;
using Service.Mappers;
using Service.Services;
using WebAPI.Providers;
using Website.Utilities;

namespace WebAPI.App_Start
{
	public class Bootstrapper
	{
		public static void Run()
		{
			AutoMapperConfiguration.Configure();
			//SetAutofacContainer();
		}

		public static IContainer SetAutofacContainer()
		{
			var builder = new ContainerBuilder();

			builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
			builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
			builder.RegisterType<DatabaseFactory>().As<IDatabaseFactory>().InstancePerRequest();
			builder.RegisterType<LicenseValidationMiddleware>().InstancePerRequest();
			builder.Register(c => new LicenseValidation()).As<ILicenseValidation>().InstancePerLifetimeScope();
			builder.RegisterAssemblyTypes(typeof(CustomerRepository).Assembly)
			.Where(t => t.Name.EndsWith("Repository"))
			.AsImplementedInterfaces().InstancePerRequest();
			builder.RegisterAssemblyTypes(typeof(CustomerService).Assembly)
		   .Where(t => t.Name.EndsWith("Service"))
		   .AsImplementedInterfaces().InstancePerRequest();

			var container = builder.Build();
			return container;
			//GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
		}
	}
}