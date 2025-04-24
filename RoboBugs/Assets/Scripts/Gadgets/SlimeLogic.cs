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
            if (collision.tag == "Player")
            {
                collision.GetComponent<Rigidbody2D>().gravityScale = 0;
                player = collision.gameObject;
                doubleJump = true;
                collision.GetComponent<Player>().vspd = 0;
                collision.GetComponent<Player>().gravity = 0;
                Debug.Log("pog");
                //collision.GetComponent<Player>().vspd = collision.GetComponent<Player>().jumpForce;
                
                //collision.GetComponent<Player>().hspd *= -transform.up.x;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != "Solids")
        {
            if (collision.tag == "Player")
            {
                collision.GetComponent<Player>().gravity = 1;
                
                collision.GetComponent<Rigidbody2D>().gravityScale = 1;
                
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Solids")
        {
            if (collision.tag == "Player")
            {
                //collision.GetComponent<Player>().vspd *=  -transform.up.y;
                //collision.GetComponent<Player>().hspd *= -transform.up.x;
            }
            else
            {
                collision.GetComponent<Rigidbody2D>().velocity = new Vector2(collision.GetComponent<Rigidbody2D>().velocity.x - collision.GetComponent<Rigidbody2D>().velocity.x * Mathf.Cos(gameObject.transform.rotation.z), collision.GetComponent<Rigidbody2D>().velocity.y - collision.GetComponent<Rigidbody2D>().velocity.y * Mathf.Sin(gameObject.transform.rotation.z));
                //Debug.Log(collision.GetComponent<Rigidbody2D>().velocity);
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        
        //If this is when the context has started, it proceeds.
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
