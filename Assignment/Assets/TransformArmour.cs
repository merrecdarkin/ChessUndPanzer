using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TransformArmour : MonoBehaviour
{
    // Start is called before the first frame update
    protected bool ActiveTransform=false;


    // Update is called once per frame
    void Update()
    {
        if ((ActiveTransform)&&(transform.position.y>7.0f)){// armour fallin down
            transform.position = new Vector3(transform.position.x, transform.position.y-(0.5f), transform.position.z);
        }
        if (transform.position.y<7.0f){
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        }
    }

    public void BeginTransform(){
        ActiveTransform=true;
    }
    public void RIP(){
        transform.position=new Vector3(0.0f, 100.0f, 0.0f);
        ActiveTransform=false;
    }
    public void BreakDown() {
        Destroy(gameObject);
    }
}
