using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyMove : MonoBehaviour {
	bool patrolWaiting;
	public float totalWaitTime = 3f;
	public float switchProbability = 0.2f;
	NavMeshAgent agent;
	int currentPatrolIndex;
	bool moving;
	bool waiting;
	bool patrolForward;
	float waitTimer;
	public List<Transform> patrolPoints;

	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		if (agent == null) {
			Debug.LogError ("There is not navMesh attached to this Enemy");
		} else {
			if (patrolPoints != null && patrolPoints.Count >= 2) {
				currentPatrolIndex = 0;
				SetDestination ();
			} else {
				Debug.LogError ("Not enought points or null");
			}
		}
	}

	public void Update(){
		if (moving && agent.remainingDistance <= 1) {
			moving = false;
			if (patrolWaiting) {
				waiting = true;
				waitTimer = 0f;
			} else {
				ChangePatrolPoint ();
				SetDestination ();
			}
		}

		if (waiting) {
			waitTimer += Time.deltaTime;
			if (waitTimer >= totalWaitTime) {
				waiting = false;
				ChangePatrolPoint ();
				SetDestination ();
			}
		}
	}

	private void SetDestination(){
		if (patrolPoints != null) {
			Vector3 targetVector = patrolPoints [currentPatrolIndex].transform.position;
			agent.SetDestination (targetVector);
			moving = true;
		}
	}

	private void ChangePatrolPoint(){
		if (UnityEngine.Random.Range (0f, 1f) <= switchProbability) {
			patrolForward = !patrolForward;
		}
		if (patrolForward) {
			currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
		} else {
			if (--currentPatrolIndex < 0) {
				currentPatrolIndex = patrolPoints.Count - 1;
			}
		}
	}
}
