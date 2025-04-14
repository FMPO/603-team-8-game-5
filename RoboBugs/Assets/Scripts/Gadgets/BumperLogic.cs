using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperLogic : MonoBehaviour
{
    public float forceMag = 5;

    // Start is called before the first frame update
    void Start()
    {

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
                collision.GetComponent<Player>().vspd = 16 * -transform.up.y;
                collision.GetComponent<Player>().hspd += 16 * -transform.up.x;
            }
            else
            {
                collision.GetComponent<Rigidbody2D>().AddForce(-transform.up * 500, ForceMode2D.Impulse);
            }            
        }
    }
}
