using System.Collections.Generic;
using System.Text.Json.Serialization;

/* Generated from the Geoscape API example at https://docs.geoscape.com.au/docs/beta-geoscape-apis/c2NoOjIyNDc2Mw-address-geo-json
 * using online serlialiser from https://json2csharp.com/. Note the alternative is NewtonSoft, e.g. using this script
 * https://app.quicktype.io/?l=csharp */
namespace RightToAskClient.Models
{
    public class GeoscapeAddressFeature
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("properties")]
        public Properties? Properties { get; set; }

        [JsonPropertyName("geometry")]
        public Geometry? Geometry { get; set; }

        [JsonPropertyName("attribution")]
        public string? Attribution { get; set; }
    }

    public class Geometry
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("coordinates")]
        public double[]? Coordinates { get; set; }
    }

    public class Properties
    {
        [JsonPropertyName("addressId")]
        public string? AddressId { get; set; }

        [JsonPropertyName("jurisdictionId")]
        public string? JurisdictionId { get; set; }

        [JsonPropertyName("addressRecordType")]
        public string? AddressRecordType { get; set; }

        [JsonPropertyName("aliasPrincipal")]
        public string? AliasPrincipal { get; set; }

        [JsonPropertyName("geoFeature")]
        public string? GeoFeature { get; set; }

        [JsonPropertyName("cadastralIdentifier")]
        public string? CadastralIdentifier { get; set; }

        [JsonPropertyName("formattedAddress")]
        public string? FormattedAddress { get; set; }

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

        [JsonPropertyName("streetType")]
        public string? StreetType { get; set; }

        [JsonPropertyName("streetTypeDescription")]
        public string? StreetTypeDescription { get; set; }

        [JsonPropertyName("asgsMain")]
        public Dictionary<string, AsgsMain>? AsgsMain { get; set; }

        /*
        [JsonPropertyName("asgsRemoteness")]
        public Dictionary<string, AsgsRemoteness> AsgsRemoteness { get; set; }
        */

        [JsonPropertyName("buildingsRolloutStatus")]
        public string? BuildingsRolloutStatus { get; set; }

        [JsonPropertyName("commonwealthElectorate")]
        public CommonwealthElectorate? CommonwealthElectorate { get; set; }

        [JsonPropertyName("localGovernmentArea")]
        public LocalGovernmentArea? LocalGovernmentArea { get; set; }

        [JsonPropertyName("relatedBuildingIds")]
        public string[]? RelatedBuildingIds { get; set; }

        [JsonPropertyName("stateElectorate")]
        public StateElectorate? StateElectorate { get; set; }
    }

    public class AsgsMain
    {
        [JsonPropertyName("mbId")]
        public string? MbId { get; set; }

        [JsonPropertyName("sa1Id")]
        public string? Sa1Id { get; set; }

        [JsonPropertyName("sa2Id")]
        public string? Sa2Id { get; set; }

        [JsonPropertyName("sa2Name")]
        public string? Sa2Name { get; set; }

        [JsonPropertyName("sa3Id")]
        public string? Sa3Id { get; set; }

        [JsonPropertyName("sa3Name")]
        public string? Sa3Name { get; set; }

        [JsonPropertyName("sa4Id")]
        public string? Sa4Id { get; set; }

        [JsonPropertyName("sa4Name")]
        public string? Sa4Name { get; set; }
    }

    // TODO: this seems wrong to me because of not having the nesting for 2011 and 2016.
    /* I'm going to try commenting it out and deserialising only part of the data structure
    public partial class AsgsRemoteness
    {
        [JsonPropertyName("categoryCode")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long CategoryCode { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("code")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Code { get; set; }
    }

     */
    
    public class CommonwealthElectorate
    {
        [JsonPropertyName("commElectoralName")]
        public string? CommElectoralName { get; set; }

        [JsonPropertyName("commElectoralPid")]
        public string? CommElectoralPid { get; set; }
    }

    public class LocalGovernmentArea
    {
        [JsonPropertyName("lgaName")]
        public string? LgaName { get; set; }

        [JsonPropertyName("lgaPid")]
        public string? LgaPid { get; set; }

        [JsonPropertyName("lgaShortName")]
        public string? LgaShortName { get; set; }
    }

    public class StateElectorate
    {
        [JsonPropertyName("stateElectoralPid")]
        public string? stateElectoralPid{ get; set; }
        
        [JsonPropertyName("stateElectoralName")]
        public string? StateElectoralName { get; set; }
        
        [JsonPropertyName("stateElectoralClassCode")]
        public string? stateElectoralClassCode { get; set; }

        [JsonPropertyName("stateElectoralType")]
        public string? StateElectoralType { get; set; }
    }
}
