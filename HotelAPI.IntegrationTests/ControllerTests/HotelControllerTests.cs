using FluentAssertions;
using HotelAPI.Entities;
using HotelAPI.IntegrationTests.Data.HotelControllerTestsData;
using HotelAPI.IntegrationTests.FakeUserFiles;
using HotelAPI.IntegrationTests.Helpers;
using HotelAPI.Models.HotelModels;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
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
                        var dbContextOptions = services
                            .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<HotelDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                        services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));


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
        [Theory]
        [InlineData("PageNumber=1&PageSize=20")]
        [InlineData("PageNumber=4&PageSize=14")]
        [InlineData("PageNumber=-1&PageSize=0")]
        [InlineData("SearchPhrase=2412&PageNumber=2&PageSize=-5")]
        [InlineData("SearchPhrase=as&PageNumber=1&PageSize=5&SortBy=Name&SortDirection=-2")]
        [InlineData("SearchPhrase=es&PageNumber=3&PageSize=5&SortBy=Name&SortDirection=14")]
        [InlineData("SearchPhrase=random&PageNumber=1&PageSize=13&SortBy=123asd123")]
        public async Task GetAll_ForInorrectQueryParameters_ReturnsBadRequestStatusCode(string searchQuery)
        {
            // act
            var result = await _client.GetAsync("/api/hotel?" + searchQuery);
            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(143)]
        [InlineData(231)]
        public async Task GetById_ForCorrectData_ReturnsOkStatusCode(int hotelId)
        {
            // act
            var result = await _client.GetAsync("/api/hotel/" + hotelId);

            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
        [Theory]
        [InlineData(-3)]
        [InlineData(211111)]
        [InlineData(-51)]
        public async Task GetById_ForIncorrectData_ReturnsNotFoundStatusCode(int hotelId)
        {
            // act
            var result = await _client.GetAsync("/api/hotel/" + hotelId);

            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [Theory]
        [ClassData(typeof(CreateHotelValidData))]
        public async Task Create_ForValidData_ReturnsCreatedStatusCode(CreateHotelDto dto)
        {
            // assert
            var query = dto.JsonHttpContent();
            // act
            var result = await _client.PostAsync("/api/hotel", query);
            // arrange
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }
    }
}
