using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IEnemyIO
{
    void OnMouseEnter();
    void OnMouseExit();
    void SetDisplayVisibility(bool visible);
    void SetEnemyID(Guid enemyID);
    void SetTextColor(DetectorStateEnum detectorState);
    void SetTextFollowingPlayer(bool shouldDisplayText, Transform playerTransform = null);
    void Update();
    void UpdateCooldownForAbility(IPlayerAbility ability);
}

public class EnemyIO : IEnemyIO
{
    private IEnemyEvents enemyEvents;
    private TextMesh textMesh;
    private OutlineMono enemyOutline;
    private Color kInactiveTextColor;
    private Color kActiveTextColor;
    private Transform ioTransform;
    private Transform cooldownScaleParent;
    private Transform playerFollowTransform;
    private Dictionary<IPlayerAbility, float> abilityCooldowns;
    private Guid enemyID;
    private float kOutlineWidth = 10f;

    public void SetDisplayVisibility(bool visible)
    {
        textMesh.gameObject.SetActive(visible);
        cooldownScaleParent.gameObject.SetActive(visible);
        enemyOutline.OutlineWidth = visible ? kOutlineWidth : 0f;
    }

    public void SetTextFollowingPlayer(bool shouldDisplayText, Transform playerTransform)
    {
        textMesh.gameObject.SetActive(shouldDisplayText);
        cooldownScaleParent.gameObject.SetActive(shouldDisplayText);

        if (shouldDisplayText)
        {
            playerFollowTransform = playerTransform;
        }
    }

    public void SetTextColor(DetectorStateEnum detectorState)
    {
        if (detectorState == DetectorStateEnum.Idle)
        {
            textMesh.color = kActiveTextColor;
        }
        else
        {
            textMesh.color = kInactiveTextColor;
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

    public EnemyIO(
        IEnemyEvents enemyEvents,
        TextMesh textMesh,
        OutlineMono enemyOutline,
        Color kInactiveTextColor,
        Color kActiveTextColor,
        Transform ioTransform,
        Transform cooldownScaleParent)
    {
        this.enemyEvents = enemyEvents;
        this.textMesh = textMesh;
        this.enemyOutline = enemyOutline;
        this.kInactiveTextColor = kInactiveTextColor;
        this.kActiveTextColor = kActiveTextColor;
        this.ioTransform = ioTransform;
        this.cooldownScaleParent = cooldownScaleParent;

        abilityCooldowns = new Dictionary<IPlayerAbility, float>();
        playerFollowTransform = null;
        this.textMesh.color = kActiveTextColor;
        this.cooldownScaleParent.localScale = new Vector3(1f, 0f, 1f);
        this.enemyID = Guid.Empty;

        SetDisplayVisibility(false);
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
                textMesh.color = kActiveTextColor;
            }
            else
            {
                textMesh.color = kInactiveTextColor;
            }

            var currentScale = cooldownScaleParent.localScale;
            cooldownScaleParent.localScale = new Vector3(
                currentScale.x, newCooldown / key.CoolDown, currentScale.z
            );
            abilityCooldowns[key] = newCooldown;
        }
    }
}
