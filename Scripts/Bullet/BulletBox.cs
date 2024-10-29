using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBox : MonoBehaviour
{
    public List<Bullet> bullets = new();
    public void SetDamage(int _damage)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            bullets[i].damage = _damage;
        }
    }
}
