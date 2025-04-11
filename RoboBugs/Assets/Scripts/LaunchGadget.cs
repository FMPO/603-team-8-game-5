using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchGadget : MonoBehaviour
{
    //Declares necessary variables.
    public GameObject bumper;
    public Camera cam;
    public float forceMag;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //A method called by player input, firing a projectile in that direction.
    void Fire(InputAction.CallbackContext context)
    {
        //If this is when the context has started, it proceeds.
        if (context.phase.ToString() == "Started")
        {
            /*Calculates the adjacent and opposite sides of a right triangle, using the player 
            and mouse position before calculating the angle*/
            float adjacent = cam.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            float opposite = cam.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;

            float angle = Mathf.Atan(opposite / adjacent) * (180 / Mathf.PI);

            //Adjusts the angle to account for the "quadrant" it's placed in. 
            if (adjacent < 0)
            {
                angle += 180;
            }
            else if (opposite < 0)
            {
                angle += 360;
            }

            //Calculates the force to be applied to the thrownGadget.
            Vector2 force = new Vector2(forceMag * Mathf.Cos(angle * Mathf.PI / 180), forceMag * Mathf.Sin(angle * Mathf.PI / 180));

            //Creates the thrown gadget and adds force in the forward direction.
            GameObject thrown = Instantiate(bumper, transform.position, Quaternion.Euler(0, 0, angle - 90));
            thrown.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        }
    }

}
