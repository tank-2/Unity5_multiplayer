using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class other_player_controller : MonoBehaviour {
	//private Dictionary <string, GameObject> playerDictionary;
	public Transform Surrogate;
	//private static other_player_controller playerManager;
	public GameObject Minion;
	public Dictionary<string, GameObject> playerDictionary;
	/*public static other_player_controller instance
	{
		get
		{
			if (!playerManager)
			{
				playerManager = FindObjectOfType (typeof (other_player_controller)) as other_player_controller;

				if (!playerManager)
				{
					Debug.LogError ("There needs to be one active playerManger script on a GameObject in your scene.");
				}
				else
				{
					playerManager.Init (); 
				}
			}

			return playerManager;
		}
	}
	*/
	void Init ()
	{
		if (playerDictionary == null)
		{
			playerDictionary = new Dictionary<string, GameObject>();
		}
	}

	public void UpdatePlayer(Telemetry tele){
		GameObject this_player = null;
		if (playerDictionary.TryGetValue (tele.identity, out this_player))
		{
			this_player.GetComponent<other_player_lerp>().pos = new Vector3 (
			//this_player.transform.position=new Vector3(
				/*(float)tele.world_position.x + (float)tele.head_position.x + (float)Surrogate.transform.position.x, 
				(float)tele.world_position.y + (float)tele.head_position.y + (float)Surrogate.transform.position.y, 
				(float)tele.world_position.z + (float)tele.head_position.z + (float)Surrogate.transform.position.z);
				*/

				/*(float)tele.head_position.x , 
				(float)tele.head_position.y , 
				(float)tele.head_position.z );*/
				(float)tele.world_position.x + (float)Surrogate.transform.position.x, 
				(float)tele.world_position.y + (float)Surrogate.transform.position.y, 
				(float)tele.world_position.z + (float)Surrogate.transform.position.z-4.0f);

			this_player.gameObject.transform.GetChild(0).gameObject.transform.localPosition = new Vector3(
				(float)tele.head_position.x , 
				(float)tele.head_position.y , 
				(float)tele.head_position.z);
			
			this_player.gameObject.transform.GetChild(0).gameObject.transform.localRotation = new Quaternion (
				(float)tele.head_rotation.x , 
				(float)tele.head_rotation.y , 
				(float)tele.head_rotation.z ,
				(float)tele.head_rotation.w);
			//this_player.gameObject.transform.GetChild(0).gameObject.transform.Rotate (new Vector3 (0, 180, 0));
			//here
			this_player.gameObject.transform.GetChild(1).gameObject.transform.localPosition=new Vector3(
				(float)tele.cleft_position.x, 
				(float)tele.cleft_position.y, 
				(float)tele.cleft_position.z);
			this_player.gameObject.transform.GetChild(1).gameObject.transform.rotation=new Quaternion(
				(float)tele.cleft_rotation.x, 
				(float)tele.cleft_rotation.y, 
				(float)tele.cleft_rotation.z,
				(float)tele.cleft_rotation.w);

			this_player.gameObject.transform.GetChild(2).gameObject.transform.localPosition=new Vector3(
				(float)tele.cright_position.x, 
				(float)tele.cright_position.y, 
				(float)tele.cright_position.z);
			this_player.gameObject.transform.GetChild(2).gameObject.transform.rotation=new Quaternion(
				(float)tele.cright_rotation.x, 
				(float)tele.cright_rotation.y, 
				(float)tele.cright_rotation.z,
				(float)tele.cright_rotation.w);
			//this_player.gameObject.GetComponentInChildren<

			//new Vector3 (float.Parse (world_position [0].ToString())+float.Parse (head_position [0].ToString())+Surrogate.transform.position.x,
				//float.Parse (world_position [1].ToString())+float.Parse (head_position [1].ToString())+Surrogate.transform.position.y,	
				//float.Parse (world_position [2].ToString())+float.Parse (head_position [2].ToString())+Surrogate.transform.position.z);

		
				
		} 
	}

	public void RegisterPlayer (string playerName)//, GameObject this_player)
	{
		GameObject this_player = null;
		if (playerDictionary.TryGetValue (playerName, out this_player))
		{
			//this_player.AddListener (listener);
		} 
		else
		{
			this_player = (GameObject)Instantiate (Minion, new Vector3 (0,0,0), Quaternion.identity); //new UnityEvent ();
			playerDictionary.Add (playerName, this_player);
		}
	}

	void Awake(){
		Init ();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
