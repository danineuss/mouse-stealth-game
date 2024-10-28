using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure;
using Player;
using UnityEngine;

namespace Enemies
{
    public enum EnemyIOTextColor
    {
        Active,
        Inactive
    }

    public interface IEnemyIO: IUpdatable
    {
        void OnMouseEnter();
        void OnMouseExit();
        void SetEnemyID(Guid enemyID);
        void SetTextColor(EnemyIOTextColor textColor);
        void SetTextVisibleAndFollowing(bool shouldDisplayText, Transform playerTransform = null);
        void UpdateCooldownForAbility(IPlayerAbility ability);
    }

    public class EnemyIO : IEnemyIO
    {
        private IEnemyEvents enemyEvents;
        private TextMesh textMesh;
        private OutlineMono enemyOutline;
        private readonly Color InactiveTextColor;
        private readonly Color ActiveTextColor;
        private Transform ioTransform;
        private Transform cooldownScaleParent;
        private Transform playerFollowTransform;
        private Dictionary<IPlayerAbility, float> abilityCooldowns;
        private Guid enemyID;
        private readonly float OutlineWidth = 10f;

        public EnemyIO(
            IEnemyEvents enemyEvents,
            TextMesh textMesh,
            OutlineMono enemyOutline,
            Color inactiveTextColor,
            Color activeTextColor,
            Transform ioTransform,
            Transform cooldownScaleParent)
        {
            this.enemyEvents = enemyEvents;
            this.textMesh = textMesh;
            this.enemyOutline = enemyOutline;
            this.InactiveTextColor = inactiveTextColor;
            this.ActiveTextColor = activeTextColor;
            this.ioTransform = ioTransform;
            this.cooldownScaleParent = cooldownScaleParent;

            abilityCooldowns = new Dictionary<IPlayerAbility, float>();
            playerFollowTransform = null;
            this.textMesh.color = activeTextColor;
            this.cooldownScaleParent.localScale = new Vector3(1f, 0f, 1f);
            this.enemyID = Guid.Empty;

            SetTextVisibleAndFollowing(false);
        }

        public void SetTextVisibleAndFollowing(bool shouldDisplayText, Transform playerTransform = null)
        {
            textMesh.gameObject.SetActive(shouldDisplayText);
            cooldownScaleParent.gameObject.SetActive(shouldDisplayText);
            enemyOutline.OutlineWidth = shouldDisplayText ? OutlineWidth : 0f;
            playerFollowTransform = playerTransform;
        }

        public void SetTextColor(EnemyIOTextColor textColor)
        {
            switch(textColor)
            {
                case EnemyIOTextColor.Active:
                    textMesh.color = ActiveTextColor;
                    break;
                case EnemyIOTextColor.Inactive:
                    textMesh.color = InactiveTextColor;
                    break;
            }
        }

        public void UpdateCooldownForAbility(IPlayerAbility ability)
        {
            if (abilityCooldowns.Keys.Contains(ability))
            {
                abilityCooldowns[ability] = ability.CoolDown;
            }
            else
            {
                abilityCooldowns.Add(ability, ability.CoolDown);
            }
        }

        public void SetEnemyID(Guid enemyID)
        {
            if (this.enemyID != Guid.Empty)
                throw new Exception("Should only set the ID once.");
        
            this.enemyID = enemyID;
        }

        public void Update()
        {
            UpdateTextOrientation();
            UpdateAbilityCooldownIndicator();
        }

        public void OnMouseEnter()
        {
            enemyEvents.CursorEnterEnemy(enemyID);
        }

        public void OnMouseExit()
        {
            enemyEvents.CursorExitEnemy();
        }

        void UpdateTextOrientation()
        {
            if (playerFollowTransform == null)
                return;

            ioTransform.LookAt(playerFollowTransform);
        }

        void UpdateAbilityCooldownIndicator()
        {
            var keys = new List<IPlayerAbility>(abilityCooldowns.Keys) ?? new List<IPlayerAbility>();
            foreach (var key in keys)
            {
                var newCooldown = abilityCooldowns[key] - Time.deltaTime;
                newCooldown = Mathf.Clamp(newCooldown, 0f, key.CoolDown);
                if (newCooldown == 0f)
                {
                    textMesh.color = ActiveTextColor;
                }
                else
                {
                    textMesh.color = InactiveTextColor;
                }

                var currentScale = cooldownScaleParent.localScale;
                cooldownScaleParent.localScale = new Vector3(
                    currentScale.x, newCooldown / key.CoolDown, currentScale.z
                );
                abilityCooldowns[key] = newCooldown;
            }
        }
    }
}