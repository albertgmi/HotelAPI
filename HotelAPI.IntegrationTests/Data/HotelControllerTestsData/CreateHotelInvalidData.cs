using HotelAPI.Models.HotelModels;
using System.Collections;
using System.Collections.Generic;

namespace HotelAPI.IntegrationTests.Data.HotelControllerTestsData
{
    public class CreateHotelInvalidData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestHotel1",
                    City = "Warszawa",
                    Street = "Nowoursynowska",
                    ContactNumber = "123-123-121"
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
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
                    ContactNumber = "123-123-124"
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestHotel5",
                    Description = "TestHotel5Description",
                    City = "Warszawa",
                    Street = "Gdańska",
                }
            };
            yield return new object[]
            {
                new CreateHotelDto()
                {
                    Name = "TestHotel6",
                    Description = "TestHotel6Description",
                    City = "Warszawa",
                    Street = "Gdańska",
                    ContactNumber = "123123124"
                }
            };
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
