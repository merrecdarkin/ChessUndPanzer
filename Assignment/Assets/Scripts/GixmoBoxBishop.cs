using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GixmoBoxBishop : MonoBehaviour
{
    // Start is called before the first frame update
     void OnDrawGizmos() {
        float maxDist = 40.0f;
        float yOffset = 7.0f;
        RaycastHit hit;
        RaycastHit hitRight;         RaycastHit hitLeft; 
        RaycastHit hitBRight;         RaycastHit hitBLeft; 

        GameObject friendly = GameObject.FindGameObjectWithTag("Friendly");
        Transform friendTransform = friendly.transform;
        BoxCollider thisPieceCollider = GetComponent<BoxCollider>();
        BoxCollider friendlyCollider = friendly.GetComponent<BoxCollider>();

        float distance = Vector3.Distance(transform.position, friendTransform.position);
        //Debug.Log("Distance = " + (distance - (myCollider.size.z / 2) - (enemyCollider.size.z/2)));

        Vector3 tankCastSource = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);


        Vector3 right = transform.forward; 
        Vector3 left= transform.forward; 

        left=Quaternion.Euler(0, -45, 0) *left;
        right=Quaternion.Euler(0, 45, 0) * right;

        Vector3 backRight= transform.forward; 
        Vector3 backLeft= transform.forward; 

        backRight=Quaternion.Euler(0, 180+45, 0) *backRight;
        backLeft=Quaternion.Euler(0, 180-45, 0) *backLeft;
        
        bool isHitRight = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, right, out hitRight, Quaternion.Euler(0, 0, 0), maxDist);
        bool isHitLeft  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, left, out hitLeft, Quaternion.Euler(0, 0, 0), maxDist);

        bool isHitBRight = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, backRight, out hitBRight, Quaternion.Euler(0, 0, 0), maxDist);
        bool isHitBLeft  = Physics.BoxCast(transform.position+ new Vector3(0.0f,yOffset,0.0f), thisPieceCollider.size/4, backLeft, out hitBLeft, Quaternion.Euler(0, 0, 0), maxDist);


        Gizmos.matrix = transform.localToWorldMatrix;
        Vector3 ray = new Vector3(0,yOffset,0.0f); 
        Vector3 boxCast = new Vector3(0,yOffset,0);
        
        if (isHitRight) {
            GameObject objR = hitRight.collider.gameObject;
            Gizmos.color = Color.yellow;

            Gizmos.DrawRay(ray, right * (hitRight.distance +0.025f));

            Gizmos.DrawCube(boxCast + right * (hitRight.distance ), transform.localScale*1);

            if (isHitLeft) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(ray, left * (hitLeft.distance +0.025f ));
                Gizmos.DrawCube(boxCast + left * (hitLeft.distance ), transform.localScale*1);
            } else {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(ray, left * (maxDist ));
                Gizmos.DrawCube(boxCast + left * (maxDist), transform.localScale*1);
            }

            

        }else if (isHitLeft) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(ray, left * (hitLeft.distance +0.025f ));
            Gizmos.DrawCube(boxCast + left * (hitLeft.distance ), transform.localScale*1);


            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray, right * (maxDist));

            Gizmos.DrawCube(boxCast + right * (maxDist), transform.localScale*1);

        } 
        else {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray, left * (maxDist +0.025f ));
            Gizmos.DrawRay(ray, right * (maxDist +0.025f));            
            Gizmos.DrawCube(boxCast + right * (maxDist), transform.localScale*1);
            Gizmos.DrawCube(boxCast + left * (maxDist), transform.localScale*1);

        }

    if (isHitBRight) {

            Gizmos.color = Color.yellow;

            Gizmos.DrawRay(ray, backRight * (hitBRight.distance +0.025f));

            Gizmos.DrawCube(boxCast + backRight * (hitBRight.distance), transform.localScale*1);

            if (isHitBLeft) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(ray, backLeft * (hitBLeft.distance +0.025f ));
                Gizmos.DrawCube(boxCast + backLeft * (hitBLeft.distance ), transform.localScale*1);
            } else {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(ray, backLeft * (maxDist ));
                Gizmos.DrawCube(boxCast + backLeft * (maxDist), transform.localScale*1);
            }

            

        }else if (isHitBLeft) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(ray, backLeft * (hitBLeft.distance +0.025f ));
            Gizmos.DrawCube(boxCast + backLeft * (hitBRight.distance ), transform.localScale*1);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray, backRight * (maxDist));
            Gizmos.DrawCube(boxCast + backRight * (maxDist), transform.localScale*1);

        } 
        else {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(ray, backLeft * (maxDist +0.025f ));
            Gizmos.DrawRay(ray, backRight * (maxDist +0.025f));            
            Gizmos.DrawCube(boxCast + backRight * (maxDist ), transform.localScale*1);
            Gizmos.DrawCube(boxCast + backLeft * (maxDist ), transform.localScale*1);

        }


    
    }
}
