using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAbilities : IPlayerAbilities
{
    public Dictionary<KeyCode, IPlayerAbility> Abilities
    {
        get; private set;
    }

    private Dictionary<IPlayerAbility, float> timesSinceLastExecute;
    private PlayerEvents playerEvents;

    //TODO: make playerEvents non-monobehaviour
    public PlayerAbilities(
        PlayerEvents playerEvents, Dictionary<KeyCode, IPlayerAbility> abilities) 
    {
        this.playerEvents = playerEvents;
        Abilities = abilities;
        timesSinceLastExecute = Abilities.Values.ToDictionary(x => x, x => -1f);
    }

    public List<KeyCode> RelevantKeyPresses
    {
        get => Abilities.Select(x => x.Value.AssociatedKey).ToList();
    }

    public List<IPlayerAbility> RelevantAbilities
    {
        get => Abilities.Select(x => x.Value).ToList();
    }

    public void ExecuteAbility(IPlayerAbility ability, EnemyVM enemyVM = null)
    {
        var lastExecute = timesSinceLastExecute[ability];
        if (Time.time - lastExecute < ability.CoolDown && lastExecute != -1f)
            return;

        var executed = ability.Execute(enemyVM);
        if (executed)
        {
            timesSinceLastExecute[ability] = Time.time;
            playerEvents.AbilityExecuted(ability);
        }
    }

    public void LearnAbility(IPlayerAbility ability)
    {
        if (Abilities.ContainsValue(ability))
            return;

        Abilities.Add(ability.AssociatedKey, ability);
        timesSinceLastExecute.Add(ability, -1f);
    }
}
