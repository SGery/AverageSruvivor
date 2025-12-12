using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    Transform player;

    [Header("Damage Feedback")]
    public Color hitColor = new Color(1, 0, 0, 1);
    public float hitFlashDuration = 0.15f;
    public float deathFadeTime = 0.5f;
    Color originalColor;
    SpriteRenderer spriteRenderer;
    EnemyMovement enemyMovement;

    void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        currentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 5f, float knockbackDecay = 0.2f)
    {
        currentHealth -= dmg;
        StartCoroutine(HitFlash());

        if(knockbackForce > 0)
        {
            Vector2 knockbackDirection = (Vector2)transform.position - sourcePosition;
            enemyMovement.Knockback(knockbackDirection.normalized * knockbackForce, knockbackDecay);
        }

        if (dmg > 0)
        {
            GameManager.FloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
        }

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    IEnumerator HitFlash()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    IEnumerator DeathFade()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float elapsed = 0f;
        float alpha = spriteRenderer.color.a;
        while(elapsed < deathFadeTime)
        {
        //  Single frame delay
            yield return wait;
            elapsed += Time.deltaTime;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, (1 - elapsed / deathFadeTime) * alpha);
        }

        Destroy(gameObject);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }

    private void OnDestroy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        es.OnEnemyKilled();
    }

    public void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }
}
