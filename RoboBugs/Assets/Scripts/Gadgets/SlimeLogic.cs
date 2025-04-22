using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeLogic : MonoBehaviour
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
                Debug.Log(collision.GetComponent<Rigidbody2D>().velocity);
            }
        }
    }
}
