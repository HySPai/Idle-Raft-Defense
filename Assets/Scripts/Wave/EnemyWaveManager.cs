using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyEntry
{
    public GameObject prefab;
    public int count = 1;
    public float spawnInterval = 0.2f;
}

[Serializable]
public class Wave
{
    public string waveName;
    public List<EnemyEntry> enemies = new List<EnemyEntry>();
}

public class EnemyWaveManager : MonoBehaviour
{
    public Transform spawnPoint;
    public float spawnRadius = 3f;
    public List<Wave> waves = new List<Wave>();
    public UnityEvent OnWaveCompleted;
    public UnityEvent OnAllWavesCompleted;

    private int currentWaveIndex = -1;
    private int aliveCount = 0;
    private bool isWaveRunning;
    private Coroutine spawnRoutine;

    private void Start()
    {
        if (waves.Count > 0)
        {
            StartWave(0);
        }
    }

    public void StartWave(int index)
    {
        if (isWaveRunning) return;
        if (index < 0 || index >= waves.Count) return;
        currentWaveIndex = index;
        spawnRoutine = StartCoroutine(SpawnWave(waves[index]));
    }

    public void StartNextWave()
    {
        int next = currentWaveIndex + 1;
        if (next < waves.Count)
        {
            StartWave(next);
        }
        else
        {
            OnAllWavesCompleted?.Invoke();
        }
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        isWaveRunning = true;
        aliveCount = 0;

        foreach (var entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.prefab);
                yield return new WaitForSeconds(entry.spawnInterval);
            }
        }

        while (aliveCount > 0)
        {
            yield return null;
        }

        isWaveRunning = false;
        OnWaveCompleted?.Invoke();
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null || spawnPoint == null) return;
        Vector2 offset = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(0f, spawnRadius);
        Vector3 spawnPos = spawnPoint.position + new Vector3(offset.x, 0, offset.y);
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.SetTarget(spawnPoint);
        }

        IHealth health = enemy.GetComponent<IHealth>();
        aliveCount++;
        if (health != null)
        {
            Action handler = null;
            handler = () =>
            {
                if (health != null) health.OnDead -= handler;
                aliveCount = Mathf.Max(0, aliveCount - 1);
            };
            health.OnDead += handler;
        }
    }

    private void OnDrawGizmos()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
        }
    }
}
