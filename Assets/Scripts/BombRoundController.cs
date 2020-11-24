using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombRoundController : MonoBehaviour
{
    //public bool isActive = false;
    //public GameObject _bomb;

    public GameObject explosionEffect;

    public AudioClip summon;
    public AudioClip hum;
    public AudioSource _bombSounds;

    public float radius = 5f;
    public float force = 700f;
    public bool inRadiusExplode = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);

        //play summon
        _bombSounds.PlayOneShot(summon, 1f);
        //_bombSounds.loop
        //play delay hum, .5 seconds
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        //play explosion effect
        Debug.Log("Round: Ka-BOOM!");
        GameObject boom = Instantiate(explosionEffect, transform.position, transform.rotation);

        _bombSounds.Stop();//stop humming

        //Get nearby objects
        //AddForce
        //Damage

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            //check if other bomb is in radius
            //PlayerMovement _player = other.gameObject.GetComponent<PlayerMovement>();
            BombRoundController _otherBomb = nearbyObject.gameObject.GetComponent<BombRoundController>();
            if (_otherBomb != null)
            {
                Debug.Log("other bomb touched!");
                //tell level controller other bomb is kill
                LevelController _lc = FindObjectOfType<LevelController>();
                _lc.bombRoundActive = false;
                _lc.bombSquareActive = false;

                if (!inRadiusExplode) {
                    _otherBomb.inRadiusExplode = true;
                    inRadiusExplode = true;
                    _otherBomb.Explode(); //will this cause an infinite loop?
                }
                //Destroy(_otherBomb.gameObject);
            }


            //Add force
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
            //Damage

        }

        //delete self
        Destroy(gameObject);
        Destroy(boom, 4f);
    }
}
