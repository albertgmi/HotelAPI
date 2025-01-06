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
                    Name = "TestHotel1",
                    Description = "TestHotel1Description",
                    City = "Warszawa",
                    Street = "Nowoursynowska",
                    ContactNumber = "123-123-121"
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestHotel2",
                    Description = "TestHotel2Description",
                    City = "Gdańsk",
                    Street = "Warszawska",
                    ContactNumber = "123-123-122"
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestHotel3",
                    Description = "TestHotel3Description",
                    City = "Poznań",
                    Street = "Łazienkowska",
                    ContactNumber = "123-123-123"
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestHotel4",
                    Description = "TestHotel4Description",
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
