using UnityEngine;
using System.Collections;

public class RoomLocations
{
    private Vector3[,] locatCoor = new Vector3[4, 4];                            // The vector3 coordinates of each location
    private string[,] locatName = new string[4, 4];                                //             *SUB-MENUS*
    private Vector3 start = new Vector3 (0,0,0);
    private Vector3 end = new Vector3 (0,0,0);

    public RoomLocations()
    {
        locatName[0, 0] = "exit1";
        locatName[0, 1] = "exit2";
        locatName[0, 2] = "exit3";
        locatName[0, 3] = "exit4";

        locatName[1, 0] = "elevator";
        locatName[1, 1] = "escalator";
        locatName[1, 2] = "staira";
        locatName[1, 3] = "stairb";

        locatName[2, 0] = "conference";
        locatName[2, 1] = "waiting";
        locatName[2, 2] = "office";
        locatName[2, 3] = "lecture";

        locatName[3, 0] = "bathroom";
        locatName[3, 1] = "utility";
        locatName[3, 2] = "lockerroom";           //Change to: Chemistry Lab
        locatName[3, 3] = "medical";              //Change to: Physic Lab

        locatCoor[0, 0] = new Vector3 (109f, 3f, -107f);    //Exit1
        locatCoor[0, 1] = new Vector3 (-115f, 3f, -175f);   //Exit2
        locatCoor[0, 2] = new Vector3 (-27f, 3f, -51f);     //Exit3
        locatCoor[0, 3] = new Vector3 (10f, 3f, 165f);      //Exit4

        locatCoor[1, 0] = new Vector3 (77f, 3f, -53f);      //Elevator
        locatCoor[1, 1] = new Vector3 (-4f, 3f, -34f);      //Escalator
        locatCoor[1, 2] = new Vector3 (103f, 3f, -17f);     //StairA
        locatCoor[1, 3] = new Vector3 (79f, 3f, 150f);      //StairB

        locatCoor[2, 0] = new Vector3 (-12f, 3f, 106f);     //Conference
        locatCoor[2, 1] = new Vector3 (-12f, 3f, 3f);       //Waiting
        locatCoor[2, 2] = new Vector3 (-85f, 3f, 24f);      //Office
        locatCoor[2, 3] = new Vector3 (-80f, 3f, 164f);     //Lecture

        locatCoor[3, 0] = new Vector3 (44f, 3f, -122.4f);   //Bathroom
        locatCoor[3, 1] = new Vector3 (-45f, 3f, -100f);    //Utility
        locatCoor[3, 2] = new Vector3 (201f, 3f, -39f);     //LockerRoom
        locatCoor[3, 3] = new Vector3 (-110f, 3f, -25f);    //Medicals
    }

    //Display name for subMenus
    public string getLocationName (int ctgry, int rm)
    {
        return (locatName.GetLength (0) < ctgry && locatName.GetLength (1) < rm) ?
               locatName[ctgry, rm] : "Location not found";
    }

    //get Coordinates of selected points (by index)
    public Vector3 getLocationCoor (int ctgry, int rm)
    {
        return (locatCoor.GetLength (0) < ctgry && locatCoor.GetLength (1) < rm) ?
               locatCoor[ctgry, rm] : new Vector3 (0,0,0);
    }

    //get Coordinates of selected points (by string)
    public Vector3 getLocationCoor (string room)
    {
        Vector3 vec = new Vector3 (0,0,0);
        room.ToLower();     //lowercase

        for (int ctgry=0; ctgry < locatCoor.GetLength (0); ctgry++)
        {
            for (int rm=0; rm < locatCoor.GetLength (1); rm++)
            {
                if (locatName[ctgry, rm] == room)
                    vec = locatCoor[ctgry, rm];
            }
        }

        return vec;
    }

    //Accessor Methods
    public Vector3 getStartingPoint()
    {
        return start;
    }
    public Vector3 getEndingPoint()
    {
        return end;
    }

    // Mutator Methods
    public void setStartingPoint (Vector3 vec)
    {
        start = vec;
        Debug.Log ("Starting point set");
    }
    public void setEndingPoint (Vector3 vec)
    {
        end = vec;
        Debug.Log ("Ending point set");
    }
    public void setStartingPoint (string room)
    {
        start = getLocationCoor (room);
        Debug.Log ("Starting point set");
    }
    public void setEndingPoint (string room)
    {
        end = getLocationCoor (room);
        Debug.Log ("Ending point set");
    }
}
