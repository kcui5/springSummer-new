using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using SimpleJSON2;

public class PlayerPathCCNY: MonoBehaviour
{

	public Transform target;
	public Transform player; 
	public NavMeshAgent agent;
	public int distanceFeet;
	public int distanceMeter;
	bool isAutoGerneratePath;
	private int totalDistanceF; // total distance in Feet
	private int totalDistanceM; // total distance in meter
	private bool isFeet; //if the unit is feet, by default is false

	//private Vector3 startPoint;
	//private Vector3 endPoint;
	//private string nextDirection;
	private string crntDirection;
	private string fourDir;
	private Vector3 startPoint;
	private Vector3 endPoint;
	//RoomLocations rm;

	private int numWayPoint;
	private Vector3 diff;
	private Vector3 mainDiff;
	//private float degree; 
	private NavMeshPath path;
	private float elapsed;
	private float updateFreq = 0.5f;

	private float timer;
	//private int once;
	private string curMsg;
	private string difMsg;

	GameObject[] rooms;
	Vector3[] roomPosition;
	double difficulty; 




	int curVal;
	NextDirection nextDir;
	RoomLocationsConf rlc;
	TheInformationBridge infoBrg;



	void OnApplicationQuit()
	{
		EasyTTSUtil.Stop();
	}

	// Use this for initialization
	void Start()
	{
		EasyTTSUtil.Initialize (EasyTTSUtil.UnitedStates);

		nextDir = new NextDirection();
		rlc = new RoomLocationsConf ();
		infoBrg = new TheInformationBridge ();
		curVal = 0;
		startPoint = rlc.setStartingPoint ();
		endPoint = rlc.setEndingPoint ();
		agent.enabled = false;
		isFeet = false;

		player.transform.position = rlc.setStartingPoint();
        Debug.Log("Player Path Starting Point 1: " + player.transform.position);
        string startingPosition = "(" + player.transform.position.x + ", " + player.transform.position.y + ", " + player.transform.position.z + ") \r\n";
        Debug.Log("Player Path Starting Point 2: " + startingPosition);
        infoBrg.setStartingPosition(startingPosition);
        Debug.Log(player.transform.position);

		//Debug.Log (player.position+"********************");
		//player.transform.rotation = ;
		nextDir.setStart(rlc.setStartingPoint());
		target.transform.position = rlc.setEndingPoint();

		agent.enabled = true;


		path = agent.path;

		//nextDirection = "";
		crntDirection = "";
		fourDir = " ";

		//degree = 0;           No longer used to get next direction to way point

		numWayPoint = path.corners.Length;
		elapsed = 0;

		isAutoGerneratePath = infoBrg.getAutoGenerateStates ();
		if (isAutoGerneratePath)
			SaveJson ();
		curMsg = "constantly redefined";
		difMsg = "redefined only is curmsg changes";
		//Debug.Log (player.position+"++++++++");
		//for (int i = 0; i < path.corners.Length; i++)
			//Debug.Log (path.corners[i]);
		calculatePath();
		adjustDirection ();
		summary ();


		/*
		if (isAutoGerneratePath) {
			if (rooms == null)
				rooms = GameObject.FindGameObjectsWithTag ("Door");
			//Debug.Log (rooms.Length);
			roomPosition = new Vector3[rooms.Length];
			int i = 0;
			foreach (GameObject room in rooms) {
				roomPosition [i] = new Vector3(room.transform.position.x, 2f, room.transform.position.z);
				Debug.Log (roomPosition[i]);
				i++;
			}


			Vector3[] a = new Vector3[2];
			for(int j = 0; j < roomPosition.Length; j++){
				for(int k = 0; k < roomPosition.Length; k++){
					if(j != k){
						Debug.Log ("start: " + roomPosition[j] + "End: " + roomPosition[k]);

						Debug.Log (totalRoomPassedBy (roomPosition[j],roomPosition[k]) + " rooms pass by. "
						           + totalPossibleTurn (roomPosition[j],roomPosition[k]) + " turns include."
						           + DifficultWeight (roomPosition[j],roomPosition[k]) + " turn weight."
						           + totalDistance(roomPosition[j],roomPosition[k]) + " Distance."
						           + (totalRoomPassedBy (roomPosition[j],roomPosition[k]) * 2 + totalPossibleTurn (roomPosition[j],roomPosition[k])* 2
						   + DifficultWeight (roomPosition[j],roomPosition[k]) * 10 + totalDistance(roomPosition[j],roomPosition[k])*0.1) + " DIfficutly.");

						difficulty = totalRoomPassedBy(roomPosition[j],roomPosition[k]) * 2
								+ totalPossibleTurn(roomPosition[j],roomPosition[k]) * 2
								+ DifficultWeight(roomPosition[j],roomPosition[k]) * 10
								+ (totalDistance(roomPosition[j],roomPosition[k]) * 0.1);

						//Debug.Log (difficulty);
						if(difficulty < 20 && difficulty >= 0){
							Debug.Log ("add success");
							levelOne.Add(a);
							//Debug.Log ("add success");
						}
						if(difficulty < 50 && difficulty >= 20){
							levelTwo.Add(a);
						}
						if(difficulty < 80 && difficulty >= 50){
							levelThree.Add(a);
						}
						if(difficulty >= 80){
							levelFour.Add(a);
						}

					}

				}
			}
			//json.Add ("Level one", levelOne.ToArray());

			//Debug.Log(levelOne);

			//for (int j = 0; j < roomPosition.Length; j++)
			//Debug.Log (roomPosition[j]);
		}*/
	}

	// Update is called once per frame
	void Update()
	{
		if (!infoBrg.getReachTarget()) {
			//Debug.Log (path.corners[1] + " - " + transform.position + " = " + (path.corners[1] - transform.position));
			//Debug.Log (transform.forward);
			elapsed += Time.deltaTime;
			if (elapsed > updateFreq) {
				elapsed -= updateFreq;
				calculatePath ();
				calculateDirection ();	
				adjustPosition ();
				infoBrg.setInstruct (newInstruct2 ());
				//infoBrg.setInstruct(nextInstruction ());// Sent to 'PlayerMovementCCNY.cs'
				distanceInformation ();
				calcAngleDifference ();
			}
			drawPath ();

		}
		//if (path.corners.Length != curVal) {
			//Debug.Log ("Num of Waypoints: " + path.corners.Length);		//<----DEBUGGING: Number of waypoints
			//curVal = path.corners.Length;
	}

		//autoWalkToNextWayPoint ();


	void SaveJson(){
		JSONObject json = new JSONObject ();
		JSONObject lvOne = new JSONObject ();
		JSONObject lvTwo = new JSONObject ();
		JSONObject lvThree = new JSONObject ();
		JSONObject lvFour = new JSONObject ();

		JSONArray startOne = new JSONArray ();
		JSONArray endOne = new JSONArray();
		JSONArray startTwo = new JSONArray ();
		JSONArray endTwo = new JSONArray();
		JSONArray startThree = new JSONArray ();
		JSONArray endThree = new JSONArray();
		JSONArray startFour = new JSONArray ();
		JSONArray endFour = new JSONArray();


			if (rooms == null)
				rooms = GameObject.FindGameObjectsWithTag ("Door");
			//Debug.Log (rooms.Length);
			roomPosition = new Vector3[rooms.Length];
			int i = 0;
			foreach (GameObject room in rooms) {
				roomPosition [i] = new Vector3(room.transform.position.x, 2f, room.transform.position.z);
				Debug.Log (roomPosition[i]);
				i++;
			}
			
			/*Debug.Log (totalRoomPassedBy (startPoint, endPoint) + " rooms pass by. "
						+ totalPossibleTurn (startPoint, endPoint) + " turns include."
			           + DifficultWeight (startPoint, endPoint) + " turn weight."
			           + totalDistance(startPoint, endPoint) + " Distance."
			           + (totalRoomPassedBy (startPoint, endPoint) * 2 + totalPossibleTurn (startPoint, endPoint)* 2
			   + DifficultWeight (startPoint, endPoint) * 10 + totalDistance(startPoint, endPoint)*0.1) + " DIfficutly.");
			*/
			Vector3[] a = new Vector3[2];
			for(int j = 0; j < roomPosition.Length; j++){
				for(int k = 0; k < roomPosition.Length; k++){
					if(j != k){
						//Debug.Log ("start: " + roomPosition[j] + "End: " + roomPosition[k]);
						
						/*Debug.Log (totalRoomPassedBy (roomPosition[j],roomPosition[k]) + " rooms pass by. "
						           + totalPossibleTurn (roomPosition[j],roomPosition[k]) + " turns include."
						           + DifficultWeight (roomPosition[j],roomPosition[k]) + " turn weight."
						           + totalDistance(roomPosition[j],roomPosition[k]) + " Distance."
						           + (totalRoomPassedBy (roomPosition[j],roomPosition[k]) * 2 + totalPossibleTurn (roomPosition[j],roomPosition[k])* 2
						   + DifficultWeight (roomPosition[j],roomPosition[k]) * 10 + totalDistance(roomPosition[j],roomPosition[k])*0.1) + " DIfficutly.");
						   */
						
						difficulty = totalRoomPassedBy(roomPosition[j],roomPosition[k]) * 2
							+ totalPossibleTurn(roomPosition[j],roomPosition[k]) * 2
								+ DifficultWeight(roomPosition[j],roomPosition[k]) * 10
								+ (totalDistance(roomPosition[j],roomPosition[k]) * 0.1);
						
						Debug.Log (difficulty);
						if(difficulty < 20 && difficulty >= 0){
							//Debug.Log ("add success");
							JSONArray posStart = new JSONArray();
							JSONArray posEnd = new JSONArray();

							posStart.Add(roomPosition[j].x);
							posStart.Add(roomPosition[j].y);
							posStart.Add(roomPosition[j].z);
							posEnd.Add (roomPosition[k].x);
							posEnd.Add (roomPosition[k].y);
							posEnd.Add (roomPosition[k].z);
							startOne.Add(posStart);
							endOne.Add(posEnd);
							lvOne.Add ("Start", startOne);
							lvOne.Add ("End", endOne);
							//Debug.Log ("add success");
						}
						if(difficulty < 50 && difficulty >= 20){
							JSONArray posStart = new JSONArray();
							JSONArray posEnd = new JSONArray();
							
							posStart.Add(roomPosition[j].x);
							posStart.Add(roomPosition[j].y);
							posStart.Add(roomPosition[j].z);
							posEnd.Add (roomPosition[k].x);
							posEnd.Add (roomPosition[k].y);
							posEnd.Add (roomPosition[k].z);
							startTwo.Add(posStart);
							endTwo.Add(posEnd);
							lvTwo.Add ("Start", startTwo);
							lvTwo.Add ("End", endTwo);
						}
						if(difficulty < 80 && difficulty >= 50){
							JSONArray posStart = new JSONArray();
							JSONArray posEnd = new JSONArray();
							
							posStart.Add(roomPosition[j].x);
							posStart.Add(roomPosition[j].y);
							posStart.Add(roomPosition[j].z);
							posEnd.Add (roomPosition[k].x);
							posEnd.Add (roomPosition[k].y);
							posEnd.Add (roomPosition[k].z);
							startThree.Add(posStart);
							endThree.Add(posEnd);
							lvThree.Add ("Start", startThree);
							lvThree.Add ("End", endThree);
						}
						if(difficulty >= 80){
							JSONArray posStart = new JSONArray();
							JSONArray posEnd = new JSONArray();
							
							posStart.Add(roomPosition[j].x);
							posStart.Add(roomPosition[j].y);
							posStart.Add(roomPosition[j].z);
							posEnd.Add (roomPosition[k].x);
							posEnd.Add (roomPosition[k].y);
							posEnd.Add (roomPosition[k].z);
							startFour.Add(posStart);
							endFour.Add(posEnd);
							lvFour.Add ("Start", startFour);
							lvFour.Add ("End", endFour);
						}
						
					}
					
				}
			}
			//json.Add ("Level one", levelOne.ToArray());
			
			//Debug.Log(levelOne);
			
			//for (int j = 0; j < roomPosition.Length; j++)
			//Debug.Log (roomPosition[j]);
		json.Add ("Level One", lvOne);
		json.Add ("Level two", lvTwo);
		json.Add ("Level three", lvThree);
		json.Add ("Level four", lvFour);
		//Debug.Log (json.ToString());
		String path = Application.persistentDataPath + "/LevleSave.json";
		Debug.Log (Application.persistentDataPath + "/LevleSave.json");
		File.WriteAllText (path, json.ToString());
	}

	void adjustPosition()
	{

		Vector3 diff = path.corners [1] - transform.position;
		//if (nextInstruction () == "Right" || nextInstruction () == "Left") {
			if (transform.forward.x != 0f && (Mathf.Abs (diff.x) < 1) 
			    && (Mathf.Abs (diff.x) > 0) && (Math.Abs (diff.z) < 1)) {
				transform.position = path.corners [1];
			} else if (transform.forward.z != 0f && (Mathf.Abs (diff.z) < 1)
			           && (Mathf.Abs (diff.z) > 0) && (Math.Abs (diff.x) < 1)) {
				transform.position = path.corners [1];
			}

		if (newInstruct2 () == "Behind")
			transform.position = path.corners [1];
		
		//}
	}

	/**
	 * adjust the avatar direction to the next waypoint
	 */
	void adjustDirection()
	{
		String direction = nextInstruction ();
		if (direction == "Right")
			transform.Rotate (0,90f,0);
		if (direction == "Left")
			transform.Rotate (0, -90f, 0);
		if (direction == "Behind")
			transform.Rotate (0, 180f, 0);
	}

	/**
	 * provide a summary about distance to the destination, desitination is in which direction
	 * and which room will be passed by
	 */
	void summary ()
	{

		// distance summary
		totalDistanceF = (int)(Mathf.Sqrt (Mathf.Pow ((endPoint.x - startPoint.x), 2) + 
			Mathf.Pow ((endPoint.z - startPoint.z), 2)) / 3);
		totalDistanceM = totalDistanceF / 3;
		EasyTTSUtil.SpeechAdd ("The distance from the destination is " + totalDistanceM + " meters");

		// direction summary
		Vector3 facing = transform.forward;
		Vector3 toEnd = endPoint - startPoint;
		Debug.Log ("Startpoint:" + startPoint + "\nEndPoint:" + endPoint);
		Debug.Log ("toEnd:" + toEnd);
		toEnd = transform.InverseTransformDirection (toEnd);
		facing = transform.InverseTransformDirection (facing);

		Debug.Log ("toEnd:" + toEnd);
		
		float angle =  Vector3.Angle(toEnd, facing);
			//Mathf.Rad2Deg * (Mathf.Acos ((Vector3.Dot (facing, toEnd)) / (facing.magnitude * toEnd.magnitude)));
		infoBrg.setAngleOfNextWP (angle);
		Debug.Log(angle + "degree");
		Debug.Log (toEnd + "toEnd value");
		if (angle > 165 && angle <= 180) {
			EasyTTSUtil.SpeechAdd ("Your destination is in your six o'clock direction");
			Debug.Log ("Six o'clock direction.");
		}
		if (angle >= 0 && angle <= 15) {
			EasyTTSUtil.SpeechAdd ("Your destination is in your twelve o'clock direction");
			Debug.Log ("twelve o'clock direction.");
		}
		if (angle >= 75 && angle < 105) {
			if (toEnd.x < 0){
				EasyTTSUtil.SpeechAdd ("Your destination is in your nine o'clock direction");
				Debug.Log ("nine  o'clock direction.");
			}else{
				EasyTTSUtil.SpeechAdd ("Your destination is in your three o'clock direction");
				Debug.Log ("three o'clock direction.");
			}
		}

		if (angle > 15 && angle <= 45) {
			if (toEnd.x < 0){
					EasyTTSUtil.SpeechAdd ("Your destination is in your eleven o'clock direction");
				Debug.Log ("eleven o'clock direction.");
			}else{
					EasyTTSUtil.SpeechAdd ("Your destination is in your one o'clock direction");
				Debug.Log ("one o'clock direction.");
				}
		}

		if (angle > 45 && angle < 75) {
			if (toEnd.x < 0){
				EasyTTSUtil.SpeechAdd ("Your destination is in your ten o'clock direction");
				Debug.Log ("ten o'clock direction.");
			}else{
				EasyTTSUtil.SpeechAdd ("Your destination is in your two o'clock direction");
				Debug.Log ("two o'clock direction.");
			}
		}

		if (angle >= 105 && angle < 135) {
			if (toEnd.x < 0){
				EasyTTSUtil.SpeechAdd ("Your destination is in your eight o'clock direction");
							Debug.Log ("eight o'clock direction.");
			}else{
				EasyTTSUtil.SpeechAdd ("Your destination is in your four o'clock direction");
				Debug.Log ("four o'clock direction.");
			}
		}

		if (angle >= 135 && angle <= 165) {
			if (toEnd.x < 0){
				EasyTTSUtil.SpeechAdd ("Your destination is in your seven o'clock direction");
								Debug.Log ("seven o'clock direction.");
			}else{
				EasyTTSUtil.SpeechAdd ("Your destination is in your five o'clock direction");
								Debug.Log ("five o'clock direction.");
			}
		}
		// room passed by summary
		RaycastHit hit;
		float radius = 10;
		string newNearByMsg = "";



		EasyTTSUtil.SpeechAdd ("You will need to turn " + totalPossibleTurn (startPoint, endPoint) 
			+ " times and pass by " + totalRoomPassedBy (startPoint, endPoint) + " rooms");

	}


	void distanceInformation()
	{	
		distanceFeet = (int)(Mathf.Sqrt(	Mathf.Pow((path.corners[1].x - path.corners [0].x),2) + 
									Mathf.Pow((path.corners[1].z - path.corners [0].z),2) )/3);
		distanceMeter = distanceFeet /3;

		infoBrg.setDistanceF (distanceFeet);
		infoBrg.setDistanceM (distanceMeter);
	}

	int totalDistance(Vector3 s, Vector3 e)
	{
		NavMeshPath testPath = new NavMeshPath();
		NavMesh.CalculatePath (s, e, NavMesh.AllAreas, testPath);
		int distance = 0;
		for(int i = 0; i < testPath.corners.Length-1; i++)
		{
			distance  += (int)(Mathf.Sqrt(	Mathf.Pow((testPath.corners[i+1].x - testPath.corners [i].x),2) + 
			                              Mathf.Pow((testPath.corners[i+1].z - testPath.corners [i].z),2) )/3);

			//dis += Vector3.Distance(path.corners[i], path.corners[i+1]);
		}
		//Debug.Log (distance/3 + "meter");
		//Debug.Log (dis);
		return distance;
	}

	public void calculatePath()
	{

		//Recalculates path
		NavMesh.CalculatePath (agent.transform.position, target.position, NavMesh.AllAreas, path);


		if (curMsg != difMsg)
		{
			Debug.Log (curMsg); 
			difMsg = curMsg;
		}
	}

	/*
	 * Draws a line connecting all waypoints from beginning 
	 * to end. *Color of line can be changed*
	 */
	public void drawPath()
	{
		// Draws out the line along the path
		for (int i = 0; i < path.corners.Length - 1; i++)
			Debug.DrawLine (path.corners[i], path.corners[i + 1], Color.white);
		
	}
	public void calculateDirection()
	{
		String s = "";
		for (int i = 0; i< path.corners.Length; i++) {
			s+= " " + path.corners[i];
		}

		//Debug.Log (s);
		// diff = Vector3 FROM currentPoint TO origin(nextPoint)
		if (path.corners != null && path.corners.Length >1) {
			diff = path.corners [1] - path.corners [0];
			//findMajorDirection (diff);
		}
		/*
                degree = calculateDegree(diff);
                nextDirection = findNextDirection(degree);

                if (crntDirection != nextDirection)
                {
                    nextDir.setNextDir(nextDirection);
                    Debug.Log("Go " + nextDirection);

                    EasyTTSUtil.SpeechAdd("Go " + nextDirection);
                    crntDirection = nextDirection;
                }
        */
	}

	public string newInstruct2()
	{

		string userDir = "";
		Vector3 diff = path.corners [1] - transform.position;
		Vector3 facing = transform.forward;
		
		Vector3 avtr2Pnt = transform.position - path.corners[1]; // A-C
		avtr2Pnt = transform.InverseTransformDirection(avtr2Pnt);
		
		float angle = Mathf.Rad2Deg * (Mathf.Acos ((Vector3.Dot (facing, diff)) / (facing.magnitude * diff.magnitude)));
		infoBrg.setAngleOfNextWP (angle);
		if (angle >= 110  && angle <= 180)
			userDir = "Behind";
		
		if (angle >= 0 && angle <= 80)
			userDir = "Forward";
		
		if (angle > 80 && angle < 110) {
			if (avtr2Pnt.x > 0)
				userDir = "Left";
			else
				userDir = "Right";
		}

		
		return userDir;
	}
		
	/*
	 * 			**FIRST PERSON INSTRUCTIONS**
	 * Determines the direction of next waypoint with
	 * respect to the avatar. Decides whether if next waypoint is
	 * either in Front, Behind, Left, or Right of the avatar.
	 * @return userDir - returns the direction of next waypoint
	 * 					 with respect to the avatar as a string.
	 */
	private string nextInstruction()
	{
		//bool isTutored = false;
		string userDir = ""; 
		Vector3 adjustPosition;
		Vector3 avtr2Pnt = transform.position - path.corners[1]; // A-C
		avtr2Pnt = transform.InverseTransformDirection(avtr2Pnt);

		if (Mathf.Abs(avtr2Pnt.x) > Mathf.Abs(avtr2Pnt.z)) {
			if(avtr2Pnt.x > 0){
				userDir = "Left";
				//if(transform.position != path.corners[1])
					//transform.position = path.corners[1];
			}
			else{ 
				userDir = "Right";
				//if(transform.position != path.corners[1])
					//transform.position = path.corners[1];
			}
		} 
		else 
		{
			if(avtr2Pnt.z > 0){
				userDir = "Behind";
				//transform.position = path.corners[1];
			}
			else {
				userDir = "Forward";
			}
		}



		//Debug.Log ("next *" + userDir +" *" + avtr2Pnt + " " + avtr2Pnt.x + " " + avtr2Pnt.z);
		return userDir;
	}

	/*
	 * 	Used for the auto-walk feature to stop avatar at the
	 *  next waypoint in its path. The continues walking until
	 *  arrived at next waypoint.
	 */
	private void autoWalkToNextWayPoint()
	{
		int curNumWP = path.corners.Length;

		if (infoBrg.hasPermission()) 
		{
			agent.destination = target.position;

			Debug.Log("Has Permission to move");
			if (numWayPoint == curNumWP) {
				agent.Resume ();
				Debug.Log ("Should be moving");
			}
			else
			{
				agent.Stop ();
				Debug.Log ("Should not be moving");
				infoBrg.setPermission (false);
				numWayPoint = curNumWP;
			}
		}
			
	}

	private void calcAngleDifference()
	{
		Vector3 facing = transform.forward;
		Vector3 toWP = path.corners [1] - path.corners [0];

		float angle = Mathf.Rad2Deg*(Mathf.Acos ((Vector3.Dot (facing, toWP))/ (facing.magnitude * toWP.magnitude)));
		infoBrg.setAngleOfNextWP (angle);
	}



	/*
	 *  Return the total room that will be passed by on the certain route.
	 */
	int totalRoomPassedBy(Vector3 s, Vector3 e)
	{
		NavMeshPath testPath = new NavMeshPath();
		NavMesh.CalculatePath (s, e, NavMesh.AllAreas, testPath);
		RaycastHit hit;
		List<Vector3> rmPos = new List<Vector3> ();
		float radius = 5;
		int roomAmount = 0;
		int pointAmount = 0;
		for (int i = 0; i < testPath.corners.Length - 1; i++) {
			pointAmount = (int)(Vector3.Distance( testPath.corners[i], testPath.corners[i+1]));
			//List<>
			/*if(pointAmount < 1){
				if (Physics.Raycast (path.corners[i + 1], transform.forward, out hit, radius * 1.5f)){
					if (hit.collider.gameObject.tag == "Door") {
						if(!rmPos.Contains(hit.collider.gameObject.transform.position)){
							rmPos.Add(hit.collider.gameObject.transform.position);
							Debug.Log (hit.collider.gameObject.transform.position);
						}
					}
				}
				if (Physics.Raycast (path.corners[i + 1], -transform.forward, out hit, radius * 1.5f)){
					if (hit.collider.gameObject.tag == "Door") {
						if(!rmPos.Contains(hit.collider.gameObject.transform.position)){
							rmPos.Add(hit.collider.gameObject.transform.position);
							Debug.Log (hit.collider.gameObject.transform.position);
						}
					}
				}
				if (Physics.Raycast (path.corners[i + 1], transform.right, out hit, radius * 1.5f)){
					if (hit.collider.gameObject.tag == "Door") {
						if(!rmPos.Contains(hit.collider.gameObject.transform.position)){
							rmPos.Add(hit.collider.gameObject.transform.position);
							Debug.Log (hit.collider.gameObject.transform.position);
						}
					}
				}
				if (Physics.Raycast (path.corners[i + 1], -transform.right, out hit, radius * 1.5f)){
					if (hit.collider.gameObject.tag == "Door") {
						if(!rmPos.Contains(hit.collider.gameObject.transform.position)){
							rmPos.Add(hit.collider.gameObject.transform.position);
							Debug.Log (hit.collider.gameObject.transform.position);
						}
					}
				}
			}else {*/
				for(int j = 1; j < pointAmount; j++)
				{
				/*Debug.DrawRay (Vector3.Lerp(path.corners[i],path.corners[i + 1],
				                            j*1f/pointAmount), -(transform.right*radius)*1.3f, Color.magenta, 50f, true);
				//Debug.DrawRay (Vector3.Lerp(path.corners[i],path.corners[i + 1],
				                            j*1f/pointAmount), (transform.right*radius)*1.3f, Color.red, 50f, true);
				//Debug.DrawRay (Vector3.Lerp(path.corners[i],path.corners[i + 1],
				                            j*1f/pointAmount), (transform.forward*radius)*1.3f, Color.white, 50f, true);
				//Debug.DrawRay (Vector3.Lerp(path.corners[i],path.corners[i + 1],
				                            j*1f/pointAmount), -(transform.forward*radius)*1.3f, Color.black, 50f, true);*/
					if (Physics.Raycast (Vector3.Lerp(testPath.corners[i],testPath.corners[i + 1],
					                    j*1f/pointAmount), transform.forward, out hit, radius * 1.5f)){
						if (hit.collider.gameObject.tag == "Door") {
							if(!rmPos.Contains(hit.collider.gameObject.transform.position)){
								rmPos.Add(hit.collider.gameObject.transform.position);
								//Debug.Log (hit.collider.gameObject.name);
							}
						}
					}
					if (Physics.Raycast (Vector3.Lerp(testPath.corners[i],testPath.corners[i + 1],
					                                  j*1f/pointAmount), -transform.forward, out hit, radius * 1.5f)){
						if (hit.collider.gameObject.tag == "Door") {
							if(!rmPos.Contains(hit.collider.gameObject.transform.position)){
								rmPos.Add(hit.collider.gameObject.transform.position);
								//Debug.Log (hit.collider.gameObject.name);
							}
						}
					}
					if (Physics.Raycast (Vector3.Lerp(testPath.corners[i],testPath.corners[i + 1],
					                                  j*1f/pointAmount), transform.right, out hit, radius * 1.5f)){
						if (hit.collider.gameObject.tag == "Door") {
							if(!rmPos.Contains(hit.collider.gameObject.transform.position)){
								rmPos.Add(hit.collider.gameObject.transform.position);
								//Debug.Log (hit.collider.gameObject.name);
							}
						}
					}
					if (Physics.Raycast (Vector3.Lerp(testPath.corners[i],testPath.corners[i + 1],
					                                  j*1f/pointAmount), -transform.right, out hit, radius * 1.5f)){
						if (hit.collider.gameObject.tag == "Door") {
							if(!rmPos.Contains(hit.collider.gameObject.transform.position)){
								rmPos.Add(hit.collider.gameObject.transform.position);
								//Debug.Log (hit.collider.gameObject.name);
							}
						}
					}

				}
			//}
		}
		
        for(int i = 0; i < rmPos.Count; i++)
        {
            Debug.Log(rmPos[i]);
        }
        /*
        for(int i = 0; i < rmPos.Count; i++)
        {
            if(compareTwoVectors(rmPos[i], rmPos[i+1]))
            {
                rmPos.Remove(rmPos[i]);
            }
        }
        */
		return rmPos.Count;
	}

    //Returns true if both vectors are within error apart
    bool compareTwoVectors(Vector3 v1, Vector3 v2)
    {
        bool similarEnough = false;
        double error = 0.05;

        if (Mathf.Abs(v1.x - v2.x) < error && Mathf.Abs(v1.y - v2.y) < error && Mathf.Abs(v1.z - v2.z) < error)
        {
            similarEnough = true;
        }

        return similarEnough;
    }

    /*
	 * 
	 */
    int totalPossibleTurn(Vector3 s, Vector3 e)
	{
		NavMeshPath testPath = new NavMeshPath();
		NavMesh.CalculatePath (s, e, NavMesh.AllAreas, testPath);
		return (testPath.corners.Length-2)/2;
	}

	int DifficultWeight(Vector3 s, Vector3 e)
	{
		NavMeshPath testPath = new NavMeshPath();
		NavMesh.CalculatePath (s, e, NavMesh.AllAreas, testPath);
		float turnDistance = 0;
		int hardTurn = 0;
		for (int i = 2; i < testPath.corners.Length - 4; i+=2) {
			turnDistance = Vector3.Distance(testPath.corners[i], testPath.corners[i+2]);
			//Debug.Log ("turn distance " + turnDistance);
			if(turnDistance < 10){
				//Debug.Log ("high turn weight");
				hardTurn++;
			}
		}

		return hardTurn;
	}

	void writeJson()
	{
	}

	/*
        Currently not in use...

	private void findMajorDirection (Vector3 line)
	{
		float h = line.x;
		float v = line.z;

		if (Mathf.Abs (h) < Mathf.Abs (v))
		{
			if (v >= 0){
				//Debug.Log ("Go Up");
				nextDirection = "Up";
			}else{
				//Debug.Log ("Go Down");
				nextDirection = "Down";
			}
		}
		else
		{
			if (h >= 0){
				//Debug.Log ("Go Right");
				nextDirection = "Right";
			}else{
				//Debug.Log ("Go Left");
				nextDirection = "Left";
			}
		}

		if (crntDirection != nextDirection)
		{
			nextDir.setNextDir (nextDirection);
			//Debug.Log ("Go " + nextDirection);

			//EasyTTSUtil.SpeechAdd("Go " + nextDirection); 
			crntDirection = nextDirection;
		}
	}
	*/
	
	/*
        Currently not in use...

	private float calculateDegree (Vector3 line)
	{
		float degreeTan = 0;
		int quadrant = 0;

		quadrant = findQuadrant (line);                 // Find line current quadrant.
		degreeTan = Mathf.Atan (line.z / line.x);       // Find angle of line.
		degreeTan = 180 * degreeTan / Mathf.PI;         // Convert from radians to degree.
		degreeTan = addToDegree (degreeTan, quadrant);  // Add number depending on what quadrant
		return degreeTan;                               // line fall under on coordinate.
	}
	*/
	
	/*
        Currently not in use...

	private int findQuadrant (Vector3 vec)
	{
		if (vec.x < 0 && vec.z < 0)
			return 1;
		else if (vec.x >= 0 && vec.z >= 0)
			return 3;
		else if (vec.x < 0 && vec.z >= 0)
			return 4;
		else
			return 2;
	}
	*/
	
	/*
        Currently not in use...

	private float addToDegree (float degree, int quadrant)
	{
		if (quadrant > 1)
		if (quadrant >= 4)
			degree += 360;
		else
			degree += 180;
		return degree;
	}
	*/
	
	/*
        Currently not in use...
        **Bird's Eye view instruction**

	private string findNextDirection (float dgre)
	{
		if (dgre >= 45 && dgre < 135)
		{
			fourDir = "Down";
			return fourDir;
		}// Go Down
		else if (dgre >= 135 && dgre < 225)
		{
			fourDir = "Right";
			return fourDir;
		}// go Right
		else if (dgre >= 255 && dgre < 345)
		{
			fourDir = "Up";
			return fourDir;
		}// Go Upward
		else if (dgre < 45 || dgre >= 345)
		{
			fourDir = "Left";
			return fourDir;
		}// Go Left
		else
			return fourDir;
	}
	*/

}
