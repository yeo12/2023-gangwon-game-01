using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Speed,
    Shield,
    Heal,
    Stop,
    Exp
}
public class Item : MonoBehaviour
{
    public ItemType itemType;
}
