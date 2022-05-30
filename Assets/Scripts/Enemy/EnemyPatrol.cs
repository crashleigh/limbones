using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
	public bool isIdle = true;
	public bool isMoving = false;

    // * ENEMY PATROL MODE CONFIGURABLES
    [SerializeField] public Transform[] patrolPath;                // list of gizmos in the scene to path towards
    [SerializeField] public float patrolSpeed = 3f;         // speed at which the goober travels when moving
    [SerializeField] public float patrolIdleMin = 5.0f;     // minimum amount of time between random idle inside patrol paths
    [SerializeField] public float patrolIdleMax = 10.0f;    // maximum amount of time between random idle inside patrol paths
    [SerializeField] public float patrolWaitMin = 0.3f;     // minimum amount of time to wait during patrol idles
    [SerializeField] public float patrolWaitMax = 2.0f;     // maximum amount of time to wait during patrol idles

    // ! ENEMY PATROL MODE INTERNAL STATES
    private float	_idle = 0f;           // stores the time until idle after it's been randomly generated
    private float	_hold = 0f;           // stores amount of time to hold in place for comparison to wait
    private int		_path = 0;            // stores the index of the patrol path we're targeting
    private bool	_stop = false;   	  // stores whether we ware stopped or not
    private float	_wait = 0f;           // stores the time to wait after it's been randomly generated

	private void Start() {
		transform.localScale = new Vector2(transform.InverseTransformPoint(patrolPath[_path].position).x > 0f ? -1 : 1, 1); // face towards direction of travel
	}
    // PATROL LOGICS
    public void Patrol()
    {
        // ? PROCESS THE GOOBER WAIT STATE
        if (_stop)
        {
			isIdle = true;
			isMoving = false;
            _hold += Time.deltaTime;
            if (_hold < _wait)
                return;
            _stop = false;
        }

        // ! CATCH THE GOOBER REACHING THE TARGET PATH POINT
        Transform _here = patrolPath[_path];
        if (Vector3.Distance(transform.position, _here.position) < 0.01f)
        {
			isIdle = false;
			isMoving = true;
            transform.position = _here.position;                // set the value of our enemy to it's target path point to keep things from drifting

            _idle = Random.Range(patrolIdleMin, patrolIdleMax); // set amount of time to idle based on stored ranges
            _wait = Random.Range(patrolWaitMin, patrolWaitMax); // set amount of time to wait based on stored ranges
            _hold = 0f;                                         // reset the hold time
            _stop = true;                                       // set wait to trigger wait logic
            _path = (_path + 1) % patrolPath.Length;            // increment path OR set to 0 if maximum length hits

			transform.localScale = new Vector2(transform.InverseTransformPoint(patrolPath[_path].position).x > 0f ? -1 : 1, 1); // face towards direction of travel
        }
        // * MOVE TOWARDS TARGET PATH POINT
        else
        {
            if (_hold < _idle)
            {
				isIdle = true;
				isMoving = false;
                _hold += Time.deltaTime;    // increment the time since last stop period
                // transform.position = Vector3.MoveTowards(
                //     transform.position,
                //     _here.position,
                //     patrolSpeed = Time.deltaTime
                // );
                //transform.LookAt(_here.position);
				transform.position = Vector2.MoveTowards( transform.position, new Vector2( _here.position.x, transform.position.y ), patrolSpeed * Time.deltaTime );
            }
            else
            {
				isIdle = true;
				isMoving = false;
                _idle = Random.Range(patrolIdleMin, patrolIdleMax); // set amount of time to idle based on stored ranges
                _wait = Random.Range(patrolWaitMin, patrolWaitMax); // set amount of time to wait based on stored ranges
                _hold = 0f;                                         // reset the hold time
                _stop = true;                                       // set wait to trigger wait logic
                                                                    //_path = (_path + 1) % patrolPath.Length;            // increment path OR set to 0 if maximum length hits

                // ! CATCH A RANDOM CHANCE TO INCREMENT TARGET
                int _turn = Random.Range(0, 100);
                if (_turn < 33)
                {
                    _path = (_path + 1) % patrolPath.Length;            // increment path OR set to 0 if maximum length hits
					transform.localScale = new Vector2(transform.InverseTransformPoint(patrolPath[_path].position).x > 0f ? -1 : 1, 1); // face towards direction of travel
                }

            }
        }
    }
}
