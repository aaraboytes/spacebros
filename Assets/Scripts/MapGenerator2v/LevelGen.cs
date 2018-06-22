using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelGen : MonoBehaviour {
	[Header("Game Elements")]
	public GameObject player;
	public GameObject enemy;
	public int enemyAmount;
	public GameObject barrel;
	public int barrelAmount;
	public GameObject scorePoint;
	public int scorePointAmount;
	public GameObject finish;
	[Header("Tiles")]
	public GameObject[] tiles;
	public GameObject indestructibleWall;
	public GameObject wall;
	public List<Vector3> createdTiles;
	public int tileAmount;
	public int tileSize;
	[Header("Probabilities")]
	public float probUp;
	public float probRight;
	public float probDown;
	[Header("Seed")]
	public int seed;
	private GameObject map;
	private GameObject walls;
	[Header("Canvas")]
	public Slider progressSlider;
	public GameObject loadingScreen;
	[Header("Extra parameters")]
	public float waitTime;

	public float minX=0f;
	public float maxX=999999f;
	public float minY=0f;
	public float maxY=999999f;
	public float xAmount;
	public float yAmount;
	public float extraWallX;
	public float extraWallY;

	void Start () {
		player.SetActive (false);
		tileAmount = PlayerPrefs.GetInt ("mapSize", 100);
		loadingScreen.gameObject.SetActive (true);
		Random.seed = PlayerPrefs.GetInt ("seed", 150);
		map = new GameObject ("Map");
		walls = new GameObject ("walls");
		walls.transform.SetParent(map.transform);
		StartCoroutine ("GenerateLevel");
	}

	IEnumerator GenerateLevel(){
		for (int i = 0; i < tileAmount; i++) {
			float randDir = Random.Range (0f, 1f);
			int tile = Random.Range (0, tiles.Length);
			CreateTile (tile);
			CallMoveGen(randDir);
			yield return new WaitForSeconds (waitTime);
			progressSlider.value = (float)i /(float)tileAmount;
			//Finish
			if (i == tileAmount - 1) {
				Finish ();
				GameManager._instance.gameStart = true;
			}
		}
		yield return 0;
	}

	void CallMoveGen(float randDir){
		if (randDir < probUp)
			MoveGen (0);
		else if (randDir < probRight)
			MoveGen (1);
		else if (randDir < probDown)
			MoveGen (2);
		else
			MoveGen (3);
	}

	void MoveGen(int dir){
		//Random move for a point
		switch (dir) {
		case 0:
			//Up
			transform.position = new Vector3 (transform.position.x, transform.position.y + tileSize, 0);
			break;
		case 1:
            //Right
			transform.position = new Vector3 (transform.position.x + tileSize, transform.position.y, 0);
			break;
		case 2:
			//Down
			transform.position = new Vector3 (transform.position.x, transform.position.y-tileSize, 0);
			break;
		case 3:
			//Left
			transform.position = new Vector3 (transform.position.x - tileSize, transform.position.y, 0);
			break;
		}
	}

	void CreateTile(int tileIndex){
		if (!createdTiles.Contains (transform.position)) {
			if (tiles.Length != 0) {
				GameObject tileObj = (GameObject)Instantiate (tiles [tileIndex], transform.position, Quaternion.identity);
				tileObj.transform.SetParent (map.transform);
				createdTiles.Add (transform.position);
			} else {
				createdTiles.Add (transform.position);
			}
		} else {
			tileAmount++;
		}
	}

	void Finish(){
		CreateWallValues ();	
		CreateWalls();
		SpawnObjects ();
		loadingScreen.gameObject.SetActive (false);
	}

	void SpawnObjects(){
		player.transform.position = createdTiles [Random.Range (0, createdTiles.Count)];
		player.SetActive (true);
		GameObject mapObjects = new GameObject ("mapObjects");
		for (int i = 0; i < enemyAmount; i++) {
			GameObject currentEnemy;
			currentEnemy = (GameObject)Instantiate (enemy, createdTiles [Random.Range (0, createdTiles.Count)], Quaternion.identity);
			currentEnemy.SetActive (true);
			currentEnemy.transform.SetParent (mapObjects.transform);
		}
		for (int i = 0; i < barrelAmount; i++) {
			GameObject currentBarrel;
			currentBarrel = (GameObject)Instantiate (barrel, createdTiles [Random.Range (0, createdTiles.Count)], Quaternion.identity);
			currentBarrel.transform.rotation = Quaternion.Euler (90f, 0f, 0);
			currentBarrel.SetActive (true);
			currentBarrel.transform.SetParent (mapObjects.transform);
		}
		for (int i = 0; i < scorePointAmount; i++) {
			GameObject currentScorePoint;
			currentScorePoint  = (GameObject)Instantiate (scorePoint, createdTiles [Random.Range (0, createdTiles.Count)], Quaternion.identity);
			currentScorePoint.SetActive (true);
			currentScorePoint.transform.SetParent (mapObjects.transform);
		}
		GameObject auxFinish = (GameObject)Instantiate (finish, createdTiles [Random.Range (0, createdTiles.Count)], Quaternion.identity);
		auxFinish.SetActive (true);
		auxFinish.transform.SetParent (mapObjects.transform);
	}

	void CreateWallValues(){
		//Create de maximum values of the generated map
		for (int i = 0; i < createdTiles.Count; i++) {
			if (createdTiles [i].y < minY) {
				minY = createdTiles [i].y;
			}
			if (createdTiles [i].y > maxY) {
				maxY = createdTiles [i].y;
			}
			if (createdTiles [i].x < minX) {
				minX = createdTiles [i].x;
			}
			if (createdTiles [i].x > maxX) {
				maxX = createdTiles [i].x;
			}
			xAmount = ((maxX - minX) / tileSize) + extraWallX;
			yAmount = ((maxY - minY) / tileSize) + extraWallY;
		}
	}

	void CreateWalls(){
		for (int x = 0; x < xAmount; x++) {
			for (int y = 0; y < yAmount; y++) {
				if (!createdTiles.Contains (new Vector3 ((minX - (extraWallX * tileSize) / 2) + (x * tileSize), (minY - (extraWallY * tileSize) / 2) + (y * tileSize),0f))) {
					GameObject auxWall;
					if (x == 0 || x == xAmount - 1 || y == 0 || y == yAmount - 1) {
						auxWall = indestructibleWall;
					} else
						auxWall = wall;
					GameObject currentWall= (GameObject)Instantiate(auxWall,new Vector3 ((minX - (extraWallX * tileSize) / 2) + (x * tileSize), (minY - (extraWallY * tileSize) / 2) + (y * tileSize),0f),transform.rotation);
					currentWall.transform.SetParent (walls.transform);
				}
			}
		}
	}

	public bool IsACornerHere(Vector3 pos){
		if (createdTiles.Contains (pos)) {
			if (createdTiles.Contains (new Vector3 (pos.x, pos.y + tileSize, 0f))) {
				//Si hay algo encima
				return false;
			} else
				//Si no hay nada encima
				return true;
		} else
			//Esta posicion no pertenece a un bloque
			return false;
	}
}
