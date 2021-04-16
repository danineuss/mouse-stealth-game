using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{   
    public Dictionary<KeyCode, IPlayerAbility> Abilities {
        get; private set;
    }

    private Dictionary<IPlayerAbility, float> timesSinceLastExecute;
    private PlayerEvents playerEvents;

    public List<KeyCode> RelevantKeyPresses {
        get => Abilities.Select(x => x.Value.AssociatedKey).ToList();
    }

    public List<IPlayerAbility> RelevantAbilities {
        get => Abilities.Select(x => x.Value).ToList();
    }
    
    public void ExecuteAbility(IPlayerAbility ability, EnemyVM enemyVM = null) {
        var lastExecute = timesSinceLastExecute[ability];
        if (Time.time - lastExecute < ability.CoolDown && lastExecute != -1f) {
            return;
        }

        var executed = ability.Execute(enemyVM);
        if (executed) {
            timesSinceLastExecute[ability] = Time.time;
            playerEvents.AbilityExecuted(ability);
        }
    }

    void Start() {
        Abilities = GetComponentsInChildren<IPlayerAbility>()
                    .ToDictionary(value => value.AssociatedKey, value => value);
        timesSinceLastExecute = Abilities.Values.ToDictionary(x => x, x => -1f);
        playerEvents = GetComponentInParent<PlayerVM>().PlayerEvents;
    }
}
