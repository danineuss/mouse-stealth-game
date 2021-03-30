using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoamingArea : MonoBehaviour
{
    public GameObject Player;
    public bool IsPlayerContained;
    private Bounds bounds;
    private float distanceEpsilon = 0.005f;
    
    void Start() {
        bounds = GetComponent<Collider>().bounds;
    }

    void Update() {
        IsPlayerContained = bounds.SqrDistance(Player.transform.position) < distanceEpsilon;
        var playerPosition = Player.transform.position;
        var posX = Mathf.Clamp(playerPosition.x, bounds.min.x, bounds.max.x);
        var posZ = Mathf.Clamp(playerPosition.z, bounds.min.z, bounds.max.z);
        Player.transform.position = new Vector3(posX, playerPosition.y, posZ);
    }
}
