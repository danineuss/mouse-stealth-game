using UnityEngine;

public class PlayerEventsMono : MonoBehaviour 
{
    public PlayerEvents PlayerEvents => playerEvents;
    private PlayerEvents playerEvents;

    void Awake() 
    {
        playerEvents = new PlayerEvents();
    }
}
