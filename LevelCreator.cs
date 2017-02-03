using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelCreator : MonoBehaviour {

	//inspector parameters
	[Tooltip("List of available weapon prefabs")]
	public GameObject[] weapons;
	public GameObject Player, upperWall, lowerWall, wall, gate, floor, GameLayer;
	public Text ammo_count;
	public Image weapon_image;
	[Tooltip("weapon images")]
	public Sprite iRifle, iHandgun, iShotgun, iKnife, iFlashlight;


	GameObject box, barrel, reward;
	float boxPos, barrelPos;
	Vector3 cameraPos, playerPos;
	PlayerHandler PH;
	int ammo_ct;


	// Use this for initialization
	void Start () {

		PH = Player.GetComponent<PlayerHandler> ();
		box = (GameObject)Resources.Load ("big_box") as GameObject;
		barrel = (GameObject)Resources.Load ("barrel") as GameObject;
		reward = GameObject.Find ("Rewards") as GameObject;
		int i = 1;
	
		//instantiates 20 copies of upper and lower walls 
		for (; i < 41; i++) {
			
			boxPos = Random.Range (-5f, 5f);

			barrelPos = Random.Range (-5f, 5f);

			GameObject upper = Instantiate (upperWall, new Vector2 (i * 5.0f, upperWall.transform.position.y), Quaternion.identity) as GameObject;
			upper.transform.parent = wall.transform;

			GameObject lower = Instantiate (lowerWall, new Vector2 (i * 5.0f, lowerWall.transform.position.y), Quaternion.identity) as GameObject;
			lower.transform.parent = wall.transform;

		}

		//gate appears in the end 
		gate.SetActive (true);

		gate.transform.position = new Vector2(i*5.0f, gate.transform.position.y);

		//instantiates boxes and barrels between the walls at random distances
		for(int k=1; k<21; k++){		
			
			GameObject boxes = Instantiate (box, new Vector2(k*10.0f + boxPos, Random.Range(-2f,2f)),Quaternion.identity) as GameObject;
			boxes.transform.parent = GameLayer.transform;

			GameObject barrels = Instantiate (barrel, new Vector2(k*10.0f + 2.0f + barrelPos, Random.Range(-2f,2f)),Quaternion.identity) as GameObject;
			barrels.transform.parent = GameLayer.transform;

			GameObject floors = Instantiate (floor, new Vector3 (k * 10.0f, floor.transform.position.y, 0.1F), Quaternion.identity) as GameObject;
			floors.transform.localEulerAngles = new Vector3(-90f,0f,0f);
			floors.transform.parent = wall.transform;

		}

		//instantiate weapons from weapons array randomly
		for (int j = 1; j < 9; j++) {
			
			GameObject weapon_spawn = Instantiate(weapons[Random.Range(0,weapons.Length)], 
				new Vector2(j*20.0f + boxPos, Random.Range(-2f,2f)),Quaternion.identity) as GameObject;
			weapon_spawn.transform.parent = reward.transform;

		}
}
	
	// Update is called once per frame
	void Update () {

		ammo_ct = PH.ammo;
		
		//Move the camera with player
		playerPos = Player.transform.position;
		cameraPos = new Vector3 (playerPos.x, 0f, -6f);
		transform.position = cameraPos;		

		//sets the UI image to current weapon
		if (PH.state == PlayerHandler.PlayerState.FlashLight)
			weapon_image.sprite = iFlashlight;
		if (PH.state == PlayerHandler.PlayerState.Rifle)
			weapon_image.sprite = iRifle;
		if (PH.state == PlayerHandler.PlayerState.ShotGun)
			weapon_image.sprite = iShotgun;
		if (PH.state == PlayerHandler.PlayerState.HandGun)
			weapon_image.sprite = iHandgun;
		if (PH.state == PlayerHandler.PlayerState.Knife)
			weapon_image.sprite = iKnife;

		//updates ammo count in UI
		ammo_count.text = "Ammo x " + ammo_ct.ToString();

	}

}
