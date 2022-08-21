using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public float speed;
    private Transform currentCollider;
    private Rigidbody myRigidBody;
    private GameObject currentColliderObj;

    private bool colliderLock;      // update player collider when false

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Run();
    }

    public void PlayerInitialization(Transform startCube)
    {
        myRigidBody = gameObject.GetComponent<Rigidbody>();
        colliderLock = true;
        currentCollider = startCube;
        UpdatePosition(GameManager.instance.GetCubeRadius(startCube));

        colliderLock = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!colliderLock)
        {
            currentColliderObj = collision.gameObject;
            currentCollider = currentColliderObj.transform;
            
            // Need to reposition current collide
            GameManager.instance.ResetColliderPos(currentCollider);
            UpdatePosition(GameManager.instance.GetCubeRadius(currentCollider));
        }
    }

    // Update player's position to be at the center of the current collider
    public void UpdatePosition(float radius)
    {
        if (currentCollider != null)
        {
            gameObject.transform.position = new Vector3(currentCollider.position.x, currentCollider.position.y + radius, currentCollider.position.z);
        }
    }

    public Vector3 GetCurrentCollider()
    {
        if (currentCollider != null)
        {
            return currentCollider.position;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public GameObject GetCurrentColliderObj()
    {
        return currentColliderObj;
    }

    private void Run()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            onPlayerMove(-1);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            onPlayerMove(1);
        }
    }

    private void onPlayerMove(int offset)
    {
        // Front and Back -- move direction along x axis
        // Right and Left -- move direction along z axis

        GameManager.RotationType currentRotation = GameManager.instance.GetCurrentRotation();

        Vector3 newVelocity = gameObject.transform.position;

        switch (currentRotation)
        {
            case GameManager.RotationType.Front:
                //newVelocity.x = 1 * offset * speed;
                newVelocity.x += GameManager.instance.GetCubeRadius(currentCollider) * offset;
                break;
            case GameManager.RotationType.Right:
                //newVelocity.z = 1 * offset * speed;
                newVelocity.z += GameManager.instance.GetCubeRadius(currentCollider) * offset;
                break;
            case GameManager.RotationType.Back:
                //newVelocity.x = -1 * offset * speed;
                newVelocity.x += GameManager.instance.GetCubeRadius(currentCollider) * -1 * offset;
                break;
            case GameManager.RotationType.Left:
                //newVelocity.z = -1 * offset * speed;
                newVelocity.z += GameManager.instance.GetCubeRadius(currentCollider) * -1 * offset;
                break;
            case GameManager.RotationType.Side:
                break;
        }

        //myRigidBody.velocity = newVelocity;
        gameObject.transform.position = newVelocity;
    }

    public void FaceDirection()
    { 
    
    }
}
