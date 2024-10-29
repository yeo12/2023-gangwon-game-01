using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{

    public int currentHealth;
    public int maxHealth;
    public bool death;
    public UnityAction onDie;
    public UnityAction onDamage;
    public UnityAction onHeal;
    public List<string> triggerTag;
    void Start()
    {
        currentHealth = maxHealth;
    }
    public void Heal(int _amount)
    {
        currentHealth += _amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHeal?.Invoke();
    }
    public bool TakeDamage(int _damage)
    {
        if (death)
        {
            return true;
        }
        currentHealth -= _damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (currentHealth <= 0)
        {
            Die();
            return true;
        }
        onDamage?.Invoke();
        return false;
    }
    public void Die()
    {
        if (death)
        {
            return;
        }
        death = true;
        onDie?.Invoke();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (triggerTag.Contains(other.tag))
        {

            Bullet bullet = other.GetComponent<Bullet>();
            Vector3 viewPort = Camera.main.WorldToViewportPoint(transform.position);
            if (!(viewPort.x >= 0 && viewPort.x <= 1 && viewPort.y >= 0 && viewPort.y <= 1 && viewPort.z > 1))
            {
                if (!bullet.dontDestroy)
                {
                    Destroy(other.gameObject);
                }
                return;
            }
            if (death)
            {
                return;
            }
            if (bullet.target != null && bullet.target != transform)
            {
                return;
            }
            if (GetComponent<PlayerController>()!=null &&(PlayerController.inst.shieldTime > Time.time || StageManager.inst.invincibility))
            {
                return;
            }
            TakeDamage(bullet.damage);
            Instantiate(GameManager.inst.hitParticle, other.transform.position, Quaternion.identity);
            if (bullet.bulletType == BulletType.Bomb)
            {
                StageManager.inst.CreateBullet(bullet.bombBullet, other.transform.position, other.transform.rotation, bullet.damage);
            }
            if (!bullet.dontDestroy)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
