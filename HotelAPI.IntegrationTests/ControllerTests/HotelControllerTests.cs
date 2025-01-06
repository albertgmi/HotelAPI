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
                         .AddDbContext<HotelDbContext>(options => options.UseInMemoryDatabase("HotelDb"));

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
        [InlineData(32)]
        [InlineData(68)]
        [InlineData(213)]
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
            var hotel = new Hotel()
            {
                Name = dto.Name,
                Description = dto.Description,
                Address = new Address()
                {
                    City = dto.City,
                    Street = dto.Street
                },
                ContactNumber = dto.ContactNumber
            };
            var query = dto.JsonHttpContent();          
            // act
            var result = await _client.PostAsync("/api/hotel", query);
            var createdHotel = GetHotelFromDb(dto.Name, dto.Description);
            // arrange
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            createdHotel.Name.Should().Be(hotel.Name);
            createdHotel.Description.Should().Be(hotel.Description);
            createdHotel.ContactNumber.Should().Be(hotel.ContactNumber);
        }
        [Theory]
        [ClassData(typeof(CreateHotelInvalidData))]
        public async Task Create_ForInvalidData_ReturnsBadRequestStatusCode(CreateHotelDto dto)
        {
            // assert
            var query = dto.JsonHttpContent();
            // act
            var result = await _client.PostAsync("/api/hotel", query);
            // arrange
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(-32)]
        [InlineData(0)]
        public async Task Delete_ForNonExistingHotel_ReturnsNotFoundStatusCode(int hotelId)
        {
            // act
            var result = await _client.DeleteAsync("/api/hotel/" + hotelId);
            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task Delete_ForHotelOwner_ReturnsNoContentStatusCode()
        {
            // arrange
            var hotel = new Hotel()
            {
                Name = "TestHotel",
                CreatedById = 1,
                Rating = 2
            };
            SeedHotel(hotel);
            // act
            var result = await _client.DeleteAsync("/api/hotel/" + hotel.Id);
            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task Delete_ForNotHotelOwner_ReturnsForbiddenStatusCode()
        {
            // arrange
            var hotel = new Hotel()
            {
                Name = "TestHotel2",
                CreatedById = 31,
                Rating = 4
            };
            SeedHotel(hotel);
            // act
            var result = await _client.DeleteAsync("/api/hotel/" + hotel.Id);
            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }
        [Theory]
        [ClassData(typeof(UpdateHotelValidData))]
        public async Task Update_ForHotelOwner_ReturnsNoContentStatusCode(UpdateHotelDto updateDto)
        {
            // arrange
            var hotel = new Hotel()
            {
                Name = "TestHotel3",
                CreatedById = 1,
                Rating = 2
            };
            SeedHotel(hotel);
            var httpContent = updateDto.JsonHttpContent();

            // act
            var result = await _client.PutAsync("api/hotel/"+hotel.Id, httpContent);
            var updatedHotelFromDb = GetHotelFromDb(updateDto.Name, updateDto.Description);

            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
            updatedHotelFromDb.Name.Should().Be(updateDto.Name);
            updatedHotelFromDb.Description.Should().Be(updateDto.Description);
        }
        [Theory]
        [ClassData(typeof(UpdateForNotOwnerData))]
        public async Task Update_ForNotHotelOwner_ReturnsForbiddenStatusCode(UpdateHotelDto updateDto)
        {
            // arrange
            var hotel = new Hotel()
            {
                Name = "TestHotel3",
                CreatedById = 3,
                Rating = 2
            };
            SeedHotel(hotel);
            var httpContent = updateDto.JsonHttpContent();

            // act
            var result = await _client.PutAsync("api/hotel/" + hotel.Id, httpContent);

            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }
        [Theory]
        [ClassData(typeof(UpdateHotelInvalidData))]
        public async Task Update_ForInvalidData_ReturnsBadRequestStatusCode(UpdateHotelDto updateDto)
        {
            // arrange
            var hotel = new Hotel()
            {
                Name = "TestHotel3",
                CreatedById = 1,
                Rating = 2
            };
            SeedHotel(hotel);
            var httpContent = updateDto.JsonHttpContent();

            // act
            var result = await _client.PutAsync("api/hotel/" + hotel.Id, httpContent);

            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
        [Theory]
        [InlineData(595)]
        public async Task GetOwner_ForValidId_ReturnsOkStatusCode(int hotelId)
        {
            // act
            var result = await _client.GetAsync($"/api/hotel/{hotelId}/owner");
            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
        private Hotel GetHotelFromDb(string name, string description)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<HotelDbContext>();
            var hotel = dbContext.Hotels
                .Where(h => h.Name == name
                    && h.Description == description)
                .FirstOrDefault();
            return hotel;
        }
        private void SeedHotel(Hotel hotel)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<HotelDbContext>();
            dbContext.Hotels.Add(hotel);
            dbContext.SaveChanges();
        }
    }
}
