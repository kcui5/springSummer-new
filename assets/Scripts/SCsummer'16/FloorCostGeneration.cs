#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class FloorCostGeneration : MonoBehaviour {

	private GameObject[] Wall;
	private GameObject[] Door;
	private GameObject plane;  
	NavMesh naviMesh;

	

	// Use this for initialization
	void Start () 
	{
		Debug.Log("Editor mode");
		//Get all GameObjects with tag "Wall"
		if (Wall == null)
			Wall = GameObject.FindGameObjectsWithTag ("Wall");
		if (Door == null)
			Door = GameObject.FindGameObjectsWithTag("Door");

		plane = GameObject.CreatePrimitive (PrimitiveType.Plane);  //The plane(GameObject) that will cloned

		// Instantiate planes at base of all walls and doors in map/scene
		foreach (GameObject w in Wall) 
			instantiateFloorPlanes(w);
		foreach (GameObject d in Door)
			createDoorPlanes(d);

		NavMeshBuilder.BuildNavMesh (); 			// BAKE THE NAVMESH

	}

	/**
	 * 	Responsible for getting a pair of 3D planes instantiated
	 * at the base of the wall.
	 * @param: Wall - gameobject tagged as wall
	 */
	void instantiateFloorPlanes(GameObject wall)
	{	
		createInnerFloorPlanes (wall);
		createOuterFloorPlanes (wall);
	}


	/**
	 * Instantiates a 3D plane at the base of a wall with a
	 * varied radius area. This plane is also assigned...
	 * COSTs: "Lava (3)" 
	 * Radus: "0.5"
	 * @param: Wall - gameobject tagged as wall
	 */
	void createInnerFloorPlanes(GameObject wall)
	{
		GameObject tempPlane;						// For newly Instantiated plane
		int purpleFloor;							// Hold cost of "Lava" navMesh area
		float innerRadius = 0.2f;					// Radius size around wall

		Quaternion rot = wall.transform.rotation;	// obtain wall's same rotation
		Vector3 pos = wall.transform.position;     	// obtain wall's same coordinate position
		pos.y = 0.3f;								// HARD-CODE the "height" value of plane

		// For scaling the instantiated plane
		float xOffset = (wall.transform.localScale.x * 0.1f);// + innerRadius; // set to have same x scale size of wall		
		float zOffset = (wall.transform.localScale.z * 0.1f);// + innerRadius; // set to have same z scale size of wall
		float yOffset = 1;								 // add small value to both x and z for "radius" around wall

		xOffset += innerRadius;
		zOffset += innerRadius;
		

		tempPlane = (GameObject)Instantiate(plane, pos, rot);					  // ----> INSTANTIATION <----
		tempPlane.transform.localScale = new Vector3 (xOffset, yOffset, zOffset); // resizing plane scale

		tempPlane.isStatic = true;									// set to static -> to apply nav area 
		purpleFloor = NavMesh.GetAreaFromName ("Lava");				// obtain cost value "Lava"
		GameObjectUtility.SetNavMeshArea (tempPlane, purpleFloor);  // assign navigation area as lava to instantiated plane
	}


	/**
	 * Instantiates a 3D plane at the base of a wall with a
	 * varied radius area. This plane is also assigned...
	 * COSTs: "Walkable" 
	 * Radus: "0.7"
	 * @param: Wall - gameobject tagged as wall
	 */
	void createOuterFloorPlanes(GameObject wall)
	{
		GameObject tempPlane;		// For newly Instantiated plane
		float outerRadius = 0.5f;   // Radius size around wall

		Quaternion rot = wall.transform.rotation; 		// obtain wall's same rotation
		Vector3 pos = wall.transform.position;			// obtain wall's same coordinate position
		pos.y = 0.2f;									// HARD-CODE the "height" value of plane

		float xOffset = (wall.transform.localScale.x * 0.1f); 	// + outerRadius; // set to have same x scale size of wall
		float zOffset = (wall.transform.localScale.z * 0.1f); 	// + outerRadius; // set to have same z scale size of wall
		float yOffset = 1;									  	// add small value to both x and z for "radius" around wall

		xOffset += outerRadius;
		zOffset += outerRadius;


		tempPlane = (GameObject)Instantiate(plane, pos, rot);						// ----> INSTANTIATION <----
		tempPlane.transform.localScale = new Vector3 (xOffset, yOffset, zOffset); 	// resizing plane scale

		tempPlane.isStatic = true;
	}

	void createDoorPlanes(GameObject door)
	{
		GameObject tempPlane;						// For newly Instantiated plane
		int jumpCost;							// Hold cost of "Lava" navMesh area
		float innerRadius = 0.2f;					// Radius size around wall
		
		Quaternion rot = door.transform.rotation;	// obtain wall's same rotation
		Vector3 pos = door.transform.position;     	// obtain wall's same coordinate position
		pos.y = 0.3f;								// HARD-CODE the "height" value of plane
		
		// For scaling the instantiated plane
		float xOffset = (door.transform.localScale.x * 0.1f);// + innerRadius; // set to have same x scale size of wall		
		float zOffset = (door.transform.localScale.z * 0.1f);// + innerRadius; // set to have same z scale size of wall
		float yOffset = 1;								 // add small value to both x and z for "radius" around wall
		
		xOffset += innerRadius;
		zOffset += innerRadius;
		
		
		tempPlane = (GameObject)Instantiate(plane, pos, rot);					  // ----> INSTANTIATION <----
		tempPlane.transform.localScale = new Vector3 (xOffset, yOffset, zOffset); // resizing plane scale
		
		tempPlane.isStatic = true;									// set to static -> to apply nav area 
		jumpCost = NavMesh.GetAreaFromName ("Jump");				// obtain cost value "Lava"
		GameObjectUtility.SetNavMeshArea (tempPlane, jumpCost);  // assign navigation area as lava to instantiated plane

	}
}
#endif 