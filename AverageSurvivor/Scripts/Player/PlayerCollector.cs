using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerCollector : MonoBehaviour
{
    PlayerStats palyer;
    CircleCollider2D colliderDetector;
    public float pullSpeed;

    void Start()
    {
        // GetComponentInParent for multiplayer purposes
        palyer = FindObjectOfType<PlayerStats>();
    }

    public void SetRadius(float radius)
    {
        if(!colliderDetector)
            colliderDetector = GetComponent<CircleCollider2D>();  // NullReferenceException guard
        colliderDetector.radius = radius;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.TryGetComponent(out Pickup collectible))
        {
            collectible.Collect(palyer, pullSpeed);
        }   
    }
}
