using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Turret")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int bulletDamage = 10;

    private float fireTimer;

    private void Awake()
    {
        if (firePoint == null) firePoint = transform;
        fireTimer = 0f;
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;
        if (fireTimer >= 1f / Mathf.Max(0.0001f, fireRate))
        {
            Fire();
            fireTimer = 0f;
        }
    }

    private void Fire()
    {
        if (bulletPrefab == null) return;
        GameObject go = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet b = go.GetComponent<Bullet>();
        if (b != null)
        {
            b.Initialize(bulletSpeed, bulletDamage);
        }
        else
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = firePoint.forward * bulletSpeed;
            }
        }
    }
}
