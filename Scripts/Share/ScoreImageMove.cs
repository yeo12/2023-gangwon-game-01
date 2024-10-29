using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreImageMove : MonoBehaviour
{
    public float speed;
    private float timer;
    RectTransform rect;
    Vector3 origin;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        origin = rect.anchoredPosition;
    }
    private void Update()
    {
        timer += Time.deltaTime * speed;
        rect.anchoredPosition = origin *(1- timer);
        if (timer >1)
        {
            Destroy(gameObject);
        }
    }
}
