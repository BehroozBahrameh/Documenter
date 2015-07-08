using System;
using System.Collections.Generic;
using Sample.Constants;

namespace Sample.Models
{
    public class SampleResponseModel
    {
        public long Id { get; set; }
        public Gender Gender { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int[] Numbers { get; set; }
        public IEnumerable<SampleResponseDetailModel> Chileds { get; set; }
    }
}