using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaSet : MonoBehaviour
{
    public float timer;
    public bool visible;
    public bool bossLog;
    public float speed = 1;
    CanvasGroup cg;
    void Start()
    {
        cg =GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bossLog)
        {
            if (timer > 1)
            {
                visible = !visible;
                    timer = 0;
            }
        }
        timer += Time.unscaledDeltaTime* speed;
        if (visible)
        {
            cg.alpha = timer;
        }
        else
        {
            cg.alpha = 1 - timer;
        }
    }
}
