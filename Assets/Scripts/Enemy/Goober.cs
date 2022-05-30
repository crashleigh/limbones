using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goober : MonoBehaviour
{
	// ? REFERENCED COMPONENTS
	private EnemyAttack goobAttack; // ? implements the attack logic for our goob
	private EnemyPatrol goobPatrol; // ? implements the patrol logic for our goob

	// ? INTERNAL REFERENCES
	private Transform _goob; // ! references the goob itself
	private Transform _play; // ! references the player

	// * GOOB MODE DISTANCES
	[SerializeField] public float distanceForAttack = 50f; // distance from which the goob will enter attack mode
	[SerializeField] public float distanceForPatrol = 300f; // distance from which the goob will enter patrol mode

	// ON START FRAME
	private void Start()
	{
		_goob = transform; // init goob locator
		_play = GameObject.FindGameObjectsWithTag("Player")[0].transform; // init player locator

		goobAttack = GetComponent<EnemyAttack>(); // init attack logic
		goobPatrol = GetComponent<EnemyPatrol>(); // init patrol logic
	}

	// ON UPDATE FRAME
	private void Update()
	{
		// ? DETECT PROXIMITY OF PLAYER
		float _near = (_goob.position - _play.position).sqrMagnitude; // determine how close the player is to our goob
		// ! ATTACK MODE when player is within shooting Limit of the goober
		if (_near < distanceForAttack)
		{
			goobAttack.Attack();
		} 
		// * PATROL MODE when player is outside that Limit
		else if (_near < distanceForPatrol)
		{
			goobPatrol.Patrol();
		}
	}
}
