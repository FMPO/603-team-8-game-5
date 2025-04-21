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
        if (collision.tag != "Solids")
        {
            if (collision.tag == "Player")
            {
                collision.GetComponent<Player>().vspd += 2f * -transform.up.y;
                collision.GetComponent<Player>().hspd += 2f * -transform.up.x;
                Debug.Log(collision.GetComponent<Player>().vspd);
            }
            else
            {
                collision.GetComponent<Rigidbody2D>().AddForce(-transform.up * 80, ForceMode2D.Impulse);
            }
        }
    }
}
