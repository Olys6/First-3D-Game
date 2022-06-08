using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Transform groundCheckTransform = null;
    [SerializeField] private LayerMask playerMask;
    
    private bool jumpKeyWasPressed;
    private float horizontalInput;
    private float verticalInput;
    private Rigidbody rigidbodyComponent;
    private int superJumpsRemaining;
    private bool isRespawnSet = false;
    private float respawnPositionX;
    private float respawnPositionY;
    private float respawnPositionZ;

    private float yaw = 0.0f, pitch = 0.0f;

    [SerializeField] float walkSpeed = 3.0f, sensitivity = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
       Cursor.lockState = CursorLockMode.Locked;
       

        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.Space) && Physics.Raycast(rigidbodyComponent.transform.position, Vector3.down, 1 + 0.001f))
        //{
        //    rigidbodyComponent.velocity = new Vector3(rigidbodyComponent.velocity.x, rigidbodyComponent.velocity.z);
        //}
        Look();
        
        // Checks if space ket is pressed down
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyWasPressed = true;
        }

        if (rigidbodyComponent.position.y < 0.0f)
        {
            if(isRespawnSet)
            {
                rigidbodyComponent.transform.position = new Vector3(respawnPositionX, respawnPositionY, respawnPositionZ);
                superJumpsRemaining++;
            } 
            else
            {
                rigidbodyComponent.transform.position = new Vector3(-3.86f, 2.6f, -0.59f);
            }
        }

        //horizontalInput = Input.GetAxis("Horizontal") * 3;
        //verticalInput = Input.GetAxis("Vertical") * 3;

        //if ((float)rigidbodycomponent.worldcenterofmass.y < 0 && (float)rigidbodycomponent.worldcenterofmass.y > -0.3)
            //debug.log($"--- you lost! because you were at pos {(float)rigidbodycomponent.worldcenterofmass.y} ---");
    }

    private void Look()
    {
        pitch -= Input.GetAxisRaw("Mouse Y") * sensitivity;   
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        yaw += Input.GetAxisRaw("Mouse X") * sensitivity;
        Camera.main.transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
    }

    // FixedUpdate() is called once every physic update
    private void FixedUpdate()
    {
        Movement();

        //rigidbodyComponent.velocity = new Vector3(verticalInput, rigidbodyComponent.velocity.y, -horizontalInput);
        
        if (Physics.OverlapSphere(groundCheckTransform.position, 0.1f, playerMask).Length == 0)
        {
            return;
        }

        if (jumpKeyWasPressed)
        {
            float jumpPower = 5f;
            if(superJumpsRemaining > 0)
            {
                jumpPower *= 2;
                superJumpsRemaining--;
            }
            rigidbodyComponent.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
            jumpKeyWasPressed = false;
        }
        //rigidbodyComponent.velocity = new Vector3(verticalInput, 0, -rigidbodyComponent.velocity.x);

        //Debug.Log((float)rigidbodyComponent.worldCenterOfMass.y);

    }

    private void Movement()
    {
        Vector2 axis = new Vector2(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * 3.2f;
        Vector3 forward = new Vector3(-Camera.main.transform.right.z, 0f, Camera.main.transform.right.x);
        Vector3 wishDirection = (forward * axis.x + Camera.main.transform.right * axis.y + Vector3.up * rigidbodyComponent.velocity.y);
        rigidbodyComponent.velocity = wishDirection;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == 7)
        {
            Debug.Log(other.gameObject.name);
            respawnPositionX = other.gameObject.transform.position.x;
            respawnPositionY = other.gameObject.transform.position.y;
            respawnPositionZ = other.gameObject.transform.position.z;
            Debug.Log($"{respawnPositionX}, {respawnPositionY}, {respawnPositionZ}");
            isRespawnSet = true;
            Destroy(other.gameObject);
            superJumpsRemaining++;

        }

        if (other.gameObject.layer == 8)
        {
            if(isRespawnSet)
            {
                rigidbodyComponent.transform.position = new Vector3(respawnPositionX, respawnPositionY, respawnPositionZ);
                superJumpsRemaining++;
            } 
            else
            {
                rigidbodyComponent.transform.position = new Vector3(-3.86f, 2.6f, -0.59f);
            }

        }

        
    }

}
