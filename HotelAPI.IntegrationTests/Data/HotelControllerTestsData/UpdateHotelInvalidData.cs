using HotelAPI.Models.HotelModels;
using System.Collections;
using System.Collections.Generic;

namespace HotelAPI.IntegrationTests.Data.HotelControllerTestsData
{
    public class UpdateHotelInvalidData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel1NameInvalid",
                    ContactNumber = "123-123-121"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Description = "UpdatedHotel2DescriptionInvalid",
                    ContactNumber = "123-123-122"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel3NameInvalid",
                    Description = "UpdatedHotel3DescriptionInvalid"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = "UpdatedHotel4NameInvalid",
                    Description = "UpdatedHotel4DescriptionInvalid",
                    ContactNumber = "123-123124"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    Name = null,
                    Description = "UpdatedHotel5DescriptionInvalid",
                    ContactNumber = "123-123-125"
                }
            };
            yield return new object[]
            {
                new UpdateHotelDto()
                {
                    ContactNumber = "adasd123"
                }
            };
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
