using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	public float totalDistanceRange;
	public float speed;
	private bool patrolWaiting = false;
	public float waitTime;
	private float waitTimer;
	private float currentWaitTimer;
	private Rigidbody enemy;
	[SerializeField]
	private Vector3 pointA;
	[SerializeField]
	private Vector3 pointB;
	[SerializeField]
	Vector3 move;
	[SerializeField]
	private Vector3 target;

	void Start () {
		enemy = GetComponent<Rigidbody> ();
		pointA = enemy.position - Vector3.right * (totalDistanceRange / 2);
		pointB = enemy.position + Vector3.right * (totalDistanceRange / 2);
		target = (Random.Range(0,1) > 0.5) ? pointA : pointB;
		move = (target-enemy.position).normalized;
	}

	void Update () {
		float deltaTime = Time.deltaTime;
		if (enemy.position.x >= pointB.x && target == pointB) {
			target = pointA;
			ChangeDirection ();
		} else if (enemy.position.x <= pointA.x && target == pointA) {
			target = pointB;
			ChangeDirection ();
		}
		if (!patrolWaiting) {
			float distancePercentaje = enemy.position.x / totalDistanceRange;
			float sinValue = (distancePercentaje * 2) - 1;
			enemy.MovePosition (enemy.position + (move * speed * deltaTime * Mathf.Sin(sinValue)));
		}
		else
			currentWaitTimer += deltaTime;

		if (currentWaitTimer > waitTime) {
			patrolWaiting = false;
			currentWaitTimer = 0;
		}
	}

	void ChangeDirection(){
		move = (target-enemy.position).normalized;
		patrolWaiting = true;
	}
}
