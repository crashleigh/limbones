using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBounce : MonoBehaviour
{
    // ! BOUNCE MODE VARIABLES
	[SerializeField] public float bounceRange = 10.0f;		// distance at which the goober goes into bounce mode
	[SerializeField] public float bounceSpeed = 5f;			// speed at which the goober bouces towards the player

    private Transform _play;

    private void Start() {
        _play = GameObject.FindGameObjectsWithTag("Player")[0].transform; // init player locator
    }

    public void Bounce()
    {
        // TODO convert to function for future calls
        // Transform _near = null;
        // float y = Mathf.Infinity;
        // foreach (Transform p in patrolPath)
        // {
        //     Vector3 d = p.position - player.position;
        //     if (d.sqrMagnitude < y)
        //     {
        //         y = d.sqrMagnitude;
        //         _near = p;
        //     }
        // }
        // transform.LookAt(_near.position);
        //return x;

        // Vector2 _side = transform.InverseTransformPoint(_play.position);
        // // ! PROCESS PLAYER IS RIGHT OF ENEMY
        // if (_side.x < 0f)
        // {
        //    // transform.position.x += 10f;
        // }
        // // ! PROCESS PLAYER IS LEFT OF ENEMY
        // if (_side.x > 0f)
        // {
        //     transform.position.x += -10f;
        // }
        transform.localScale = new Vector2(transform.InverseTransformPoint(_play.position).x > 0f ? -1 : 1, 1); // face towards direction of attack

        transform.position = Vector2.MoveTowards( transform.position, new Vector2( _play.position.x, transform.position.y ), bounceSpeed * Time.deltaTime );

        // transform.position = Vector3.MoveTowards(
        //             transform.position,
        //             _play.position,
        //             bounceSpeed = Time.deltaTime
        //         );

        // TODO make the goober pounce towards the player but not out of the bounds of the path
        //transform.position += transform.forward * Time.deltaTime * bounceSpeed;
    }
}
