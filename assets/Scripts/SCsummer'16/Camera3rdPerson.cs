using UnityEngine;
using System.Collections;

public class Camera3rdPerson : MonoBehaviour {

	Transform playerTransform;
	Vector3 standardPos;
	Vector3 relCameraPos;
	public float smooth;
	public float smoothRot;
	float height = 4f;
	float distance = 9f;
	float playerRotationAngle;

	// Use this for initialization
	void Start () 
	{
		playerTransform = GameObject.FindGameObjectWithTag ("Player").transform;
		transform.position = playerTransform.position  + new Vector3 (0,5,0);
		smooth = 5.5f;
		smoothRot = 10f;
		relCameraPos = new Vector3 (0,5,0);
		Screen.orientation = ScreenOrientation.Portrait;
	}
	
	// Update is called once per frame
	void Update () 
	{	
		Vector3 camFocus = playerTransform.position + new Vector3 (0, 3, 0);
		//transform.LookAt(camFocus);

		playerRotationAngle = playerTransform.rotation.eulerAngles.y;

		if (playerRotationAngle >= 240 && playerRotationAngle <= 300)					// For 270
			relCameraPos = new Vector3 (distance, height, 0);
		else if (playerRotationAngle >= 140 && playerRotationAngle <= 220)				// For 180
			relCameraPos = new Vector3 (0, height, distance);
		else if (playerRotationAngle >= 50 && playerRotationAngle <= 130)				// For 90
			relCameraPos = new Vector3 (-distance, height, 0);
		else if (playerRotationAngle >= 320 || playerRotationAngle <= 40)				// For 0 - 360
			relCameraPos = new Vector3 (0, height, -distance);
		
		standardPos = playerTransform.position + relCameraPos;



		if (Vector3.Distance (transform.position, standardPos) > 1) {
			transform.position = Vector3.Lerp (transform.position, standardPos, smooth * Time.deltaTime);
		} else {
			transform.position = standardPos;
		}
		rotateCamera (camFocus);
		cameraThroughWall (camFocus);

	}

	void cameraThroughWall(Vector3 camFocus)
	{
		RaycastHit hit;
		Debug.DrawRay (camFocus, transform.position - camFocus, Color.yellow);

		if (Physics.Raycast (camFocus, transform.position - camFocus, out hit, 9f)) {
			if (hit.collider.transform.tag == "Wall") {
				distance = hit.distance - 0.1f;
			} 
			if (hit.collider.transform.tag == "Door") {
				distance = hit.distance - 0.1f;
			} 
		} else {
			distance = 9f;
		}

	}
	 
	void rotateCamera(Vector3 playerPos) {
		Vector3 direction = playerPos - transform.position;
		Quaternion newRot = Quaternion.LookRotation (direction);
		//if (Quaternion.Angle (transform.rotation, newRot) > 1f) {
		//	transform.rotation = Quaternion.Lerp (transform.rotation, newRot, smoothRot * Time.deltaTime);
		//} else {
			transform.rotation = newRot;
		//}
	}
}
