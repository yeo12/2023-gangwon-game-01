using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float spawnedTime;
    private float spawnedTimer;
    public GameObject[] enemies;
    private int index;
    private void Start()
    {
    }
    private void Update()
    {
        spawnedTimer += Time.deltaTime;
        if (spawnedTimer > spawnedTime)
        {
            spawnedTimer = 0;
            index++;
            index = Mathf.Clamp(index, 0, enemies.Length);
        }
    }
    public void SpawnEnemy()
    {
        StageManager.inst.CreateEnemy(enemies[Random.Range(0, index)],transform.position + new Vector3(Random.Range(-3,4),0, Random.Range(-3,4)),transform.rotation);
    }
}
