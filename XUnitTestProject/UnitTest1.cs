using Microsoft.AspNetCore.Identity;
using MvcMovie.Controllers;
using MvcMovie.Data;
using Xunit;

namespace XUnitTestProject
{
    public class UnitTest1
    {
        private MoviesController movieController;
        private readonly MvcMovieContext context;
        private readonly UserManager<ApplicationUser> userManager;

        [Fact]
        public async void Test1()
        {
            var name = "When Harry Met Sally";
            movieController = new MoviesController(context, userManager);
            await movieController.Details(1);
        }
    }
}
