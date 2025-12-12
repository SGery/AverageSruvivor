using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float lifespan = 0.5f;
    protected PlayerStats target;
    protected float speed;
    Vector2 startPos;
    float bobbingOffset;

    [System.Serializable]
    public struct BobbingSettings
    {
        public Vector2 direction;
        public float frequency;
    }

    public BobbingSettings bobbingSettings = new BobbingSettings
    {
        direction = new Vector2(0, 0.3f),
        frequency = 2f
    };

    [Header("Bonuses")]
    public int experience;
    public int health;

    protected virtual void Start()
    {
        startPos = transform.position;
        bobbingOffset = Random.Range(0f, bobbingSettings.frequency);
    }

    protected virtual void Update()
    {
        if (target)
        {
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)(distance.normalized * speed * Time.deltaTime);
            else
                Destroy(gameObject);
        }

        else
        {
            transform.position = startPos + bobbingSettings.direction * Mathf.Sin((Time.time + bobbingOffset
                ) * bobbingSettings.frequency);
        }
    }



    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0.0f)
    {
        if(!this.target)
        {
            this.target = target;
            this.speed = speed;
            if(lifespan > 0.0f)
                this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(this.lifespan, 0.1f));
            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!target)
        {
            return;
        }

        if(experience != 0)
        {
            target.IncreaseExperience(experience);
        }

        if(health != 0)
        {
            target.RestoreHealth(health);
        }
    }
}
