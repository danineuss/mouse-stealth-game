using System;
using UnityEngine;

public class PlayerAbilityDistract : IPlayerAbility 
{
    public KeyCode AssociatedKey => KeyCode.F;
    public float CoolDown => 10f;

    private Guid targetID;

    public void SetTarget(Guid targetID)
    {
        this.targetID = targetID;
    }

    public void Execute(IPlayerEvents playerEvents) 
    {
        playerEvents.DistractEnemy(targetID);
    }
}
