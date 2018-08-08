using UnityEngine;
using System.Collections;

public class TheInformationBridge
{
	public static bool gestModeOpen;
	public static bool permission;
	public static string instruction;
	public static int distFeet;
	public static int distMeter;
	public static float angleOfNextWP;
	public static bool music;
	public static int mode;
	public static string fileName;
	public static string titleInfor;

	public static string accessLog;
	public static string correctNum;
	public static string accessNum;
	public static int content = 0;
	public static bool isAutoGenerate;
	public static bool isReachTarget;
	public static int titleNum;
    public static string startingPosition;
	// Use this for initialization
	void Start () { 
		permission = false;
		gestModeOpen = false;
		music = true;
		mode = 0;
		correctNum = "";
		accessNum = "";
		isReachTarget = false;
		titleInfor = string.Empty;
		titleNum = 1;
        startingPosition = "";
	}
	 
	// Setters (Mutator)
	public void setInstruct(string instr)
	{
		instruction = instr;
	}
	public void setGestureModeActivity(bool g)
	{
		gestModeOpen = g;
	}
	public void setPermission(bool p)
	{
		permission = p;
	}
	public void setDistanceF(int df)
	{
		distFeet = df;
	}
	public void setDistanceM(int dm)
	{
		distMeter = dm;
	}
	public void setAngleOfNextWP(float dgre)
	{
		angleOfNextWP = dgre;
	}
	public void setMusicOfforOn(bool on)
	{
		music = on;
	}
	public void setTutorialMode(int i)
	{
		mode = i;
	}
	public void setAccessLog(string s)
	{
		accessLog += s;
		accessLog += "\n";
		//content ++;
	}
	public void setCorrectNum(string s)
	{
		correctNum += s;
		correctNum += "\n";
	}
	public void setAccessNum(string s)
	{
		accessNum += s;
		accessNum += "\n";
	}
	public void setFileName(string s)
	{
		fileName += s;
	}
	public void setAutoGenerateStates(bool b)
	{
		isAutoGenerate = b;
	}
	public void setReachTarget(bool b)
	{
		isReachTarget = b;
	}
	public void setTitleInfor(string s)
	{
		titleInfor = s;
	}
	public void setTitleNum(int i){
		titleNum += i;
	}
    public void setStartingPosition(string pos)
    {
        startingPosition = pos;
        Debug.Log("Info Brg Starting Point 1: " + startingPosition);
    }

	//Getters (Accessor)
	public string getNextInstruction()
	{
		return instruction;
	}
	public bool isGestureModeActive()
	{
		return gestModeOpen;
	}
	public bool hasPermission()
	{
		return permission;
	}
	public int getDistanceF()
	{
		return distFeet;
	}
	public int getDistanceM()
	{
		return distMeter;
	}
	public float getAngleOfNextWP()
	{
		return angleOfNextWP;
	}
	public bool musicOn()
	{
		return music;
	}

	public int getTutorialMode()
	{
		return mode;
	}
	public string getAccessLog()
	{
		return accessLog;
	}
	public string getFileName()
	{
		return fileName;
	}
	public string getCorrectNum()
	{
		return correctNum;
	}
	public string getAccessNum()
	{
		return accessNum;
	}
	public bool getAutoGenerateStates()
	{
		return isAutoGenerate;
	}
	public bool getReachTarget()
	{
		return isReachTarget;
	}
	public string getTitleInfor()
	{
		return titleInfor;
	}
	public int getTitleNum()
	{
		return titleNum;
	}
    public string getStartingPosition()
    {
        Debug.Log("Info Brg Starting Point 2: " + startingPosition);
        return startingPosition;
    }

}
