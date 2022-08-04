using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerMovement : MonoBehaviour
{
    [Range(1f, 10f)]
    public float mouseSensitive = 3;
    [Range(-180f, 180f)]
    public float minCameraRotY = -60f;
    [Range(-180f, 180f)]
    public float maxCameraRotY = 40f;
    private Transform Hcamera;
    private float rotationY = 20f;

    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float crouchSpeed = 2f;
    private float speed;
    public float jumpPower = 3f;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 movement = Vector3.zero;
    public Animator animator;

    public Transform cameraPack;
    public Rig RHandRig;
    public Rig WeaponRig; 
    public Rig LHandRig;

    private float gravity = Physics.gravity.y;
    private Vector3 oldPos;
    private bool crouch = true;
    private bool freezMovement = false;
    private float radius;
    private float height;
    private bool isReload = false;
    private bool jumpOver = false;
    public Gun usingGun;


    void Awake(){
        controller = GetComponent<CharacterController>();
        Hcamera = cameraPack.GetChild(0);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        radius = controller.radius;
        height = controller.height;
    }
    void Start(){
        speed = walkSpeed;
        oldPos = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    void LateUpdate(){
        if (!freezMovement){
            Movement();
            AnimatorSystem();
            if (!isReload) {
                if (Input.GetMouseButton(0) && usingGun.CheckAmo() && !isReload) {
                    usingGun.Shoot();
                }
            }
        }
        GravitySystem();
        CameraPack();

        // float moveScale = new Vector3(animator.GetFloat("MoveX"), 0f, animator.GetFloat("MoveZ")).magnitude;
        //controller.Move(new Vector3(moveDirection.x * moveScale, gravity, moveDirection.z * moveScale) * Time.deltaTime);
        controller.Move(new Vector3(moveDirection.x, gravity, moveDirection.z) * Time.deltaTime);

    }


    void Movement() {

        if (controller.isGrounded)
        {
            movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")).normalized;
            moveDirection = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * movement * speed;
        }
        transform.Rotate(0f, Input.GetAxis("Mouse X") * mouseSensitive * 100f * Time.deltaTime, 0f);


        if (Input.GetMouseButton(1)) {
            crouch = true;
        }
        else { 
            if (Input.GetKeyDown(KeyCode.C))
            {
                crouch = !crouch;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && movement == Vector3.zero){
            speed = Mathf.Lerp(speed, runSpeed, Time.deltaTime * 5f);
            moveDirection = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * Vector3.forward * speed;
        }
        else if(movement == Vector3.zero){
            speed = Mathf.Lerp(speed, 0f, Time.deltaTime * 5f);
        }
        else{
            if (crouch){
                speed = Mathf.Lerp(speed, crouchSpeed, Time.deltaTime * 5f);
            }
            else{
                speed = Mathf.Lerp(speed, walkSpeed, Time.deltaTime * 5f);
            }

        }

        if (!isReload)
        {
            if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.LeftShift) && controller.isGrounded)
            {
                StartCoroutine(FlipForward(1.633f / 2f));
            }

            if (Input.GetKeyDown(KeyCode.F) && !Input.GetKeyDown(KeyCode.LeftShift) && controller.isGrounded)
            {
                StartCoroutine(Kick(1.2f / 1.5f));
            }
        }
    }

    void GravitySystem() {
        if (controller.isGrounded)
        {
            gravity = Physics.gravity.y;

            if (Input.GetButton("Jump"))
            {
                RaycastHit hit;
                if(Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), transform.forward, out hit, 1.5f) && !isReload)
                {
                    if (hit.transform.tag == "JumpOver")
                    {
                        gravity = jumpPower / 2f;
                        StartCoroutine(JumpOver(0.5f));
                    }
                    else {
                        gravity = jumpPower;
                    }
                }
                else
                {
                    if (!jumpOver)
                    {
                        gravity = jumpPower;
                    }
                }
            }
        }
        else
        {
            gravity += Physics.gravity.y * Time.deltaTime;
        }
    }

    void CameraPack() {
        cameraPack.position = Vector3.Lerp(cameraPack.position, transform.position, Time.deltaTime * 20f);
        cameraPack.rotation = transform.rotation;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitive * 100f * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, minCameraRotY, maxCameraRotY);
        Hcamera.rotation = Quaternion.Euler(rotationY, Hcamera.eulerAngles.y, Hcamera.eulerAngles.z);

    }

    void AnimatorSystem()
    {

        Vector3 animMove = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        if (crouch)
        {
            animMove = new Vector3(animMove.x / 2f, 0f, animMove.z / 2f);
        }
        float velocity = (oldPos - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude * 100f;
        if (velocity < 0.5f)
        {
            animator.SetBool("Sprint", false);
        }
        else
        {
            animator.SetBool("Sprint", (Input.GetKey(KeyCode.LeftShift) && movement == Vector3.zero));
        }
        velocity = Mathf.Clamp(velocity, 0f, 1f);

        oldPos = new Vector3(transform.position.x, 0f, transform.position.z);
        //animator.SetFloat("MoveX", Mathf.Lerp(animator.GetFloat("MoveX"), animMove.x * velocity, Time.deltaTime * 10f));
        //animator.SetFloat("MoveZ", Mathf.Lerp(animator.GetFloat("MoveZ"), animMove.z * velocity, Time.deltaTime * 10f));
        animator.SetFloat("MoveX", Mathf.Lerp(animator.GetFloat("MoveX"), animMove.x, Time.deltaTime * 20f));
        animator.SetFloat("MoveZ", Mathf.Lerp(animator.GetFloat("MoveZ"), animMove.z, Time.deltaTime * 20f));
        float turnValue = Input.GetAxis("Mouse X") * 3f;
        turnValue = Mathf.Clamp(turnValue, -1f, 1f);
        animator.SetFloat("TurnValue", Mathf.Lerp(animator.GetFloat("TurnValue"), Input.GetAxis("Mouse X") * 2f, Time.deltaTime * 3f));
        //animator.SetBool("Sprint", Input.GetKey(KeyCode.LeftShift));
        if (!isReload)
        {
            RHandRig.weight = Mathf.Lerp(RHandRig.weight, (Input.GetKey(KeyCode.LeftShift) && movement == Vector3.zero) ? 0f : 1f, Time.deltaTime * 10f);
            WeaponRig.weight = Mathf.Lerp(WeaponRig.weight, (Input.GetKey(KeyCode.LeftShift) && movement == Vector3.zero) ? 0f : 1f, Time.deltaTime * 10f);

            LHandRig.weight = Mathf.Lerp(LHandRig.weight, 1f, Time.deltaTime * 10f);
        }

        animator.SetBool("OnAir", !controller.isGrounded);
        animator.SetBool("Shoot", Input.GetMouseButton(0) && usingGun.CheckAmo() && !isReload);
        animator.SetBool("Aim", Input.GetMouseButton(1));
        animator.SetFloat("ShootType", Mathf.Lerp(animator.GetFloat("ShootType"), Input.GetMouseButton(1) ? 1f : 0f, Time.deltaTime * 5f));

        if (Input.GetKeyDown(KeyCode.R) && !isReload && usingGun.GetAllAmo() > 0)
        {
            StartCoroutine(Reload(2.4f / 1.2f));
        }

        /* RaycastHit hit;
         if (Physics.Raycast(usingGun.shootOut.position, usingGun.shootOut.forward, out hit, 1f))
         {
             animator.SetBool("HitWall", true);
             RHandRig.weight = 0f;
         }
         else
         {
             animator.SetBool("HitWall", false);
             RHandRig.weight = 1f;
         }*/

        if (Input.GetKeyDown(KeyCode.Q) && !isReload)
        {
            StartCoroutine(ChangeWeapon(1.9f / 2f));
        }

    }

    IEnumerator FlipForward(float duration)
    {
        animator.SetTrigger("FlipForward");
        RHandRig.weight = 0f;
        WeaponRig.weight = 0f;
        freezMovement = true;
        speed = 6;
        controller.radius = 1f;
        moveDirection = transform.forward * speed;
        yield return new WaitForSeconds(duration);
        controller.radius = radius;
        moveDirection = Vector3.zero;
        freezMovement = false;
    }

    IEnumerator Kick(float duration)
    {
        animator.SetTrigger("Kick");
        RHandRig.weight = 0f;
        WeaponRig.weight = 0f;
        LHandRig.weight = 0f;
        freezMovement = true;
        moveDirection = Vector3.zero;
        yield return new WaitForSeconds(duration);
        freezMovement = false;
    }

    IEnumerator Reload(float duration)
    {
        animator.SetTrigger("Reload");
        usingGun.StartReload();
        isReload = true;
        RHandRig.weight = 0f;
        WeaponRig.weight = 0f;
        LHandRig.weight = 0f;      
        yield return new WaitForSeconds(duration);
        isReload = false;
        usingGun.EndReload();
    }

    IEnumerator JumpOver(float duration)
    {
        animator.SetTrigger("JumpOver");
        isReload = true;
        RHandRig.weight = 0f;
        WeaponRig.weight = 0f;
        LHandRig.weight = 0f;
        controller.height = 0.5f;
        speed = 8f;
        freezMovement = true;
        jumpOver = true;
        moveDirection = transform.forward * 5f;
        yield return new WaitForSeconds(duration);
        controller.height = height;
        isReload = false;
        freezMovement = false;
        jumpOver = false;
        moveDirection = Vector3.zero;
    }


    IEnumerator ChangeWeapon(float duration)
    {
        animator.SetTrigger("ChangeWeapon");
        isReload = true;
        RHandRig.weight = 0f;
        WeaponRig.weight = 0f;
        LHandRig.weight = 0f;
        yield return new WaitForSeconds(duration);
        isReload = false;
    }
   
}
