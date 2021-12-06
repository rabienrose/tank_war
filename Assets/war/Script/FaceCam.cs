using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update() 
    {
        // Vector3 v = Camera.main.transform.position - transform.position;
        // v.x = v.z = 0.0f;
        // transform.LookAt( Camera.main.transform.rotation - v ); 
        transform.rotation=Camera.main.transform.rotation;
    }
}
