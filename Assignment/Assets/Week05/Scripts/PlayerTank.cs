using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class PlayerTank : MonoBehaviour {

	protected float moveSpeed = 22.0f;  // units per second
	protected float rotateSpeed = 3.0f;

	protected int health = 100; 
	//Ammunition counts
	protected int bulletCount; 		public int rocketCount;  	
	//Round Points
	protected int VictoryPoint; protected int DefeatPoint; public int RoundVictorNeed;

	private Transform _transform;
	private Rigidbody _rigidbody;

	public GameObject turret;		public GameObject HealthCylinder;	
	public GameObject bullet;	public GameObject Rokket; 
	public GameObject bulletSpawnPoint; 	public GameObject RokketSpawnPoint;

	public AudioSource FiringSound;

		//UI stuff (there is alot of them)
		public Mask ammoMask; public Mask loadMask; 		public Mask defeatMask; public Mask victoryMask;
		public Image VictoryImg; public Image DefeatIMG;
		    public TextMeshProUGUI PointsGUI;     public TextMeshProUGUI RokketNumbers;
				public AnimationCurve curve; 


	protected float turretRotSpeed = 3.0f;

	//Bullet shooting rate
	protected float elapsedTime; 	protected float shootCD;

	// Use this for initialization
	void Start () {

		RokketNumbers.text= $"{rocketCount} X ";
		PointsGUI.text = $"VP: {VictoryPoint} / {RoundVictorNeed} vs DP {DefeatPoint*2}";

		bulletCount=10;	VictoryPoint=0;  DefeatPoint=0;  
		_transform = transform;
		_rigidbody = GetComponent<Rigidbody>();					FiringSound= GetComponent<AudioSource>(); 	

		rotateSpeed = rotateSpeed * 180 / Mathf.PI; // convert from rad to deg for rot function
		UpdateAmmoCount();
		if (defeatMask) {//hide defeat screen
			RectTransform rectTransformDefeat =defeatMask.GetComponent<RectTransform>();
			rectTransformDefeat.sizeDelta = new Vector2(0f,rectTransformDefeat.sizeDelta.y);
		}
		if (victoryMask) {//hide deVictoryfeat screen
			RectTransform rectTransformVictory =victoryMask.GetComponent<RectTransform>();
			rectTransformVictory.sizeDelta = new Vector2(0f,rectTransformVictory.sizeDelta.y);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		// check for input
		float rot = _transform.localEulerAngles.y + rotateSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
		Vector3 fwd = _transform.forward * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");

		// Tank Chassis is rigidbody, use MoveRotation and MovePosition
		_rigidbody.MoveRotation(Quaternion.AngleAxis(rot, Vector3.up));
		//_rigidbody.MovePosition(_rigidbody.position + fwd);

		if (Mathf.Abs(_rigidbody.velocity.z) < 20.5f)
		{
			_rigidbody.AddForce(transform.forward * Input.GetAxis("Vertical") * 6.5f, ForceMode.VelocityChange);
		}

		if (turret) {
			Plane playerPlane = new Plane(Vector3.up, _transform.position + new Vector3(0, 0, 0));
			
			// Generate a ray from the cursor position
			Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			// Determine the point where the cursor ray intersects the plane.
			float HitDist = 0;
			
			// If the ray is parallel to the plane, Raycast will return false.
			if (playerPlane.Raycast(RayCast, out HitDist))
			{
				// Get the point along the ray that hits the calculated distance.
				Vector3 RayHitPoint = RayCast.GetPoint(HitDist);
				
				Quaternion targetRotation = Quaternion.LookRotation(RayHitPoint - _transform.position);
				turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
			}
		}

		if(Input.GetButton("Fire1"))//main weapon
		{			
			if ((bulletCount > 0)&& (shootCD>0.5f) )
			{
				//Sound Trigger
				

				//Reset the time

				shootCD=0.0f;
				Debug.Log("fire");
				//Also Instantiate over the PhotonNetwork
				if ((bulletSpawnPoint) & (bullet))
					Instantiate(bullet, bulletSpawnPoint.transform.position, bulletSpawnPoint.transform.rotation);
				bulletCount-=1;
				FiringSound.Play();
			}
			UpdateAmmoCount();

		}
		if(Input.GetButton("Fire2")){//Rokket
			if ((shootCD>0.5f) && (rocketCount>0)){
				shootCD=0.0f; 
				if ((RokketSpawnPoint) & (Rokket))
					Instantiate(Rokket, RokketSpawnPoint.transform.position, RokketSpawnPoint.transform.rotation);
				rocketCount-=1;
				RokketNumbers.text= $"{rocketCount} X ";
			}
		}

		// Update the time
		elapsedTime += Time.deltaTime;
		shootCD+= Time.deltaTime;
		UpdateLoader();
		if (elapsedTime>4.5f){//reload per 4.5s
				elapsedTime = 0.0f;
				bulletCount+=3;
				if (bulletCount>10){ bulletCount=10;}//max 10 bullets
				UpdateAmmoCount();
		}
		//Animate game end UI
		if ((VictoryPoint>=RoundVictorNeed) && (health>=0)){
			if (VictoryImg.fillAmount<1){VictoryImg.fillAmount+= 1.0f/4.0f*Time.deltaTime;} //Filling the image
			health=400;
		}
		if ((DefeatPoint>(RoundVictorNeed/2)) || (health<=0)) {
			if (DefeatIMG.fillAmount<1){DefeatIMG.fillAmount+= 1.0f/4.0f*Time.deltaTime;}
			health=-400;
		}
	}

	// Apply Damage if hit by bullet
	public void ApplyDamage(int damage ) {
		if (health>60){ HealthCylinder.SendMessage("BreakDown");}//Visual Loss health
		health -= damage;
		if ((health<=0 )&& (VictoryPoint<RoundVictorNeed)){DefeatedThreshhold();}
		
	}  

	public void shockWaveLostAmmo() {//lost ammo due to knight tremour
		bulletCount-=4;
		if (bulletCount<0){bulletCount=0;}
		UpdateAmmoCount();
		StartCoroutine(Shaking());//Visualise by a shake screen
	}

	public void UpdateAmmoCount() {//update GUI
		if (ammoMask) {
			RectTransform rectTransform =ammoMask.GetComponent<RectTransform>();
			rectTransform.sizeDelta = new Vector2(13f *bulletCount,rectTransform.sizeDelta.y);
		}
	}


	public void UpdateLoader() {//update GUI
		if (loadMask) {
			RectTransform rectTransformLoad =loadMask.GetComponent<RectTransform>();
			rectTransformLoad.sizeDelta = new Vector2(40f *elapsedTime,rectTransformLoad.sizeDelta.y);
		}
	}
	//Point get from message
	public void getVitoryPoint(int vicPoint){ 
		VictoryPoint+=vicPoint;
		PointsGUI.text = $"VP: {VictoryPoint} / {RoundVictorNeed} vs DP {DefeatPoint*2}";
		if (VictoryPoint>=RoundVictorNeed){VictoryAchived();}

	}
	public void getDefeatPoint(int defPoint){ 
		DefeatPoint+= defPoint;
		PointsGUI.text = $"VP: {VictoryPoint} / {RoundVictorNeed} vs DP {DefeatPoint*2}";
		if (DefeatPoint>(RoundVictorNeed/2)) {DefeatedThreshhold();}
	}


	public void RoundDecidedPlayer(){//disable all remaining active pieces
		GameObject[] RedSide;  GameObject[] WhiteSide; GameObject[] TanksEne; 
		RedSide=  GameObject.FindGameObjectsWithTag("Enemy");
        WhiteSide=  GameObject.FindGameObjectsWithTag("Friendly");
		TanksEne= GameObject.FindGameObjectsWithTag("EnemyTank");
				//Deactive Pieces adn tanks
		for (int i = 0; i < RedSide.Length; ++i){
			RedSide[i].SendMessage("RoundEndedUnit");}
		
		for (int i = 0; i < WhiteSide.Length; ++i)
			{WhiteSide[i].SendMessage("RoundEndedUnit");}
		for (int i = 0; i < TanksEne.Length; ++i){
			TanksEne[i].SendMessage("ApplyDamage", 200);}


	}
	//Animite End end mask
	public void VictoryAchived(){
		RoundDecidedPlayer();
		if (victoryMask) {
			RectTransform rectTransformVictory =victoryMask.GetComponent<RectTransform>();
			rectTransformVictory.sizeDelta = new Vector2(400,rectTransformVictory.sizeDelta.y);
			}
		}
	

	public void DefeatedThreshhold(){//Eneable Defeat Screen
		RoundDecidedPlayer();
		if (defeatMask) {
			RectTransform rectTransformDefeat =defeatMask.GetComponent<RectTransform>();
			rectTransformDefeat.sizeDelta = new Vector2(400,rectTransformDefeat.sizeDelta.y);
		}
	}
	//Animate Tremmor from enemy Knights
	IEnumerator Shaking(){
		Vector3 startPost= transform.position;
		float shaketime=0.0f;
		while (shaketime<0.75f){
			shaketime+=Time.deltaTime;
			float shakeStrength= curve.Evaluate(shaketime/0.75f);
			transform.position= startPost+ Random.insideUnitSphere*shakeStrength;
			yield return null;
		}
		transform.position=startPost;
	}
}
