using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class Altitude : MonoBehaviour {
	private Vector3 telemetry;
	private Vector3 head_position;
	private Quaternion head_rotation;
	private Vector3 cleft_pos;
	private Vector3 cright_pos;
	private Quaternion cleft_rot;
	private Quaternion cright_rot;
	public StatefulMain wsServer;
	private TextMesh textupdate;
	public GameObject mainCamera;
	public GameObject cLeft;
	public GameObject cRight;
	private void Start() {
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		//cLeft = GameObject.FindGameObjectWithTag ("Controller (left)");
		//cRight = GameObject.FindGameObjectWithTag ("Controller (right)");
		wsServer = transform.parent.gameObject.GetComponent<StatefulMain> ();
		textupdate = gameObject.GetComponentInChildren<TextMesh> ();
		InvokeRepeating("RepeatingFunction", 1, 0.03333f); //30Hz
	}
	//70.7,1722.03,-130.66
	void RepeatingFunction(){
		head_position = mainCamera.transform.localPosition;//.ToString ("F4");
		head_rotation = mainCamera.transform.localRotation;//.ToString ("F4");
		cleft_pos = cLeft.transform.localPosition;//.ToString ("F4");
		cleft_rot = cLeft.transform.localRotation;//.ToString ("F4");
		cright_pos = cRight.transform.localPosition;//.ToString ("F4");
		cright_rot = cRight.transform.rotation;//.ToString ("F4");
		telemetry = transform.parent.Find("Surrogate").transform.position;//.ToString("F4");
		if (wsServer.ws.IsConnected & wsServer.identified)
			wsServer.ws.SendAsync("[{\"tele\":" +
				"{\"world_position\":{"+
					"\"x\":"+telemetry.x.ToString("F6")+","+
					"\"y\":"+telemetry.y.ToString("F6")+","+
					"\"z\":"+telemetry.z.ToString("F6")+"},"+
				"\"timestamp\":"+ Time.realtimeSinceStartup.ToString()+","+
				"\"head_position\":{"+
					"\"x\":"+head_position.x.ToString("F6")+","+
					"\"y\":"+head_position.y.ToString("F6")+","+
					"\"z\":"+head_position.z.ToString("F6")+"},"+
				"\"cleft_position\":{"+ 
					"\"x\":"+cleft_pos.x.ToString("F6")+","+
					"\"y\":"+cleft_pos.y.ToString("F6")+","+
					"\"z\":"+cleft_pos.z.ToString("F6")+"},"+
				"\"cleft_rotation\":{"+
					"\"x\":"+cleft_rot.x.ToString("F4")+","+
					"\"y\":"+cleft_rot.y.ToString("F4")+","+
					"\"z\":"+cleft_rot.z.ToString("F4")+","+
					"\"w\":"+cleft_rot.w.ToString("F4")+"},"+
				"\"cright_position\":{"+
					"\"x\":"+cright_pos.x.ToString("F4")+","+
					"\"y\":"+cright_pos.y.ToString("F4")+","+
					"\"z\":"+cright_pos.z.ToString("F4")+"},"+

				"\"cright_rotation\":{"+ 
					"\"x\":"+cright_rot.x.ToString("F4")+","+
					"\"y\":"+cright_rot.y.ToString("F4")+","+
					"\"z\":"+cright_rot.z.ToString("F4")+","+
					"\"w\":"+cright_rot.w.ToString("F4")+"},"+
				"\"head_rotation\":{"+ 
					"\"x\":"+head_rotation.x.ToString("F4")+","+
					"\"y\":"+head_rotation.y.ToString("F4")+","+
					"\"z\":"+head_rotation.z.ToString("F4")+","+
					"\"w\":"+head_rotation.w.ToString("F4")+"}"+
				"}}]", OnSendComplete);
		textupdate.text = telemetry.ToString();

	}

	private void OnSendComplete(bool success) {
		//Debug.Log("Message sent successfully? " + success);
	}
}