using UnityEngine;
using System.Collections;

public class Rokket : MonoBehaviour
{
    //Explosion Effect
    public GameObject Explosion;

    public float speed = 10.0f;
    public float lifeTime = 7.0f;
    public int damage = 50;	public float elapsedPhase=0.0f;

	private Vector3 newPos;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
		// future position if bullet doesn't hit any colliders
		newPos = transform.position + transform.forward * speed * Time.deltaTime;
		elapsedPhase+=Time.deltaTime;
		if (elapsedPhase<1.0f){//float up
			transform.Rotate(25.0f*Time.deltaTime, 0.0f, 0.0f);
			if (transform.transform.position.y< 20.0f){
				transform.position= new Vector3(transform.position.x, transform.position.y+(0.35f), transform.position.z);
			}
		} else{// Then go forth
			RaycastHit hit;
			if (Physics.Linecast(transform.position, newPos, out hit))
			{
				if (hit.collider){
					GameObject[] RedSide;
					RedSide=  GameObject.FindGameObjectsWithTag("Enemy");
					for (int i = 0; i < RedSide.Length; i++) {
						float distanceToExplosion = Vector3.Distance(transform.position, RedSide[i].transform.position);
						if(distanceToExplosion < 38.0f){
							RedSide[i].SendMessage("gotCaptured", 12.0f);
							RedSide[i].SendMessage("plateBreak");
						} 
					}
					


					// apply damage to object
					GameObject obj = hit.collider.gameObject;
					if (obj.tag!= "Friendly"){// create explosion and destroy bullet
						Destroy(gameObject);
						transform.position = hit.point;
						if (Explosion){
							Instantiate(Explosion, hit.point, Quaternion.identity);
						}
					}

					if (obj.tag == "EnemyTank")
					{
						SimpleFSM tank = (SimpleFSM) obj.GetComponent(typeof(SimpleFSM));
						tank.ApplyDamage(damage);

					}else if (obj.tag == "Enemy")
					{
						Unit tank = (Unit) obj.GetComponent(typeof(Unit));
						obj.SendMessage("gotCaptured", 7.0f);
						obj.SendMessage("plateBreak");
					}
				}
			}
			else
			{
				// didn't hit - move to newPos
				transform.position = newPos;
			}   
		}
		// see if bullet hits a collider
		  
    }

}