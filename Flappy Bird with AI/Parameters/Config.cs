using Flappy_Bird_with_AI.Parameters.Enums;
using Newtonsoft.Json;
using System.IO;

namespace Flappy_Bird_with_AI.Parameters
{
    class Config
    {
        [JsonIgnore]
        public static Config Data;

        static Config()
        {
            Data = JsonConvert.DeserializeObject<Config>(File.ReadAllText("Parameters/Config.json"));
        }

        [JsonProperty("start_type")]
        public StartType StartType { get; set; }

        [JsonProperty("is_tubes_random")]
        public bool IsTubesRandom { get; set; }

        [JsonProperty("tubes_list")]
        public int[] TubesList { get; set; }

        [JsonProperty("tube_hole")]
        public int TubeHole { get; set; }

        [JsonProperty("tube_timer")]
        public double TubeTimer { get; set; }

        [JsonProperty("star_probability")]
        public double StarProbability { get; set; }

        [JsonProperty("fps")]
        public int Fps { get; set; }

        [JsonProperty("draw_fps")]
        public int DrawFps { get; set; }
    }
}