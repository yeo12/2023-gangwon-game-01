using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public float attackSpeed;
    private float attackColltime;

    public GameObject targettingBullet;

    public Transform bulletShotPos;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.inst.openUi)
        {
            return;
        }
        if (attackColltime < Time.time)
        {
            attackColltime = Time.time + attackSpeed;
            Attack();
        }
    }
    public void Attack()
    {
        StageManager.inst.CreateBullet(targettingBullet, bulletShotPos.position, bulletShotPos.rotation, GameManager.inst.damage).GetComponent<Bullet>().target = StageManager.inst.FindNearestEnemy(transform);
    }
}
