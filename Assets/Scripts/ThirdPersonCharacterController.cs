using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour {
    public float speed;
    private Collider currentCollider = null;

    void Update() {
        PlayerMovement();
    }

    void LateUpdate() {
        RestrictPlayerMovement();        
    }

    void OnTriggerEnter (Collider collider) {
        if (collider.tag != "MouseTunnel" || collider == currentCollider) { return; }
        currentCollider = collider;
    }

    void PlayerMovement() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(horizontal, 0f, vertical).normalized * speed * Time.deltaTime;

        transform.Translate(playerMovement, Space.Self);
    }

    void RestrictPlayerMovement() {
        if (currentCollider == null) { return; } 
        
        var closestPoint = currentCollider.ClosestPoint(transform.position);
        var newPosition = new Vector3(closestPoint.x, transform.position.y, closestPoint.z);
        transform.position = newPosition;
    }
}
