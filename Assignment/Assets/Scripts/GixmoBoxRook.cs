using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GixmoBoxRook : MonoBehaviour
{
    // Start is called before the first frame update
     void OnDrawGizmos() {
        float maxDist = 50.0f;
        float yOffset = 1.5f;
        RaycastHit hit;
        RaycastHit hitRight;         RaycastHit hitLeft; 
        RaycastHit hitFront;         RaycastHit hitBack; 

        GameObject friendly = GameObject.FindGameObjectWithTag("Friendly");
        Transform friendTransform = friendly.transform;
        BoxCollider thisPieceCollider = GetComponent<BoxCollider>();
        BoxCollider friendlyCollider = friendly.GetComponent<BoxCollider>();

        float distance = Vector3.Distance(transform.position, friendTransform.position);
        //Debug.Log("Distance = " + (distance - (myCollider.size.z / 2) - (enemyCollider.size.z/2)));

        Vector3 tankCastSource = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);


        Vector3 right = transform.forward; 
        Vector3 left= transform.forward; 

        left=Quaternion.Euler(0, -90, 0) *left;
        right=Quaternion.Euler(0, 90, 0) * right;

        Vector3 front= transform.forward; 
        Vector3 back= -transform.forward; 
        
        bool isHitRight = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, right, out hitRight, Quaternion.Euler(0, 0, 0), maxDist);
        bool isHitLeft  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, left, out hitLeft, Quaternion.Euler(0, 0, 0), maxDist);

        bool isHitFront = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, front, out hitFront, Quaternion.Euler(0, 0, 0), maxDist);
        bool isHitBack  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, back, out hitBack, Quaternion.Euler(0, 0, 0), maxDist);


        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 ray = new Vector3(0,1.5f,0.0f); 
        Vector3 boxCast = new Vector3(0,1.5f,0);
        
        if (isHitRight) {
            GameObject objR = hitRight.collider.gameObject;
            Gizmos.color = Color.yellow;

            Gizmos.DrawRay(ray, right * (hitRight.distance +0.025f));

            Gizmos.DrawCube(boxCast + right * (hitRight.distance - yOffset), transform.localScale*1);

            if (isHitLeft) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(ray, left * (hitLeft.distance +0.025f ));
                Gizmos.DrawCube(boxCast + left * (hitLeft.distance - yOffset), transform.localScale*1);
            } else {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(ray, left * (maxDist ));
                Gizmos.DrawCube(boxCast + left * (maxDist), transform.localScale*1);
            }

            

        }else if (isHitLeft) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(ray, left * (hitLeft.distance +0.025f ));
            Gizmos.DrawCube(boxCast + left * (hitLeft.distance - yOffset), transform.localScale*1);


            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray, right * (maxDist));

            Gizmos.DrawCube(boxCast + right * (maxDist), transform.localScale*1);

        } 
        else {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray, left * (maxDist +0.025f ));
            Gizmos.DrawRay(ray, right * (maxDist +0.025f));            
            Gizmos.DrawCube(boxCast + right * (maxDist - yOffset), transform.localScale*1);
            Gizmos.DrawCube(boxCast + left * (maxDist - yOffset), transform.localScale*1);

        }

    if (isHitFront) {

            Gizmos.color = Color.yellow;

            Gizmos.DrawRay(ray, front * (hitFront.distance +0.025f));

            Gizmos.DrawCube(boxCast + front * (hitFront.distance - yOffset), transform.localScale*1);

            if (isHitBack) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(ray, back * (hitBack.distance +0.025f ));
                Gizmos.DrawCube(boxCast + back * (hitBack.distance - yOffset), transform.localScale*1);
            } else {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(ray, back * (maxDist ));
                Gizmos.DrawCube(boxCast + back * (maxDist), transform.localScale*1);
            }

            

        }else if (isHitBack) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(ray, back * (hitBack.distance +0.025f ));
            Gizmos.DrawCube(boxCast + back * (hitBack.distance - yOffset), transform.localScale*1);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray, front * (maxDist));
            Gizmos.DrawCube(boxCast + front * (maxDist), transform.localScale*1);

        } 
        else {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray, front * (maxDist +0.025f ));
            Gizmos.DrawRay(ray, back * (maxDist +0.025f));            
            Gizmos.DrawCube(boxCast + front * (maxDist - yOffset), transform.localScale*1);
            Gizmos.DrawCube(boxCast + back * (maxDist - yOffset), transform.localScale*1);

        }


    
    }
}
