using System;
using Newtonsoft.Json;

namespace CodeJay
{
    [Serializable]
    public class SaveData
    {
        public const int Version = 1;
        public int CurrentVersion { get; set; } = Version;
#if UNITY_EDITOR
        [JsonIgnore]
        public bool IsTest { get; set; } = false;
#endif

    }
}