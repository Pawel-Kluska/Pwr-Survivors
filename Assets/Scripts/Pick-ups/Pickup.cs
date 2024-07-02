using UnityEngine;

public class Pickup : MonoBehaviour
{

    public float lifespan = 0.5f;
    protected PlayerStats target;
    protected float speed;
    Vector2 initialPosition;
    float initialOffset;

    AudioManager audioManager;

    [System.Serializable]
    public struct BobbingAnimation
    {
        public float frequency;
        public Vector2 direction;
    }

    public BobbingAnimation bobbingAnimation = new BobbingAnimation
    {
        frequency = 2f,
        direction = new Vector2(0, 0.3f)
    };

    protected virtual void Start()
    {
        initialPosition = transform.position;
        initialOffset = Random.Range(0, bobbingAnimation.frequency);
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    protected virtual void Update()
    {
        if (target)
        {
            Vector2 distance = target.transform.position - transform.position;
            if (distance.sqrMagnitude > speed * speed * Time.deltaTime)
                transform.position += (Vector3)distance.normalized * speed * Time.deltaTime;
            else
                Destroy(gameObject);
        }
        else
        {
            transform.position = initialPosition + bobbingAnimation.direction * Mathf.Sin((Time.time + initialOffset) * bobbingAnimation.frequency);
        }
    }

    public virtual bool Collect(PlayerStats target, float speed, float lifespan = 0f)
    {
        if (!this.target)
        {
            if (audioManager != null && audioManager.collectSound != null)
                audioManager.PlaySFX(audioManager.collectSound);
            this.target = target;
            this.speed = speed;
            if (lifespan > 0) this.lifespan = lifespan;
            Destroy(gameObject, Mathf.Max(0.01f, this.lifespan));

            return true;
        }
        return false;
    }

    protected virtual void OnDestroy()
    {
        if (!target) return;
    }
}
