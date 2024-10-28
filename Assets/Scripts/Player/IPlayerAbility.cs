using System;
using UnityEngine;

namespace Player
{
    public interface IPlayerAbility: IEquatable<IPlayerAbility> {
        KeyCode AssociatedKey { get; }
        float CoolDown { get; }

        void SetTarget(Guid targetID);

        void Execute(IPlayerEvents playerEvents);
    }
}
