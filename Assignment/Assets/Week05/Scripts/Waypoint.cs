using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnDrawGizmos() {
        Gizmos.DrawIcon(new Vector3(transform.position.x, 0.5f,transform.position.z), "Assets/Week05/Gizmos/wayPoint.png");
    }
}
