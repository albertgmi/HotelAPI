using FluentAssertions;
using HotelAPI.Entities;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace HotelAPI.IntegrationTests.ControllerTests
{
    public class HotelControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        public HotelControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions =
                            services.SingleOrDefault(service =>
                                    service.ServiceType == typeof(DbContextOptions<HotelDbContext>));

                        services.Remove(dbContextOptions);

                        services
                        .AddDbContext<HotelDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    });
                });
            _client = _factory.CreateClient();
        }
        [Theory]
        [InlineData("PageNumber=1&PageSize=5")]
        [InlineData("PageNumber=4&PageSize=10")]
        [InlineData("PageNumber=2&PageSize=15")]
        [InlineData("SearchPhrase=des&PageNumber=2&PageSize=15")]
        [InlineData("SearchPhrase=as&PageNumber=1&PageSize=5&SortBy=Name&SortDirection=1")]
        [InlineData("SearchPhrase=es&PageNumber=3&PageSize=5&SortBy=Name&SortDirection=0")]
        [InlineData("SearchPhrase=random&PageNumber=1&PageSize=5&SortBy=Name")]
        public async Task GetAll_ForCorrectQueryParameters_ReturnsOkStatusCode(string searchQuery)
        {
            // act
            var result = await _client.GetAsync("/api/hotel?"+searchQuery);
            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
