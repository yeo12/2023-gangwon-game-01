using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BulletType
{
    Nomal,
    Target,
    Bomb,
    Vaccine
}
public class Bullet : MonoBehaviour
{
    public BulletType bulletType;
    public bool dontDestroy;
    public int damage;
    private Rigidbody rigid;
    public float moveForce;
    public float deltime;
    public Transform target;
    public GameObject bombBullet;
    // Use this for initialization
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        deltime += Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (deltime< Time.time)
        {
            Destroy(gameObject);
        }
        if (target == null)
        {
            if (bulletType == BulletType.Target|| bulletType == BulletType.Vaccine)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (bulletType == BulletType.Vaccine)
            {
                transform.LookAt(target.position + Vector3.up);
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target.position + Vector3.up - transform.position), 100 / Vector3.Distance(transform.position, target.position) * Time.deltaTime);
            }
        }
        rigid.velocity = moveForce * StageManager.inst.stageMultifly * transform.forward;
    }
}
