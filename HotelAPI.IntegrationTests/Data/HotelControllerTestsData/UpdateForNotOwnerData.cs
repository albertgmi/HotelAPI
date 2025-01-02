using HotelAPI.Models.HotelModels;
using System.Collections;
using System.Collections.Generic;

namespace HotelAPI.IntegrationTests.Data.HotelControllerTestsData
{
    public class UpdateForNotOwnerData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel1NameNotOwner",
                    Description = "UpdatedHotel1DescriptionNotOwner",
                    ContactNumber = "123-123-121"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel2NameNotOwner",
                    Description = "UpdatedHotel2DescriptionNotOwner",
                    ContactNumber = "123-123-122"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel3NameNotOwner",
                    Description = "UpdatedHotel3DescriptionNotOwner",
                    ContactNumber = "123-123-123"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel4NameNotOwner",
                    Description = "UpdatedHotel4DescriptionNotOwner",
                    ContactNumber = "123-123-124"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel5NameNotOwner",
                    Description = "UpdatedHotel5DescriptionNotOwner",
                    ContactNumber = "123-123-125"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel6NameNotOwner",
                    Description = "UpdatedHotel6DescriptionNotOwner",
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
