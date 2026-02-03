using AutoFixture;
using DNN.Modules.UserVoice.Adapters;
using DNN.Modules.UserVoice.Controllers.Context;
using DNN.Modules.UserVoice.Providers;
using DNN.Modules.UserVoice.Services.Localization;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using System;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace IntegrationTests
{
    public abstract class FakeControllerContext : FakeDataContext, IDisposable
    {
        protected readonly Fixture fixture;
        protected readonly IServiceScope Scope;
        protected readonly IServiceProvider ServiceProvider;
        protected readonly CancellationToken token = new CancellationToken();
        protected readonly IUserControllerAdapter userController;
        protected readonly ILocalizationService localizationService;
        protected readonly IDateTimeProvider dateTimeProvider;

        protected readonly IDnnRequestContext RequestContext;
        protected readonly HttpContextBase HttpContext;

        protected FakeControllerContext()
        {
            this.fixture = new Fixture();
            this.RequestContext = Substitute.For<IDnnRequestContext>();
            this.HttpContext = Substitute.For<HttpContextBase>();
            this.userController = Substitute.For<IUserControllerAdapter>();
            this.localizationService = Substitute.For<ILocalizationService>();
            var localizationViewModel = new LocalizationViewModel();
            this.localizationService.ViewModel.Returns(localizationViewModel);
            this.dateTimeProvider = Substitute.For<IDateTimeProvider>();
            this.dateTimeProvider.GetUtcNow().Returns(fixture.Create<DateTime>());

            // Prepopulate RequestContext defaults
            this.RequestContext.User.UserID.Returns(fixture.Create<int>());
            this.RequestContext.Tab.Returns(new TabInfo { TabID = fixture.Create<int>() });
            this.RequestContext.Module.Returns(new ModuleInfo { ModuleID = fixture.Create<int>() });
            this.userController
                .GetUserById(RequestContext.PortalSettings.PortalId, RequestContext.User.UserID)
                .Returns(this.RequestContext.User);

            // DI Setup
            IServiceCollection services = new ServiceCollection();
            var startup = new DNN.Modules.UserVoice.Startup();
            startup.ConfigureServices(services);

            services.Replace(ServiceDescriptor.Scoped(_ => this.HttpContext));
            services.Replace(ServiceDescriptor.Scoped(_ => this.RequestContext));
            services.Replace(ServiceDescriptor.Scoped(_ => this.dataContext));
            services.Replace(ServiceDescriptor.Scoped(_ => this.localizationService));
            services.Replace(ServiceDescriptor.Singleton(_ => this.dateTimeProvider));

            services.AddTransient(_ => this.userController);

            this.ServiceProvider = services.BuildServiceProvider();
            this.Scope = this.ServiceProvider.CreateScope();
        }

        protected TController GetController<TController>() where TController : ApiController
            => ActivatorUtilities.CreateInstance<TController>(this.Scope.ServiceProvider);

        public new void Dispose()
        {
            base.Dispose();
            this.Scope.Dispose();
            (this.ServiceProvider as IDisposable)?.Dispose();
        }
    }
}
