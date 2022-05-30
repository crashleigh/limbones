using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private void OnEnable()
    {
        Destroy(this.gameObject, 1f);
    }

}
