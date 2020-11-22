using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public LevelController _levelController;

    public CharacterController controller;

    public float speed = 12f;
    public float run = 1.5f;
    public float jumpHeight = 3f;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public bool pmMenuIsOpen = false;

    Vector3 velocity;
    bool isGrounded;
    float runMultiplyer = 1f;

    //public AudioClip jumpSound;
    //public AudioSource _playerSounds;

    void Update()
    {
        if (!pmMenuIsOpen)
        {
            //check if player is on the ground
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            //Running
            if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
            {
                runMultiplyer = run;
                //_levelController.SprintFOV(true);
                //Debug.Log("Gotta go fast! Run Multiplyer == " + runMultiplyer);
            }
            else if (!Input.GetKey(KeyCode.LeftShift))
            {
                runMultiplyer = 1f;
                //_levelController.SprintFOV(false);
            }

            //move character with a and d
            Vector3 move = ((transform.right * x) + (transform.forward * z));

            //move with respect to speed and time
            controller.Move(move * (speed * runMultiplyer) * Time.deltaTime);

            //jump
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                //_playerSounds.PlayOneShot(jumpSound, 1f);
            }

            //set velocity to gravity
            velocity.y += gravity * Time.deltaTime;

            //move with velocity with time squared
            controller.Move(velocity * Time.deltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        //_levelController.TakeDamage(damage);
    }

    //public void 
}
