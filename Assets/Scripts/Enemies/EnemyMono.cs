using Audio;
using Enemies.VisionCone;
using Infrastructure;
using UnityEngine;

namespace Enemies
{
    public class EnemyMono : MonoBehaviour
    {
        [SerializeField] private EventsMono eventsMono = null;
        //TODO: make audio into interface and Mono as well.
        [SerializeField] private AudioVM audioVM = null;

        public IEnemyVM EnemyVM { get; private set; }

        private EnemyIOMono enemyIOMono;
        private VisionConeMono visionConeMono;
        //TODO: make mono
        private SoundEmitter soundEmitter;
    
        void Awake()
        {
            enemyIOMono = GetComponentInChildren<EnemyIOMono>();
            visionConeMono = GetComponentInChildren<VisionConeMono>();
            soundEmitter = GetComponentInChildren<SoundEmitter>();
        }

        void Start()
        {
            EnemyVM = new EnemyVM(
                visionConeMono.PlayerDetector,
                audioVM,
                enemyIOMono.EnemyIO,
                soundEmitter,
                eventsMono.PlayerEvents,
                eventsMono.EnemyEvents
            );
        }
    }
}