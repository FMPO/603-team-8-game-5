using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    //Declares necessary variables.
    public GameObject placedGadget;
    public GameObject alternateGadget;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //Updates the rotation so that it will always face forward.
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, rb.velocity.normalized);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 20 * Time.deltaTime);
    }

    //A method that detects if something has entered this gameObject's trigger.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Left, top, right, bottom.
        float[] bounds = 
        {   collision.transform.position.x - collision.transform.localScale.x / 2,
            collision.transform.position.y + collision.transform.localScale.y / 2,
            collision.transform.position.x + collision.transform.localScale.x / 2,
            collision.transform.position.y - collision.transform.localScale.y / 2
        };

        //If a wall has entered the trigger, this gameObject is replaced with a "placedBumper."
        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);

            if (!gameObject.name.Contains("Slime"))
            {
                Instantiate(placedGadget, gameObject.transform.position, gameObject.transform.rotation);
            }
            else
            {

                Vector3 newPosition;

                if (transform.position.x > bounds[0] && transform.position.x < bounds[2])
                {
                    //Top Side
                    if (transform.position.y > collision.transform.position.y)
                    {
                        newPosition = new Vector3(transform.position.x, bounds[1], transform.position.z);

                        Instantiate(placedGadget, newPosition, Quaternion.Euler(0, 0, 180));
                    }
                    //Bottom Side
                    else
                    {
                        newPosition = new Vector3(transform.position.x, bounds[3], transform.position.z);
                        Instantiate(placedGadget, newPosition, Quaternion.Euler(0, 0, 0));
                    }
                }
                else if (transform.position.y > bounds[3] && transform.position.y < bounds[1])
                {
                    //Left Side
                    if (transform.position.x < collision.transform.position.x)
                    {
                        newPosition = new Vector3(bounds[0], transform.position.y, transform.position.z);
                        Instantiate(placedGadget, newPosition, Quaternion.Euler(0, 0, 270));
                    }
                    //Right Side
                    else
                    {
                        newPosition = new Vector3(bounds[2], transform.position.y, transform.position.z);
                        Instantiate(placedGadget, newPosition, Quaternion.Euler(0, 0, 90));
                    }
                }
                //Left Corner Test
                else if (transform.position.x < collision.transform.position.x)
                {
                    //Top Left
                    if (transform.position.y > collision.transform.position.y)
                    {
                        newPosition = new Vector3(bounds[0], bounds[1], transform.position.z);

                        Instantiate(alternateGadget, newPosition, Quaternion.Euler(0, 0, 270));
                    }
                    //Bottom Left
                    else
                    {
                        newPosition = new Vector3(bounds[0], bounds[3], transform.position.z);
                        Instantiate(alternateGadget, newPosition, Quaternion.Euler(0, 0, 0));
                    }
                }
                //Right Corner Test
                else if (transform.position.x > collision.transform.position.x)
                {
                    //Top Right
                    if (transform.position.y > collision.transform.position.y)
                    {
                        newPosition = new Vector3(bounds[2], bounds[1], transform.position.z);

                        Instantiate(alternateGadget, newPosition, Quaternion.Euler(0, 0, 180));
                    }
                    //Bottom Right
                    else
                    {
                        newPosition = new Vector3(bounds[2], bounds[3], transform.position.z);
                        Instantiate(alternateGadget, newPosition, Quaternion.Euler(0, 0, 90));
                    }
                }
            }
        }
    }
}
