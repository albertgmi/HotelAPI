using HotelAPI.Models.HotelModels;
using System.Collections;
using System.Collections.Generic;

namespace HotelAPI.IntegrationTests.Data.HotelControllerTestsData
{
    public class UpdateHotelValidData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel1Name",
                    Description = "UpdatedHotel1Description",
                    ContactNumber = "123-123-121"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel2Name",
                    Description = "UpdatedHotel2Description",
                    ContactNumber = "123-123-122"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel3Name",
                    Description = "UpdatedHotel3Description",
                    ContactNumber = "123-123-123"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel4Name",
                    Description = "UpdatedHotel4Description",
                    ContactNumber = "123-123-124"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel5Name",
                    Description = "UpdatedHotel5Description",
                    ContactNumber = "123-123-125"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel6Name",
                    Description = "UpdatedHotel6Description",
                    ContactNumber = "123-123-126"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
