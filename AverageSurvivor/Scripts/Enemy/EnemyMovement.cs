using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    EnemyStats enemy;
    Transform player;

    Vector2 knockbackVelocity;
    float knockbackDecay;

    void Start()
    {
        enemy = GetComponent<EnemyStats>();
        player = FindObjectOfType<PlayerMovement>().transform;
    }

    void Update()
    {
        if (knockbackDecay > 0)
        {
            transform.position += (Vector3)(knockbackVelocity * Time.deltaTime);
            knockbackDecay -= Time.deltaTime;
        }

        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, enemy.currentMoveSpeed * Time.deltaTime);
    }

    public void Knockback(Vector2 velocity, float decay)
    {
        if (knockbackDecay > 0) return;

        knockbackVelocity = velocity;
        knockbackDecay = decay;
    }
}
