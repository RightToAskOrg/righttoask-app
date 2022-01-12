using System.Text.Json.Serialization; 

namespace RightToAskClient.Models
{
    public class GeoscapeAddressFeatureCollection
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("features")]
        public GeoscapeAddressFeature[]? AddressDataList { get; set; }
        
        [JsonPropertyName("query")]
        public string? Query { get; set; }
        
        [JsonPropertyName("parsedQuery")]
        public ParsedQuery? ParsedQuery { get; set; }
        
        [JsonPropertyName("messages")]
        public string[]? Messages { get; set; }
        
        [JsonPropertyName("attribution")]
        public string? Attribution { get; set; }
    }
    
    public class ParsedQuery
    {
       [JsonPropertyName("localityName")]
       public string? LocalityName { get; set; }

       [JsonPropertyName("postcode")]
       public string? Postcode { get; set; }

       [JsonPropertyName("stateTerritory")]
       public string? StateTerritory { get; set; }

       [JsonPropertyName("streetName")]
       public string? StreetName { get; set; }

       [JsonPropertyName("streetNumber1")]
       public string? StreetNumber1 { get; set; }

       [JsonPropertyName("streetNumber2")]
       public string? StreetNumber2 { get; set; }

       [JsonPropertyName("complexUnitIdentifier")]
       public string? ComplexUnitIdentifier { get; set; }

       [JsonPropertyName("complexUnitType")]
       public string? ComplexUnitType { get; set; }

       [JsonPropertyName("siteName")]
       public string? SiteName { get; set; }

       [JsonPropertyName("streetType")]
       public string? StreetType { get; set; }

       [JsonPropertyName("lotIdentifier")]
       public string? LotIdentifier { get; set; }

       [JsonPropertyName("streetSuffix")]
       public string? StreetSuffix { get; set; }

       [JsonPropertyName("streetPrefix")]
       public string? StreetPrefix { get; set; }

       [JsonPropertyName("complexLevelIdentifier")]
       public string? ComplexLevelIdentifier { get; set; }

       [JsonPropertyName("complexLevelType")]
       public string? ComplexLevelType { get; set; }
   }
}