using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool stage1;
    public float gravity;
    public float rotateSpeed;
    public float rotationAngle;
    private bool isSprint;
    private CharacterController cc;
    public CameraMovement cm;
    public Transform mesh;
    private Animator anim;
    private float attackDelay;
    void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = PlayerController.inst.anim;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.inst.openUi)
        {
            return;
        }
        if (stage1 && Input.GetMouseButton(0))
        {
            attackDelay = Time.time + 0.4f;
        }
        if (attackDelay > Time.time)
        {
            if (anim != null) anim.SetInteger("Move", 0);
            return;
        }
        isSprint = Input.GetKey(KeyCode.LeftShift);
        float axisH = Input.GetAxisRaw("Horizontal");
        float axisV = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(axisH, 0, axisV).normalized;
        if (cm!=null)
            transform.localEulerAngles = cm.transform.localEulerAngles;
        moveDirection = transform.TransformDirection(moveDirection);
        float speed = (isSprint ? GameManager.inst.sprintSpeed : GameManager.inst.moveSpeed) + PlayerController.inst.addSpeed;
        Vector3 velocity = speed * Time.deltaTime * moveDirection;
        if (stage1)
        {
            if (moveDirection != Vector3.zero)
            {
                mesh.rotation = Quaternion.Slerp(mesh.rotation, Quaternion.LookRotation(moveDirection), rotateSpeed * Time.deltaTime);
                if (anim != null) anim.SetInteger("Move", isSprint ? 2 : 1);
            }
            else
            {
                if (anim != null) anim.SetInteger("Move", 0);
            }
            if (!cc.isGrounded)
            {
                velocity.y -= gravity * Time.deltaTime;
            }
            cc.Move(velocity);
        }
        else
        {
            mesh.rotation = Quaternion.Slerp(mesh.rotation, Quaternion.Euler(0, 0, -rotationAngle * axisH), rotateSpeed * Time.deltaTime);
            Vector3 viewPort = Camera.main.WorldToViewportPoint(transform.position + velocity);
            viewPort.x = Mathf.Clamp(viewPort.x, 0.03f, 0.97f);
            viewPort.y = Mathf.Clamp(viewPort.y, 0.1f, 0.9f);
            transform.position = Camera.main.ViewportToWorldPoint(viewPort);
        }


    }
}
