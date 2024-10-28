using Infrastructure;
using UnityEngine;

namespace Player
{
    public interface IPlayerInput 
    {
        float Vertical { get; }
        float Horizontal { get; }
        float CursorX { get; }
        float CursorY { get; }
        bool GetKeyDown(KeyCode keyCode);
        void HandleGenericPlayerInput();
    }

    public class PlayerInput : MustInitialize<IPlayerEvents>, IPlayerInput 
    {
        public float Vertical => Input.GetAxis("Vertical");
        public float Horizontal => Input.GetAxis("Horizontal");
        public float CursorX => Input.GetAxis("Mouse X");
        public float CursorY => Input.GetAxis("Mouse Y");

        private KeyCode Pause {
            get {
#if UNITY_EDITOR
                return KeyCode.E;
#else
            return KeyCode.Escape;
#endif
            }
        }
        private readonly float buttonDelay = 0.2f;
        private float timeSinceLastPause = 0.0f;
        private IPlayerEvents playerEvents;

        public PlayerInput(IPlayerEvents playerEvents) : base(playerEvents)
        {
            this.playerEvents = playerEvents;
        }

        public bool GetKeyDown(KeyCode keyCode) 
        {
            return Input.GetKeyDown(keyCode);
        }

        public void HandleGenericPlayerInput()
        {
            if (GetKeyDown(Pause) && (Time.unscaledTime - timeSinceLastPause > buttonDelay))
            {
                playerEvents.PressPauseButton();
                timeSinceLastPause = Time.unscaledTime;
            }
        }
    }
}