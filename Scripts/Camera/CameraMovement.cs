using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public Transform cam;
    public LayerMask layer;
    public Vector3 offset;
    public float sensitivity;
    private float scroll;
    public float scrollSpeed;

    private void Update()
    {
        if (GameManager.inst.openUi)
        {
            return;
        }
        transform.localEulerAngles += new Vector3(0, Input.GetAxisRaw("Mouse X"), 0) * sensitivity;
        scroll -= Input.GetAxisRaw("Mouse ScrollWheel") * scrollSpeed;
        scroll = Mathf.Clamp(scroll, 0.5f, 1);
        Vector3 dis = offset * scroll;
        if (Physics.Raycast(transform.position, cam.position - transform.position, out RaycastHit hit, Vector3.Distance(transform.position, cam.position)+0.1f, layer))
        {
            cam.position = hit.point;
        }
        else
        {
            cam.localPosition = dis;
        }
        transform.position = target.position + Vector3.up * 1.5f;
    }
}
