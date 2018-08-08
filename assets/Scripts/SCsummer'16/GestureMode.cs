using UnityEngine;
using System.Collections;

public class GestureMode : MonoBehaviour {

	float heldDown;
	float minHoldTime = 2f;
	Vector2 gestStartPos;
	float maxSwipeDist = 20f;	
	float minSwipeDist = 10f;
	Vector2 startpos;
	bool isHold;
	bool couldBeSwipe;
	bool couldBeSwipeG;

	GameObject panel;
	bool popUp;	
	private GUIStyle style;
	RuntimePlatform platform = Application.platform;
	TheInformationBridge infoBrg;

	// Use this for initialization
	void Start () 
	{
		panel = GameObject.FindWithTag ("Gesture");
		heldDown = 0;
		popUp = false;
		infoBrg = new TheInformationBridge ();
		style = new GUIStyle ();
		style.fontSize = 50;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer) {
			pressForGestureMode ();
			holdDownForGestureMode ();
		}else if (platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer){
			pressForGestureMode ();
			holdDownForGestureMode ();
		}
		else
			holdDownForGestureMode ();

		//if (popUp)
			//panel.SetActive(true);
		//else
			//panel.SetActive(false);
	}

	/*
	 * 	Open Mode with a press of the 'G'
	 *  key on the keyboard for debugging
	 */
	void pressForGestureMode()
	{
		if (Input.GetKey (KeyCode.G)) {
			popUp = true;
			infoBrg.setGestureModeActivity (true);
		} else {
			popUp = false;
			infoBrg.setGestureModeActivity (false);
		}
	}

	void holdDownForGestureMode()
	{
		Touch touch;

		if (Input.touchCount > 0) 
		{
			touch = Input.GetTouch (0);					// Obtain 1st finger touch

			switch (touch.phase) 					
			{
			case TouchPhase.Began:
					//Debug.Log ("TouchDown");
				heldDown = 0;						// Reset 'finger held down' timer 
				isHold = true;						// Potential to be a valid hold
				startpos = touch.position;			// Store first touch position
				break;

	// <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <>

			case TouchPhase.Moved: 
				if (!popUp)  						//GestureMode not open?
				{	
					Debug.Log ("Moving");
					float dist = (touch.position - startpos).magnitude;			// Get current touch position and start position difference
				if (dist > maxSwipeDist)										// If touch hold moves too far...
						isHold = false;											// Not considered as hold
				}
				else 								//GestureMode is open?
				{	
					Debug.Log ("GestureMode: Swipe Moving");				
					float distG = (touch.position - gestStartPos).magnitude;	// Get swipe Distance during gesture mode
					if (minSwipeDist > distG)									// If swipe is too short...
						couldBeSwipeG = false;									// Not considered as swipe
					else                                                        // else...
						couldBeSwipeG = true;									// Potiential to be swipe
				}
				break;

		// <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <>

			case TouchPhase.Stationary:
					//Debug.Log ("Touch Still");
				heldDown += Time.deltaTime;										// Keep track of time of touch held down

				if (heldDown > minHoldTime && !popUp && isHold) 				// Touch held for ___ time?? & GestureMode not open?? & potential to be hold??
				{
					Debug.Log ("GESTURE MODE ACTIVE: (Time held: " + heldDown + ")");	
					popUp = true;												// Open Gesture Mode Panel
					infoBrg.setGestureModeActivity(true);						// used to freeze avatar's movement
					//Handheld.Vibrate();											// vibration feedback

					// GestureMode variables initialization
					gestStartPos = (touch.position);							// Store position of touch when gesture mode activated
					couldBeSwipeG = true;										// Potential to be considered as swipe
				}
				break;

		// <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <> <>

			case TouchPhase.Ended:
				Debug.Log ("Touch Released **");
				Vector2 gestSwipeDist = touch.position - gestStartPos;
				
				if(couldBeSwipeG && minSwipeDist < gestSwipeDist.magnitude && popUp)	// Valid Swipe?
				{
					if(Mathf.Abs(gestSwipeDist.x) > Mathf.Abs(gestSwipeDist.y))	// Vertical or Horizontal Swipe?
					{
						Debug.Log("Horizontal Gesture Swipe");	
						if (gestSwipeDist.x > 0){					// Left or Right?
							Debug.Log ("Right Swipe Action Gesture");		// <---Insert Gesture Action (1) Here *Right*
							EasyTTSUtil.SpeechFlush("Right Gesture. Norbu is awesome");
						}
						else{
							Debug.Log ("Left Swipe Action Gesture");		// <---Insert Gesture Action (2) Here *Left*
							EasyTTSUtil.SpeechFlush("Left Gesture. Juan is awesome");
						}
					}
					else
					{
						Debug.Log("Verticcal Gesture Swipe");
						if (gestSwipeDist.y > 0)
						{						// Up or Down?
							Debug.Log ("Up Swipe Action Gesture");			// <---Insert Gesture Action (3) Here *Up*
							EasyTTSUtil.SpeechFlush("Up Gesture. music on");
							infoBrg.setMusicOfforOn (true);
						}
						else
						{
							Debug.Log ("Down Swipe Action Gesture");		// <---Insert Gesture Action (4) Here *Down*
							EasyTTSUtil.SpeechFlush("Down Gesture. music off");
							infoBrg.setMusicOfforOn (false);
						}
					}

				}	

					popUp = false;												// Close GestureMode Panel
					infoBrg.setGestureModeActivity(false);						// Used to unfreeze avatar's movement
					break;


				default:
					break;
			} 

		}
	}

	/*
	 // 		Currently not in use...
	 //  		(Soon to be deleted)

	void gestureModeActive(Vector2 startpos)
	{
		Touch touch;
		float swipeDist = 0;

		if (Input.touchCount > 0 && infoBrg.isGestureModeActive()) 
		{
			touch = Input.GetTouch (0);
			switch(touch.phase)
			{
			case TouchPhase.Began:							// v TOUCH BEGINNING PHASE v
				Debug.Log("GEST TOUCH DOWN");
				couldBeSwipe = true;						//  	chance of being a swipe
				//startpos = touch.position;				//  	take of first position touched	
				break;			

				//******************************************************************

			case TouchPhase.Moved:									// v TOUCH WHILE MOVING v
				//swipeDist = touch.position.y - startpos.y; 			//	take note of how far swiped
				Debug.Log("Gest: Swipe in progress");
				if (Mathf.Abs (touch.position.y - startpos.y) > comfortZone) 		// is swipe too far?
				{																	// >> if so, not a valid swipe
					couldBeSwipe = false;											
				}
				break;

				//*******************************************************************

			case TouchPhase.Stationary:								// v TOUCH HELD DOWN (NOT MOVING) v
				swipeDist = touch.position.y - startpos.y;			// take note of how far swiped
				Debug.Log("Gest: Swipe is stationary");
				//if(minSwipeDist < Mathf.Abs(swipeDist))				// CONDITION: 	Is swipe distance greater than minSwipeDistance?	
					//moveInAndroid (swipeDist * speedAndroid);   	// ACTION: 		WHILE SWIPE IS HELD DOWN AT A POINT
					//autoWalkInAndroid(swipeDist * speedAndroid);	// 				call the method *moveInAndroid()*
				break;
				//*******************************************************************
			case TouchPhase.Ended:
				swipeDist = (touch.position - startpos).magnitude;
				Debug.Log("Gest: Swipe Finished");
				if (couldBeSwipe && (swipeDist > minSwipeDist)) 
				{
					float xDistance = touch.position.x - startpos.x;
					float yDistance = touch.position.y - startpos.y;	

					if (Mathf.Abs (xDistance) > Mathf.Abs (yDistance)) 
					{
						Debug.Log ("Horizontal Swipe");
						if (xDistance >= 0)
							Debug.Log ("Right");
						else
							Debug.Log("Left");
					} else
					{
						Debug.Log ("Vertical Swipe");
						if (yDistance >= 0)
							Debug.Log ("Up");
						else 
							Debug.Log("Down");	
						
					}
				}
				break;
			default:
				break;	
			}
		
		}
	}
	*/


}
