using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace fourInRow
{
    public class TodoItem
    {
        public string Id { get; set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        [JsonProperty(PropertyName = "complete")]
        public bool Complete { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }

    }
}
