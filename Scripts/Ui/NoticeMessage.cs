using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NoticeMessage : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float timer;
    private void Awake()
    {
        timer += Time.time;
    }
    private void Update()
    {
        if (GameManager.inst.openUi)
        {
            return;
        }
        if (timer < Time.time)
        {
            Destroy(gameObject);
        }
    }
}
