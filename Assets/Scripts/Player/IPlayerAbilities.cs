using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAbilities
{
    Dictionary<KeyCode, IPlayerAbility> Abilities { get; }
    List<KeyCode> RelevantKeyPresses { get; }
    List<IPlayerAbility> RelevantAbilities { get; }

    void ExecuteAbility(IPlayerAbility ability, EnemyVM enemyVM = null);
    void LearnAbility(IPlayerAbility ability);
}
