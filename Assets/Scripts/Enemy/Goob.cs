using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goob : MonoBehaviour
{
	public bool isBounce = false;
	// ? REFERENCED COMPONENTS
	private EnemyAttack goobAttack; // ? implements the attack logic for our goob
	private EnemyBounce goobBounce; // ? implements the bounce logic for our goob
	private EnemyPatrol goobPatrol; // ? implements the patrol logic for our goob

	// ? INTERNAL REFERENCES
	private Transform _self; // ! references the goob itself
	private Transform _play; // ! references the player

	// * GOOB CONFIGURABLES
	[SerializeField] public float distanceForAttack = 25f; // distance from which the goob will enter attack mode
	[SerializeField] public float distanceForBounce = 10f; // distance from which the goob will enter bounce mode
	[SerializeField] public float distanceForPatrol = 300f; // distance from which the goob will enter patrol mode

	[SerializeField] public	bool isAttack;
	[SerializeField] public bool isIdle;
	[SerializeField] public bool IsMoving;


	// ON START FRAME
	private void Start()
	{
		_self = transform; // pull enemy transform
		_play = GameObject.FindGameObjectsWithTag("Player")[0].transform; // pull player transform
		goobAttack = GetComponent<EnemyAttack>(); // init attack logic
		goobBounce = GetComponent<EnemyBounce>(); // init bounce logic
		goobPatrol = GetComponent<EnemyPatrol>(); // init patrol logic
	}

	// ON UPDATE FRAME
	private void Update()
	{
		// ? DETECT PROXIMITY OF PLAYER
		float _near = (_self.position - _play.position).sqrMagnitude; // determine how close the player is to our goob
		// ! BOUNCE MODE when player is close enough for melee shenans
		if (_near < distanceForBounce)
		{
			goobBounce.Bounce();
		}
		// ! ATTACK MODE when player is within shooting Limit of the goober
		else if (_near < distanceForAttack)
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
