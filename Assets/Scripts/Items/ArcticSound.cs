using UnityEngine;
using UnityEngine.Audio;

namespace Assets.Scripts.Items
{
    [System.Serializable]
    public class ArcticSound
    {
        public string name;
        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        [Space]
        [Range(0f, 1f)] public float volume = 1;
        [Range(.3f, 3f)] public float pitch = 1;
        public bool loop;
        public bool stopOnSceneChance;
        public bool autoLoadToMemory = true;
        [HideInInspector] public AudioSource source;
    }

}
