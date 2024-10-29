using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyType
{
    Nomal,
    Fast,
    Bomb,
    Slime,
    Random,
    stage2Nomal,
    stage2triple,
    Stage1Boss,
    Stage2Boss,
    Stage3Boss,
    stage2streightone

}
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType;
    private NavMeshAgent nav;
    public float moveSpeed;
    public bool lookAtTarget;
    public bool navMove;
    public int damage;
    public Transform target;
    public Vector3 targetPos;
    public Health health;
    public Animator anim;
    public AnimationCurve curve;
    public int dropExp;
    public int dropScore;


    private Material originMat;
    public Material changeMat;
    public MeshRenderer mesh;

    public GameObject bombBullet;
    public GameObject bombEachBullet;

    private float lostWaytimer;
    public float lostWaytime;
    public float randomRange;

    private float slimeTimer;
    public float slimeCount;

    public List<Vector3> movePoints;
    private float movetimer;
    public float movetime;
    private int moveIndex;

    public float attackDelay;
    private float attackColltime;
    public float attackSpeed;
    public GameObject bullet;
    public Transform bulletShotPos;

    public float angle;

    private bool itemGet;
    public GameObject itemBuff;

    public List<Bullet> bossBullets = new();
    void Start()
    {
        StartCoroutine(ScaleAnimation(true));
        if (mesh != null)
        {
            originMat = mesh.material;
        }

        nav = GetComponent<NavMeshAgent>();
        nav.speed = moveSpeed;

        target = PlayerController.inst.transform;
        targetPos = target.position;

        health = GetComponent<Health>();
        health.onDamage = TakeDamage;
        health.onDie = Die;

        slimeTimer = 10;
        attackColltime = Time.time + attackDelay;
        if (Random.Range(0, 5) == 0 && (enemyType != EnemyType.Stage1Boss && enemyType != EnemyType.Stage2Boss && enemyType != EnemyType.Stage3Boss))
        {
            itemGet = true;
            itemBuff.SetActive(true);
        }
    }

    void Update()
    {
        if (health.death || GameManager.inst.openUi)
        {
            if (nav != null)
            {
                nav.velocity = Vector3.zero;
            }
            return;
        }
        if (attackColltime < Time.time)
        {
            attackColltime = Time.time + attackSpeed;
            StartCoroutine(Attack());
        }
        if (enemyType == EnemyType.Stage1Boss || enemyType == EnemyType.Stage2Boss || enemyType == EnemyType.Stage3Boss)
        {
            UiManager.inst.bossHpBar.image.fillAmount = (float)health.currentHealth / health.maxHealth;
            UiManager.inst.bossHpBar.text.SetText(string.Format("{0} / {1}", health.currentHealth, health.maxHealth));
        }
        Vector3 randomVector = (new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * randomRange);
        if (enemyType == EnemyType.Fast)
        {
            nav.speed = GameManager.inst.sprintSpeed * 1.5f;
        }
        if (enemyType == EnemyType.Slime && slimeCount > 0)
        {
            slimeTimer -= Time.deltaTime;
            if (slimeTimer < 0)
            {
                for (int i = 0; i < Random.Range(2, 4); i++)
                {
                    GameObject obj = StageManager.inst.CreateEnemy(gameObject, transform.position + randomVector, transform.rotation);
                    obj.transform.localScale = transform.localScale / 2;
                    slimeCount--;
                    obj.GetComponent<Enemy>().slimeCount = slimeCount;
                }
                StageManager.inst.enemies.Remove(this);
                Destroy(gameObject);
                return;
            }
        }
        if (enemyType == EnemyType.Random)
        {
            lostWaytimer -= Time.deltaTime;
            if (lostWaytimer < 0)
            {
                lostWaytimer = lostWaytime;
                targetPos = transform.position + randomVector;
            }
            else if (lostWaytimer < (lostWaytime / 3))
            {
                targetPos = target.position;
            }
        }
        else
        {
            targetPos = target.position;
        }
        if (navMove)
        {
            NavMove(targetPos);
        }
        else
        {
            TransformMove();
        }
    }
    public IEnumerator ChangedMat()
    {
        mesh.material = changeMat;
        yield return new WaitForSeconds(0.05f);
        mesh.material = originMat;
    }
    public void TakeDamage()
    {
        if (anim != null) anim.SetTrigger("DoHit");
        if (mesh != null)
        {
            StartCoroutine(ChangedMat());
        }
    }
    public void Die()
    {
        if (itemGet)
        {
            Instantiate(GameManager.inst.items[Random.Range(0, GameManager.inst.items.Length)], transform.position + Vector3.up, Quaternion.identity);
        }
        StageManager.inst.CreateScoreImage(Camera.main.WorldToScreenPoint(transform.position));
        Instantiate(GameManager.inst.dieParticle, transform.position + Vector3.up, Quaternion.identity);
        if (StageManager.inst.enemyDeathCount.ContainsKey(enemyType))
        {
            StageManager.inst.enemyDeathCount[enemyType]++;
        }
        else
        {
            StageManager.inst.enemyDeathCount.Add(enemyType, 1);
        }
        StageManager.inst.enemies.Remove(this);
        StageManager.inst.enemyScore += dropScore;
        GameManager.inst.AddExperience(dropExp);
        GameManager.inst.AddScore(dropScore, 1);
        if (enemyType == EnemyType.Bomb)
        {
            Bomb();
        }
        else if (enemyType == EnemyType.Stage1Boss || enemyType == EnemyType.Stage2Boss || enemyType == EnemyType.Stage3Boss)
        {
            StageManager.inst.StageClear(true);
        }
        if (anim != null) anim.SetTrigger("DoDie");
        Destroy(gameObject);
    }
    public void Bomb()
    {
        StageManager.inst.CreateBullet(bombBullet, transform.position, transform.rotation, damage);
        for (int i = 0; i < Random.Range(2, 4); i++)
        {
            StageManager.inst.CreateBullet(bombEachBullet, transform.position, transform.rotation, damage).GetComponent<Rigidbody>().AddForce(
                new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * randomRange, ForceMode.Impulse); ;
        }
    }
    public IEnumerator ScaleAnimation(bool _sizeUP)
    {
        Vector3 start = transform.localScale;
        for (float i = 0; i < 1; i += Time.deltaTime * 2)
        {
            if (_sizeUP)
            {
                transform.localScale = (Vector3.one * 0.1f) + start * (1 - curve.Evaluate(i));
            }
            else
            {
                transform.localScale = start * curve.Evaluate(i);
            }
            yield return null;
        }
    }
    public IEnumerator Attack()
    {
        if (enemyType == EnemyType.stage2Nomal)
        {
            StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
        }
        else if (enemyType == EnemyType.stage2streightone)
        {
            for (int i = 0; i < 3; i++)
            {
                bulletShotPos.localEulerAngles = new Vector3(0, -angle + angle * i, 0);
                StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 5; i++)
            {
                bulletShotPos.localEulerAngles = new Vector3(0, -angle * 2 + angle * i, 0);
                StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < 3; i++)
            {
                bulletShotPos.localEulerAngles = new Vector3(0, -angle + angle * i, 0);
                StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
            }
        }
        else if (enemyType == EnemyType.stage2triple)
        {
            for (int i = 0; i < 3; i++)
            {
                bulletShotPos.localEulerAngles = new Vector3(0, -angle + angle * i, 0);
                StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
            }
        }
        else if (enemyType == EnemyType.Stage2Boss)
        {
            int shotAmount = 8;
            float angle = 360 / shotAmount;
            switch (Random.Range(0, 3))
            {
                case 0:
                    for (int i = 0; i < shotAmount; i++)
                    {
                        bulletShotPos.rotation = Quaternion.Euler(0, angle * i, 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    yield return new WaitForSeconds(0.2f);
                    for (int i = 0; i < shotAmount; i++)
                    {
                        bulletShotPos.rotation = Quaternion.Euler(0, angle * i + (angle / 2), 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    break;
                case 1:
                    angle = 20;
                    for (int i = 0; i < 7; i++)
                    {
                        bulletShotPos.localEulerAngles = new Vector3(0, -angle * 3 + angle * i, 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    yield return new WaitForSeconds(0.2f);
                    for (int i = 0; i < 5; i++)
                    {
                        bulletShotPos.localEulerAngles = new Vector3(0, -angle * 2 + angle * i, 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    yield return new WaitForSeconds(0.2f);
                    for (int i = 0; i < 3; i++)
                    {
                        bulletShotPos.localEulerAngles = new Vector3(0, -angle + angle * i, 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    break;
                case 2:
                    angle = 20;
                    for (int j = 0; j < 3; j++)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            bulletShotPos.localEulerAngles = new Vector3(0, angle * i, 0);
                            StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                            yield return new WaitForSeconds(0.1f);
                        }
                        angle *= -1;
                    }
                    break;
            }
        }
        else if (enemyType == EnemyType.Stage3Boss)
        {
            int shotAmount = 16;
            float angle = 360 / shotAmount;
            switch (Random.Range(0, 4))
            {
                case 0:
                    for (int i = 0; i < shotAmount; i++)
                    {
                        bulletShotPos.rotation = Quaternion.Euler(0, angle * i, 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    yield return new WaitForSeconds(0.2f);
                    for (int i = 0; i < shotAmount; i++)
                    {
                        bulletShotPos.rotation = Quaternion.Euler(0, angle * i + (angle / 2), 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    yield return new WaitForSeconds(0.2f);
                    for (int i = 0; i < shotAmount; i++)
                    {
                        bulletShotPos.rotation = Quaternion.Euler(0, angle * i, 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    yield return new WaitForSeconds(0.2f);
                    for (int i = 0; i < shotAmount; i++)
                    {
                        bulletShotPos.rotation = Quaternion.Euler(0, angle * i + (angle / 2), 0);
                        StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                    }
                    break;
                case 1:
                    angle = 20;
                    for (int j = 0; j < 3; j++)
                    {

                        for (int i = 0; i < 7; i++)
                        {
                            bulletShotPos.localEulerAngles = new Vector3(0, -angle * 3 + angle * i, 0);
                            StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                        }
                    yield return new WaitForSeconds(0.2f);
                    }
                    for (int j = 0; j < 3; j++)
                    {

                        for (int i = 0; i < 5; i++)
                        {
                            bulletShotPos.localEulerAngles = new Vector3(0, -angle * 2 + angle * i, 0);
                            StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                        }
                    yield return new WaitForSeconds(0.2f);
                    }
                    for (int j = 0; j < 3; j++)
                    {

                        for (int i = 0; i < 3; i++)
                        {
                            bulletShotPos.localEulerAngles = new Vector3(0, -angle + angle * i, 0);
                            StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                        }
                    yield return new WaitForSeconds(0.2f);
                    }
                    break;
                case 2:
                    angle = 10;
                    for (int j = 0; j < 30; j++)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            bulletShotPos.localEulerAngles = new Vector3(0, -angle * 2 + angle * i, 0);
                            StageManager.inst.CreateBullet(bullet, bulletShotPos.position, bulletShotPos.rotation, damage);
                        }
                        yield return new WaitForSeconds(0.1f);
                    }
                    break;
                case 3:
                    bulletShotPos.rotation = Quaternion.Euler(0,-90,0);
                    for (int j = 0; j < 2; j++)
                    {
                        for (int i = 0; i < bossBullets.Count; i++)
                        {
                            StageManager.inst.CreateBullet(bossBullets[i].gameObject, bulletShotPos.position, bulletShotPos.rotation, damage);
                            yield return new WaitForSeconds(0.5f);
                        }
                    }
                    bulletShotPos.rotation = Quaternion.identity;
                    break;
            }
        }
    }
    public void TransformMove()
    {
        if (movePoints.Count <= 0)
        {
            return;
        }
        movetimer += Time.deltaTime;
        if (movetimer >= movetime)
        {
            movetimer = 0;
            moveIndex++;
            if (moveIndex >= movePoints.Count)
            {
                moveIndex = 0;
            }
        }

        transform.position += moveSpeed * Time.deltaTime * movePoints[moveIndex].normalized;
    }
    public void NavMove(Vector3 _pos)
    {
        if (StageManager.inst.enemyStopTime > Time.time)
        {
            if (anim != null) anim.SetBool("Move", false);
            nav.velocity = Vector3.zero;
            return;
        }
        if (anim != null) anim.SetBool("Move", true);
        transform.LookAt(new Vector3(_pos.x, transform.position.y, _pos.z));
        NavMeshPath path = new();
        nav.CalculatePath(_pos, path);
        nav.SetPath(path);
    }
}
