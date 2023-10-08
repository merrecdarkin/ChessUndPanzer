using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class SimpleFSM : MonoBehaviour 
{
    public enum FSMState
    {
        None,
        Patrol,
        Chase,
        Attack,
        Dead,
    }

	// Current state that the NPC is reaching
	public FSMState curState;

	protected Transform playerTransform;// Player Transform
	
	public GameObject[] waypointList; // List of waypoints for patrolling

	// Turret
	public GameObject turret;
	public float turretRotSpeed = 4.0f;
	
    // Bullet
	public GameObject bullet;
	public GameObject bulletSpawnPoint;

	// Bullet shooting rate
	public float shootRate = 3.0f;
	protected float elapsedTime;

    // Whether the NPC is destroyed or not
    protected bool bDead;
    public int health = 100;

	// Ranges for chase and attack
	public float chaseRange = 35.0f;
	public float attackRange = 20.0f;
	public float attackRangeMin = 10.0f;

	public GameObject explosion;
	public GameObject smokeTrail;
	
	private NavMeshAgent nav;

	// current waypoint in list
	private int curWaypoint = -1;
	private bool setDest = false;

	public float pathCheckTime = 1.0f;
	private float elapsedPathCheckTime;

	private GameObject objPlayer;

	/*
     * Initialize the Finite state machine for the NPC tank
     */
	void Start() {

        curState = FSMState.Patrol;

        bDead = false;
        elapsedTime = 0.0f;

        // Get the target enemy(Player)
        objPlayer = GameObject.FindGameObjectWithTag("Player");
        waypointList= GameObject.FindGameObjectsWithTag("WayPoints");
        playerTransform = objPlayer.transform;

        if(!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

		//reference the navmeshagent so we can access it
		nav = GetComponent<NavMeshAgent>();

		// if there are waypoints in the list set our destination to be the current waypoint
		if (waypointList.Length > 0)
			curWaypoint = 0;

		// set to pathCheckTime so it will trigger first time
		elapsedPathCheckTime = pathCheckTime;
	}


    // Update each frame
    void Update() {
        switch (curState) {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        // Update the time
        elapsedTime += Time.deltaTime;
		elapsedPathCheckTime += Time.deltaTime;

        // Go to dead state if no health left
        if (health <= 0)
            curState = FSMState.Dead;
    }

	/*
     * Patrol state
     */
    protected void UpdatePatrolState() {
        
		// only move if there are waypoints in list for object
		if (curWaypoint > -1) {
			// check if close to current waypoint
			if (Vector3.Distance(transform.position, waypointList[curWaypoint].gameObject.transform.position) <= 2.0f) {
				// get next waypoint
				curWaypoint++;
				// if we have travelled to last waypoint, go back to the first
				if (curWaypoint > (waypointList.Length - 1))
					curWaypoint = 0;

				setDest = false;
			}

			if (!setDest) {
				// NavMeshAgent move
				nav.SetDestination(waypointList[curWaypoint].gameObject.transform.position);
				setDest = true;
			}

			// Turn the turret to face the direction of travel
			if (turret) {
				if (transform.forward != turret.transform.forward) {
					Quaternion turretRotation = Quaternion.LookRotation(transform.forward - turret.transform.forward);
					turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, turretRotation, Time.deltaTime * turretRotSpeed); 
				}
			}
		}

        // Check the distance with player tank
        // When the distance is near, transition to chase state
		if(objPlayer != null)
        {
			if (Vector3.Distance(transform.position, playerTransform.position) <= chaseRange)
			{

				// see if playerTank is Line of Sight
				RaycastHit hit;
				if (Physics.Linecast(transform.position + new Vector3(0f, 1f, 0f), playerTransform.position + new Vector3(0f, 1f, 0f), out hit))
				{
					if (hit.collider.gameObject.tag == "Player")
					{
						curState = FSMState.Chase;
					}
				}
			}
        }
        
    }


    /*
     * Chase state
	 */
    protected void UpdateChaseState() {

		if(objPlayer != null)
        {
			// NavMeshAgent move
			if (elapsedPathCheckTime >= pathCheckTime)
			{
				nav.SetDestination(playerTransform.position);
				elapsedPathCheckTime = 0f;
			}

			// Turn the turret to face the direction of travel
			if (turret)
			{
				if (transform.forward != turret.transform.forward)
				{
					Quaternion turretRotation = Quaternion.LookRotation(transform.forward - turret.transform.forward);
					turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, turretRotation, Time.deltaTime * turretRotSpeed);
				}
			}

			// Check the distance with player tank
			// When the distance is near, transition to attack state
			float dist = Vector3.Distance(transform.position, playerTransform.position);
			if (dist <= attackRange)
			{
				curState = FSMState.Attack;
			}
			// Go back to patrol is it become too far
			else if (dist >= chaseRange)
			{
				curState = FSMState.Patrol;
				setDest = false;
			}
		}
        else
        {
			curState = FSMState.Patrol;
        }
		
	}
	

	/*
	 * Attack state
	 */
    protected void UpdateAttackState() {

		if(objPlayer != null)
        {
			// Check the distance with the player tank
			float dist = Vector3.Distance(transform.position, playerTransform.position);
			if (dist >= attackRangeMin && dist <= attackRange)
			{
				// move toward target
				if (elapsedPathCheckTime >= pathCheckTime)
				{
					nav.isStopped = false;
					nav.SetDestination(playerTransform.position);
					elapsedPathCheckTime = 0f;
				}
			}
			else
			{
				nav.isStopped = true;
			}

			//Return to chase state if player moves out of attack range
			if (dist > attackRange)
			{
				nav.isStopped = false;
				curState = FSMState.Chase;
			}

			// Always Turn the turret towards the player
			if (turret)
			{
				Quaternion turretRotation = Quaternion.LookRotation(playerTransform.position - transform.position);
				turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, turretRotation, Time.deltaTime * turretRotSpeed);
			}

			// see if playerTank is Line of Sight
			RaycastHit hit;
			if (Physics.Linecast(transform.position + new Vector3(0f, 1f, 0f), playerTransform.position + new Vector3(0f, 1f, 0f), out hit))
			{
				if (hit.collider.gameObject.tag == "Player")
				{
					// Shoot the bullets
					ShootBullet();
				}
			}
		}
        else
        {
			curState = FSMState.Patrol;
        }
		
    }


    /*
     * Dead state
     */
    protected void UpdateDeadState() {
        // Show the dead animation with some physics effects
        if (!bDead) {
			nav.isStopped = true;
			nav.enabled = false;
            bDead = true;
            Explode();

			// add to player score
        }
    }


    /*
     * Shoot Bullet
     */
    private void ShootBullet() {
        if (elapsedTime >= shootRate) {
			if ((bulletSpawnPoint) & (bullet)) {
            	// Shoot the bullet
            	Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
			}
            elapsedTime = 0.0f;
        }
    }

    // Apply Damage if hit by bullet
    public void ApplyDamage(int damage ) {
    	health -= damage;
    }


    protected void Explode() {
        float rndX = Random.Range(8.0f, 12.0f);
        float rndZ = Random.Range(8.0f, 12.0f);

		GetComponent<Rigidbody>().angularDrag = 0.05f;
		GetComponent<Rigidbody>().drag = 0.05f;

        for (int i = 0; i < 3; i++) {
            GetComponent<Rigidbody>().AddExplosionForce(10.0f, transform.position - new Vector3(rndX, 2.0f, rndZ), 45.0f, 40.0f);
            GetComponent<Rigidbody>().velocity = transform.TransformDirection(new Vector3(rndX, 10.0f, rndZ));
        }

		if (smokeTrail) {
			GameObject clone = Instantiate(smokeTrail, transform.position, transform.rotation) as GameObject;
			clone.transform.parent = transform;
		}
		Invoke ("CreateFinalExplosion", 1.4f);
		Destroy(gameObject, 1.5f);
	}
	
	
	protected void CreateFinalExplosion() {
		if (explosion) 
			Instantiate(explosion, transform.position, transform.rotation);
	}


	void OnDrawGizmos () {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, chaseRange);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
	}

}
