using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour {
	//Caja para guardar los rooms
	public int maxRooms;
	public GameObject Rooms;
	//Templates
	public GameObject[] rightRooms;
	public GameObject[] leftRooms;
	public GameObject[] topRooms;
	public GameObject[] bottomRooms;
	[Header("Limit Rooms")]
	//Limites
	public GameObject left;
	public GameObject right;
	public GameObject top;
	public GameObject bottom;
	//Cuarto cerrado
	public GameObject closedRoom;

	public List<GameObject> rooms;
	public float timeToBoss;
	private float currentTimeToBoss;
	public bool spawnedBoss;
	public GameObject boss;

	void Start(){
		currentTimeToBoss = timeToBoss;
		if (maxRooms < 4) {
			Debug.Log ("El numero minimo de rooms debe ser 4");
			maxRooms = 4;
		}
		Rooms = new GameObject ("Rooms");
	}
	void Update(){
		if (currentTimeToBoss <= 0 && !spawnedBoss) {
			for (int i = 0; i < rooms.Count; i++) {
				if (i == rooms.Count - 1) {
					/*if (rooms [i].GetComponent<ClosedRoom> ()) {
						Destroy (GameObject);
					}*/
					Instantiate (boss, rooms [i].transform.position, Quaternion.identity);
					spawnedBoss = true;
				}
			}
		} else
			currentTimeToBoss -= Time.deltaTime;
	}
	public void restartTime(){
		currentTimeToBoss = timeToBoss;
	}
}
