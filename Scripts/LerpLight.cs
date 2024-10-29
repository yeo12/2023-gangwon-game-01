using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpLight : MonoBehaviour
{
    Light light1;
    public Color color1;
    public Color color2;
    public float timer = 120;
    void Start()
    {
        light1 = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        light1.color = Color.Lerp(color1, color2,StageManager.inst.playTime / timer);
    }
}
