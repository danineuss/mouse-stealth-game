using UnityEngine;

namespace Audio
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private AudioViewModel audioViewModel;
        [SerializeField] private string nameOfSound = "";
        private SoundEmitter soundEmitter;

        void Awake() {
            soundEmitter = GetComponentInChildren<SoundEmitter>();
        }

        void OnTriggerEnter() {
        
            Sound sound = audioViewModel.SoundWithName(nameOfSound);
            soundEmitter.PlaySound(sound);
        }
    }
}
