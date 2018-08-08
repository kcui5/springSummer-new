using UnityEngine;
using System.Collections;

public class RoomLocationsConf
{
    //private Vector3[,] locatCoor = new Vector3[4, 4];                            // The vector3 coordinates of each location
    //private string[,] locatName = new string[4, 4];                                //             *SUB-MENUS*

    //Jagged Array
    private string[][] floorRoom = new string[4][];
    private Vector3[][] floorCoor = new Vector3[4][];
    public int axis1 =  2;
    public int axis2 = 202;
    public int axis3 = 402;
	NextDirection nextDir;

	private static Vector3 start;
	private static Vector3 end;

    /*
    private string[][] firstFloorRoom = new string[4][];
    private Vector3[][] firstFloorCoor = new Vector3[4][];
    public int axis2 = 200;

    //private string[][] thirdFloorRoom = new string[4][];
    //private Vector3[][] thirdFloorCoor = new Vector3[4][];
    //public int axis3 = 400;
    */
    //string tay = Dynamicrooms.getStartingPoint();
    //private string startingRoom;
    //private string destinationRoom;
    // private Vector3 startPoint = new Vector3(0, 0, 0); //setStartingPoint(tayo)
    // private Vector3 endPoint = new Vector3(0, 0, 0);

    /*
    **SECOND FLOOR**

    Rooms
    1.    Gaslamp A
    2.    Gaslamp B
    3.    Gaslamp C
    4.    Gaslamp D
    5.    La Jolla A
    6.    La Jolla B
    7.    Old Town A
    8.    Old Town B
    9.    Balbora A
    10.    Balbora B
    11.    Balbora C

    Comforts
    1.    Seaport Foyer
    2.    Palm Foyer
    3.    Cyber Café
    4.    Bathroom near Showcase Suite G
    5.    Bathroom near Showcase Suite H
    6.    East Bathroom

    Services
    1.    Registration
    2.    Showcase Suite A
    3.    Showcase Suite B
    4.    Showcase Suite F
    5.    Showcase Suite G
    6.    Showcase Suite H
    7.    Seaport Ball Room

    Entrances/Exits
    1.    Main Entrance
    2.    East Elevator
    3.    West Elevator
    4.    Stairs by Showcase Suite A
    5.    Stairs by Showcase Suite B
    6.    Stairs by East Bathroom
    */
    public RoomLocationsConf()
    {
      
        //String[]: Rooms_____11      // First Floor Rooms
        floorRoom[0] = new string[] { "Grand Hall",
                                      // Second Floor Rooms
                                      "Gaslamp A",    "Gaslamp B",    "Gaslamp C",    "Gaslamp D", "La Jolla A",
                                      "La Jolla B",   "Old Town A",   "Old Town B",   "Balbora A", "Balbora B",
                                      "Balbora C",
                                      // Third Floor Rooms
                                      "Torrey Hills A","Torrey Hills B","Golden Hill A","Golden Hill B",
                                      "HillCrest A","HillCrest B","HillCrest C","HillCrest D","Cortez A","Cortez B","Cortez C",
                                      "Bankers Hill","Mission Beach A","Mission Beach B","Mission Beach C","Solana Beach A",
                                      "Solana Beach B","Ocean Beach","Promenade A","Promenade B","Pier","Cove"
                                    };

        //String[]: Comforts__6         // First Floor Comforts
        floorRoom[1] = new string[] { "Bathroom", "Sports Bar",
                                      // Second Floor Comforts
                                      "Seaport Foyer",   "Palm Foyer",   "Cyber Cafe",   "Bathroom near Showcase Suite G",
                                      "Bathroom near Showcase Suite H",  "East Bathroom 2",
                                      // Third Floor Comforts
                                      "West Bathroom","East Bathroom 3"
                                    };

        //String[]: Services__7         // Second Floor Services
        floorRoom[2] = new string[] { "Registration",     "Showcase Suite A", "Showcase Suite B", "Showcase Suite F",
                                      "Showcase Suite G", "Showcase Suite H", "Seaport Ball Room",
                                      // Third Floor Services
                                      "Foyer 3"
                                    };
        //String[]: Entr/Ext__6         // First Floor Ext/Ent
        floorRoom[3] = new string[] {   "West Elevator","East Elevator","Staircase X","Staircase Y","Staircase Z","Emergency Exit",
                                        "Main Entrance",
                                        // Second Floor Ext/Ent
                                        "Main Entrance", "East Elevator", "West Elevator", "Stairs by Showcase Suite A",
                                        "Stairs by Showcase Suite B",     "Stairs by East Bathroom",
                                        // Third Floor Ext/Ent
                                        "West Elevator","East Elevator", "Staircase North", "Staircase South"
                                    };

        //Vector3[]: Rooms11                // First Floor rooms
        floorCoor[0] = new Vector3[] {          new Vector3 (13, axis1, 40),
                      // Second Floor rooms
                      new Vector3 (165, axis2, -65),   new Vector3 (165, axis2, -89),   new Vector3 (165, axis2, -113),
                      new Vector3 (165, axis2, -137),  new Vector3 (133, axis2, -66),   new Vector3 (133, axis2, -91),
                      new Vector3 (133, axis2, -113),  new Vector3 (133, axis2, -138),  new Vector3 (165, axis2, 8),
                      new Vector3 (165, axis2, -17),   new Vector3 (165, axis2, -400),
                      //Third Floor rooms
                      new Vector3 (138, axis3, -34 ),  new Vector3 (138, axis3, -57),   new Vector3 (138, axis3, -78),
                      new Vector3 (138, axis3, -98),   new Vector3 (166, axis3, -34),   new Vector3 (166, axis3, -56),
                      new Vector3 (166, axis3, -77),   new Vector3 (166, axis3, -97),   new Vector3 (166, axis3, 29),
                      new Vector3 (166, axis3, 9),     new Vector3 (166, axis3, -12),   new Vector3 (166, axis3, 53),
                      new Vector3 (-17, axis3, 133),   new Vector3 (-40, axis3, 133),   new Vector3 (-63, axis3, 133),
                      new Vector3 (26, axis3, 133),    new Vector3 (4, axis3, 133),     new Vector3 (49, axis3, 133),
                      new Vector3 (-112, axis3, 107),  new Vector3 (-112, axis3, 89),   new Vector3 (-112,axis3,69),
                      new Vector3 (-112, axis3, 50)
        };

        ////Vector3[]: Comforts6            // First Floor Comforts
        floorCoor[1] = new Vector3[] {          new Vector3 (-98, axis1, -42),   new Vector3 (166,axis1,-86),
                      // Second Floor Comforts
                      new Vector3 (20, axis2, -130),   new Vector3 (132, axis2, -134),  new Vector3 (63, axis2, -121),
                      new Vector3 (139, axis2, -81),   new Vector3 (134, axis2, -29),   new Vector3 (-102, axis2, -87),
                      // Third Floor Comforts
                      new Vector3 (-177,axis3,52),     new Vector3 (115,axis3,100)
        };

        ////Vector3[]: Services7
        floorCoor[2] = new Vector3[] {      // First floor no service rooms
                    // Second Floor Services
                        new Vector3 (-18, axis2, -122),  new Vector3 (50, axis2, 121),  new Vector3 (84, axis2, 56),
                        new Vector3 (-75, axis2, 120),   new Vector3 (-128, axis2, 62),   new Vector3 (-122, axis2, 31),
                        new Vector3 (-11, axis2, 120),
                    //Third floor Service 1
                        new Vector3 (-62, axis3, 159)
        };

        ////Vector3[]: Entr/Ext6            // First Floor ent/ext 7
        floorCoor[3] = new Vector3[] {          new Vector3 (-150, axis1, 0),    new Vector3 (174, axis1, -57),   new Vector3 (116, axis1, 117),
                      new Vector3 (-144, axis1, 84),   new Vector3 (-93, axis1, -30),   new Vector3 (-3, axis1, 127),
                      new Vector3 (18, axis1, -29),
                      // Second Floor Ent/Exts
                      new Vector3 (6, axis2, -152),    new Vector3 (-134, axis2, 5),    new Vector3 (188, axis2, -48),
                      new Vector3 (-75, axis2, -139),  new Vector3 (-90, axis2, -58),   new Vector3 (-105, axis2, -72),
                      // Third floor ent/exit 5
                      new Vector3 (-141, axis3, -61),  new Vector3 (144, axis3, 16),    new Vector3 (86, axis3, 144),
                      new Vector3 (105, axis3, 74)
        };

    }

    /*
    //Display name for subMenus
    public string getLocationName(int ctgry, int rm)
    {
        return (floorRoom.Length < ctgry && floorRoom.Length < rm) ?
            floorRoom[ctgry][rm] : "Location not found";
    }

    //get Coordinates of selected points (by index)
    public Vector3 getLocationCoor(int ctgry, int rm)
    {
        return (floorCoor.Length < ctgry && floorCoor.Length < rm) ?
            floorCoor[ctgry][rm] : new Vector3(0, 0, 0);
    }

    //get Coordinates of selected points (by string)
    public Vector3 getLocationCoor(string room)
    {
        Vector3 vec = new Vector3(0, 0, 0);
        room.ToLower();     //lowercase

        for (int ctgry = 0; ctgry < floorCoor.Length; ctgry++)
        {
            for (int rm = 0; rm < floorCoor.Length; rm++)
            {
                if (floorRoom[ctgry][rm] == room)
                    vec = floorCoor[ctgry][rm];
            }
        }

        return vec;
    }
    */

	public void getStartingPoint(Vector3 vec)
	{
		start = vec;
	}

    public Vector3 setStartingPoint()
    {
		return start;
		//return new Vector3(44f,2f,138.7f);//getStart();

        //Vector3 vec = new Vector3 (100, 2, -103);
        //room.ToLower();     //lowercase

		// set rotation to face the door

       /* for (int ctgry = 0; ctgry < floorRoom.GetLength (0); ctgry++)
        {
            for (int rm = 0; rm < floorRoom[ctgry].Length; rm++)
            {
                if (floorRoom[ctgry][rm].Equals (getStartPoint()))
                {
                    vec = floorCoor[ctgry][rm];
                    return vec;
                }
            }
        }
        Debug.Log ("Room does not exist: " + getStartPoint());
        return vec;*/
    }

	public void getEndingPoint(Vector3 vec)
	{
		end = vec;
	}

    public Vector3 setEndingPoint()
    {
		return end;
		//return new Vector3(-62.6f,2f,168.5f);//getEnd();
		/*
        //      string room = getEndPoint();
		Vector3 vec = new Vector3 (144,2,-57);
        //room.ToLower();     //lowercase

        for (int ctgry = 0; ctgry < floorRoom.GetLength (0); ctgry++)
        {
            for (int rm = 0; rm < floorRoom[ctgry].Length; rm++)
            {
                if (floorRoom[ctgry][rm].Equals (getEndPoint()))
                {
                    vec = floorCoor[ctgry][rm];
                    return vec;
                }
            }
        }
        Debug.Log ("Room does not exist: " + getEndPoint());
        return vec;
        */
    }

}

    