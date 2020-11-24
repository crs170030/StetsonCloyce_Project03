using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    //this will only be active as long as the hold key is down in LevelController

    //public GameObject holdPosition;
    public LevelController _lc;

    void Start()
    {
        //LevelController _lc = FindObjectOfType<LevelController>();
    }

    private void OnTriggerStay(Collider other)
    {
        //PlayerMovement _player = other.gameObject.GetComponent<PlayerMovement>();
        BombRoundController bomb = other.gameObject.GetComponent<BombRoundController>();

        if(bomb != null)
        {
            //lift bomb
            Debug.Log("Bomb Grabbed!");
            //bomb.gameObject.transform.position = holdPosition;
            _lc.Hold(bomb.gameObject, true);
        }
    }
}
