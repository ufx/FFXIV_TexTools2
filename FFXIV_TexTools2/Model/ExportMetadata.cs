using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Windows.Media.Imaging;

namespace FFXIV_TexTools2.Model
{
    public class ExportHashRepository
    {
        static HashAlgorithm _hashAlgorithm = MD5.Create();
        HashSet<string> _writtenHashes = new HashSet<string>();
        string _hashRepoPath;

        public ExportHashRepository(string hashRepoPath)
        {
            _hashRepoPath = hashRepoPath;
        }

        public string Write(string extension, byte[] data)
        {
            if (data == null)
                return null;

            var hash = Hash(data);
            if (!_writtenHashes.Contains(hash))
            {
                _writtenHashes.Add(hash);
                var hashBasePath = Path.Combine(_hashRepoPath, hash.Substring(0, 2));
                var hashPath = Path.Combine(hashBasePath, hash) + extension;
                if (!File.Exists(hashPath))
                {
                    Directory.CreateDirectory(hashBasePath);
                    File.WriteAllBytes(hashPath, data);
                }
            }

            return hash;
        }

        public string Write(BitmapSource source)
        {
            if (source == null)
                return null;

            var encoder = new PngBitmapEncoder();
            using (var stream = new MemoryStream())
            {
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream);
                return Write(".png", stream.ToArray());
            }
        }

        static string Hash(byte[] data)
        {
            var hash = _hashAlgorithm.ComputeHash(data);
            return string.Join("", hash.Select(b => b.ToString("X2")));
        }
    }

    public class ExportMetadata
    {
        [JsonProperty("sets")]
        public List<ExportSetMetadata> Sets = new List<ExportSetMetadata>();
    }

    public class ExportSetMetadata
    {
        [JsonProperty("raceGender")]
        public string RaceGender; // e.g. Hyur Midlander Female

        [JsonProperty("models")]
        public List<ExportModelMetadata> Models = new List<ExportModelMetadata>();
    }

    public class ExportModelMetadata
    {
        [JsonProperty("obj")]
        public string Obj;

        [JsonProperty("alpha")]
        public string Alpha;

        [JsonProperty("diffuse")]
        public string Diffuse;

        [JsonProperty("emissive")]
        public string Emissive;

        [JsonProperty("normal")]
        public string Normal;

        [JsonProperty("specular")]
        public string Specular;
    }
}
