using HotelAPI.Models.HotelModels;
using System.Collections;
using System.Collections.Generic;

namespace HotelAPI.IntegrationTests.Data.HotelControllerTestsData
{
    public class CreateHotelValidData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestRestaurant1",
                    Description = "TestRestaurant1Description",
                    City = "Warszawa",
                    Street = "Nowoursynowska",
                    ContactNumber = "123-123-121"
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestRestaurant2",
                    Description = "TestRestaurant2Description",
                    City = "Gdańsk",
                    Street = "Warszawska",
                    ContactNumber = "123-123-122"
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestRestaurant3",
                    Description = "TestRestaurant3Description",
                    City = "Poznań",
                    Street = "Łazienkowska",
                    ContactNumber = "123-123-123"
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestRestaurant4",
                    Description = "TestRestaurant4Description",
                    City = "Warszawa",
                    Street = "Gdańska",
                    ContactNumber = "123-123-124"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
