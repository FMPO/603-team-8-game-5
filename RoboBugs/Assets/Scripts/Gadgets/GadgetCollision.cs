using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NewBehaviourScript : MonoBehaviour
{
    //Declares necessary variables.
    public GameObject placedGadget;
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
        Debug.Log(collision.transform.localScale);
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

            }
        }
    }
}
