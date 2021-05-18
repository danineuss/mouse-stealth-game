using System.Collections;
using UnityEngine;

public abstract class DetectorState
{
    protected IPlayerDetector playerDetector;
    protected EventsMono eventsMono;

    protected DetectorState(IPlayerDetector playerDetector, EventsMono eventsMono) 
    {
        this.playerDetector = playerDetector;
        this.eventsMono = eventsMono;
    }

    public abstract bool AttemptDistraction(IVisionConeVM visionConeVM);
}

public class DetectorStateIdle: DetectorState
{
    private const float DistractionDuration = 5f;
    private float timeOfLastDistraction;

    public DetectorStateIdle(IPlayerDetector playerDetector, EventsMono eventsMono) 
        : base(playerDetector, eventsMono) 
    {
    }

    public override bool AttemptDistraction(IVisionConeVM visionConeVM)
    {
        timeOfLastDistraction = Time.time;
        visionConeVM.SetStateDistracted(true);
        playerDetector.TransitionTo(new DetectorStateDistracted(playerDetector, eventsMono));
        
        eventsMono.StartCoroutine(ResetDistraction(visionConeVM));
        return true;
    }

    IEnumerator ResetDistraction(IVisionConeVM visionConeVM)
    {
        while (Time.time - timeOfLastDistraction < DistractionDuration)
            yield return null;

        visionConeVM.SetStateDistracted(false);
        playerDetector.TransitionTo(new DetectorStateIdle(playerDetector, eventsMono));
    }

}

public class DetectorStateDistracted : DetectorState
{
    public DetectorStateDistracted(IPlayerDetector playerDetector, EventsMono eventsMono) 
        : base(playerDetector, eventsMono)
    {
    }
    
    public override bool AttemptDistraction(IVisionConeVM visionConeVM)
    {
        return false;
    }
}