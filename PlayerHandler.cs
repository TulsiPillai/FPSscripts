using UnityEngine;
using System;
using System.Collections;

public class PlayerHandler : MonoBehaviour {

	//enumerate all states of player weapons
	public enum PlayerState{
		FlashLight,
		Knife,
		ShotGun,
		HandGun,
		Rifle
	};
	[HideInInspector]
	public PlayerState state;


	//Inspector parameters
	[Tooltip("Player's upper and lower body animators")]
	public Animator anim, anim_feet;
	[Tooltip("Light source for flashlight")]
	public GameObject flashLight;
	[Tooltip("Fire script")]
	public Fire fire;

	//variables used by other classes
	[HideInInspector]
	public int ammo =0;
	[HideInInspector]
	public float power = 500f;				//how hard to shoot the bullets 

	Light flashSource;
	float moveX, moveY, speed, attack=  0.0f;
	float time = 0.2f;
	float flip;					 	
	Rigidbody2D rb;
	bool faceForward = true, startTimer;
	int dominantIndex;
	Transform bulletSpawn;
	private int numberOfStates;				//number of items in playerstate

	//Animator controller parameters
	private AnimatorStateInfo currentBaseState;
	static int idleState = Animator.StringToHash("Base Layer.Idle");
	static int walkState = Animator.StringToHash("Base Layer.Move");

	// Use this for initialization
	void Start () {

		numberOfStates = Enum.GetValues (typeof(PlayerState)).Length;
		flashSource = flashLight.GetComponent<Light> ();
		state = PlayerState.FlashLight;
		speed = 5.0f;
		rb = this.gameObject.GetComponent<Rigidbody2D> ();
		bulletSpawn = transform.FindChild ("bulletSpawnPoint");


	}
	
	void FixedUpdate () {

		//get the input axes for player movements
		moveX = Input.GetAxis ("Horizontal");

		moveY = Input.GetAxis ("Vertical");

		rb.velocity = new Vector2 (moveX * speed, moveY * speed);

		//sets the value of animator parameters
		//speed - we only want the magnitude not direction
		anim.SetFloat ("Speed", Mathf.Abs (moveX));

		anim_feet.SetFloat ("MoveX", Mathf.Abs(moveX));

		//we want negative values as well
		anim_feet.SetFloat ("MoveY", moveY);


		//flips player sprite
		if (moveX > 0 && !faceForward) {			
			turnBack ();
		
		} else if (moveX < 0 && faceForward) {			
			turnBack ();
		}

		currentBaseState = anim.GetCurrentAnimatorStateInfo (0);


		//check for inputs across all animator layers
		if (Input.GetButton ("Attack")) {
			
			if (currentBaseState.fullPathHash == idleState) {
				anim.SetTrigger ("AttackIdle");
			} else if (currentBaseState.fullPathHash == walkState)
				anim.SetTrigger ("Attack");				
			
		} 

		if (Input.GetButton ("Fire1")) {
			if (state != PlayerState.FlashLight && state != PlayerState.Knife) {
				
				if (currentBaseState.fullPathHash == idleState) {
					anim.SetTrigger ("ShootIdle");
					fire.fireBullet ();
				} else if (currentBaseState.fullPathHash == walkState) {
					anim.SetTrigger ("Shoot");
					fire.fireBullet ();
				}			
			}
		} else if (Input.GetButton ("Reload")) {
			anim.SetTrigger ("Reload");
			ammo = 2;
		} 


		if (Input.GetButton ("FlashLight")) {
			if (state == PlayerState.FlashLight)
				flashSource.enabled = !flashSource.enabled;
			else {
				state = PlayerState.FlashLight;
				setLayers ();
			}
		}else if (state != PlayerState.FlashLight)
			flashSource.enabled = false;

		if (Input.GetButton ("switch")) {			
			cycleWeapons ();
			setLayers ();
			if (state == PlayerState.FlashLight)
				flashSource.enabled = true;
			else
				flashSource.enabled = false;
		}


		//play strafe animation when player moves along y axis
		if(Input.GetButton("Vertical")) anim_feet.SetTrigger("strafe");
	}

	//replaces prev dominant animator layer with new layer
	void setLayers(){
		//set player states
		if (state == PlayerState.FlashLight) {		

			ammo = 0;

			resetDominantLayer ();

		} else if (state == PlayerState.ShotGun) {

			resetDominantLayer ();

			anim.SetLayerWeight (1, 1.0f);

			ammo = 2;

			power = 400f;

		}else if (state == PlayerState.Rifle) {

			resetDominantLayer ();

			anim.SetLayerWeight (2, 1.0f);

			ammo = 10;

			power = 500f;

		}else if (state == PlayerState.HandGun) {

			resetDominantLayer ();

			anim.SetLayerWeight (3, 1.0f);

			ammo = 8;

			power = 1000f;

		}else if (state == PlayerState.Knife) {

			resetDominantLayer ();

			anim.SetLayerWeight (4, 1.0f);
		}

	}

	//flips the player sprite while walking backwards 
	void turnBack(){


		faceForward = !faceForward;
		//switches between negative and positive x scale
		flip += 180.0f;
		transform.localEulerAngles = new Vector3(0f,flip,0f);

	}

	//finds the animator layer with highest weight and sets it back to zero
	void resetDominantLayer(){
		
		float weight = 0f;

		float highest_weight = 0f;

		for (int i = 0; i < anim.layerCount; i++) {
			weight = anim.GetLayerWeight (i);
			if (weight > highest_weight) {
				weight = highest_weight;
				dominantIndex = i;
			}
		}

		if (dominantIndex != 0)
			anim.SetLayerWeight (dominantIndex, 0.0f);
	}


	//On going triggers
	void OnTriggerEnter2D(Collider2D item){
		try{
			item.gameObject.GetComponent<SpriteRenderer> ().enabled = false;	
		}catch(Exception e){
			Debug.LogException (e, this);
		}
			

		//changes playerstate when player picks up a new item
		if (item.tag == "rifle") { 
			
			state = PlayerState.Rifle;
			setLayers ();

		} else if (item.tag == "knife") {
			
			state = PlayerState.Knife;
			setLayers ();

		} else if (item.tag == "shotgun") {
			
			state = PlayerState.ShotGun;
			setLayers ();

		}else if (item.tag == "handgun") {

			state = PlayerState.HandGun;
			setLayers ();
		}
	}

	// iterates through the list of playerstates
	void cycleWeapons(){
		
		time -= Time.deltaTime;
		if (time <= 0) {
			state += 1;
			if ((int)state == numberOfStates)
				state = 0;
			time = 0.2f;
		}

	}


}
