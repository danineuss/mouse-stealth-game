using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyIO : MonoBehaviour
{
    [SerializeField] private Color kActiveTextColor = new Color(0, 71, 188, 255);
    [SerializeField] private Color kInactiveTextColor = new Color(0, 75, 75, 75);

    private EnemyVM enemyVM;
    private TextMesh textDisplay;
    private Outline enemyOutline;
    private Transform cooldownScaleParent;
    private Transform playerFollowTransform = null;
    private Dictionary<IPlayerAbility, float> abilityCooldowns;

    public void SetDisplayVisibility(bool visible) {
        textDisplay.gameObject.SetActive(visible);
        cooldownScaleParent.gameObject.SetActive(visible);
        enemyOutline.OutlineWidth = visible ? 10 : 0;
    }

    public void SetTextFollowingPlayer(bool shouldDisplayText, Transform playerTransform = null) {
        textDisplay.gameObject.SetActive(shouldDisplayText);
        cooldownScaleParent.gameObject.SetActive(shouldDisplayText);

        if (shouldDisplayText) {
            playerFollowTransform = playerTransform;
        }
    }

    public void SetTextColor(DetectorState detectorState) {
        if (detectorState == DetectorState.Idle) { 
            textDisplay.color = kActiveTextColor;
        } else {
            textDisplay.color = kInactiveTextColor;
        }
    }

    public void UpdateCooldownForAbility(IPlayerAbility ability) {
        if (abilityCooldowns.Keys.Contains(ability)) {
            abilityCooldowns[ability] = ability.CoolDown;
        } else {
            abilityCooldowns.Add(ability, ability.CoolDown);
        }
    }

    void Awake() {
        abilityCooldowns = new Dictionary<IPlayerAbility, float>();
    }
    void Start() {
        enemyVM = GetComponentInParent<EnemyVM>();
        textDisplay = GetComponentInChildren<TextMesh>();
        textDisplay.color = kActiveTextColor;
        cooldownScaleParent = GetComponentsInChildren<Transform>()
                                .Where(x => x.CompareTag("ScaleParent"))
                                .First();
        cooldownScaleParent.localScale = new Vector3(1f, 0f, 1f);
        enemyOutline = enemyVM.GetComponentsInChildren<Transform>()
                        .Where(x => x.CompareTag("Model"))
                        .First()
                        .GetComponent<Outline>();

        SetDisplayVisibility(false);
    }

    void Update() {
        UpdateTextOrientation();
        UpdateAbilityCooldownIndicator();
    }

    void OnMouseEnter() {
        enemyVM.EnemyEvents.CursorEnterEnemy(enemyVM);
    }

    void OnMouseExit() {
        enemyVM.EnemyEvents.CursorExitEnemy();
    }

    void UpdateTextOrientation() {
        if (playerFollowTransform == null) {
            return;
        }

        transform.LookAt(playerFollowTransform);
    }

    void UpdateAbilityCooldownIndicator() {
        var keys = new List<IPlayerAbility>(abilityCooldowns.Keys) ?? new List<IPlayerAbility>();
        foreach (var key in keys) {
            var newCooldown = abilityCooldowns[key] - Time.deltaTime;
            newCooldown = Mathf.Clamp(newCooldown, 0f, key.CoolDown);
            if (newCooldown == 0f) {
                textDisplay.color = kActiveTextColor;
            } else {
                textDisplay.color = kInactiveTextColor;
            }

            var currentScale = cooldownScaleParent.localScale;
            cooldownScaleParent.localScale = new Vector3(
                currentScale.x, newCooldown / key.CoolDown, currentScale.z
            );
            abilityCooldowns[key] = newCooldown;
        }
    }
}
