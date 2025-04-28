using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlimeLogic : MonoBehaviour
{
    private GameObject player;
    private bool doubleJump;

    // Start is called before the first frame update
    void Start()
    {
        doubleJump = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Solids")
        {
            //When the player enters the area, it disables the gravity scale, vertical speed, and allows them to double jump.
            if (collision.tag == "Player")
            {
                collision.GetComponent<Rigidbody2D>().gravityScale = 0;
                player = collision.gameObject;

                doubleJump = true;

                collision.GetComponent<Player>().vspd = 0;
                collision.GetComponent<Player>().gravity = 0;
            }
            //If a bug enters the area, gravity is disabled and velocity is cancelled out.
            else if (collision.tag == "Bug")
            {
                collision.GetComponent<Rigidbody2D>().gravityScale = 0;
                collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Solids")
        {
            //When the player leaves, gravity is restored.
            if (collision.tag == "Player")
            {
                collision.GetComponent<Player>().gravity = 1;
                
                collision.GetComponent<Rigidbody2D>().gravityScale = 1;
                
            }
            //When the bug leaves, gravity is restored.
            else if (collision.tag == "Bug")
            {
                collision.GetComponent<Rigidbody2D>().gravityScale = collision.GetComponent<BugMovement>().BugGravity;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Ensures the bug stays stunned while within the range of the object
        if (collision.tag == "Bug")
        {
            collision.GetComponent<BugMovement>().EnterStunnedState();
        }
    }

    //Grants the player a second jump if attached to a wall!
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.phase.ToString() == "Started" && Time.timeScale != 0)
        {
            if (doubleJump)
            {
                doubleJump = false;
                player.GetComponent<Player>().vspd = player.GetComponent<Player>().jumpForce;
            }
        }
    }
}
