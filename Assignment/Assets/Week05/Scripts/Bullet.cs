using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    //Explosion Effect
    public GameObject Explosion;

    public float speed = 300.0f;
    public float lifeTime = 3.0f;
    public int damage = 50;

	private Vector3 newPos;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
		// future position if bullet doesn't hit any colliders
		newPos = transform.position + transform.forward * speed * Time.deltaTime;

		// see if bullet hits a collider
		RaycastHit hit;
		if (Physics.Linecast(transform.position, newPos, out hit))
		{
			if (hit.collider)
			{
				// create explosion and destroy bullet
				transform.position = hit.point;
				if (Explosion)
					Instantiate(Explosion, hit.point, Quaternion.identity);
				Destroy(gameObject);

				// apply damage to object
				GameObject obj = hit.collider.gameObject;
				if (obj.tag == "Player")
				{
					PlayerTank tank = (PlayerTank) obj.GetComponent(typeof(PlayerTank));
					tank.ApplyDamage(damage);

				}
				else if (obj.tag == "EnemyTank")
				{
					SimpleFSM tank = (SimpleFSM) obj.GetComponent(typeof(SimpleFSM));
					tank.ApplyDamage(damage);

				}else if (obj.tag == "Enemy")// stun and break plates
				{
					Unit tank = (Unit) obj.GetComponent(typeof(Unit));
					obj.SendMessage("gotCaptured", 7.0f);
					obj.SendMessage("plateBreak");

				}else if (obj.tag == "Friendly")//boost allies
				{
					Unit tank = (Unit) obj.GetComponent(typeof(Unit));
					obj.SendMessage("boosted", 7.0f);
				}
			}
		}
		else
		{
			// didn't hit - move to newPos
			transform.position = newPos;
		}     
    }

}