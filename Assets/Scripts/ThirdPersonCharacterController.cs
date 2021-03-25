using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour {
    public float speed;

    private Collision currentCollision = null;
    private Vector3 currentCollisionNormal = Vector3.zero;
    private float currentDistanceFromCollision = 0;

    void Update() {
        PlayerMovement();
        RestrictPlayerMovement();        
    }

    void OnCollisionEnter (Collision collision) {
        if (collision.collider.tag != "Sticky") { return; }
        
        currentCollision = collision;
        currentCollisionNormal = collision.GetContact(0).normal;
        currentDistanceFromCollision = Vector3.Distance(collision.GetContact(0).point, transform.position);
    }

    void PlayerMovement() {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(horizontal, 0f, vertical).normalized * speed * Time.deltaTime;

        transform.Translate(playerMovement, Space.Self);
    }

    void RestrictPlayerMovement() {
        if (currentCollision == null) { return; } 
        
        var closestPoint = currentCollision.collider.ClosestPoint(transform.position);
        transform.position = closestPoint + currentDistanceFromCollision * currentCollisionNormal;
    }
}
