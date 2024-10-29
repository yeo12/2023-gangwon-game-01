using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main.transform.position.z > transform.position.z && Camera.main.transform.position.z - transform.position.z > 30)
        {
            transform.position += new Vector3(0, 0, 50);
        }       
    }
}
