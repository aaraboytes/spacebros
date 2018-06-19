using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour {
	public int openingDirection;
	//1 Bottom Door
	//2 Top Door
	//3 Left Door
	//4 Right Door
	private RoomTemplates templates;
	private int rand;
	private bool spawned = false;

	void Start(){
		templates = GameObject.FindGameObjectWithTag ("Rooms").GetComponent<RoomTemplates> ();
		Invoke ("Spawn",0.1f);
	}
	void Spawn(){
		if (!spawned) {
			GameObject room;
			if (openingDirection == 1) {
				if (templates.maxRooms > 1) {
					rand = Random.Range (0, templates.bottomRooms.Length);
					room = (GameObject)Instantiate (templates.bottomRooms [rand], transform.position, Quaternion.identity);
				} else
					room = (GameObject)Instantiate (templates.bottom,  transform.position, Quaternion.identity);
			} else if (openingDirection == 2) {
				if (templates.maxRooms > 1) {
					rand = Random.Range (0, templates.topRooms.Length);
					room = (GameObject)Instantiate (templates.topRooms [rand],  transform.position, Quaternion.identity);
				} else
					room = (GameObject)Instantiate (templates.top,  transform.position, Quaternion.identity);
			} else if (openingDirection == 3) {
				if (templates.maxRooms > 1) {
					rand = Random.Range (0, templates.leftRooms.Length);
					room = (GameObject)Instantiate (templates.leftRooms [rand],  transform.position, Quaternion.identity);
				} else
					room = (GameObject)Instantiate (templates.left,  transform.position, Quaternion.identity);
			} else if (openingDirection == 4) {
				if (templates.maxRooms > 1) {
					rand = Random.Range (0, templates.rightRooms.Length);
					room = (GameObject)Instantiate (templates.rightRooms [rand],  transform.position, Quaternion.identity);
				} else
					room = (GameObject)Instantiate (templates.right,  transform.position, Quaternion.identity);
			} else
				room = new GameObject ();
			room.transform.SetParent (templates.Rooms.transform);
			templates.rooms.Add (room);
			templates.restartTime ();
			templates.maxRooms--;
			spawned = true;
		}
	}
	void Update(){
		if (templates.spawnedBoss) {
			Destroy (gameObject);
		}
	}
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("SpawnPoint")) {
			if (!other.GetComponent<RoomSpawner> ().spawned && !spawned) {
				GameObject closedRoom = Instantiate (templates.closedRoom,  transform.position, Quaternion.identity);
				templates.rooms.Add (closedRoom);
				templates.maxRooms--;
				Destroy (gameObject);
			}
			Destroy (gameObject);
			spawned = true;
		}
	}
}
