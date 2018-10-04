using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_TexTools2.Model
{
    public class ExportMetadata
    {
        [JsonProperty("modelKey")]
        public string ModelKey; // e.g., 4/m0328v01

        [JsonProperty("sets")]
        public List<ExportSetMetadata> Sets = new List<ExportSetMetadata>();
    }

    public class ExportSetMetadata
    {
        [JsonProperty("raceGender")]
        public string RaceGender; // e.g. Hyur Midlander Female

        [JsonProperty("raceGenderKey")]
        public string RaceGenderKey; // e.g. 0201

        [JsonProperty("models")]
        public List<ExportModelMetadata> Models = new List<ExportModelMetadata>();
    }

    public class ExportModelMetadata
    {
        [JsonProperty("alpha")]
        public bool Alpha;

        [JsonProperty("diffuse")]
        public bool Diffuse;

        [JsonProperty("emissive")]
        public bool Emissive;

        [JsonProperty("normal")]
        public bool Normal;

        [JsonProperty("specular")]
        public bool Specular;
    }
}
