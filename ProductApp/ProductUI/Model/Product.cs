using Newtonsoft.Json;


namespace ProductUI.Model
{
   public class Product
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("barcode")]
        public string? BarCode { get; set; }

        [JsonProperty("price")]
        public string? Price { get; set; }
    }
}
