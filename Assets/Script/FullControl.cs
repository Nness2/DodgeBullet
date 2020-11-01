using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class FullControl : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;
    public float turnSmoothTime = 0.0f;
    float turnSmoothVelocity;
    // Start is called before the first frame update

    private GameObject MainCamera;

    //JUMP
    public float gravity = -9.81f; // default : Earth gravity
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.1f;
    public LayerMask groundMask;
    private bool isGrounded;
    private Vector3 _move;

    [SerializeField]
    private Vector3 velocity; // for falling speed

    //Fire  
    public GameObject bulletPrefab;
    public Transform bulletSpawn;


    void Start()
    {
        MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        controller = gameObject.GetComponent<CharacterController>();
        Assert.IsNotNull(groundCheck);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Jump();
        if (Input.GetMouseButtonDown(0))
            Fire();
        
        transform.rotation = new Quaternion(transform.localRotation.x, MainCamera.transform.localRotation.y, transform.localRotation.z, transform.localRotation.w);

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
         
        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

    }


    /*private void MovePlayer()
    {
        _move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        _move *= speed * Time.deltaTime;

        controller.Move(_move);
    }*/


    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Jumping : y = √(h * -2 * g)
        }

        velocity.y += gravity * Time.deltaTime; // falling : ∆y = 1/2g * t^2 

        controller.Move(velocity * Time.deltaTime);


    }

    void Fire()
    {
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 50;

        Destroy(bullet, 2.0f);
    }
}
