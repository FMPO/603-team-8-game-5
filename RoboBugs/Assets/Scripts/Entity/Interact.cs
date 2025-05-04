using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    //An active list of every gameObject within the trigger's range.
    private List<GameObject> objectsInRange;

    // Start is called before the first frame update
    void Start()
    {
        objectsInRange = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*If the player interacts with a gadget or terrarium, it will be
        added to the list and the button will be displayed*/
        if (collision.tag == "Bumper")
        {
            gameObject.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
            objectsInRange.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Removes the terrarium or gadgets from the list.
        if (collision.tag == "Bumper")
        {
            objectsInRange.Remove(collision.gameObject);
        }
        //If there are no more objects in range, the overhead display is disabled.
        if (objectsInRange.Count == 0)
        {
            gameObject.transform.parent.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void Interaction(InputAction.CallbackContext context)
    {
        if (context.phase.ToString() == "Started" && Time.timeScale != 0)
        {
            //If there's an interactable object in range, ther various behaviors will trigger.
            if (objectsInRange.Count != 0)
            {
                for (int i = 0; i < objectsInRange.Count; i += 0)
                {
                    //For now, if a gadget is in range, it is destroyed.
                    if (objectsInRange[i].tag == "Bumper")
                    {
                        Destroy(objectsInRange[i]);
                    }
                    //Otherwise, we increment the index.
                    else
                    {
                        i++;
                    }
                }
            }
        }
    }
}
