using System.Linq;
using FluentValidation;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using TellMe.Core.Contracts;
using TellMe.Core.Contracts.BusinessLogic;
using TellMe.Core.Contracts.DataServices;
using TellMe.Core.Contracts.DataServices.Local;
using TellMe.Core.Contracts.DataServices.Remote;
using TellMe.Core.Types.DataServices;
using TellMe.iOS.Controllers;
using TellMe.iOS.Core.UI;
using UIKit;

namespace TellMe.iOS.Core
{
    public static class IoC
    {
        private static readonly Container Container;

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
            Container.Register(typeof(AbstractValidator<>), new[] { typeof(AbstractValidator<>).Assembly });
            RegisterAll<IBusinessLogic>(Container);
            RegisterControllers(window);

            Container.Verify();
        }

        public static T GetInstance<T>() where T : class
        {
            return Container.GetInstance<T>();
        }

        private static void RegisterControllers(UIWindow window)
        {
            window.InvokeOnMainThread(() =>
            {
                Container.Register<EventsViewController>();
                var registration = Container.GetRegistration(typeof(EventsViewController)).Registration;
                registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "UIViewController registration");
            });
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