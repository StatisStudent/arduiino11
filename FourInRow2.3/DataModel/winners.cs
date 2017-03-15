using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace fourInRow
{
    public class winners
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "amount_of_plays")]
        public int AmountOfPlays { get; set; }
    }
}
