using System.Linq;
using FluentValidation;
using SimpleInjector;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Types.DataServices;
using TellMe.iOS.Controllers;
using UIKit;

namespace TellMe.iOS.Core
{
    public static class IoC
    {
        public static readonly Container Container;

        static IoC()
        {
            Container = new Container();
        }

        public static void Initialize(UIWindow window)
        {
            Container.Register<IRouter>(() => new Router(window), Lifestyle.Singleton);
            Container.Register<IApiProvider, ApiProvider>();
            Container.Register<INotificationHandler, NotificationHandler>();
            Container.Register<IApplicationDataStorage, ApplicationDataStorage>(Lifestyle.Singleton);

            RegisterAll<IRemoteDataService>(Container);
            RegisterAll<ILocalDataService>(Container);
            Container.Register(typeof(AbstractValidator<>), new[] {typeof(AbstractValidator<>).Assembly});
            RegisterAll<IBusinessLogic>(Container);
            RegisterControllers();
            
            Container.Verify();
        }

        private static void RegisterControllers()
        {
            Container.Register<EventsViewController>();
        }

        private static void RegisterAll<T>(Container container)
        {
            var targetType = typeof(T);
            var registrations =
                from type in targetType.Assembly.GetExportedTypes()
                where type.IsClass && !type.IsAbstract && targetType != type && targetType.IsAssignableFrom(type) &&
                      type.GetInterfaces().Any()
                select new
                {
                    Service = type.GetInterfaces().First(x => x != targetType),
                    Implementation = type
                };

            foreach (var reg in registrations)
                container.Register(reg.Service, reg.Implementation);
        }
    }
}