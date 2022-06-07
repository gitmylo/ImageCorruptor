using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Corruptor
{
    [Serializable]
    public class Config
    {
        [JsonProperty]
        [Description("Corrupt each pixel by shifting the values.")]
        public bool perPixel { get; set; } = false;
        
        [JsonProperty]
        [Description("Shuffle some random bytes.")]
        public bool shuffle { get; set; } = false;
        
        [JsonProperty]
        [Description("The size of the shuffle.")]
        public int shuffleSize { get; set; } = 4;
        
        [JsonProperty]
        [Description("Shuffle chunks of bytes (causes shifts).")]
        public bool chunkShuffle { get; set; } = false;
        
        [JsonProperty]
        [Description("The size of the chunks.")]
        public int chunkShuffleSize { get; set; } = 64;
        
        [JsonProperty]
        [Description("Add artifacts (shows as strips of random colors).")]
        public bool artifactAdd { get; set; } = false;
        
        [JsonProperty]
        [Description("The amount of bytes to artifact over.")]
        public int artifactSize { get; set; } = 64;
        
        [JsonProperty]
        [Description("The maximum amount of artifacts.")]
        public int maxArtifacts { get; set; } = 200;
    }
}