using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool isAttack = false;
    // ! ATTACK MODE VARIABLES
	public Rigidbody2D attackBullet;						// used to duplicate the bullet from an existing prefab
    [SerializeField] public float attackArch = 0f;	        // distance of the goob fired from our goober
	[SerializeField] public float attackLife = 1f;			// distance of the goob fired from our goober
    [SerializeField] public AudioSource attackSound;        // sound our goob makes on an attack
	[SerializeField] public float attackRate = 3f;			// rate at which the boober fires their goobs
	[SerializeField] public float attackRange = 25.0f;		// distance at which the goober goes into attack mode

	private float	_loop = 0f;			// stores the amount of time passed between goob shots

    private Transform _play;


    private void Start() {
        _play = GameObject.FindGameObjectsWithTag("Player")[0].transform; // init player locator
    }

    public void Attack()
	{
		// ! CATCH THE GOOBER FIRE LOOP
		if (_loop < attackRate)
		{
			_loop += Time.deltaTime;
		}
		// * PREPARE THE NEXT GOOB SHOT
		else
		{
            GetComponent<Goob>().isAttack = true;
            // TODO add a little wait here
			Rigidbody2D _bull = Instantiate(attackBullet, transform.position, Quaternion.identity) as Rigidbody2D;

            Vector2 _side = transform.InverseTransformPoint(_play.position);
    
            // ! PROCESS PLAYER IS RIGHT OF ENEMY
            attackSound.Play();
            if (_side.x < 0f)
            {
                transform.localScale = new Vector2(1, 1); // face towards direction of attack
                _bull.GetComponent<Rigidbody2D>().AddForce(new Vector2(-10f,attackArch), ForceMode2D.Impulse);
            }
            // ! PROCESS PLAYER IS LEFT OF ENEMY
            if (_side.x > 0f)
            {
                transform.localScale = new Vector2(-1, 1); // face towards direction of attack
                _bull.GetComponent<Rigidbody2D>().AddForce(new Vector2(10f,attackArch), ForceMode2D.Impulse);
            }
			//_bull.GetComponent<Rigidbody2D>().AddForce(transform.forward * 650);
            //_bull.GetComponent<Rigidbody2D>().AddForce(new Vector2(-10,0), ForceMode2D.Impulse);
			_loop = 0f;

            GetComponent<Goob>().isAttack = false;

			Destroy(_bull, attackLife); // kills the force on the object and removes the internal reference
		}
	}

}
