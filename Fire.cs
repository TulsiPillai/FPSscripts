using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	//Inspector parameters
	public GameObject bullet_prefab; 			//you can add more prefabs and customize the bullet sprite
	public Transform bulletSpawn;				//position from where the bullet needs to be instantiated

	PlayerHandler PH;
	float instantiateTime;

	// Use this for initialization
	void Start () {
		
		PH = gameObject.GetComponent<PlayerHandler> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void fireBullet(){

		//fires every 0.2 seconds and destroys the bullet instance after 1 second
		instantiateTime -= Time.deltaTime;

		if (instantiateTime <= 0) {
			if ( PH.ammo > 0) {
				
				GameObject bullet = Instantiate (bullet_prefab) as GameObject;

				bullet.transform.position = bulletSpawn.position;

				bullet.name = "Bullet";

				bullet.GetComponent<Rigidbody2D> ().AddForce (this.transform.right * PH.power);

				instantiateTime = 0.2f;

				Destroy (bullet, 1.0f);

				PH.ammo--;
			} else {
				
				//Debug.Log ("No ammo");
			}
		}
	}
}
