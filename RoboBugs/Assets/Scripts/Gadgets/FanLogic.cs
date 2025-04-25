using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanLogic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "Solids" && Time.timeScale != 0)
        {
            //If a player enters the collider, add velocity in that direction.
            if (collision.tag == "Player")
            {
                collision.GetComponent<Player>().vspd += 2f * -transform.up.y;
                collision.GetComponent<Player>().hspd += 2f * -transform.up.x;
            }
            //Otherwise, add a force to the object's rigidbody. If it's a bug, it will enter the Stunned state.
            else
            {
                collision.GetComponent<Rigidbody2D>().AddForce(-transform.up * 80, ForceMode2D.Impulse);

                if (collision.tag == "Bug")
                {
                    collision.GetComponent<BugMovement>().EnterStunnedState();
                }
            }
        }
    }
}
