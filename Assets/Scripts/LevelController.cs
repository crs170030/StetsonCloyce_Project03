using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public GameObject _RuneUI;
    public GameObject _bombIcons;
    public GameObject _LoadWheelGroup;
    public Image _LoadingBar;
    public GameObject _roundIcon;
    public GameObject _squareIcon;

    public GameObject _player;
    public Transform _holdPosition;
    public GameObject _roundBomb;
    public GameObject _squareBomb;
    public GameObject _grab;

    GameObject currentRoundBomb;
    GameObject currentSquareBomb;

    public int activeRune = 0; //0 for round, 1 for square
    public bool bombRoundActive = false;
    public bool bombSquareActive = false;
    public float timerSpeed = 25f;
    public float timerLimit = 100f;
    public float yeetStrength = 3f;

    Vector3 bombPosition;
    bool runeHasChanged = false;
    float currentWheelValRound = 100f; //set to max as default
    float currentWheelValSquare = 100f;
    bool isHolding = false;
    public GameObject bombHeld = null;
    public Rigidbody rigidHeld = null;

    public AudioClip flick;
    public AudioClip ping;
    public AudioSource _menuSounds;

    // Start is called before the first frame update
    void Start()
    {
        _RuneUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //pause game if rune menu is open
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenMenu(true);
        }
        //resume game when button is lifted
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            OpenMenu(false);
        }
        //if menu is open and player uses a or d, move bomb icons
        if (_RuneUI.activeSelf) //Input.GetAxis("Horizontal") != 0
        {
            if (Input.GetKeyDown(KeyCode.A))//moving ui to the left
            {
                if(_bombIcons.transform.localPosition.x < -100)
                {
                    _bombIcons.transform.localPosition += Vector3.right * 125;
                    //change active bomb to Round
                    activeRune = 0;
                    runeHasChanged = true;
                    _menuSounds.PlayOneShot(flick, 1f);

                    if (_LoadWheelGroup.activeSelf)//if the delay timer is active, switch icons
                    {
                        _roundIcon.SetActive(true);
                        _squareIcon.SetActive(false);
                    }
                }
            }else if (Input.GetKeyDown(KeyCode.D)) //moving UI to the right
            {
                if (_bombIcons.transform.localPosition.x > -100)
                {
                    _bombIcons.transform.localPosition += Vector3.left * 125;
                    //change active bomb to Square
                    activeRune = 1;
                    runeHasChanged = true;
                    _menuSounds.PlayOneShot(flick, 1f);

                    if (_LoadWheelGroup.activeSelf)//if the delay timer is active, switch icons
                    {
                        _squareIcon.SetActive(true);
                        _roundIcon.SetActive(false);
                    }
                }
            }
            //Debug.Log("Menu Open, Pushing A or D!");
        }

        //summon/explode bomb with Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isHolding)
            {
                //call bomb script depending on active Rune
                if (activeRune == 0 && currentWheelValRound >= timerLimit)
                {
                    Bomb();
                }
                else if (activeRune == 1 && currentWheelValSquare >= timerLimit)
                {
                    Bomb();
                }
            }
            else
            {
                //if the player is holding a bomb, dismiss it
                if(activeRune == 0)
                    bombRoundActive = false;
                else
                    bombSquareActive = false;

                _menuSounds.PlayOneShot(ping, 1f);
                Destroy(bombHeld);

                isHolding = false;
                bombHeld = null;
                rigidHeld = null;

            }
        }

        //call pick up with E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
                StartCoroutine(PickUp()); //watchout for wierd behavior with multiple pickups
            else
                ThrowObject(yeetStrength);
        }
        //throw weakly(put on ground)
        if(isHolding && Input.GetKeyDown(KeyCode.R))
        {
            ThrowObject(.1f);
        }

        //if timer is active, add to timer
        if (_LoadWheelGroup.activeSelf)
        {
            if(currentWheelValRound < timerLimit)
                currentWheelValRound += timerSpeed * Time.deltaTime;
            
            if (currentWheelValSquare < timerLimit)
                currentWheelValSquare += timerSpeed * Time.deltaTime;
            
            //if both timers are done, hide timer
            if(currentWheelValRound >= timerLimit && currentWheelValSquare >= timerLimit)
                _LoadWheelGroup.SetActive(false);

            if(activeRune == 0)
                _LoadingBar.fillAmount = currentWheelValRound / 100;
            else
                _LoadingBar.fillAmount = currentWheelValSquare / 100;
        }
    }

    void OpenMenu(bool opening)
    {
        if (opening)
        {
            //pause time
            Time.timeScale = 0;
            _menuSounds.PlayOneShot(flick, 1f);
            //show rune ui
            _RuneUI.SetActive(true);
            runeHasChanged = false;
        }
        else
        {
            _RuneUI.SetActive(false);
            //Debug.Log("Game Unpausing...");
            Time.timeScale = 1;
            if (runeHasChanged)
                _menuSounds.PlayOneShot(ping, 1f);
        }
    }

    //Bomb Rune Script
    void Bomb()
    {
        //if the active rune =: 0 - Round Bomb, 1 - Square Bomb
        if (activeRune == 0)
        {
            Debug.Log("Round Bomb active == " + bombRoundActive);
            //if the round bomb has not been summoned, summon it
            if (!bombRoundActive)
            {
                //ERROR: BOMB IS SUMMONED BELOW PLAYER... WHYYYYY??!!
                bombPosition = _holdPosition.position;//_player.transform.position + Vector3.up * 2.5f;
                GameObject roundGO = Instantiate(_roundBomb, bombPosition, transform.rotation);
                roundGO.SetActive(true);
                currentRoundBomb = roundGO;

                bombRoundActive = true;
            }
            else
            {
                //if bomb is active, call explosion
                bombRoundActive = false;
                activateTimer();

                BombRoundController _roundCon = currentRoundBomb.GetComponent<BombRoundController>();
                _roundCon.Explode();
            }
        }
        else
        {
            Debug.Log("Square Bomb active == " + bombSquareActive);
            //if the square bomb has not been summoned, summon it
            if (!bombSquareActive)
            {
                bombPosition = _holdPosition.position;// _player.transform.position + Vector3.up * 2.5f;
                GameObject squareGO = Instantiate(_squareBomb, bombPosition, transform.rotation);
                squareGO.SetActive(true);
                currentSquareBomb = squareGO;

                bombSquareActive = true;
            }
            else
            {
                //if bomb is active, call explosion
                bombSquareActive = false;
                activateTimer();

                BombRoundController _squareCon = currentSquareBomb.GetComponent<BombRoundController>();
                _squareCon.Explode();
            }
        }
    }

    void FixedUpdate()
    {
        if (isHolding)
        {
            bombHeld.transform.position = _holdPosition.position;
            bombHeld.transform.rotation = _holdPosition.rotation;
        }
    }

    void activateTimer()
    {
        _LoadWheelGroup.SetActive(true);
        if (activeRune == 0)
            currentWheelValRound = 0;
        else
            currentWheelValSquare = 0;
    }

    IEnumerator PickUp()
    {
        _grab.SetActive(true);
        yield return new WaitForSeconds(.01f);
        _grab.SetActive(false);
    }

    public void Hold(GameObject heldBomb, bool grabbed)
    {
        isHolding = grabbed;
        bombHeld = heldBomb;

        rigidHeld = bombHeld.GetComponent<Rigidbody>();
        rigidHeld.useGravity = false;
        /*if(isHolding)
            heldBomb.transform.position = _holdPosition.position;
        */
    }

    void ThrowObject(float strength)
    {
        Debug.Log("Yeet!");
        isHolding = false;

        rigidHeld.useGravity = true;
        //rigidHeld.AddForce(transform.forward * yeetStrength);
        rigidHeld.AddRelativeForce(0,0,strength);

        bombHeld = null;
        rigidHeld = null;
    }
}
