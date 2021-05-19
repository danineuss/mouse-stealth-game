using System;
using UnityEngine;

public class PlayerAbilityDistract : IPlayerAbility 
{
    public KeyCode AssociatedKey => KeyCode.F;
    public float CoolDown => 10f;

    private float distractionDuration;
    private Guid targetID;

    public PlayerAbilityDistract(float distractionDuration)
    {
        this.distractionDuration = distractionDuration;
    }

    public void SetTarget(Guid targetID)
    {
        this.targetID = targetID;
    }

    public void Execute(IPlayerEvents playerEvents) 
    {
        playerEvents.DistractEnemy(targetID, distractionDuration);
    }
}
