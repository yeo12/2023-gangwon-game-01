using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    public float attackSpeed;
    private float attackColltime;
    public float autoAttackSpeed;
    private float autoAttackColltime;

    public List<GameObject> nomalBullets;
    public GameObject targettingBullet;
    public GameObject BombBullet;

    public Transform bulletShotPos;
    public Transform autoShotPos;
    void Start()
    {
        anim = PlayerController.inst.anim;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.inst.openUi)
        {
            return;
        }
        if (Input.GetMouseButton(0) && attackColltime < Time.time)
        {
            attackColltime = Time.time + attackSpeed;
            if (anim != null) anim.SetTrigger("DoAttack");
            Attack();
        }
        if (autoAttackColltime < Time.time)
        {
            StartCoroutine(AutoAttack());
            autoAttackColltime = Time.time + autoAttackSpeed;
        }
    }
    public void Attack()
    {
        Instantiate(GameManager.inst.attack, Vector3.zero, Quaternion.identity);
        GameManager.inst.PlayerAbilities.TryGetValue(Ability.NomalAttack, out int nember);
        StageManager.inst.CreateBullet(nomalBullets[nember], bulletShotPos.position, bulletShotPos.rotation, GameManager.inst.damage);
    }
    public IEnumerator AutoAttack()
    {
        int count;
        if (GameManager.inst.PlayerAbilities.TryGetValue(Ability.BombAttack, out count))
        {
            for (int i = 0; i < count; i++)
            {
                Transform target = StageManager.inst.FindNearestEnemy(transform);
                if (target!=null)
                {
                    autoShotPos.LookAt(target.position + Vector3.up);
                }
                StageManager.inst.CreateBullet(BombBullet, autoShotPos.position, autoShotPos.rotation, GameManager.inst.damage);
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield return new WaitForSeconds(0.1f);

        if (GameManager.inst.PlayerAbilities.TryGetValue(Ability.GuidedAttack, out count))
        {
            for (int i = 0; i < count; i++)
            {
                StageManager.inst.CreateBullet(targettingBullet, autoShotPos.position, autoShotPos.rotation, GameManager.inst.damage).GetComponent<Bullet>().target = StageManager.inst.FindNearestEnemy(transform);

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
