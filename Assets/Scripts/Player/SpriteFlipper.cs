using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    private IPlayerCOntroller player;

    private void Awake()
    {
        player = GetComponentInParent<IPlayerCOntroller>();

    }

    private void Update()
    {
        if (player == null) return;

        //sprite flipper
        if (player.Input.X != 0)
        {
            transform.localScale = new Vector3(player.Input.X > 0 ? 1 : -1, 1, 1);
        }


    }
}