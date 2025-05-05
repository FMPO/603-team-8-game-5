using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchGadget : MonoBehaviour
{
    //Declares necessary variables.
    public GameObject[] thrownObjects;
    public Camera cam;
    public float forceMag;
    private int gadgetIndex;
    private float adjacent;
    private float opposite;
    [SerializeField] private Player player;

    // Start is called before the first frame update
    void Start()
    {
        gadgetIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        adjacent = cam.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;

        opposite = cam.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
    }

    //A method called by player input, firing a projectile in that direction.
    public void Fire(InputAction.CallbackContext context)
    {
        //Test to see if this is the right character. If not, return.
        if (player == null || player.characterId !=1)
        {
            return;
        }

        //If this is when the context has started, it proceeds.
        if (context.phase.ToString() == "Started" && Time.timeScale != 0)
        {
            int[] gadgetCount = new int[] { 0, 0, 0 };

            

            /*Calculates the adjacent and opposite sides of a right triangle, using the player 
            and mouse position before calculating the angle*/
            float adjacent = cam.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            float opposite = cam.ScreenToWorldPoint(Input.mousePosition).y - (transform.position.y + 50);

            Vector2 newOrigin = new Vector2(transform.position.x, transform.position.y + 50);

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
            GameObject thrown = Instantiate(thrownObjects[gadgetIndex], newOrigin, Quaternion.Euler(0, 0, angle - 90));
            thrown.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            //Iterates for each gadget in the world.
            foreach (GameObject n in GameObject.FindGameObjectsWithTag("Bumper"))
            {
                string name = n.name;
                if (name.Contains("Bumper"))
                {
                    //If the name contains "Bumper", it counts as a gadget.
                    gadgetCount[0]++;
                }
                else if (name.Contains("Slime"))
                {
                    gadgetCount[1]++;
                }
                else if (name.Contains("Fan"))
                {
                    gadgetCount[2]++;
                }
            }

            //If there are more than three, destroy the oldest one until only three are left.
            foreach (GameObject n in GameObject.FindGameObjectsWithTag("Bumper"))
            {
                //if (gadgetCount >= 3)
                //{
                //    Destroy(n);
                //    gadgetCount--;
                //}
                string name = n.name;
                if (name.Contains("Bumper"))
                {
                    //If the name contains "Bumper", it counts as a gadget.
                    if (gadgetCount[0] > player.atlasAllowedGadgetCounts[0])
                    {
                        Destroy(n);
                        gadgetCount[0]--;
                    }
                }
                else if (name.Contains("Slime"))
                {
                    if (gadgetCount[1] > player.atlasAllowedGadgetCounts[1])
                    {
                        Destroy(n);
                        gadgetCount[1]--;
                    }
                }
                else if (name.Contains("Fan"))
                {
                    if (gadgetCount[2] > player.atlasAllowedGadgetCounts[2])
                    {
                        Destroy(n);
                        gadgetCount[2]--;
                    }
                }
            }
        }
    }

    //Changes the current equipped gadget when called. 
    public void Scroll(InputAction.CallbackContext context)
    {
        //If the game is paused, return.
        if (Time.timeScale == 0)
        {
            return;
        }

        //If the player scrolls down, they will go to the previous gadget index.
        if (context.action.ReadValue<float>() < 0)
        {
            if (gadgetIndex == 0)
            {
                gadgetIndex = 2;
            }
            else
            {
                gadgetIndex--;
            }
        }
        //Otherwise, if they scroll up, they will go to the next gadget index.
        else if (context.action.ReadValue<float>() > 0)
        {
            if (gadgetIndex == 2)
            {
                gadgetIndex = 0;
            }
            else
            {
                gadgetIndex++;
            }
        }
    }

    //Changes the current equipped gadget when called. 
    public void ButtonSelect(InputAction.CallbackContext context)
    {
        //If the game is paused, return.
        if (Time.timeScale == 0)
        {
            return;
        }

        switch (context.action.name)
        {
            case "select1":
                gadgetIndex = 0;
                break;
            case "select2":
                gadgetIndex = 1;
                break;
            case "select3":
                gadgetIndex = 2;
                break;
            default:
                break;
        }
    }

}
