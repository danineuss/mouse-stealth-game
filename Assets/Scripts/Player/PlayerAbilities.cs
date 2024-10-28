using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public interface IPlayerAbilities
    {
        Dictionary<KeyCode, IPlayerAbility> Abilities { get; }
        List<KeyCode> RelevantKeyPresses { get; }
        List<IPlayerAbility> RelevantAbilities { get; }

        void ExecuteAbility(IPlayerAbility ability, Guid enemyID);
        void LearnAbility(IPlayerAbility ability);
    }

    public class PlayerAbilities : IPlayerAbilities
    {
        public Dictionary<KeyCode, IPlayerAbility> Abilities
        {
            get;
        }

        private readonly Dictionary<IPlayerAbility, float> timesSinceLastExecute;
        private readonly IPlayerEvents playerEvents;

        public PlayerAbilities(
            IPlayerEvents playerEvents, Dictionary<KeyCode, IPlayerAbility> abilities) 
        {
            this.playerEvents = playerEvents;
            Abilities = abilities;
            timesSinceLastExecute = Abilities.Values.ToDictionary(x => x, x => -1f);
        }

        public List<KeyCode> RelevantKeyPresses => Abilities.Select(x => x.Value.AssociatedKey).ToList();

        public List<IPlayerAbility> RelevantAbilities => Abilities.Select(x => x.Value).ToList();

        public void ExecuteAbility(IPlayerAbility ability, Guid targetID)
        {
            if (!Abilities.ContainsValue(ability))
                return;

            var lastExecute = timesSinceLastExecute[ability];
            if (Time.time - lastExecute < ability.CoolDown && lastExecute != -1f)
                return;

            timesSinceLastExecute[ability] = Time.time;
            ability.SetTarget(targetID);
            ability.Execute(playerEvents);
            playerEvents.ExecuteAbility(ability);
        }

        public void LearnAbility(IPlayerAbility ability)
        {
            if (Abilities.ContainsValue(ability))
                return;

            Abilities.Add(ability.AssociatedKey, ability);
            timesSinceLastExecute.Add(ability, -1f);
        }
    }
}