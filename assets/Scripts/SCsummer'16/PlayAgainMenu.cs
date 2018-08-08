using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
//using UnityEngine.SceneManagement;

public class PlayAgainMenu : MonoBehaviour
{
	public Button yesButton, NoButton;
	
	GameController1 gameController;
	TheInformationBridge inforBrg;
	private int selection;
	
	void Start()
	{
		EasyTTSUtil.Initialize (EasyTTSUtil.UnitedStates);
		gameController = gameObject.GetComponent<GameController1> ();
		Button yes = yesButton.GetComponent<Button> ();
		Button no = NoButton.GetComponent<Button> ();
		
		selection = 0;
		inforBrg = new TheInformationBridge ();
		inforBrg.setReachTarget (false);

		EasyTTSUtil.SpeechAdd("Do you want play again");
		Debug.Log("DO you want play again?");
		yes.onClick.AddListener (ClickYes);
		no.onClick.AddListener (ClickNo);
		
		
	}
	
	void Update()
	{
		
	}
	
	void ClickYes()
	{
		EasyTTSUtil.SpeechAdd ("Yes");
		Debug.Log ("yes");
		//StartCoroutine (MyCoroutine());
		//if(selection > 0)
		inforBrg.setTitleNum (1);
			Application.LoadLevel ("CCNYGrove");
		//selection ++;
	}
	
	void ClickNo()
	{
		EasyTTSUtil.SpeechAdd ("no");
		Debug.Log ("no");
		//StartCoroutine (MyCoroutine());
		//if(selection > 0)
			Application.LoadLevel ("1ModeSelect");
		//selection ++;
	}
	

	private IEnumerator MyCoroutine()
	{
		//This is a coroutine
		Debug.Log (Time.time);
		
		yield return new WaitForSeconds (1.5f);    //Wait one frame
		selection--;
		Debug.Log ("waiting  time ");
		
	}
	
}