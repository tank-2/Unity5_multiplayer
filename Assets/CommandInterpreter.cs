using UnityEngine;
using WebSocketSharp;
using LitJson;
//using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net.Sockets;
//using LitJson;
using System.Net;
using System.Collections.Generic;

//[Serializable]
public class Pos{
	public double x { get; set; }
	public double y { get; set; }
	public double z { get; set; }
}

//[Serializable]
public class Rot{
	public double  x { get; set; }
	public double y { get; set; }
	public double z { get; set; }
	public double w { get; set; }
}

//[Serializable]
public class Telemetry{
	public Pos head_position { get; set; }
	public double timestamp { get; set; }
	public string identity {get;set;}
	public Pos cleft_position { get; set; }
	public Rot cright_rotation { get; set; }
	public Pos cright_position { get; set; }
	public Pos world_position { get; set; }
	public Rot head_rotation { get; set; }
	public Rot cleft_rotation { get; set; }
}

/*
[Serializable]
public class Telemetry{
	public Vector3 head_position { get; set; }
	public float timestamp { get; set; }
	public string identity {get;set;}
	public Vector3 cleft_position { get; set; }
	public Vector4 cright_rotation { get; set; }
	public Vector3 cright_position { get; set; }
	public Vector3 world_position { get; set; }
	public Vector4 head_rotation { get; set; }
	public Vector4 cleft_rotation { get; set; }
}

 * [Serializable]
public class Telemetry{
	public string identity { get; set; }
	public string noid { get; set; }
}*/

[Serializable]
public class Command{
	public string this_command { get; set;}
}
[Serializable]
public class Packet{
	public Telemetry tele { get; set; }
	public Command command { get; set; }
}



public class CommandInterpreter : MonoBehaviour {

	public other_player_controller playerController;
	public ArrayList other_players_uuids;
	public void process(string command){
		Packet this_packet = JsonMapper.ToObject<Packet> (command);
		//Packet test_pack = JsonUtility.FromJson<Packet> (command);
		//Debug.Log (test_pack.tele.identity);
		if (this_packet.tele != null) {
			if (!other_players_uuids.Contains (this_packet.tele.identity)) {
				//if (!other_players_uuids.Contains (data ["tele"]["identity"].ToString())) {
				other_players_uuids.Add (this_packet.tele.identity);
				Debug.Log ("added new uuid");
				playerController.RegisterPlayer(this_packet.tele.identity);
				//SPAWN player now from main thread
				//UnityMainThreadDispatcher.Instance ().Enqueue (MAKEplayer(data ["tele"]["identity"].ToString()));
				//register an event listener
				//other_players_EventManager.StartListening (data ["tele"]["identity"].ToString(), updatePlayer);
			} else {
				playerController.UpdatePlayer(this_packet.tele);
				//UnityMainThreadDispatcher.Instance ().Enqueue (UPDATEplayer (data ["tele"] ["identity"].ToString (), data ["tele"] ["world_position"], data ["tele"] ["head_position"]));
				//use world position and head position
				//Debug.Log(data["tele"]["head_position"][0].ToString());
				//UnityMainThreadDispatcher.Instance ().Enqueue (UPDATEplayer(data ["tele"]["identity"].ToString(), data['tele']['world_position']);
				//fire an event for updating the players state
			}
		}


	}

	// Use this for initialization
	void Start () {
		other_players_uuids = new ArrayList ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
