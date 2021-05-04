using UnityEngine;

public interface IPlayerInput {
    float Vertical { get; }
    float Horizontal { get; }
    float CursorX { get; }
    float CursorY { get; }
    bool GetKeyDown(KeyCode keyCode);
}

public class PlayerInput : IPlayerInput {
    public float Vertical => Input.GetAxis("Vertical");
    public float Horizontal => Input.GetAxis("Horizontal");
    public float CursorX => Input.GetAxis("Mouse X");
    public float CursorY => Input.GetAxis("Mouse Y");
    public static KeyCode Escape {
        get {
            #if UNITY_EDITOR
            return KeyCode.E;
            #else
            return KeyCode.Escape;
            #endif
        }
    }
    
    public bool GetKeyDown(KeyCode keyCode) {
        return Input.GetKeyDown(keyCode);
    }
}
