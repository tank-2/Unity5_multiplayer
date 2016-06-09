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
/*
[Serializable]
public class Pos{
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
}

[Serializable]
public class Rot{
	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
	public float w { get; set; }
}

[Serializable]
public class Telemetry{
	public Pos head_position { get; set; }
	public float timestamp { get; set; }
	public string identity {get;set;}
	public Pos cleft_position { get; set; }
	public Rot cright_rotation { get; set; }
	public Pos cright_position { get; set; }
	public Pos world_position { get; set; }
	public Rot head_rotation { get; set; }
	public Rot cleft_rotation { get; set; }
}
*/
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
}
[Serializable]
public class Command{
	public string this_command { get; set;}
}
[Serializable]
public class Packet{
	public Telemetry tele { get; set; }
	public Command command { get; set; }
}
*/

public class StatefulMain : MonoBehaviour {
    public StateMachine stateMachine;
	public other_player_controller playerController;
	public CommandInterpreter mCommandInterpreter;
    public WebSocket ws;

	//TODO move this to a custom handler
	public string thisname = "";
	public string text = "";
	public string this_command = "";
	public float l = 0;
	public float w = 0;
	public float h = 0;
	public float x = 0;
	public float y = 0;
	public float z = 0;
	public float colorr,colorg,colorb;
	public int input = 0;
	public int output = 0;
	public int aux=0; 
	public bool identified =false;
	//public string named_inputs ="";

	public Transform Tube;
	public Transform NamedInput;
	public Transform NewNode;
	public Transform InputConnection;
	public ArrayList other_players_uuids;
	//public System.List<string> other_players_uuids;// = new List<string>();
	//private string[] other_players_uuids;// = new string[];
    void Start() {
		other_players_uuids = new ArrayList ();//new List<string>();//string[] {"test"};
		//other_players_uuids.
		//other_players_uuids=new List<String>();
        ws = new WebSocket("ws://www.doodlegrid.com:9000");
		//other_players_uuids.Add ("test");
        ws.OnOpen += OnOpenHandler;
        ws.OnMessage += OnMessageHandler;
        ws.OnClose += OnCloseHandler;

        stateMachine.AddHandler(State.Running, () => {
            new Wait(this, 0.1f, () => {
				//automatically connect
                ws.ConnectAsync();
            });
        });

        stateMachine.AddHandler(State.Connected, () => {
            stateMachine.Transition(State.Ping);
			new Wait(this, 0.05f, () => {
				//automatically connect
				string uniqueID = SystemInfo.deviceUniqueIdentifier;
				ws.SendAsync("[{\"proto\":{\"identity\":\""+uniqueID+"\"}}]", OnIdentComplete);
			});
        });
			

        stateMachine.AddHandler(State.Pong, () => {
        });

        stateMachine.Run();
    }

    private void OnOpenHandler(object sender, System.EventArgs e) {
        Debug.Log("WebSocket connected!");
        stateMachine.Transition(State.Connected);
    }
	private void updatePlayer(){
		//update code
	}

	private void OnMessageHandler(object sender, MessageEventArgs e) {
		//Debug.Log (e.Data);
		UnityMainThreadDispatcher.Instance ().Enqueue (InterpretCommand(e.Data));
		//Packet test_pack = JsonMapper.ToObject<Packet> (e.Data);
		//Debug.Log (test_pack.tele.identity);
		//Debug.Log (test_pack.tele.head_position.y);
	}
    private void XOnMessageHandler(object sender, MessageEventArgs e) {

		Packet this_packet = JsonUtility.FromJson<Packet> (e.Data);
		Packet test_pack = JsonMapper.ToObject<Packet> (e.Data);
		Debug.Log (e.Data);
		Debug.Log (test_pack.tele.identity);
		Debug.Log (test_pack.tele.head_position.y);
		//Debug.Log (this_packet.command);
		//Debug.Log (this_packet.tele.identity);
		//Debug.Log (e.Data);
		//Debug.Log ((this_packet.command.this_command==null));
		JsonData data = JsonMapper.ToObject(e.Data);
		if (this_packet.tele!=null){
		//if (data.Keys.Contains ("tele")) {
			//JsonData this_data = data ["tele"];
			//this_data.

			//Telemetry tele = JsonMapper.ToObject<Telemetry> (data ["tele"]);
			//Debug.Log ("tele ident");
			//Debug.Log (tele.identity);
			//Debug.Log ("done");
			Debug.Log(this_packet.tele.identity);
			if (!other_players_uuids.Contains(this_packet.tele.identity)){
			//if (!other_players_uuids.Contains (data ["tele"]["identity"].ToString())) {
				other_players_uuids.Add (data ["tele"]["identity"].ToString());
				Debug.Log("added new uuid");
				//SPAWN player now from main thread
				UnityMainThreadDispatcher.Instance ().Enqueue (MAKEplayer(data ["tele"]["identity"].ToString()));
				//register an event listener
				//other_players_EventManager.StartListening (data ["tele"]["identity"].ToString(), updatePlayer);
			} else {
				UnityMainThreadDispatcher.Instance ().Enqueue (UPDATEplayer (data ["tele"] ["identity"].ToString (), data ["tele"] ["world_position"], data ["tele"] ["head_position"]));
				//use world position and head position
				//Debug.Log(data["tele"]["head_position"][0].ToString());
				//UnityMainThreadDispatcher.Instance ().Enqueue (UPDATEplayer(data ["tele"]["identity"].ToString(), data['tele']['world_position']);
				//fire an event for updating the players state
			}
		}
		if (data.Keys.Contains("command")){
			this_command = data ["command"].ToString ();
			if (this_command == "make_block") {
				thisname = data ["name"].ToString ();
				text = data ["text"].ToString ();
				output = int.Parse (data ["output"].ToString ());
				input = int.Parse (data ["input"].ToString ());
				l = float.Parse (data ["l"].ToString ());
				w = float.Parse (data ["w"].ToString ());	
				h = float.Parse (data ["h"].ToString ());	
				x = float.Parse (data ["x"].ToString ());	
				y = float.Parse (data ["y"].ToString ());	
				z = float.Parse (data ["z"].ToString ());
				colorr = float.Parse (data ["r"].ToString ());
				colorg = float.Parse (data ["g"].ToString ());
				colorb = float.Parse (data ["b"].ToString ());
				CallMainThread (input, output, x, y, z, l, w, h, thisname, text, colorr, colorg, colorb, data["named_inputs"]);
			}
		}
    }


	public IEnumerator InterpretCommand(string command){
			mCommandInterpreter.process(command);
		yield return null;
	}
	public IEnumerator MAKEplayer(string identity){
		playerController.RegisterPlayer(identity);
		yield return null;
	}

	public IEnumerator UPDATEplayer(string identity, LitJson.JsonData world_position, LitJson.JsonData head_position){
		//playerController.UpdatePlayer(identity, world_position, head_position);
		yield return null;
	}

	public IEnumerator MAKEblocks(int input, int output, float x, float y, float z, float l, float w, float h, string name, string text, float colorr, float colorg, float colorb, LitJson.JsonData named_inputs){
		//this will be executed on the main thread
		//create a new main node block and draw the name of it
		Transform newNodeTransform = (Transform)Instantiate (NewNode, new Vector3 (x, y, z), Quaternion.identity);
		Renderer rend = newNodeTransform.GetComponent<Renderer>();
		rend.material.color = new Color(colorr,colorg,colorb);
		TextMesh nodeTxtMesh = newNodeTransform.GetComponent<TextMesh> ();
		nodeTxtMesh.text = name;

		//create the leaving tube
		Transform tubeTransform = (Transform)Instantiate (Tube, new Vector3 (x-0.07f, y, z-0.005f), Quaternion.identity);
		tubeTransform.transform.parent = newNodeTransform;

		//create this main nodes input connector
		Transform mainInputTransform = (Transform)Instantiate (InputConnection, new Vector3 (x+0.07f, y, z-0.005f), Quaternion.identity);
		mainInputTransform.transform.parent = newNodeTransform; 

		//create the auxiliary inputs 
		for (int i = 0; i < named_inputs.Count; i++) {
			Transform namedInputTransform = (Transform)Instantiate (NamedInput, new Vector3 (x, y - (i+1)*0.05f, z), Quaternion.identity);
			namedInputTransform.transform.parent = newNodeTransform; 
			TextMesh txtMesh = namedInputTransform.GetComponent<TextMesh> ();
			txtMesh.text = named_inputs[i].ToString();
			//create this auxiliary inputs connnetor
			Transform inputTransform = (Transform)Instantiate (InputConnection, new Vector3 (x+0.07f, y - (i+1)*0.05f, z-0.005f), Quaternion.identity);
			inputTransform.transform.parent = newNodeTransform; 
		}
		yield return null;
	}

	public void CallMainThread(int input, int output, float x, float y, float z, float l, float w, float h, string name, string text, float colorr, float colorg, float colorb, LitJson.JsonData named_inputs){
		UnityMainThreadDispatcher.Instance ().Enqueue (MAKEblocks (input, output, x, y, z, l, w, h, name, text, colorr, colorg, colorb, named_inputs));
	}

    private void OnCloseHandler(object sender, CloseEventArgs e) {
        Debug.Log("WebSocket closed with reason: " + e.Reason);
        stateMachine.Transition(State.Done);
    }

    private void OnSendComplete(bool success) {
        Debug.Log("Message sent successfully? " + success);
    }
	private void OnIdentComplete(bool success) {
		identified = success;
	}
}
