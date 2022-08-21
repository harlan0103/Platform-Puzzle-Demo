using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum RotationType
    { 
        Front,
        Right,
        Back,
        Left,
        Side,
        Up
    }

    public GameObject cubes;     // Contains all environment cubes
    public float camTransSpeed;
    public GameObject colliderList;
    public GameObject colliderPrimitive;
    public GameObject starCollider;
    public GameObject player;
    public Transform startCube;

    private int idx = 0;        // Rotation index
    private RotationType currentRotation;
    private Vector3 maxPos;
    private Vector3 minPos;
    private Vector3 centerPos;

    private float radius;   // cube radius
    private float viewOffset = 10;
    private bool initial = true;

    private Vector3 targetRotation;
    private Vector3 targetPos;

    private Vector3[] colliderPos;
    private Transform[] colliders;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        maxPos = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        minPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        centerPos = GetCenterPoint();
        currentRotation = RotationType.Front;
        GenerateColliders();
        RotateCam(currentRotation);

        Player.instance.PlayerInitialization(startCube);

        initial = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && isSpinable())
        {
            Debug.Log("Update");
            if (idx == 0)
            {
                idx = 4;
            }
            else
            {
                idx--;
            }

            currentRotation = (RotationType)idx;
            RotateCam(currentRotation);
        }
        else if (Input.GetKeyDown(KeyCode.E) && isSpinable())
        {
            if (idx == 4)
            {
                idx = 0;
            }
            else
            {
                idx++;
            }

            currentRotation = (RotationType)idx;
            RotateCam(currentRotation);
        }
    }


    private void LateUpdate()
    {
        if (!initial)
        {
            //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime * camTransSpeed);
            Vector3 lerpVal = Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime * camTransSpeed);
            Camera.main.transform.position = lerpVal;

            // Update player position when approaching the target transform
            if (Vector2.Distance(Camera.main.transform.position, targetPos) < 0.01f)
            {
                RotateCollider(currentRotation);
            }

            Vector3 targetAngle = new Vector3(
                Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.x, targetRotation.x, Time.deltaTime * camTransSpeed),
                Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.y, targetRotation.y, Time.deltaTime * camTransSpeed),
                Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.z, targetRotation.z, Time.deltaTime * camTransSpeed));

            Camera.main.transform.eulerAngles = targetAngle;
        }
    }

    public Vector3 GetCenterPoint()
    {
        Transform[] transforms = cubes.GetComponentsInChildren<Transform>();
        radius = GetCubeRadius(transforms[1]);

        for (int i = 1; i < transforms.Length; i++)
        {
            Transform trans = transforms[i];

            maxPos.x = Mathf.Max(maxPos.x, trans.position.x);
            maxPos.y = Mathf.Max(maxPos.y, trans.position.y);
            maxPos.z = Mathf.Max(maxPos.z, trans.position.z);
            minPos.x = Mathf.Min(minPos.x, trans.position.x);
            minPos.y = Mathf.Min(minPos.y, trans.position.y);
            minPos.z = Mathf.Min(minPos.z, trans.position.z);
        }

        return (maxPos + minPos) / 2;
    }

    public void RotateCam(RotationType currentType)
    {
        Vector3 camRotation = new Vector3();
        Vector3 camPos = new Vector3();

        switch (currentType)
        {
            case RotationType.Front:
                // Front view
                //camRotation = new Vector3();
                camPos = new Vector3(centerPos.x, centerPos.y, minPos.z - viewOffset * radius);
                break;
            case RotationType.Right:
                // Right view
                camRotation = new Vector3(0, -90, 0);
                camPos = new Vector3(maxPos.x + viewOffset * radius, centerPos.y, centerPos.z);
                break;
            case RotationType.Back:
                // Back view
                camRotation = new Vector3(0, 180, 0);
                camPos = new Vector3(centerPos.x, centerPos.y, maxPos.z + viewOffset * radius);
                break;
            case RotationType.Left:
                // Left view
                camRotation = new Vector3(0, 90, 0);
                camPos = new Vector3(minPos.x - viewOffset * radius, centerPos.y, centerPos.z);
                break;
            case RotationType.Side:
                // Side view
                camRotation = new Vector3(38, 137, 0);
                camPos = centerPos - Quaternion.Euler(camRotation) * Vector3.forward * viewOffset * radius;
                break;
        }

        if (initial)
        {
            Camera.main.transform.position = camPos;
            Camera.main.transform.rotation = Quaternion.Euler(camRotation);
            targetPos = camPos;
            targetRotation = camRotation;
            //RotateCollider(currentRotation);
        }
        else
        {
            // Smooth transition
            targetPos = camPos;
            targetRotation = camRotation;
        }
    }

    public void RotateCollider(RotationType currentRotation)
    {
        // Rotate colliders around Player current position
        Vector3 currentCollider = Player.instance.GetCurrentCollider();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (currentRotation == RotationType.Front || currentRotation == RotationType.Back)
            {
                //colliders[i].position = new Vector3(colliderPos[i].x, colliderPos[i].y, 0);
                //colliders[i].position = new Vector3(colliderPos[i].x, colliderPos[i].y, colliderPos[FindColliderIdx(currentCollider)].z);     // colliderPos[currentColliderIdx].z
                colliders[i].position = new Vector3(colliderPos[i].x, colliderPos[i].y, currentCollider.z);     // colliderPos[currentColliderIdx].z
            }
            else if (currentRotation == RotationType.Left || currentRotation == RotationType.Right)
            {
                //colliders[i].position = new Vector3(0, colliderPos[i].y, colliderPos[i].z);
                //colliders[i].position = new Vector3(colliderPos[FindColliderIdx(currentCollider)].x, colliderPos[i].y, colliderPos[i].z);     // colliderPos[currentColliderIdx].x
                colliders[i].position = new Vector3(currentCollider.x, colliderPos[i].y, colliderPos[i].z);
            }
            else
            { 
                // Need to deal with side view
            }
        }

        //Update player to correct position
    }

    public float GetCubeRadius(Transform trans)
    {
        return trans.gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x;
    }

    public void GenerateColliders()
    {
        Transform[] transforms = cubes.GetComponentsInChildren<Transform>();
        colliderPos = new Vector3[transforms.Length];
        colliders = new Transform[transforms.Length];

        for (int i = 1; i < transforms.Length; i++)
        {
            // Create a new collider object and add into collider list
            GameObject newCollider = Instantiate(colliderPrimitive);
            newCollider.transform.position = transforms[i].position;
            newCollider.transform.parent = colliderList.transform;
            
            colliders[i - 1] = newCollider.transform;
            colliderPos[i - 1] = newCollider.transform.position;
            //newCollider.gameObject.layer = 10;
        }

        // Generate star collider
        GameObject starColliderObj = Instantiate(starCollider);
        starColliderObj.transform.position = Star.instance.GetPos();
        starColliderObj.transform.parent = colliderList.transform;

        colliders[transforms.Length - 1] = starColliderObj.transform;
        colliderPos[transforms.Length - 1] = starColliderObj.transform.position;
    }

    public void PlayerMove(int idx)
    {
        
    }

    public RotationType GetCurrentRotation()
    {
        return currentRotation;
    }

    private int FindColliderIdx(Vector3 collider)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (collider == colliders[i].position)
            {
                return i;
            }
        }

        return 0;
    }

    // Reset collider position to align with its mesh
    public void ResetColliderPos(Transform collider)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (collider == colliders[i])
            {
                // Find the current collider
                // Reset its position
                if (collider.position != colliderPos[i])
                {
                    collider.position = colliderPos[i];
                }
                break;
            }
        }
        RotateCollider(currentRotation);
    }

    private bool isSpinable()
    {
        GameObject currentCollider = Player.instance.GetCurrentColliderObj();

        Transform[] transforms = cubes.GetComponentsInChildren<Transform>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (currentCollider.transform == colliders[i])
            {
                if (transforms[i + 1].GetComponent<Cube>().spin)
                {
                    return true;
                }
                break;
            }
        }

        return false;
    }
}
