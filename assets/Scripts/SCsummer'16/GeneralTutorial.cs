using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GeneralTutorial : MonoBehaviour {

    private int selection;
    public Button backButton;

	void Start () {
        EasyTTSUtil.Initialize(EasyTTSUtil.UnitedStates);

        selection = 0;
        Button back = backButton.GetComponent<Button>();
        back.onClick.AddListener(Back);
    }
	
	void Update () {
	
	}

    void Back()
    {
        EasyTTSUtil.SpeechFlush("Back");
        Debug.Log("Back");

        StartCoroutine(MyCoroutine());
        if(selection > 0)
            Application.LoadLevel("1ModeSelect");
        selection++;
    }

    private IEnumerator MyCoroutine()
    {
        //This is a coroutine
        Debug.Log(Time.time);

        yield return new WaitForSeconds(1.5f);    //Wait one frame
        selection--;
        Debug.Log("waiting time");

    }
}
