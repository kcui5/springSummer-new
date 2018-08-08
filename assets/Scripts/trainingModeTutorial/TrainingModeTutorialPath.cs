using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TrainingModeTutorialPath: MonoBehaviour
{
	
	public Transform target;
	public Transform player; 
	public NavMeshAgent agent;
	public int distanceFeet;
	public int distanceMeter;
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
	
	int curVal;
	NextDirection nextDir;
	RoomLocationsConf rlc;
	TheInformationBridge infoBrg;

	//private bool isTutored;
	private int instructMode;
	private int moveTurnInstruct;
	private bool moveTurnInstructEnd;
	private bool isFirstTime;
	
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
		startPoint = new Vector3 (151.3f, 2f, -137.1f);//rlc.setStartingPoint ();
		endPoint = new Vector3 (52f, 2f, 17f);//rlc.setEndingPoint ();
		agent.enabled = false;
		isFeet = false;
		
		player.transform.position = startPoint;
		//Debug.Log (player.position+"********************");
		//player.transform.rotation = ;
		nextDir.setStart(startPoint);
		target.transform.position = endPoint;
		
		agent.enabled = true;
		

		//isTutored = false;
		instructMode = 0;
		path = agent.path;
		
		//nextDirection = "";
		crntDirection = "";
		fourDir = " ";
		
		//degree = 0;           No longer used to get next direction to way point
		
		calculatePath();
		isFirstTime = true;
		adjustDirection ();
		numWayPoint = path.corners.Length;
		elapsed = 0;
		
		curMsg = "constantly redefined";
		difMsg = "redefined only is curmsg changes";
		moveTurnInstruct = 1;
		moveTurnInstructEnd = false;
		//Debug.Log (player.position+"++++++++");
		//for (int i = 0; i < path.corners.Length; i++)
		//Debug.Log (path.corners[i]);
		totalDistance ();
		
	}
	
	// Update is called once per frame
	void Update()
	{
		/*if (isFirstTime) {
			introTutorial ();
			Debug.Log ("training mode instruction");
			isFirstTime = false;
		}*/
		//Debug.Log (path.corners[1] + " - " + transform.position + " = " + (path.corners[1] - transform.position));
		//Debug.Log (transform.forward);
		if (!infoBrg.getReachTarget ()) {
			elapsed += Time.deltaTime;
			if (elapsed > updateFreq) {
				elapsed -= updateFreq;
				calculatePath ();
				calculateDirection ();							
				//infoBrg.setInstruct(nextInstruction ());// Sent to 'PlayerMovementCCNY.cs'
				infoBrg.setInstruct (newInstruct2 ());
				distanceInformation ();
				calcAngleDifference ();
				adjustPosition ();
			}
			drawPath ();
		}
		
		
		//if (path.corners.Length != curVal) {
		//Debug.Log ("Num of Waypoints: " + path.corners.Length);		//<----DEBUGGING: Number of waypoints
		//curVal = path.corners.Length;
	}
	
	//autoWalkToNextWayPoint ();
	

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
		if (isFirstTime){
			String direction = nextInstruction ();
			if (direction == "Right")
				transform.Rotate (0, 90f, 0);
			if (direction == "Left")
				transform.Rotate (0, -90f, 0);
			if (direction == "Behind")
				transform.Rotate (0, 180f, 0);
		}
	}
	
	/**
	 * provide a summary about distance to the destination, desitination is in which direction
	 * and which room will be passed by
	 */
	void introTutorial()
	{
		EasyTTSUtil.SpeechAdd ("Welcome to the training mode tutorial. In this tutorial, we will learn" +
                               "some details through a sample training mode.");


    }
	
	
	void distanceInformation()
	{	
		distanceFeet = (int)(Mathf.Sqrt(	Mathf.Pow((path.corners[1].x - path.corners [0].x),2) + 
		                                Mathf.Pow((path.corners[1].z - path.corners [0].z),2) )/3);
		distanceMeter = distanceFeet /3;
		
		infoBrg.setDistanceF (distanceFeet);
		infoBrg.setDistanceM (distanceMeter);
	}
	
	void totalDistance()
	{
		int distance = 0;
		for(int i = 0; i < path.corners.Length-1; i++)
		{
			distance  += (int)(Mathf.Sqrt(	Mathf.Pow((path.corners[i+1].x - path.corners [i].x),2) + 
			                              Mathf.Pow((path.corners[i+1].z - path.corners [i].z),2) )/3);
		}
		Debug.Log (distance/3 + "meter");
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
		bool isTutored = false;
		string userDir = "";
		Vector3 diff = path.corners [1] - transform.position;
		Vector3 facing = transform.forward;

		Vector3 avtr2Pnt = transform.position - path.corners[1]; // A-C
		avtr2Pnt = transform.InverseTransformDirection(avtr2Pnt);

		float angle = Mathf.Rad2Deg * (Mathf.Acos ((Vector3.Dot (facing, diff)) / (facing.magnitude * diff.magnitude)));
		infoBrg.setAngleOfNextWP (angle);
		if (angle >= 105  && angle <= 180)
			userDir = "Behind";
		
		if (angle >= 0 && angle <= 85)
			userDir = "Forward";
		
		if (angle > 85 && angle < 105) {
			if (avtr2Pnt.x > 0)
				userDir = "Left";
			else
				userDir = "Right";
		}
		/*
		if (!moveTurnInstructEnd) {
			switch (moveTurnInstruct) {


			case 1:
				if (userDir == "Forward") {
					EasyTTSUtil.SpeechAdd ("Walk Forward instruction indicates that the player need to move forward in " +
						"a certain amount of distance.");
					Debug.Log ("Forward tutorial");
					moveTurnInstruct ++;
				}
				break;
			case 3:
				if (userDir == "Left") {
					EasyTTSUtil.SpeechAdd ("Turn left instruction indicates that the player need to turn left " +
						"when the next turning point is on the left side. After you complete turning " +
						"left, the System will indicate left 90 degree turn made.");
					Debug.Log ("Left tutorial");
					moveTurnInstruct ++;
				}
				break;
			case 2:
				if (userDir == "Right") {
					EasyTTSUtil.SpeechAdd ("Turn right instruction indicates that the player need to turn right " +
						"when the next turning point is on the right side.When you complete turning " +
						"right, the System will indicate right 90 degree turn made.");
					Debug.Log ("Right tutorial");
					moveTurnInstruct ++;
				}
				break;
			case 4:
				moveTurnInstructEnd = true;
				break;

			default:
				break;

			}
		}*/

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
		bool isTutored = false;
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
				transform.position = path.corners[1];
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
	void totalRoomPassedBy()
	{

	}
	
	/*
	 * 
	 */
	void totalPossibleTurn()
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
