using System;
using UnityEngine;

namespace Audio
{
    [Serializable]
    public struct Sound {
        public string Name;
        public AudioClip Clip;
        [Range(0f, 1f)] public float Volume;
        public bool Loop;
    }
}
