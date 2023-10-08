using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    transform.Rotate(0.0f, 1.0f, 0.0f);
    }

     void OnTriggerEnter(Collider other) {
        // do something
        if (other.gameObject.tag=="Player"){
            Debug.Log("Collected");
            PlayerTank tank = (PlayerTank) other.gameObject.GetComponent(typeof(PlayerTank));
            }
        Destroy(gameObject);
    }
    
}
