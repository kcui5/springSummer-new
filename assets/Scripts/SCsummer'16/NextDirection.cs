using UnityEngine;
using System.Collections;

public class NextDirection {

    private static string nextDir;
    private static Vector3 start;
    private static int numOfCollisions;
    private static string endTime; 

    // Use this for initialization
    void Start () {

    }

    public void setNextDir (string dir)
    {
        nextDir = dir;
    }
    public string getNextDir()
    {
        return nextDir;
    }

    public void setStart(Vector3 pos)
    {
        start = pos;
    }
    public Vector3 getStart()
    {
        return start;
    }

    public void setEndTime(string t)
    {
        endTime = t;
    }
    public string getEndTime()
    {
        return endTime;
    }


    public void setNumOfCollisions(int c)
    {
        numOfCollisions=c;
    }
    public int getNumOfCollisions()
    {
        return numOfCollisions;
    }
}
