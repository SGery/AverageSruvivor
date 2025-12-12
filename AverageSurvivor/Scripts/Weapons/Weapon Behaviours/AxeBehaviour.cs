using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeBehaviour : ProjectileWeaponBehaviour
{
    [Header("Axe Settings")]
    public float gravity = -9.8f;
    public float spinSpeed = 720f;
    public float initialThrowAngle = 45f;

    Vector2 velocity;

    protected override void Start()
    {
        base.Start();

        float sign = Mathf.Sign(direction.x);
        if (sign == 0f) sign = 1f;

        float rad = initialThrowAngle * Mathf.Deg2Rad;
        Vector2 throwDir = new Vector2(sign * Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

        velocity = throwDir * currentSpeed;

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        velocity.y += gravity * Time.deltaTime;
        transform.position += (Vector3)(velocity * Time.deltaTime);

        float sign = Mathf.Sign(velocity.x);
        if (sign == 0f) sign = 1f;
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime * -sign);
    }
}
