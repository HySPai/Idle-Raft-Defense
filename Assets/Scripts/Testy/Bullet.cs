using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private bool useRigidbody = false;

    private float timer;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        timer = 0f;
        if (rb != null && rb.isKinematic && useRigidbody)
        {
            rb.isKinematic = false;
        }
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    public void Initialize(float speedValue, int damageValue)
    {
        speed = speedValue;
        damage = damageValue;
        if (rb != null && useRigidbody)
        {
            rb.velocity = transform.forward * speed;
        }
    }

    private void Update()
    {
        if (useRigidbody)
        {
            timer += Time.deltaTime;
            if (timer >= lifeTime) Destroy(gameObject);
            return;
        }
        transform.position += transform.forward * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= lifeTime) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.gameObject == gameObject) return;

        IHealth health = other.GetComponent<IHealth>();
        if (health == null)
        {
            health = other.GetComponentInParent<IHealth>();
        }

        if (health != null)
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Destroy(gameObject);
    }
}
