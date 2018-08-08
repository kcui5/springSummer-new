using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
//using UnityEditor;
//using UnityEngine.SceneManagement;// use in the version 5.3

public class TrainingModeTutorialMove : MonoBehaviour { 
	
	/**
	 * Learning position: 
	 * Learning one route: Start(161,2,-140), r648, End(18,2,-7), r615; normal
	 *					   S    (104,2,-103), u1026, E (-51,2,-88), u1033; simple
	 *					   S    (80,2,-53.6), u1021, E (-23,2,173), r628; harder
	 *
	 * Same destination:   S    (161,2,-140), r648, E (104,2,-103), u1026; 
	 *					   S    (100,2,-142), u654, E (104,2,-103), u1026; 
	 *					   S    (-60,2,146), u1011, E (104,2,-103), u1026; 
	 *					   S    (67.5,2,133), u1013, E (104,2,-103), u1026; 
	 *
	 * Test position:
	 * Test one route:     Start(161,2,-140), r648, End(18,2,-7), r615; normal
	 *					   S    (104,2,-103), u1026, E (-51,2,-88), u1033; simple
	 *					   S    (80,2,-53.6), u1021, E (-23,2,173), r628; harder
	 *
	 * Same destination:   S    (143.1,2,-158), r645, E (104,2,-103), u1026; 
	 *					   S    (-101,2,-104), r606, E (104,2,-103), u1026; 
	 *					   S    (-44,2,64), r619, E (104,2,-103), u1026; 
	 *					   S    (70,2,29), r640, E (104,2,-103), u1026;
	 */				
	public float speed;
	public float speedAndroid;
	public float rotateSpeed = 90f;
	public Text collisionText;
	public Text timerText;
	public int moveMode = 0;
	public bool autoWalk = false;
	
	float restartTime;
	float colTime;
	float interTime;
	
	int countColli;
	int countWrongStep;
	string instruct;
	const int MOVEMODE0 = 0;
	const int MOVEMODE1 = 1;
	
	Vector2 startpos; //use for phone geasture start position
	bool couldBeSwipe;
	float comfortZone = 5000f;
	float minSwipeDist = 100f;
	
	string nearBy;
	string[] nearByQueue;
	Vector2 touchDeltaPos;
	Vector2 basePos;
	Vector3 playerStartingPoint;
	
	bool isMoving;
	AudioSource source;
	public AudioClip footSteps; 
	RuntimePlatform platform = Application.platform;
	ArrayList myCols;
	Vector3 movement;
	Rigidbody playerRigidbody;
	NavMeshAgent agent;
	NextDirection nexDir;
	TheInformationBridge infoBrg;
	RoomLocationsConf rlc; //related to startpoint class
	
	//Possibly defined by tayo AS script
	string start;
	string end;
	
	// Huang's work
	Vector3 constraintPos;
	bool endOfGame;
	public Transform target;
	private int firstTimeHit;

	//Access log
	private string correctDirection;
	private string accessDirection;
	private int logCount;
	private int turnlogCount;
	bool isAllowMove;
	bool isReachTarget;
	private bool isMoveTurnInstructEnd;
	private int moveTurnInstruct;
	
	// Use this for initialization
	void Awake()
	{
		// Initialize the player
		EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates);	// text to speech
		//EasyTTSUtil.OpenTTSSetting ();
		playerRigidbody = GetComponent<Rigidbody>();
		infoBrg = new TheInformationBridge ();
		agent = GetComponent<NavMeshAgent> ();
		source = GetComponent<AudioSource> ();
		rlc = new RoomLocationsConf ();
		
		playerStartingPoint = rlc.setStartingPoint ();
		start = "0,0,0";
		//"Not defined yet";
		end = "0,0,0";
		//"Not defined yet";
		
		colTime 	= Time.timeSinceLevelLoad;
		myCols 		= new ArrayList();
		countColli 	= 0;
		countWrongStep = 0;
		endOfGame  	= false;
		isMoving 	= false;
		interTime 	= 1.0f;
		restartTime = 0.0f;
		instruct 	= "";
		nearBy 		= "";
		nearByQueue = new string[3];
		source.clip = footSteps;
		source.volume = 1f;
		firstTimeHit = 0;

		correctDirection = "";
		accessDirection = "";
		logCount = 0;
		turnlogCount = 0;
		isAllowMove = true;
		isReachTarget = false;
		infoBrg.setReachTarget (isReachTarget);
		isMoveTurnInstructEnd = false;
		moveTurnInstruct = 1;
		
		if (collisionText != null)
			collisionText.text = "Collision count: " + countColli.ToString();
		introMessage ();
		
	}
	
	// Update is called once per frame
	void Update()
	{
		//Debug.Log (playerRigidbody.position+"------");
		timer ();
		restartFunction ();
		printNewInstruction ();
		quitFunction ();
		distanceFromWayPoint2 ();
		//preventDriftingRotation ();
		if (isAllowMove) {
			walkingSound ();
			inputFunction ();
		}else{
			isMoving = false;
		}
		doorNearby2 ();

	}

	void inputFunction()
	{
		if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer){	
			windowsInputFunction ();
			//androidTouchInput2 ();
		}
		if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
			androidTouchInput2 ();
		if (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer)
			OSXInputFunction ();
	}

	private IEnumerator MyCoroutine(float waitTime)
	{
		//This is a coroutine
		Debug.Log (Time.time);
		
		yield return new WaitForSeconds (waitTime);    //Wait one frame
		if(isReachTarget)
			Application.LoadLevel("1ModeSelect");
		isAllowMove = true;
		source.Stop ();
		Debug.Log ("waiting  time ");
		
	}

	void introMessage()
	{
		EasyTTSUtil.SpeechAdd ("Welcome to the training mode tutorial. In this tutorial, we will learn" +
                               "some details through a sample training mode.");
        isAllowMove = false;
		StartCoroutine (MyCoroutine(15));
	}
	
	/*void FixedUpdate(){
		printNewInstruction ();
		if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer){	
			windowsInputFunction ();
			androidTouchInput2 ();
		}
		if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
			androidTouchInput2 ();
		if (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer)
			OSXInputFunction ();
	}*/
	
	
	/*
	 * 				v	.__                .  .___          ,           	v
	 *			v	v	[ __ _ ._  _ ._. _.|  [__ . .._  _.-+-* _ ._  __	v	v
	 *		v	v	v	[_./(/,[ )(/,[  (_]|  |   (_|[ )(_. | |(_)[ )_) 	v	v	v
     *  v	v	v	v                                        				v	v	v	v
	 */
	
	
	/*  
	 * Updates the timer label on the Canvas
	 * >>Should be in Unity's Update Function<<
	 */
	
	void walkingSound()
	{
		if (isMoving) {
			
			if (!source.isPlaying) {
				source.loop = true;
				source.PlayScheduled(5f);
				source.Play ();
			}
			//Debug.Log ("Hear walking sound");
		} else {
			source.Stop ();
			//Debug.Log ("Not hear walking sound");
		}
	}
	
	void timer()
	{
		int secs = (int)(Time.timeSinceLevelLoad - restartTime) % 60;
		int mins = (int)(Time.timeSinceLevelLoad - restartTime) / 60;
		
		if (timerText != null)
			timerText.text = "Timer: " + string.Format("{0}", mins) + " : " + string.Format("{0:00}", secs);
	}
	
	/*
	void doorNearby()
	{
		RaycastHit hit;
		float radius = 6;
		string newNearByMsg = "";

		// Draw Ray vertically and horizontally
		Debug.DrawRay (transform.position, -(transform.right*radius)*1.5f, Color.green, 0f, true);
		Debug.DrawRay (transform.position, (transform.right*radius)*1.5f, Color.green, 0f, true);
		Debug.DrawRay (transform.position, (transform.forward*radius)*1.5f, Color.yellow, 0f, true);
		Debug.DrawRay (transform.position, -(transform.forward*radius)*1.5f, Color.red, 0f, true);

		// Draw Ray diagonally
		Debug.DrawRay (transform.position, (transform.forward+transform.right)*(radius), Color.blue, 0f, true);
		Debug.DrawRay (transform.position, (transform.forward-transform.right)*(radius), Color.magenta, 0f, true);
		Debug.DrawRay (transform.position, -(transform.forward+transform.right)*(radius), Color.grey, 0f, true);
		Debug.DrawRay (transform.position, -(transform.forward-transform.right)*(radius), Color.white, 0f, true);

		// Shoot Raycast Forward
		if (Physics.Raycast (transform.position, transform.forward, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "Door") {
				//Debug.Log ("RAYCAST HIT in FRONT: ---> " + hit.collider.gameObject.name);
				newNearByMsg = "room: " + hit.collider.gameObject.name + " in front.";

				if (nearBy != newNearByMsg)
				{
					EasyTTSUtil.SpeechAdd (newNearByMsg);
					Debug.Log (newNearByMsg);
					if(hit.transform != null)
						hit.transform.SendMessage ("AvatarNearBy");
					nearBy = newNearByMsg;
				}
			}
		}

		// Shoot Raycast Behind
		if (Physics.Raycast (transform.position, -transform.forward, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "Door") { 
				Debug.Log ("RAYCAST HIT in BEHIND: ---> " + hit.collider.gameObject.name);
			}
		}
		// Shoot Raycast Left
		if (Physics.Raycast (transform.position, -transform.right, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "Door") {
				//Debug.Log ("Passing by room: <--- " + hit.collider.gameObject.name + " door on LEFT");
				newNearByMsg = "...: " + hit.collider.gameObject.name + " to your LEFT.";

				if (nearBy != newNearByMsg)
				{
					EasyTTSUtil.SpeechAdd (newNearByMsg);
					Debug.Log (newNearByMsg);
					if(hit.transform != null)
						hit.transform.SendMessage ("AvatarNearBy");
					nearBy = newNearByMsg;
				}
			}
		}
		// Shoot Raycast Right
		if (Physics.Raycast (transform.position, transform.right, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "Door") {
				//Debug.Log ("Passing by room: ---> " + hit.collider.gameObject.name + " door on RIGHT");
				newNearByMsg = "...: " + hit.collider.gameObject.name + " to your Right.";

				if (nearBy != newNearByMsg)
				{
					EasyTTSUtil.SpeechAdd (newNearByMsg);
					Debug.Log (newNearByMsg);
					if(hit.transform != null)
						hit.transform.SendMessage ("AvatarNearBy");
					nearBy = newNearByMsg;
				}
			}
		}			
	}
	*/
	void doorNearby2()
	{
		RaycastHit hit;
		float radius = 6;
		string newNearByMsg = "";
		
		// Draw Ray vertically and horizontally
		Debug.DrawRay (transform.position, -(transform.right*radius)*1.5f, Color.magenta, 0f, true);
		Debug.DrawRay (transform.position, (transform.right*radius)*1.5f, Color.red, 0f, true);
		Debug.DrawRay (transform.position, (transform.forward*radius)*1.5f, Color.white, 0f, true);
		Debug.DrawRay (transform.position, -(transform.forward*radius)*1.5f, Color.black, 0f, true);
		
		// Shoot Raycast Forward
		if (Physics.Raycast (transform.position, transform.forward, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "Door") {
				//Debug.Log ("RAYCAST HIT in FRONT: ---> " + hit.collider.gameObject.name);
				newNearByMsg = " Room: " + hit.collider.gameObject.name + " ahead.";
				
				if (!inQueue(newNearByMsg))
				{
					EasyTTSUtil.SpeechAdd (newNearByMsg);
					Debug.Log (newNearByMsg);
					if(hit.transform != null)
						hit.transform.SendMessage ("AvatarNearBy");
					addToQueue(newNearByMsg);
				}
			}
		}
		
		// Shoot Raycast Behind
		if (Physics.Raycast (transform.position, -transform.forward, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "Door") { 
				//Debug.Log ("RAYCAST HIT in BEHIND: ---> " + hit.collider.gameObject.name);
				//Debug.Log ("Passing by room: <--- " + hit.collider.gameObject.name + " door on LEFT");
				newNearByMsg = " Room: " + hit.collider.gameObject.name + " Behind you.";
				
				if (!inQueue(newNearByMsg))
				{
					EasyTTSUtil.SpeechAdd (newNearByMsg);
					Debug.Log (newNearByMsg);
					if(hit.transform != null)
						hit.transform.SendMessage ("AvatarNearBy");
					addToQueue(newNearByMsg);
				}
			}
		}
		// Shoot Raycast Left
		if (Physics.Raycast (transform.position, -transform.right, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "Door") {
				if(hit.collider.gameObject.name == "1 42" && firstTimeHit == 0)
				{
					doorPassByTutorial();
					firstTimeHit ++;
				}
				//Debug.Log ("Passing by room: <--- " + hit.collider.gameObject.name + " door on LEFT");
				newNearByMsg = " Room: " + hit.collider.gameObject.name + " to your LEFT.";
				
				if (!inQueue(newNearByMsg))
				{
					EasyTTSUtil.SpeechAdd (newNearByMsg);
					Debug.Log (newNearByMsg);
					if(hit.transform != null)
						hit.transform.SendMessage ("AvatarNearBy");
					addToQueue(newNearByMsg);
				}
			}
		}
		// Shoot Raycast Right
		if (Physics.Raycast (transform.position, transform.right, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "Door") {
				//Debug.Log ("Passing by room: ---> " + hit.collider.gameObject.name + " door on RIGHT");
				newNearByMsg = " Room: " + hit.collider.gameObject.name + " to your Right.";
				
				if (!inQueue(newNearByMsg))
				{
					EasyTTSUtil.SpeechAdd (newNearByMsg);
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(2));
					Debug.Log (newNearByMsg);
					if(hit.transform != null)
						hit.transform.SendMessage ("AvatarNearBy");
					addToQueue(newNearByMsg);
				}
			}
		}			
	}
	
	void doorPassByTutorial()
	{
		EasyTTSUtil.SpeechAdd ("The notification of a door nearby indicates that there is a door near the player in a" +
                               " certain direction.");

        Debug.Log ("room pass by tutorial");
		isAllowMove = false;
		source.Stop();
		StartCoroutine (MyCoroutine(5));
	}
	
	bool inQueue(string value)
	{
		for (int i = 0; i < nearByQueue.Length; i++)
			if (nearByQueue [i] == value)
				return true;
		return false;
	}
	
	void addToQueue(string value)
	{
		for (int i = nearByQueue.Length - 2; i >= 0; i--)
			nearByQueue[i+1] = nearByQueue [i];
		
		nearByQueue [0] = value;
	}
	void restartFunction()
	{
		if (Input.GetKeyDown(KeyCode.Space))	
			resetGame();
	}
	void distanceFromWayPoint2()
	{
		if (Input.GetKeyDown(KeyCode.DownArrow)) 
		{
			Debug.Log ("Next-Waypoint is about " + infoBrg.getDistanceF () + " feet away");
			Debug.Log ("Next-Waypoint is about " + infoBrg.getDistanceM () + " meters away");
		}
	}
	void quitFunction()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			Application.Quit();	
	}
	
	void windowsInputFunction()
	{
		string direction = "";
		// Get from Input
		if (Input.GetKey (KeyCode.UpArrow) || Input.GetKey (KeyCode.W)) 
		{
			isMoving = true;
			direction = "forward";
			if (autoWalk)
				autoWalkInEditor ();
			else if (infoBrg.getNextInstruction () == direction)
				moveForward ();
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) {
			//isMoving = true;
			direction = "left";
			if (infoBrg.getNextInstruction () == direction)
				rotateAngle (1);
		}
		if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D)) {
			//isMoving = true;
			direction = "right";
			if (infoBrg.getNextInstruction () == direction)
				rotateAngle (2);
		}
		accessDirection = direction;
		accessLog ();

		if (Input.GetKeyUp (KeyCode.UpArrow))
			isMoving = false;
	}

	void accessLog()
	{
		string timeLog = System.DateTime.Now.ToString ();
		if (accessDirection != "") {
			if (correctDirection == accessDirection) {
				//Debug.Log ("correct access" + accessDirection);
				if (accessDirection == "Forward") {
					if (logCount < 1) {
						infoBrg.setAccessLog ("Correct access: move" + accessDirection + ". " + timeLog);
						infoBrg.setCorrectNum (2 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						infoBrg.setAccessNum (2 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
					}
				} else {
					if (turnlogCount < 1) {
						infoBrg.setAccessLog ("Correct access: turn" + accessDirection + ". " + timeLog);
						if (accessDirection == "Left") {
							infoBrg.setCorrectNum (3+ " ");// + " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
							infoBrg.setAccessNum (4 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						}
						if (accessDirection == "Right") {
							infoBrg.setAccessNum (0 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
							infoBrg.setCorrectNum (1+ " ");// + " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);

						}
					}
				}
				//logCount ++;
			} else {
				//Debug.Log ("wrong access" + accessDirection);
				if (accessDirection == "Forward") {
					if (logCount < 1) {
						infoBrg.setAccessLog ("Wrong access: move" + accessDirection + ".(Wrong access!!!) correct access is move" +
							" " + correctDirection + " " + timeLog);
						infoBrg.setAccessNum (2 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
					
						if (correctDirection == "Forward") {
							infoBrg.setCorrectNum (2 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						} else if (correctDirection == "Left")
							infoBrg.setCorrectNum (3 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						else if (correctDirection == "Right")
							infoBrg.setCorrectNum (1 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);

					}
				} else {
					if (turnlogCount < 1) {
						infoBrg.setAccessLog ("Wrong access: turn" + accessDirection + ".(Wrong access!!!) correct access is move" +
							" " + correctDirection + " " + timeLog);
				
						if (correctDirection == "Forward") {
							infoBrg.setCorrectNum (2 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						} else if (correctDirection == "Left")
							infoBrg.setCorrectNum (3 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						else if (correctDirection == "Right")
							infoBrg.setCorrectNum (1 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
				
						if (accessDirection == "Left")
							infoBrg.setAccessNum (4 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						if (accessDirection == "Right")
							infoBrg.setAccessNum (0 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
					}
				}

			}

			/*if (accessDirection == null) {
			infoBrg.setAccessLog ("*************************");
			infoBrg.setAccessNum("***************************");
			infoBrg.setCorrectNum("***************************");
		}

		
		/*if (correctDirection == "Forward") {
			if (logCount < 1)
				infoBrg.setCorrectNum (2 + " ");
		}else if (correctDirection == "Left")
			infoBrg.setCorrectNum (1 + " ");
		else if (correctDirection == "Right")
			infoBrg.setCorrectNum (3 + " ");
		if (accessDirection == "Forward") {
			if (logCount < 1)
				infoBrg.setAccessNum (2 + " ");
		}else if (accessDirection == "Left")
			infoBrg.setAccessNum (1 + " ");
		else if (accessDirection == "Right")
			infoBrg.setAccessNum (3 + " ");
			*/
			logCount ++;
			turnlogCount ++;
			if (logCount > 25)
				logCount %= logCount;
			if (turnlogCount > 4)
				turnlogCount %= turnlogCount;
		}
	}
	
	void OSXInputFunction()
	{
		string direction = "";
		string newInstruct = infoBrg.getNextInstruction ();
		// Get from Input
		if (Input.GetKey (KeyCode.UpArrow)) 
		{
			isMoving = true;
			direction = "Forward";
			accessDirection = direction;
			if (autoWalk)
				autoWalkInEditor ();
			else if (newInstruct == direction)
				moveForward ();
			else {
				//print (countWrongStep + " ");
				countWrongStep++;
				if (countWrongStep == 10){
					Debug.Log ("Waypoint direction: " + newInstruct);
					countWrongStep %= countWrongStep;
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) {
			//isMoving = true;
			direction = "Left";
			accessDirection = direction;
			if (newInstruct == direction || newInstruct == "Behind")
				rotateAngle (1);
			else 
				Debug.Log ("Waypoint direction: " + newInstruct);
		}
		if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D)) {
			//isMoving = true;
			direction = "Right";
			accessDirection = direction;
			if (newInstruct == direction || newInstruct == "Behind")
				rotateAngle (2);
			else 
				Debug.Log ("Waypoint direction: " + newInstruct);
		}
		//accessDirection = direction;
		accessLog ();
		
		if (Input.GetKeyUp (KeyCode.UpArrow))
			isMoving = false;
	}
	
	void preventDriftingRotation()
	{
		//Prevents Slight Drifting of Y rotation
		if (transform.rotation.eulerAngles.y > 170 && transform.rotation.eulerAngles.y < 190) {
			transform.rotation = Quaternion.Euler (0, 180, 0);
			//Debug.Log ("bug1");
			//EasyTTSUtil.SpeechAdd ("Ahhh bug");
		}
		if (transform.rotation.eulerAngles.y > 350 && transform.rotation.eulerAngles.y < 10) {
			transform.rotation = Quaternion.Euler (0, 0, 0);
			//Debug.Log("bug2");
		}
		if (transform.rotation.eulerAngles.y >= 89 && transform.rotation.eulerAngles.y <= 91) {
			transform.rotation = Quaternion.Euler (0, 90, 0);
			//Debug.Log("bug3");
		}
		if (transform.rotation.eulerAngles.y > 260 && transform.rotation.eulerAngles.y < 280) {
			transform.rotation = Quaternion.Euler (0, 270, 0);
			//Debug.Log("bug4");
		}
	}
	
	
	/*
	 *					v   .  .     ,      .___  .  ,         .___          ,           	v
	 *				v	v	|  |._ *-+-  .  [__  _|*-+- _ ._.  [__ . .._  _.-+-* _ ._  __	v	v
	 *			v	v	v	|__|[ )| | \_|  [___(_]| | (_)[    |   (_|[ )(_. | |(_)[ )_) 	v	v	v
     *		v	v	v	v  	           ._|                                                	v	v	v	v
	 */
	
	
	
	/**
	 * Movement method to move the avatar with keyboard 
	 * on the COMPUTER (Unity Editor).
	 * Use Arrow Keys: Up, Down, Left, Right
	 * Use LetterKeys: W, S, A, D
	*/
	void moveForward()
	{
		movement = transform.forward;
		movement = movement.normalized * speed * Time.deltaTime * 10;
		playerRigidbody.MovePosition(transform.position + movement);
		//Debug.Log ("Step: " + transform.position + " x:" + transform.position.x + " z:" + transform.position.z);
	}
	
	void autoWalkInEditor()	
	{		
		// Moves the avatar forward
		playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		infoBrg.setPermission(true);
		//playerRigidbody.constraints = ~RigidbodyConstraints.FreezeRotationY;
	}
	void rotateAngle(int direction)
	{
		Vector3 rot = transform.rotation.eulerAngles;
		Vector3 newRot;
		if (direction == 1) 
		{ 						// Left
			transform.Rotate (0, -90f, 0);
			newRot = transform.rotation.eulerAngles;
			Debug.Log("Degree Turned: " + readDegreeTurnMade (rot, newRot));
			//Debug.Log ("Left Turn...");
		} 
		else if (direction == 2) 
		{ 						// Right
			transform.Rotate (0, 90f, 0);
			newRot = transform.rotation.eulerAngles;
			Debug.Log("Degree Turned: " + readDegreeTurnMade (rot, newRot));
			//Debug.Log ("Right Turn...");
		}
		
	}
	
	float readDegreeTurnMade(Vector3 oldPoint, Vector3 newPoint)
	{
		if(Mathf.Abs(oldPoint.y - newPoint.y) >= 185)
			return Mathf.Abs(oldPoint.y - newPoint.y) - 180;
		else
			return Mathf.Abs(oldPoint.y - newPoint.y);
	}
	
	/*
	 * Controls the number of times directional instruction 
	 * is printed in the update Function.		
	 */
	void printNewInstruction()
	{
		string newInstruct = infoBrg.getNextInstruction ();
		
		//Debug.Log (playerRigidbody.position);
		/*if (playerRigidbody.position.Equals(playerStartingPoint)) 
		{
			Debug.Log (playerRigidbody.position);
			if (newInstruct == "Left") 
			{ 						// Left
				transform.Rotate (0, -90f, 0);;
				Debug.Log ("Left Turn...*********************");
			} 
			else if (newInstruct == "Right") 
			{ 						// Right
				transform.Rotate (0, 90f, 0);
				Debug.Log ("Right Turn...********************");
			}
		}*/
		
		if (newInstruct != instruct) 
		{
			if(newInstruct != null)
				Debug.Log ("Waypoint direction: " + newInstruct);  		// <----DEBUGGING
			
			if(newInstruct == "Forward")
				EasyTTSUtil.SpeechAdd("Walk " + newInstruct);
			else if (newInstruct == "Behind")
				EasyTTSUtil.SpeechAdd(/* "Waypoint is " + newInstruct */"");
			else if(newInstruct != null)
				EasyTTSUtil.SpeechAdd("Turn " + newInstruct);
			
			instruct = newInstruct;	
		}

		correctDirection = newInstruct;

		if (!isMoveTurnInstructEnd) {
			switch (moveTurnInstruct) {
				
				
			case 1:
				if (newInstruct == "Forward") {
					EasyTTSUtil.SpeechAdd ("The Walk Forward instruction indicates that the player needs to move forward " +
                                           "a certain amount of distance.");
                        Debug.Log ("Forward tutorial");
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(10));
					moveTurnInstruct ++;
				}
				break;
			case 3:
				if (newInstruct == "Left") {
					EasyTTSUtil.SpeechAdd ("The turn left instruction indicates that the player needs to turn left " +
                                           "After you complete turning " +
                                           "left, the System will indicate a left 90 degree turn made.");
                        Debug.Log ("Left tutorial");
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(10));
					moveTurnInstruct ++;
				}
				break;
			case 2:
				if (newInstruct == "Right") {
					EasyTTSUtil.SpeechAdd ("The turn right instruction indicates that the player needs to turn right " +
                                           "When you complete turning " +
                                           "right, the System will indicate a right 90 degree turn made.");
                        Debug.Log ("Right tutorial");
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(10));
					moveTurnInstruct ++;
				}
				break;
			case 4:
				isMoveTurnInstructEnd = true;
				break;
				
			default:
				break;
				
			}
		}
	}
	
	
	
	/*
 *   			v	   .__.     .         .  .__                 .___          ,          		v 
 *   		v	v      [__]._  _|._. _ * _|  |  \ _ .  ,* _. _   [__ . .._  _.-+-* _ ._  __		v	v
 *   	v	v	v      |  |[ )(_][  (_)|(_]  |__/(/, \/ |(_.(/,  |   (_|[ )(_. | |(_)[ )_) 		v	v	v
 *   v	v	v	v                                                               				v	v	v	v
 */
	
	void androidTouchInput2()
	{
		Touch touch;
		float swipeDist = 0;
		//string direction = "";
		//string newInstruct = infoBrg.getNextInstruction ();
		
		if (Input.touchCount > 0 && !infoBrg.isGestureModeActive()) 
		{
			
			//***********************************************************************					
			if (Input.GetTouch (0).tapCount == 3)	// Triple Tap Function 
				resetGame ();						// on Android Device
			
			//***********************************************************************
			
			touch = Input.GetTouch (0);
			switch(touch.phase)
			{
			case TouchPhase.Began:					// 						v TOUCH BEGINNING PHASE v
				couldBeSwipe = true;				//  					chance of being a swipe
				startpos = touch.position;			//  					take of first position touched	
				break;
				
				//******************************************************************
				
			case TouchPhase.Moved:									// 		v TOUCH WHILE MOVING v
				swipeDist = touch.position.y - startpos.y; 			//		take note of how far swiped
				
				if(minSwipeDist < Mathf.Abs(swipeDist))				// 			CHOOSE ONLY 1 OR 2 (BELOW)	
				{
					isMoving = true;
					if(!autoWalk)
						moveInAndroid (swipeDist * speedAndroid); //	<-1-- Allows Free-Movement in Android Device
					else
						autoWalkInAndroid(swipeDist * speedAndroid);	//	<-2-- Auto-walks to next waypoint 
				}
				if (Mathf.Abs (touch.position.y - startpos.y) > comfortZone) 		// is swipe too far?
				{																	// not a valid swipe
					couldBeSwipe = false;											
				}
				break;
				
				//*******************************************************************
				
			case TouchPhase.Stationary:								//	 	v TOUCH HELD DOWN (STATIONARY) v
				swipeDist = touch.position.y - startpos.y;			// take note of how far swiped
				
				if (minSwipeDist < Mathf.Abs (swipeDist)) {				// CONDITION: 	Is swipe distance greater than minSwipeDistance?	
					isMoving = true;
					if (!autoWalk)
						moveInAndroid (swipeDist * speedAndroid); 		//	<-1-- Allows Free-Movement in Android Device
					else
						autoWalkInAndroid (swipeDist * speedAndroid);	
				}
				break;
				//*******************************************************************
			case TouchPhase.Ended:										// 		v TOUCH OFF SCREEN (ENDED)
				swipeDist = (touch.position - startpos).magnitude;		
				isMoving = false;
				if (couldBeSwipe && (swipeDist > minSwipeDist)) 
				{
					float xDistance = touch.position.x - startpos.x;
					float yDistance = touch.position.y - startpos.y;	
					
					if (Mathf.Abs (xDistance) > Mathf.Abs (yDistance)) { 	// If horizontal swipe// rotate the avatar 
						if (!autoWalk)
							rotate (xDistance);								// (Turn avatar left or right 90 degrees)
						else
							autoRotate (xDistance);
						
					} else if (Mathf.Abs (xDistance) < Mathf.Abs (yDistance)) {
						if (yDistance < 0)
							distanceFromWayPoint ();
					}
				}
				break;
			default:
				break;	
			}
			
		}
		
	}
	
	/* 
	 * Movement method to move the avatar FORWARD with 
	 * swipes on the ANDROID device (Mobile Application).
	 * @param v - Vertical direction swipe
	 */
	void moveInAndroid(float v)		
	{
		string direction = "";
		string newInstruct = infoBrg.getNextInstruction ();

		if (v >= 0) { 		// Only takes upward swipes
			// Moves the avatar forward
			direction = "Forward";
			accessDirection = direction;
			if (newInstruct == direction) {
				movement.Set (0f, 0f, v);						
				movement = transform.forward;
				movement = movement.normalized * speed * Time.deltaTime * 10;
				playerRigidbody.MovePosition (transform.position + movement);
			}
			else 
				countWrongStep++;
			if(countWrongStep == 150){
				if(newInstruct == "Forward")
					EasyTTSUtil.SpeechAdd("Walk " + newInstruct);
				else if (newInstruct == "Behind")
					EasyTTSUtil.SpeechAdd(/* "Waypoint is " + newInstruct */"");
				else if (newInstruct != null)
					EasyTTSUtil.SpeechAdd("Turn " + newInstruct);
				countWrongStep %= countWrongStep;
			}

			accessLog ();
		}
	}
	
	void distanceFromWayPoint()
	{
		Debug.Log ("Next-Waypoint is about " + infoBrg.getDistanceF () + " feet away");
		Debug.Log ("Next-Waypoint is about " + infoBrg.getDistanceM () + " meters away");
		EasyTTSUtil.SpeechFlush( "" + infoBrg.getDistanceF () + " feet away. " + infoBrg.getDistanceM() + " meters away.");
	}
	
	/* 
	 * Auto-Movement method to direct the avatar to walk to 
	 * the next waypoint on his path to destination.
	 * @param v - Vertical direction swipe
	 */
	void autoWalkInAndroid(float v)		
	{
		if (v >= 0) 		// Only takes upward swipes
		{
			// Moves the avatar forward
			playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			infoBrg.setPermission(true);
			//playerRigidbody.constraints = ~RigidbodyConstraints.FreezeRotationY;
		}
		
	}
	
	/*
	 * Movement method to rotate the avatar left or right
	 * 90 DEGREES with swipes on the ANDROID device (Mobile Application).
	 * @param h - Horizontal direction swipe
	 */
	void rotate(float h)	
	{
		string direction = "";
		string newInstruct = infoBrg.getNextInstruction ();
		if (h >= 0) 								// Determines if left or right swipe
		{
			direction = "Right";
			accessDirection = direction;
			if (newInstruct == direction || newInstruct == "Behind"){
				transform.Rotate (0, 90f, 0);			//** Right Rotation
				EasyTTSUtil.SpeechFlush ("Right " + 90 + " degree turn made");
			}
			else if(newInstruct == "Forward")
				EasyTTSUtil.SpeechAdd("Walk " + newInstruct);
			else if (newInstruct == "Behind")
				EasyTTSUtil.SpeechAdd(/* "Waypoint is " + newInstruct */"");
			else if (newInstruct != null)
				EasyTTSUtil.SpeechAdd("Turn " + newInstruct);
			accessLog ();
			
		}else {
			direction = "Left";
			accessDirection = direction;
			if (newInstruct == direction || newInstruct == "Behind"){
				transform.Rotate (0, -90f, 0);			// **Left Rotation
				EasyTTSUtil.SpeechFlush ("Left " + 90 + " degree turn made");
			}
			else if(newInstruct == "Forward")
				EasyTTSUtil.SpeechAdd("Walk " + newInstruct);
			else if (newInstruct == "Behind")
				EasyTTSUtil.SpeechAdd(/* "Waypoint is " + newInstruct */"");
			else if (newInstruct != null)
				EasyTTSUtil.SpeechAdd("Turn " + newInstruct);
			accessLog ();
		}
	}
	
	void autoRotate(float h)	
	{
		//Debug.Log ("Facing: " + transform.rotation.eulerAngles.y);
		//Debug.Log ("Difference: " + (infoBrg.getAngleOfNextWP()) );
		//Debug.Log ();
		
		playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		if (h >= 0) 								// Determines if left or right swipe
			transform.rotation = Quaternion.Euler(0,(transform.rotation.eulerAngles.y + infoBrg.getAngleOfNextWP()),0);		//** Left Rotation
		else
			transform.rotation = Quaternion.Euler(0,(transform.rotation.eulerAngles.y - infoBrg.getAngleOfNextWP()),0);		// **Right Rotation
		playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
	}
	
	/*
	 * Currently not in use...

	bool isMoveAllowed(string dir)
	{
		nexDir = new NextDirection();	
		if (nexDir.getNextDir().Equals(dir))	
			return true;
		return false; 
	}
    */
	
	/*
	 * Currently not in use...

	Vector2 constraint(Vector2 v)
	{
		float x = v.x;
		float y = v.y;

		if (x >= y)
		{
			if (x > -y)
				v = Vector2.right;
			else
				v = Vector2.down;
		}
		else
		{
			if (x > -y)
				v = Vector2.up;
			else
				v = Vector2.left;
		}

		return new Vector2(x, y);
	}
	*/
	
	/*
	 * When avatar comes in contact with an object
	 * @param col - the object that was collided
	 */
	void OnCollisionEnter(Collision col)
	{
		
		if (col.gameObject.CompareTag("Base_Wall") || col.gameObject.CompareTag("Wall"))
		{
			Debug.Log("Enter " + col.transform.name);
			if (myCols.Count == 0)
			{
				interTime = Time.timeSinceLevelLoad - colTime;
				colTime = Time.timeSinceLevelLoad;
				if (colTime > 0.1f)
				{
					Handheld.Vibrate();
					countColli++;
					collisionText.text = "Collision count: " + countColli.ToString();
				}
			}
			else
			{
				Vector3 myNormal = col.contacts[0].normal.normalized;
				Vector3 wallForward = col.transform.forward;
				Debug.Log(col.transform.name + myNormal);
				Debug.Log(col.transform.name + "forward: " + wallForward);
				foreach (ContactPoint n in col.contacts)
					Debug.Log("Contact Point " + n.normal);
				
				if (myNormal == wallForward || myNormal == -wallForward)
				{
					interTime = Time.timeSinceLevelLoad - colTime;
					colTime = Time.timeSinceLevelLoad;
					if (interTime > 0.3f)
					{
						Vector3 myDirection = playerRigidbody.transform.forward;
						
						float dotProduct = Vector3.Dot(myNormal, myDirection);
						if (dotProduct < -0.2f)
						{
							Debug.Log(col.transform.name);
							Handheld.Vibrate();
							countColli++;
							collisionText.text = "Collision count: " + countColli.ToString();
							
						}
						Debug.Log(myDirection);
					}
				}
			}
			
			myCols.Add(col);
			Debug.Log("Collision: " + countColli);
		}
	}
	
	
	void OnCollisionExit(Collision col)
	{
		if (col.gameObject.CompareTag("Base_Wall") || col.gameObject.CompareTag("Wall"))
		{
			Debug.Log("Exist " + col.transform.name);
			myCols.RemoveAt(myCols.Count - 1);
		}
	}
	
	
	/*
	 * Resets avatar to their starting position and resets
	 * the timer.
	 */ 
	void resetGame()
	{
		nexDir = new NextDirection();
		NavMeshAgent agent = GetComponent<NavMeshAgent>();
		
		agent.enabled = false;  
		transform.rotation = Quaternion.Euler(0,0,0);
		playerRigidbody.MovePosition(playerStartingPoint);
		EasyTTSUtil.SpeechFlush("Your location has been reset to your original starting point");
		agent.enabled = true;
		
		countColli = 0;
		
		collisionText.text = "Collision count: " + countColli.ToString();
		
		restartTime = Time.timeSinceLevelLoad;
		Debug.Log("resartTime " + restartTime);
	}
	
	/*
	 * When avatar come into contact with a TRIGGER
	 * such as the destination cube.
	 * @param col - the trigger that was collided
	 */ 
	void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "Door") 
		{
			Debug.Log ("Walking through room " + col.gameObject.name + " door");
			EasyTTSUtil.SpeechAdd("Walking through room " + col.gameObject.name + " door");
		}
		
		
		if (col.gameObject.CompareTag("target"))
		{
			endOfGame = true;
			nexDir = new NextDirection();
			nexDir.setEndTime(timerText.text);
			nexDir.setNumOfCollisions(countColli);
			WriteString ();
			source.Stop();
			EasyTTSUtil.SpeechAdd("Congratulations, you've reached your destination.");

			Debug.Log("Game Over, you've completed the course");
			isAllowMove = false;
			isReachTarget = true;
			infoBrg.setReachTarget(isReachTarget);
			StartCoroutine(MyCoroutine(5));
			//Application.LoadLevel("DifficultyController");
			
			//Application.Quit();
			
			/****************************************** not for test
            //Invoke("EndGame", 4); // Wait 4 seconds and then call the "changeToEndScene" Method.

            /////// Get UnityPlayerActivity class from Android Side ////////////////
            using (AndroidJavaClass jc = new AndroidJavaClass("com.jastworld.interfaceplugin.UnityPlayerActivity"))
            {
                if (jc != null)
                {
                    //jc.CallStatic("UnitySendMessage", "text", "setText", "Got UnityPlayerActivity Class!");
                    //print("Got UnityPlayerActivity Class!");

                    /////// Get the instance of UnityPlayerActivity class from Android Side ////////////////
                    using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        if (jo != null)
                        {
                            //jc.CallStatic("UnitySendMessage", "text", "setText", "Got UnityPlayerActivity Object!");
                            //print("Got UnityPlayerActivity Object!");

                            //////// Call goToEnd() method in the instance of UnityPlayerActivity /////////
                            jo.Call("EndScreen", timerText.text, (countColli+""));
                            //jo.Call("goToEnd");
                        }
                        else
                        {
                            //jc.CallStatic("UnitySendMessage", "text", "setText", "NOT Got UnityPlayerActivity Object!");
                            print("NOT Got UnityPlayerActivity Object!");
                        }
                    }
                }
                else
                {
                    jc.CallStatic("UnitySendMessage", "text", "setText", "NOT Got UnityPlayerActivity Class!");
                    print("NOT Got UnityPlayerActivity Class!");
                }

            }**********************************************/
		}
		
		
	}

	//[MenuItem("Tools/Write file")]
	void WriteString()
	{
		string path = "";
		string[] resultLog = new string[1];
		if (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer) {
			//path = Application.persistentDataPath + "/testLog.txt";
			path = "/Users/ryan4/Desktop" + "/" + infoBrg.getFileName() + ".txt";
			Debug.Log (path);
			resultLog[0] = infoBrg.getAccessLog();
			//Write some text to the test.txt file
			StreamWriter writer = new StreamWriter(path, true);
			writer.WriteLine(resultLog[0]);
			writer.Close();
			path = "/Users/ryan4/Desktop" + "/" + infoBrg.getFileName() + "Correct.txt";
			StreamWriter correct = new StreamWriter(path, true);
			correct.WriteLine(infoBrg.getCorrectNum());
			Debug.Log("ALL WRITE DOWN");
			correct.Close();
			path = "/Users/ryan4/Desktop" + "/" + infoBrg.getFileName() + "UserAccess.txt";
			StreamWriter userAccess = new StreamWriter(path, true);
			userAccess.WriteLine(infoBrg.getAccessNum());
			Debug.Log("ALL WRITE DOWN");
			userAccess.Close();


		}else if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer) {

			//path = "/root/storage/sdcard1/test.txt";
			path = Application.persistentDataPath + "/" + infoBrg.getFileName() + ".txt";//"/test.txt";
			//EasyTTSUtil.SpeechAdd("android");
			Debug.Log (path);
			resultLog[0] = infoBrg.getAccessLog();
			//Write some text to the test.txt file
			File.WriteAllLines(path,resultLog);
		}

		
		//Re-import the file to update the reference in the editor
		//AssetDatabase.ImportAsset(path); 
		//TextAsset asset = Resources.Load("test");
		
		//Print the text from the file
		//Debug.Log(asset.text);
	}
	
	void changeToEndGameScene()
	{
		//SceneManager.LoadScene("EndScreen");
		Application.LoadLevel ("EndScreen");
	}
	
	
	// CALLED FROM ANDROID STUDIO						// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
	public void setStartInUnity(string startpos)		// <<<<<<<<<<<<<<<<<<<<<<<<<<<<      <<<<<<<<<<<
	{													// <<<<<<<<<<<   <<<<<<<<<<<<   <<<<<  <<<<<<<<
		start = startpos;								// <<<<<<<<<<  <  <<<<<<<<<<   <<<<<<<<<<<<<<<<
	}													// <<<<<<<<<  <<<  <<<<<<<<<<<   <<<<<<<<<<<<<<
	public void setEndInUnity(string endpos)			// <<<<<<<<  <<<<<  <<<<<<<<<<<<   <<<<<<<<<<<<
	{													// <<<<<<<           <<<<<<<<<<<<<<  <<<<<<<<<<
		end = endpos;									// <<<<<<  <<<<<<<<<  <<<<<  <<<<<<<  <<<<<<<<<<<
	}													// <<<<<  <<<<<<<<<<<  <<<<<<        <<<<<<<<<<<
}  														// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
// <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
// For Text art:  
//		http://patorjk.com/software/taag/#p=display&f=Contessa&t=Android%20Device%20Functions