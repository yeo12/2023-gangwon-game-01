using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController inst;
    public Health health;

    public Animator anim;

    public float shieldTime;
    public float speedTime;
    public GameObject vaccineBullet;

    public int currentMana;
    public int maxMana;
    public Transform droneRoot;
    public float addSpeed;

    public GameObject shield;
    public GameObject speedParticle;
    public Transform speedtrain;

    public AlphaSet hit;
    void Awake()
    {
        inst = this;
    }
    private void Start()
    {
        health = GetComponent<Health>();
        health.onDamage = TakeDamage;
        health.onHeal = Heal;
        health.onDie = Die;
        StartCoroutine(ManaRecovery());
    }
    private void Update()
    {
        if (GameManager.inst.openUi)
        {
            return;
        }
        if (speedParticle.activeSelf != speedTime > Time.time)
        {
            speedParticle.SetActive(speedTime > Time.time);
        }
        if (speedtrain != null)
        {
            speedtrain.localScale = Vector3.one * (0.2f * GameManager.inst.speed);
        }
        if (shield.activeSelf != shieldTime > Time.time)
        {

            shield.SetActive(shieldTime > Time.time);
        }
        addSpeed = (GameManager.inst.speed * 2) + (speedTime > Time.time ? 3 : 0);
        droneRoot.Rotate(Vector3.up, 20 * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSkill(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseSkill(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseSkill(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseSkill(4);
        }
    }
    private IEnumerator ManaRecovery()
    {
        while (true)
        {
            UpdateMana(1);
            yield return new WaitForSeconds(1f);
        }
    }
    public void UpdateMana(int _amount)
    {
        currentMana += _amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
    }
    public void UseSkill(int _skill)
    {
        if (currentMana < GameManager.inst.skillManaCoast[_skill - 1])
        {
            StageManager.inst.CreateNoticeMessage(2, "스킬 사용이 불가능 합니다.");
            return;
        }
        else
        {
            UpdateMana(-GameManager.inst.skillManaCoast[_skill - 1]);
            GameManager.inst.PlayerAbilities.TryGetValue((Ability)(_skill - 1), out int level);
            switch (_skill)
            {
                case 1:
                    VaccineSkill(10 + 5 * level);
                    break;
                case 2:
                    shieldTime = Time.time + 10 + 5 * level;
                    break;
                case 3:
                    speedTime = Time.time + 10 + 5 * level;
                    break;
                case 4:
                    health.Heal(10 + 5 * level);
                    break;
            }
        }
    }
    public void VaccineSkill(int _damage)
    {
        for (int i = 0; i < StageManager.inst.enemies.Count; i++)
        {
            Vector3 viewPort = Camera.main.WorldToViewportPoint(StageManager.inst.enemies[i].transform.position);
            if (viewPort.x >= 0 && viewPort.x <= 1 && viewPort.y >= 0 && viewPort.y <= 1 && viewPort.z > 1)
            {
                GameObject bullet = StageManager.inst.CreateBullet(vaccineBullet, transform.position + Vector3.up, Quaternion.Euler(-90, 0, 0), _damage);
                bullet.GetComponent<Bullet>().target = StageManager.inst.enemies[i].transform;
            }
        }
    }
    public void Heal()
    {

    }
    public void TakeDamage()
    {
        hit.timer = 0;
        Instantiate(GameManager.inst.hitParticle, transform.position + Vector3.up, Quaternion.identity);
        if (anim != null) anim.SetTrigger("DoHit");
    }
    public void Die()
    {
        Instantiate(GameManager.inst.dieParticle, transform.position, Quaternion.identity);
        if (anim != null) anim.SetTrigger("DoDie");
        StageManager.inst.StageClear(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>() != null)
        {
            if (shieldTime > Time.time || StageManager.inst.invincibility)
            {
                return;
            }
            int damage = other.GetComponent<Enemy>().damage;
            UpdateMana(damage);
            health.TakeDamage(damage);
        }
        if (other.GetComponent<Item>() != null)
        {
            StageManager.inst.itemScore += 100;
            GameManager.inst.AddScore(100, 0);

            Item item = other.GetComponent<Item>();
            if (StageManager.inst.itemGetCount.ContainsKey(item.itemType))
            {
                StageManager.inst.itemGetCount[item.itemType]++;
            }
            else
            {
                StageManager.inst.itemGetCount.Add(item.itemType, 1);
            }
            switch (item.itemType)
            {
                case ItemType.Speed:
                    StageManager.inst.CreateNoticeMessage(2, string.Format("스피드 아이템을 획득했습니다."));
                    if (GameManager.inst.speed < 5)
                    {
                        GameManager.inst.speed++;
                    }
                    break;
                case ItemType.Shield:
                    StageManager.inst.CreateNoticeMessage(2, string.Format("쉴드 아이템을 획득했습니다."));
                    shieldTime = Time.time + 10;
                    break;
                case ItemType.Heal:
                    StageManager.inst.CreateNoticeMessage(2, string.Format("회복 아이템을 획득했습니다."));
                    health.Heal(10);
                    UpdateMana(10);
                    break;
                case ItemType.Stop:
                    StageManager.inst.CreateNoticeMessage(2, string.Format("적 멈춤 아이템을 획득했습니다."));
                    StageManager.inst.enemyStopTime = Time.time + 5;
                    break;
                case ItemType.Exp:
                    StageManager.inst.CreateNoticeMessage(2, string.Format("경험치 아이템을 획득했습니다."));
                    GameManager.inst.AddExperience(10);
                    break;
            }
            Instantiate(GameManager.inst.getParticle, transform.position + Vector3.up, Quaternion.identity);
            Destroy(other.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {

    }
}
