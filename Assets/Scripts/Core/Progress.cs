using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Progress : MonoBehaviour
{

    [SerializeField] public Transform progressInitPoint; // * transform to mark as init point
    [SerializeField] public Transform progressExitPoint; // * transform to mark as exit point

    private float _dist; // ! total distance from init to exit
    private float _here; // ! current distance from exit
    private Transform _play; // ! player transform reference

    private Slider _progress;

    // ON START FRAME
    void Start()
    {
        _play = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        _dist = Vector3.Distance(progressExitPoint.position, progressInitPoint.position);
        _progress = GetComponent<Slider>();
    }

    // ON UPDATE FRAME
    void Update()
    {
        _here = Vector3.Distance(_play.position, progressInitPoint.position);

        _progress.value = (_here / _dist) * 100f;
    }
}
