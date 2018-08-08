using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
//using UnityEngine.SceneManagement;// use in the version 5.3

public class PlayerMovement_Tutorial : MonoBehaviour { 
	

	public float speed;
	public float speedAndroid;
	public float rotateSpeed = 90f;

	Vector3 movement;
	Vector3 origin;
	Vector3 practicePosition;
	RuntimePlatform platform = Application.platform;
	Rigidbody playerRigidbody;
	public int mode;
	private int trainingTimes;
	private float movedDistance;

	bool couldBeSwipe;
	float comfortZone = 5000f;
	float minSwipeDist = 100f;
	Vector2 startpos;
	//Vector2 touchDeltaPos;
	//Vector2 basePos;

	AudioSource source;
	public AudioClip footSteps;
	bool isMoving;

	string start;
	string end;
	String instruct;
	TheInformationBridge infoBrg;

	string correctDirection;
	string accessDirection;
	private int logCount;
	bool isAllowMove;
	bool isReachTarget;


	void Start()
	{
		// Initialize the player
		EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates);	// text to speech
		//EasyTTSUtil.OpenTTSSetting ();
		playerRigidbody = GetComponent<Rigidbody>();
		source = GetComponent<AudioSource> ();
		mode = 0;
		origin = new Vector3 (-40f, 2f, -53f);
		practicePosition = new Vector3 (50.4f, 2f, -50.7f);
		transform.position = origin;
		start = "0,0,0";
		//"Not defined yet";
		end = "0,0,0";
		//"Not defined yet";
		isMoving = false;
		source.clip = footSteps;
		source.volume = 1f;

		trainingTimes = 2;
		movedDistance = 0;
		instruct = "";
		infoBrg = new TheInformationBridge ();
		correctDirection = "";
		accessDirection = "";
		logCount = 0;
		isAllowMove = true;
		isReachTarget = false;

		
	}
	

	// Update is called once per frame
	void Update()
	{

		if (mode == 0) {
			introMessage ();
			mode ++;
		}
		if (mode == 5) {
			printNewInstruction();
		}
		adjustToTarget ();
		calculateDistance ();
		if (isAllowMove) {
			walkingSound ();
			inputFunction ();
		}else{
			isMoving = false;
		}
		preventDriftingRotation ();
	}

	void introMessage()
	{
		EasyTTSUtil.SpeechAdd ("Welcome to the Sight City game tutorial. " +
                               "In this tutorial, we are going to control a player to explore a floor layout. First " +
                               "Let's study how to control the player. Please put your finger on the screen and " +
                               "swipe up to move forward.");
        isAllowMove = false;
		StartCoroutine (MyCoroutine(10));

		Debug.Log ("moveforward tutorial");
		correctDirection = "Forward";
	}

	void resetTutorial()
	{
		if (mode != 0) {
			
			switch (mode) {
			case 2: // move forward practice
				EasyTTSUtil.SpeechAdd("Great! You got it! Now you know how to move forward. Let's practice." +
                                      "We need to move 10 meters forwards. Notice that if you want to keep the player " +
                                      "moving, please hold your finger on the screen after you swipe. When you hear the" +
                                      "step sound, that means you are moving");
                    Debug.Log (" training finished");
				isAllowMove = false;
				StartCoroutine (MyCoroutine(14));
				trainingTimes = 2;
				movedDistance = 0;
				transform.position = origin;
				infoBrg.setTutorialMode(mode);
				correctDirection = "Forward";

				break;
			case 3: 
				EasyTTSUtil.SpeechAdd ("Excellent job. Now you are going to learn how to turn the player to the left." +
                                        "Please put your finger on the screen" +
                                        "and swipe it to the left. Let's try it.");
                    Debug.Log ("left turn tutorial");// turn left tutorial
				isAllowMove = false;
				source.Stop();
				StartCoroutine (MyCoroutine(6));
				trainingTimes = 2;
				movedDistance = 0;
				transform.position = origin;
				correctDirection = "Left";

				break;
			case 4: // turn right tutorial
				EasyTTSUtil.SpeechAdd ("Excellent. Now you are going to learn how to turn the player to the right." +
                                        "Please put your finger on the screen" +
                                        "and swipe it to the right. Let's try it.");
                    Debug.Log ("Right turn tutorial");// turn left tutorial
				isAllowMove = false;
				source.Stop();
				StartCoroutine (MyCoroutine(6));
				trainingTimes = 2;
				movedDistance = 0;
				transform.position = origin;
				correctDirection = "Right";
				break;
			case 5: // combination of all movement, try a simple route
				EasyTTSUtil.SpeechAdd("Excellent. Let's practice with a sample route.");
				Debug.Log (" practice.");
				isAllowMove = false;
				source.Stop();
				StartCoroutine (MyCoroutine(5));
				trainingTimes = 1;
				movedDistance = 0;
				transform.position = practicePosition;
				infoBrg.setTutorialMode(mode);
				break;
			case 6: // finish this part and explain the training mode and game mode
				EasyTTSUtil.SpeechAdd ("Very good. You learned how to access the player movements. You can now" +
                                       "start your own training.");
                    //"Remember that in the training mode you can only follow" +
                    //" the instruction to help you memorize the route. In the game mode, you can have more free" +
                    //" movment.");
                    Debug.Log ("Explaination about training mode and game mode.");
				source.Stop();
				Debug.Log ("start wait " + Time.time);
				StartCoroutine (MyCoroutine(10));
				Debug.Log ("end wait " + Time.time);


				//Application.LoadLevel("TrainingModeTutorial");
				break;
			default :
				break;
				
			}
		}
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
					infoBrg.setAccessLog ("Correct access: turn" + accessDirection + ". " + timeLog);
					if (accessDirection == "Left") {
						infoBrg.setCorrectNum (3 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						infoBrg.setAccessNum (4 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
					}
					if (accessDirection == "Right") {
						infoBrg.setAccessNum (0 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						infoBrg.setCorrectNum (1 + " ");//+ " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
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
							infoBrg.setCorrectNum (2+ " ");// + "access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						} else if (correctDirection == "Left")
							infoBrg.setCorrectNum (3+ " ");// + " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
						else if (correctDirection == "Right")
							infoBrg.setCorrectNum (1+ " ");// + " access: " + accessDirection + " correct: " + correctDirection + "; " + timeLog);
					}
				} else {
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
			if (logCount > 25)
				logCount %= logCount;
		}
	}


	void inputFunction()
	{
		if (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer)
			OSXInputFunction ();
		if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
			androidTouchInput2 ();
		/*if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer){	
		windowsInputFunction ();
		androidTouchInput2 ();
		}*/

	}

	void calculateDistance()
	{
		movedDistance = (int)(Mathf.Sqrt (Mathf.Pow ((transform.position.x - origin.x), 2) + 
		                                 Mathf.Pow ((transform.position.z - origin.z), 2)) / 3); 
	}
	/*  
	 * Updates the timer label on the Canvas
	 * >>Should be in Unity's Update Function<<
	 */
	

	/*
	
	void windowsInputFunction()
	{
		string direction = "";
		// Get from Input
		if (Input.GetKey (KeyCode.UpArrow)) 
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
		
		if (Input.GetKeyUp (KeyCode.UpArrow))
			isMoving = false;
	}*/
	
	void OSXInputFunction()
	{
		//string direction = "";
		//string newInstruct = infoBrg.getNextInstruction ();
		// Get from Input
		if (Input.GetKey (KeyCode.UpArrow)) 
		{
			accessDirection = "Forward";
			moveForward ();
			isMoving = true;
			accessLog ();
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow) || Input.GetKeyDown (KeyCode.A)) {
			accessDirection = "Left";
		
			rotateAngle (1);
			accessLog ();
		}
		if (Input.GetKeyDown (KeyCode.RightArrow) || Input.GetKeyDown (KeyCode.D)) {
			accessDirection = "Right";
		
			rotateAngle (2);
			accessLog ();
		}
		
		if (Input.GetKeyUp (KeyCode.UpArrow))
			isMoving = false;
	}


	void androidTouchInput2()
	{
		Touch touch;
		float swipeDist = 0;
		
		if (Input.touchCount > 0) 
		{
			
			//***********************************************************************					
			if (Input.GetTouch (0).tapCount == 3)	// Triple Tap Function 
				resetTutorial ();						// on Android Device
			
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
					moveInAndroid (swipeDist * speedAndroid);//	<-1-- Allows Free-Movement in Android Device

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
					moveInAndroid (swipeDist * speedAndroid); 		//	<-1-- Allows Free-Movement in Android Device
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
		
							rotate (xDistance);	
						
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
	

	void walkingSound()
	{
		if (isMoving && isAllowMove) {
			
			if (!source.isPlaying) {
				source.loop = true;
				source.PlayScheduled(5f);
				source.Play ();
			}
			//Debug.Log ("Hear walking sound");
		} else {
			source.loop = false;
			source.Stop ();
			//Debug.Log ("Not hear walking sound");
		}
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

		if(mode == 1 ){
				mode++;
				resetTutorial();
		}

		if (mode == 2) {
			
			if(movedDistance >= 10){
				if(trainingTimes >  0){
                    EasyTTSUtil.SpeechAdd("Good job. Let's try" + trainingTimes);
                    if (trainingTimes == 1)
                        EasyTTSUtil.SpeechAdd("more time.");
                    if (trainingTimes > 1)
                        EasyTTSUtil.SpeechAdd("more times.");
                    Debug.Log (trainingTimes + " more time(training)");
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(3));
					trainingTimes --;
					transform.position = origin;
					movedDistance = 0;
				} else{
					mode++;
					resetTutorial();
				}
			}
		}

		//Debug.Log ("Step: " + transform.position + " x:" + transform.position.x + " z:" + transform.position.z);
	}


	void rotateAngle(int direction)
	{
		Vector3 rot = transform.rotation.eulerAngles;
		Vector3 newRot;
		if (direction == 1) 
		{ 						// Left
			transform.Rotate (0, -90f, 0);
			newRot = transform.rotation.eulerAngles;

			if (mode == 3)
			{
				if(trainingTimes >  0){
                    EasyTTSUtil.SpeechAdd("Good job. Let's try" + trainingTimes);
                    if (trainingTimes == 1)
                        EasyTTSUtil.SpeechAdd("more time.");
                    if (trainingTimes > 1)
                        EasyTTSUtil.SpeechAdd("more times.");
                    Debug.Log (trainingTimes + " more time(training)");
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(3));
					trainingTimes --;
					transform.position = origin;
				} else{
					mode++;
					resetTutorial();
				}

			}
			//Debug.Log("Degree Turned: " + readDegreeTurnMade (rot, newRot));
			//Debug.Log ("Left Turn...");
		} 
		else if (direction == 2) 
		{ 						// Right
			transform.Rotate (0, 90f, 0);
			newRot = transform.rotation.eulerAngles;
			//Debug.Log("Degree Turned: " + readDegreeTurnMade (rot, newRot));
			//Debug.Log ("Right Turn...");

			if (mode == 4)
			{
				if(trainingTimes >  0){
                    EasyTTSUtil.SpeechAdd("Good job. Let's try" + trainingTimes);
                    if (trainingTimes == 1)
                        EasyTTSUtil.SpeechAdd("more time.");
                    if (trainingTimes > 1)
                        EasyTTSUtil.SpeechAdd("more times.");
                    Debug.Log (trainingTimes + " more time(training)");
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(3));
					trainingTimes --;
					transform.position = origin;
				} else{
					mode++;
					resetTutorial();
				}
				
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
		accessDirection = "Forward";

		if (v >= 0) { 		// Only takes upward swipes
			// Moves the avatar forward
			movement.Set (0f, 0f, v);						
			movement = transform.forward;
			movement = movement.normalized * speed * Time.deltaTime * 10;
			playerRigidbody.MovePosition (transform.position + movement);

			accessLog ();

			if(mode == 1 ){
					mode++;
					resetTutorial();
			}
			
			if (mode == 2) {
				
				if(movedDistance >= 10){
					if(trainingTimes >  0){
                        EasyTTSUtil.SpeechAdd("Good job. Let's try" + trainingTimes);
                        if (trainingTimes == 1)
                            EasyTTSUtil.SpeechAdd("more time.");
                        if (trainingTimes > 1)
                            EasyTTSUtil.SpeechAdd("more times.");
                        Debug.Log (trainingTimes + " more time(training)");
						isAllowMove = false;
						source.Stop();
						StartCoroutine (MyCoroutine(3));
						trainingTimes --;
						transform.position = origin;
						movedDistance = 0;
					} else{
						mode++;
						resetTutorial();
					}
				}
			}
		}

	}

	/*
	 * Movement method to rotate the avatar left or right
	 * 90 DEGREES with swipes on the ANDROID device (Mobile Application).
	 * @param h - Horizontal direction swipe
	 */
	void rotate(float h)	
	{
		if (h >= 0) 								// Determines if left or right swipe
		{
			accessDirection = "Right";
			transform.Rotate (0, 90f, 0);//** Right Rotation
			accessLog ();
			EasyTTSUtil.SpeechFlush ("Right " + 90 + " degree turn made");
			if (mode == 4)
			{
				if(trainingTimes >  0){
                    EasyTTSUtil.SpeechAdd("Good job. Let's try" + trainingTimes);
                    if (trainingTimes == 1)
                        EasyTTSUtil.SpeechAdd("more time.");
                    if (trainingTimes > 1)
                        EasyTTSUtil.SpeechAdd("more times.");
                    Debug.Log (trainingTimes + " more time(training)");
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(3));
					trainingTimes --;
					transform.position = origin;
				} else{
					mode++;
					resetTutorial();
				}
				
			}
			
		}else {
			accessDirection = "Left";
			transform.Rotate (0, -90f, 0);			// **Left Rotation
			accessLog();
			EasyTTSUtil.SpeechFlush ("Left " + 90 + " degree turn made");
			if (mode == 3)
			{
				if(trainingTimes >  0){
                    EasyTTSUtil.SpeechAdd("Good job. Let's try" + trainingTimes);
                    if (trainingTimes == 1)
                        EasyTTSUtil.SpeechAdd("more time.");
                    if (trainingTimes > 1)
                        EasyTTSUtil.SpeechAdd("more times.");
                    Debug.Log (trainingTimes + " more time(training)");
					isAllowMove = false;
					source.Stop();
					StartCoroutine (MyCoroutine(3));
					trainingTimes --;
					transform.position = origin;
				} else{
					mode++;
					resetTutorial();
				}
				
			}

		}
	}

	void printNewInstruction()
	{
		string newInstruct = infoBrg.getNextInstruction ();

		
		if (newInstruct != instruct) 
		{
			if(newInstruct != null)
				Debug.Log ("Waypoint direction: " + newInstruct);  		// <----DEBUGGING
			
			if(newInstruct == "Forward"){
				EasyTTSUtil.SpeechAdd("Walk " + newInstruct);
				correctDirection = "Forward";
			}
			else if (newInstruct == "Behind"){
				EasyTTSUtil.SpeechAdd(" Turn left.");
				correctDirection = "Left";
			}
			else if(newInstruct != null){
				EasyTTSUtil.SpeechAdd("Turn " + newInstruct);
				correctDirection = newInstruct;
			}
			
			instruct = newInstruct;	
		}
	}

	void adjustToTarget()
	{
		RaycastHit hit;
		float radius = 3;

		Debug.DrawRay (transform.position, (transform.forward*radius)*1.5f, Color.white, 0f, true);

		if (Physics.Raycast (transform.position, transform.forward, out hit, radius * 1.5f)) {
			if (hit.collider.gameObject.tag == "target") {
				transform.position = hit.collider.gameObject.transform.position;
			}
		}
	}


	void OnTriggerEnter(Collider col)
	{

		if (col.gameObject.CompareTag("target"))
		{
			EasyTTSUtil.SpeechAdd("Congratulations, you've reached your destination.");
			Debug.Log("Game Over, you've completed the course");
			isAllowMove = false;
			isReachTarget = true;
			infoBrg.setReachTarget(isReachTarget);
			source.Stop();


			mode++;
			resetTutorial();

		}
		
		
	}

	void distanceFromWayPoint()
	{
		Debug.Log ("Next-Waypoint is about " + movedDistance + " meters away");
		EasyTTSUtil.SpeechFlush( "" + movedDistance + " meters walked.");
	}

	public void setStartInUnity(string startpos){
		start = startpos;
	}

	public void setEndInUnity(string endpos){
		end = endpos;
	}
}