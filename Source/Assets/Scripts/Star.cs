using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Star : MonoBehaviour
{
    public static Star instance;
    public float spinSpeed;

    private float rotation_y;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rotation_y = gameObject.transform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        SpinAround();
    }

    private void SpinAround()
    {
        rotation_y += spinSpeed;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, rotation_y, 0));
    }

    public Vector3 GetPos()
    {
        return gameObject.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            gameObject.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex + 1< SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
