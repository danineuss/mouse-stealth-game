using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneEvents : MonoBehaviour
{
    public event Action OnPlayerEnterDoor;
    public event Action OnPlayerReachedSceneEnd;

    public void PlayerEnterDoor() {
        if (OnPlayerEnterDoor == null) {
            return;
        }
        OnPlayerEnterDoor();
    }

    public void PlayerReachedSceneEnd() {
        if (OnPlayerReachedSceneEnd == null) {
            return;
        }
        OnPlayerReachedSceneEnd();
    }
}
