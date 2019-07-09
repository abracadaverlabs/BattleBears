using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseTeddy
{
    public float movementSpeed;
    public float jumpHeight;
    public float respawnTime;

    public RaycastHit groundCheck;

    //Will use this new value to keep track of the total mouse inputs, necessary to clamp camera
    float totalMouseXInput;
    float totalMouseYInput;

    //These values will hold my preferred clamp values, set in the inspector
    public float backwardsClamp;
    public float strafeClamp;

    public GameObject camPivot;

    private Rigidbody rb;

    private Vector3 respawnPosition;
    private int startingHealth;
    private float onGroundCheck = .1f;

    // Start is called before the first frame update
    protected override void Start()
    {
        //After overriding, we have to call the function in the base-class (in this case, BaseTeddy) as well
        base.Start();
        rb = GetComponent<Rigidbody>();
        respawnPosition = transform.position;
        startingHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            return;
        }

        //This returns any value between -1 and 1.
        //The value changes based on the keyboard (WASD/Arrow Keys) or Controller input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //Clamps vertical and horizontal input so that player is slower when moving backwards or sideways
        verticalInput = Mathf.Clamp(verticalInput, backwardsClamp, 1f);
        horizontalInput = Mathf.Clamp(horizontalInput, -strafeClamp, strafeClamp);


        //We update these parameters in the Animator, the Animator handles the animation itself.
        anim.SetFloat("HorizontalInput", horizontalInput);
        anim.SetFloat("VerticalInput", verticalInput);

        //We set the x and z values based on the userinput. y should not change, so we set it to whatever the velocity already was
        Vector3 input = new Vector3(horizontalInput * movementSpeed, rb.velocity.y, verticalInput * movementSpeed);


        //We rotate the input vector based on the rotation of the player
        //This way, the player moves in it's own forward direction when forward is pressed.
        Vector3 rotatedInput = transform.TransformVector(input);


        if (Physics.Raycast(transform.position, Vector3.down, out groundCheck, onGroundCheck))
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                rotatedInput.y = jumpHeight;
                anim.SetTrigger("Jump");
            }
        }



        //Instead of using transform.position to change the objects position, we use velocity and the physics engine moves the character
        //This avoids issues when colliding with objects
        rb.velocity = rotatedInput;

        float mouseXInput = Input.GetAxis("Mouse X");
        float mouseYInput = Input.GetAxis("Mouse Y");

        //Keep total value of mouse input to later help clamp our camPivot
        totalMouseXInput += Input.GetAxis("Mouse X");
        totalMouseYInput += Input.GetAxis("Mouse Y");

        //We rotate the character to look left and right
        transform.Rotate(0, mouseXInput, 0);

        //We set the campivot rotation to totalMouseY (clamped to a 170 degree area) and totalmouseXInput. This makes the camera rotate and move at the same time, but also limits vertical movement
        camPivot.transform.eulerAngles = new Vector3(-Mathf.Clamp(totalMouseYInput, -70f, 90f), totalMouseXInput, 0);

        //If left mouse button is pressed...
        if (Input.GetMouseButtonDown(0))
        {
            //We create a ray starting from the camera, going towards where the camera is looking at
            Ray camRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hitInfo;

            //If we hit anything...
            if (Physics.Raycast(camRay, out hitInfo))
            {
                //If the bear also can see that object (and is not obstructed by e.g. a wall)
                if (CanSee(hitInfo.transform, hitInfo.point))
                {
                    Debug.Log("We hit " + hitInfo.transform.gameObject.name);
                    //We shoot lasers at the object
                    ShootLasers(hitInfo.transform, hitInfo.point);
                }
            }
        }
    }

    protected override void Die()
    {
        base.Die();
        //Stop the body from floating on
        rb.velocity = Vector3.zero;
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);
        transform.position = respawnPosition;
        health = startingHealth;
        anim.SetBool("IsDead", false);
    }
}
