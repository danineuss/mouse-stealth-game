using System;
using UnityEngine;

public interface IEnemyVM: IIdentifiable
{
    bool GetDistracted();
    void PlaySound(Sound sound);
}

public class EnemyVM : IEnemyVM
{
    public Guid ID => id;

    private IPlayerDetector playerDetector;
    private AudioVM audioVM;
    private IEnemyIO enemyIO;
    private SoundEmitter soundEmitter;
    private IPlayerEvents playerEvents;
    private IEnemyEvents enemyEvents;
    private Guid id;

    public bool GetDistracted()
    {
        if (playerDetector.DetectorStateEnum != DetectorStateEnum.Idle)
        {
            return false;
        }

        playerDetector.SetStateDistracted();
        enemyIO.SetTextColor(DetectorStateEnum.Distracted);
        return true;
    }

    public void PlaySound(Sound sound)
    {
        soundEmitter.PlaySound(sound);
    }

    public EnemyVM(
        IPlayerDetector playerDetector,
        AudioVM audioVM,
        IEnemyIO enemyIO,
        SoundEmitter soundEmitter,
        IPlayerEvents playerEvents,
        IEnemyEvents enemyEvents)
    {
        this.playerDetector = playerDetector;
        this.audioVM = audioVM;
        this.enemyIO = enemyIO;
        this.soundEmitter = soundEmitter;
        this.playerEvents = playerEvents;
        this.enemyEvents = enemyEvents;
        
        id = Guid.NewGuid();
        this.enemyIO.SetEnemyID(id);

        InitializeEvents();
    }

    void InitializeEvents()
    {
        enemyEvents.OnCursorEnterEnemy += OnCursorEnterEnemy;
        enemyEvents.OnCurserExitEnemy += OnCurserExitEnemy;
        enemyEvents.OnDetectorStateChanged += OnDetectorStateChanged;

        playerEvents.OnSendPlayerLocation += OnReceivePlayerLocation;
        playerEvents.OnRemovePlayerLocation += OnRemovePlayerLocation;
        playerEvents.OnAbilityExecuted += OnPlayerAbilityExecuted;
    }

    void OnCursorEnterEnemy(Guid enemyID)
    {
        if (enemyID != this.ID)
            return;

        enemyIO.SetDisplayVisibility(true);
    }

    void OnCurserExitEnemy()
    {
        enemyIO.SetDisplayVisibility(false);
    }

    void OnDetectorStateChanged(PlayerDetector playerDetector)
    {
        if (playerDetector != this.playerDetector)
            return;

        enemyIO.SetTextColor(playerDetector.DetectorStateEnum);
        audioVM.PlaySoundAtEnemy(this, playerDetector.DetectorStateEnum);

        if (playerDetector.DetectorStateEnum == DetectorStateEnum.Alarmed)
            enemyEvents.FailGame();
    }

    void OnReceivePlayerLocation(
        Guid enemyID, bool shouldDisplayText, Transform playerTransform = null)
    {
        if (enemyID != this.ID)
            return;

        enemyIO.SetTextFollowingPlayer(shouldDisplayText, playerTransform);
    }

    void OnRemovePlayerLocation(Guid enemyID)
    {
        if (enemyID != this.ID)
            return;

        enemyIO.SetTextFollowingPlayer(false);
    }

    void OnPlayerAbilityExecuted(IPlayerAbility ability)
    {
        ability.Execute(this);
        enemyIO.UpdateCooldownForAbility(ability);
    }
}
