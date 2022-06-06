﻿using System;
using Newtonsoft.Json;

namespace Corruptor
{
    [Serializable]
    public class Config
    {
        [JsonProperty]
        public bool perPixel { get; set; } = false;
        
        [JsonProperty]
        public bool shuffle { get; set; } = false;
        
        [JsonProperty]
        public int shuffleSize { get; set; } = 4;
        
        [JsonProperty]
        public bool chunkShuffle { get; set; } = false;
        
        [JsonProperty]
        public int chunkShuffleSize { get; set; } = 64;
        
        [JsonProperty]
        public bool artifactAdd { get; set; } = true;
        
        [JsonProperty]
        public int artifactSize { get; set; } = 64;
        
        [JsonProperty]
        public int maxArtifacts { get; set; } = 200;
    }
}