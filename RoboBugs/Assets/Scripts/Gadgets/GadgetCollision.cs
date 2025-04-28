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

            //If this isn't a slime, place normally.
            if (!gameObject.name.Contains("Slime"))
            {
                Instantiate(placedGadget, gameObject.transform.position, gameObject.transform.rotation);
            }
            //Otherwise, it must be a slime and more checks must be made.
            else
            {
                Vector3 newPosition;

                //If the collision between the min x and max x of the object.
                if (transform.position.x > bounds[0] && transform.position.x < bounds[2])
                {
                    //Test to see if it's on the top and place it there.
                    if (transform.position.y > collision.transform.position.y)
                    {
                        newPosition = new Vector3(transform.position.x, bounds[1], transform.position.z);

                        Instantiate(placedGadget, newPosition, Quaternion.Euler(0, 0, 180));
                    }
                    //Otherwise, place on the bottom side of the object.
                    else
                    {
                        newPosition = new Vector3(transform.position.x, bounds[3], transform.position.z);
                        Instantiate(placedGadget, newPosition, Quaternion.Euler(0, 0, 0));
                    }
                }
                //If the collision between the min y and max y of the object.
                else if (transform.position.y > bounds[3] && transform.position.y < bounds[1])
                {
                    //Test to see if it's on the left and place it there.
                    if (transform.position.x < collision.transform.position.x)
                    {
                        newPosition = new Vector3(bounds[0], transform.position.y, transform.position.z);
                        Instantiate(placedGadget, newPosition, Quaternion.Euler(0, 0, 270));
                    }
                    //Otherwise, place on the right side of the object.
                    else
                    {
                        newPosition = new Vector3(bounds[2], transform.position.y, transform.position.z);
                        Instantiate(placedGadget, newPosition, Quaternion.Euler(0, 0, 90));
                    }
                }
                //Left Corner Collision.
                else if (transform.position.x < collision.transform.position.x)
                {
                    //Top Left.
                    if (transform.position.y > collision.transform.position.y)
                    {
                        newPosition = new Vector3(bounds[0], bounds[1], transform.position.z);

                        Instantiate(alternateGadget, newPosition, Quaternion.Euler(0, 0, 270));
                    }
                    //Bottom Left.
                    else
                    {
                        newPosition = new Vector3(bounds[0], bounds[3], transform.position.z);
                        Instantiate(alternateGadget, newPosition, Quaternion.Euler(0, 0, 0));
                    }
                }
                //Right Corner Collision.
                else if (transform.position.x > collision.transform.position.x)
                {
                    //Top Right.
                    if (transform.position.y > collision.transform.position.y)
                    {
                        newPosition = new Vector3(bounds[2], bounds[1], transform.position.z);

                        Instantiate(alternateGadget, newPosition, Quaternion.Euler(0, 0, 180));
                    }
                    //Bottom Right.
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
