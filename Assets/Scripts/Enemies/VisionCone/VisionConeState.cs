using System.Collections;
using UnityEngine;

public abstract class VisionConeState
{
    protected IVisionConeVM visionConeVM;

    protected VisionConeState(IVisionConeVM visionConeVM)
    {
        this.visionConeVM = visionConeVM;
    }
}

public class VisionConeStateIdle : VisionConeState
{
    public VisionConeStateIdle(IVisionConeVM visionConeVM) : base(visionConeVM)
    {
    }
}

public class VisionConeStatePatrolling : VisionConeState
{
    public VisionConeStatePatrolling(IVisionConeVM visionConeVM) : base(visionConeVM)
    {
    }
}

public class VisionConeStateFollowingPlayer : VisionConeState
{
    public VisionConeStateFollowingPlayer(IVisionConeVM visionConeVM) : base(visionConeVM)
    {
    }
}

public class VisionConeStateDistracted : VisionConeState
{
    public VisionConeStateDistracted(IVisionConeVM visionConeVM) : base(visionConeVM)
    {
    }
}