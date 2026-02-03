using AutoFixture;
using DNN.Modules.UserVoice.Controllers;
using DNN.Modules.UserVoice.Controllers.Context;
using DNN.Modules.UserVoice.Entities.ProblemDetails;
using DNN.Modules.UserVoice.Services.Ideas;
using DNN.Modules.UserVoice.Services.Ideas.DTOs;
using DNN.Modules.UserVoice.Services.Localization;
using DotNetNuke.Entities.Modules;
using NSubstitute;
using OneOf.Types;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Xunit;

namespace UnitTests.Controllers
{
    public class IdeaControllerTests
    {
        private readonly CancellationToken token;
        private readonly Fixture fixture;
        private readonly IDnnRequestContext requestContext;
        private readonly IIdeaService ideaService;
        private readonly LocalizationViewModel localizationViewModel;
        private readonly ILocalizationService localizationService;

        private readonly IdeaController ideaController;

        public IdeaControllerTests()
        {
            this.token = new CancellationToken();
            this.fixture = new Fixture();
            this.requestContext = Substitute.For<IDnnRequestContext>();
            this.ideaService = Substitute.For<IIdeaService>();
            this.localizationService = Substitute.For<ILocalizationService>();
            this.localizationViewModel = this.fixture.Create<LocalizationViewModel>();
            this.localizationService.ViewModel.Returns(localizationViewModel);

            this.ideaController = new IdeaController(
                this.requestContext,
                this.ideaService,
                this.localizationService);
        }

        [Fact]
        public async Task Save_ReportsErrorsCorrectly()
        {
            // Arrange
            var dto = new SaveIdeaDto
            {
                Title = "Test Idea",
                Description = "This is a test idea description.",
            };
            this.ideaService
                .SaveIdeaAsync(Arg.Any<SaveIdeaDtoWithContext>(), this.token)
                .Returns(new Error<IEnumerable<string>>(new List<string> { this.localizationViewModel.ModelValidation.TitleTooLong }));
            var module = new ModuleInfo { ModuleID = 123 };
            this.requestContext
                .Module
                .Returns(module);

            // Act
            var response = await this.ideaController.SaveIdea(dto, token);

            // Assert
            var badRequestResult = Assert.IsType<NegotiatedContentResult<ProblemDetails>>(response);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            var problemDetails = badRequestResult.Content;
            Assert.Equal(this.localizationViewModel.ModelValidation.ValidationErrorTitle, problemDetails.Title);
            var error = Assert.Single(problemDetails.Errors);
            Assert.Equal(this.localizationViewModel.ModelValidation.TitleTooLong, error);
        }
    }
}
