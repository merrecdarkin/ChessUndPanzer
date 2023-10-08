using UnityEngine;
using System.Collections;

public class Chaser : MonoBehaviour {

	private Transform target; // the target's transform (position, rotation)
    private Rigidbody _rigidbody; // The rigidbody of the chaser

	public float moveSpeed = 3.0f; // move speed
	public float rotationSpeed = 3.0f; // speed of turning
	
	public bool chase = true; // whether chaser will chase or not
	public float chaseDist = 10.0f; // if target within this distance, chaser will chase
	public float stopDist = 3.0f; // if target within this distance, chaser will stop


	// Use this for initialization
	

	void Start () {
				// Exercise 1 goes here...
		if (target == null){
			target= GameObject.FindWithTag("Player").GetComponent<Transform>();
		}
		_rigidbody=GetComponent<Rigidbody>();
	}


	// Update is called once per frame
	void Update () {

		if ((target) & (chase)) { // if chase is false don't perform behaviour

			// check distance between chaser and target
			float distance = Vector3.Distance(transform.position, target.transform.position);

			if ((distance <= chaseDist) & (distance > stopDist)) {

				// Exercise 2 goes here...
				Vector3 chaserPos= new Vector3 (_rigidbody.transform.position.x,0.0f,_rigidbody.transform.position.z);
				Vector3 targetPos= new Vector3 (target.transform.position.x, 0.0f,target.transform.position.z);

				_rigidbody.MoveRotation(Quaternion.Slerp(_rigidbody.transform.rotation, Quaternion.LookRotation(targetPos - chaserPos), rotationSpeed * Time.deltaTime));
				
				// Exercise 3 goes here...

				 _rigidbody.MovePosition(transform.position+transform.forward*moveSpeed*Time.deltaTime);
			}
		}
		
	}

	// Exercise 4 goes here...
void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.25F);
        Gizmos.DrawWireSphere(transform.position, chaseDist);
		Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDist);

		
    }
}
