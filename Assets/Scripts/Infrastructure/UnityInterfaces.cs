using UnityEngine;

public interface IUpdatable
{
    void Update();
}

public interface ILateUpdatable
{
    void LateUpdate();
}

public interface ITriggerEnterable
{
    void OnTriggerEnter(Collider collider);
}