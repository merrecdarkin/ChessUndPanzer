using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinToPlayer : MonoBehaviour
{
    protected Transform playerTransform;// Player Transform

    // Start is called before the first frame update
    void Start()
    {
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        if(!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion playerRotationDir = Quaternion.LookRotation(playerTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, playerRotationDir, Time.deltaTime * 2.5f);
        
    }
}
