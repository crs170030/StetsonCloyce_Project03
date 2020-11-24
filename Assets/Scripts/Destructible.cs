using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    //public GameObject destroyedVersion;

    public void Destroy()
    {
        Destroy(gameObject, .3f); //die after a split second
    }
}
