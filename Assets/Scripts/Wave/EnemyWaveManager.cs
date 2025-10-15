using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWaveManager : SingletonBehaviour<EnemyWaveManager>
{
    [Header("Spawn Setup")]
    [SerializeField] private Transform spawnCenter;
    [SerializeField] private Transform target;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 5;
    [SerializeField] private float spawnRadius = 5f;

    [Header("Wave Events")]
    public UnityEvent OnWaveCompleted;

    private readonly List<EnemyController> aliveEnemies = new List<EnemyController>();

    protected override void Awake()
    {
        MakeSingleton(false);
    }

    private void Start()
    {
        SpawnWave();
    }

    public void SpawnWave()
    {
        ClearList();

        for (int i = 0; i < enemyCount; i++)
        {
            float angle = Random.Range(-180f, 180f);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 dir = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
            Vector3 pos = spawnCenter.position + dir * Random.Range(spawnRadius * 0.5f, spawnRadius);
            pos.y = 0;
            GameObject enemyObj = Instantiate(enemyPrefab, pos, Quaternion.identity);
            EnemyController enemy = enemyObj.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemyObj.transform.LookAt(target);
                SetEnemyTarget(enemy);
                aliveEnemies.Add(enemy);
            }
        }
    }

    private void SetEnemyTarget(EnemyController enemy)
    {
        var field = typeof(EnemyController).GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null) field.SetValue(enemy, target);

        enemy.GetComponent<HealthController>().OnDead += () => HandleEnemyDeath(enemy);
    }

    private void HandleEnemyDeath(EnemyController enemy)
    {
        if (aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
            if (aliveEnemies.Count == 0)
            {
                OnWaveCompleted?.Invoke();
            }
        }
    }

    private void ClearList()
    {
        aliveEnemies.Clear();
    }
}
