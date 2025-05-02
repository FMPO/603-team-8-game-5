using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is a Hurtbox Script Written by Patrick Emmons for the game "Pin Brawl" in 2024
/// </summary>
public class Hurtbox : MonoBehaviour
{
    public GameObject owner;
    public bool hurtboxActive;
    public int xoffset;
    public int yoffset;
    public int width;
    public int height;

    BoxCollider2D hurtCollider;

    public UnityEvent<float> OnHurt;
    
    // Start is called before the first frame update
    void Start()
    {
        hurtCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateHurtbox( int xoffset, int yoffset, int width, int height)
    {
        this.xoffset = xoffset;
        this.yoffset = yoffset;
        this.width = width;
        this.height = height;

        if(owner.GetComponent<BugMovement>() != null)
        {
            gameObject.transform.position = new Vector3(
                owner.transform.position.x + (xoffset + (width / 2)) * 2 * (owner.GetComponent<SpriteRenderer>().flipX ? 1 : -1),
                owner.transform.position.y + (yoffset - (height / 2)) * 2 - 2,
                0);
        }
        else if (owner.GetComponent<Player>() != null)
        {
            gameObject.transform.position = new Vector3(
                owner.transform.position.x + (xoffset + (width / 2)) * 2 * (owner.GetComponent<Player>().facingRight ? 1 : -1),
                owner.transform.position.y + (yoffset - (height / 2)) * 2 - 2,
                0);
        }

        gameObject.transform.rotation = Quaternion.identity;

        gameObject.transform.localScale = new Vector3(width * 2, height * 2, 1);
    }


    //when the hurtbox detects a collision with a hitbox, it will deal damage to the hitbox's owner given specific conditions
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<Hitbox>() == null)
        {
            return;
        }
        Hitbox hitHitbox = collision.gameObject.GetComponent<Hitbox>();
        Player hitPlayer = hitHitbox.owner.GetComponent<Player>();
        if (hitHitbox != null && hitHitbox.owner != this.owner && (!hitHitbox.ignorePlayers.Contains(owner)) && hitPlayer != null)
        {
            //if owner has a BugMovement component,... 
            if (owner.GetComponent<BugMovement>() != null)
            {
                BugMovement hurtBug = owner.GetComponent<BugMovement>();
                if (hurtBug == null)
                {
                    // the hurtbox is not tied to a player
                    Debug.Log($"hitbox hit object for {hitHitbox.damage} damage");
                    OnHurt?.Invoke(hitHitbox.damage);
                    return;
                }
                //Debug.Log("Hurtbox hit: " + owner.GetInstanceID());
                //if (!hurtBug.isAlive || !hitPlayer.isAlive || (hurtBug.invincibilityCounter > 0))
                //{
                //    return;
                //}

                //Tell bug to enter the stunned state
                hurtBug.EnterStunnedState();

                //apply a force to the bug in a direction based on the player's attack direction
                Vector2 knockBackDirection = new Vector2(hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback);
                hurtBug.rb.AddForce(knockBackDirection.normalized * hurtBug.damagedForce * (1f + (float)hitPlayer.punchForce/10), ForceMode2D.Impulse);
                //Debug.Log("Knockback by: " + knockBackDirection.normalized * hurtBug.damagedForce);

                //generate the location of the hit or block spark
                BoxCollider2D hurtboxCollider = gameObject.GetComponent<BoxCollider2D>();
                BoxCollider2D hitboxCollider = collision.gameObject.GetComponent<BoxCollider2D>();

                Vector2 overlapCenter = GetOverlapCenter(hurtboxCollider, hitboxCollider);
                //if (overlapCenter != Vector2.zero)
                //{
                //    Debug.Log("Overlap Center: " + overlapCenter);
                //}
                //make the damaged player actually take damage
                hitHitbox.ignorePlayers.Add(owner);
                hitHitbox.canCancel = true;
                //hurtPlayer.TakeDamage(hitHitbox.owner, hitHitbox.damage, hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun, overlapCenter, hitHitbox.owner.GetComponent<SpriteRenderer>().material.GetTexture("_PaletteTex"));
                //hurtPlayer.hitstopVal = 10;
                //hurtPlayer.animator.enabled = false;
                //hitHitbox.hitstopVal = 10;
                //hitHitbox.animator.enabled = false;
                //GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(GameManager.Instance.screenShakeIntensity, (1f / 6f)); //shake the camera when hit
            }
            //else if owner has a Player component,...
            else if (owner.GetComponent<Player>() != null)
            {
                var hurtPlayer = owner.GetComponent<Player>();
                if (hurtPlayer == null)
                {
                    // the hurtbox is not tied to a player
                    Debug.Log($"hitbox hit object for {hitHitbox.damage} damage");
                    OnHurt?.Invoke(hitHitbox.damage);
                    return;
                }
                //Debug.Log("Hurtbox hit: " + owner.GetInstanceID());
                if (!hurtPlayer.isAlive || !hitPlayer.isAlive || (hurtPlayer.invincibilityCounter > 0))
                {
                    return;
                }

                //generate the location of the hit or block spark
                BoxCollider2D hurtboxCollider = gameObject.GetComponent<BoxCollider2D>();
                BoxCollider2D hitboxCollider = collision.gameObject.GetComponent<BoxCollider2D>();

                Vector2 overlapCenter = GetOverlapCenter(hurtboxCollider, hitboxCollider);
                //if (overlapCenter != Vector2.zero)
                //{
                //    Debug.Log("Overlap Center: " + overlapCenter);
                //}
                //make the damaged player actually take damage
                hitHitbox.ignorePlayers.Add(owner);
                hitHitbox.canCancel = true;
                //hurtPlayer.TakeDamage(hitHitbox.owner, hitHitbox.damage, hitHitbox.xKnockback * (hitPlayer.facingRight ? 1 : -1), hitHitbox.yKnockback, hitHitbox.hitstun, overlapCenter, hitHitbox.owner.GetComponent<SpriteRenderer>().material.GetTexture("_PaletteTex"));
                hurtPlayer.hitstopVal = 10;
                hurtPlayer.animator.enabled = false;
                hitPlayer.hitstopVal = 10;
                hitPlayer.animator.enabled = false;
                GameManager.Instance.gameObject.GetComponent<CameraShake>().Shake(GameManager.Instance.screenShakeIntensity, (1f / 6f)); //shake the camera when hit
            }
        }
    }

    public Vector2 GetOverlapCenter(BoxCollider2D collider1, BoxCollider2D collider2)
    {
        Bounds bounds1 = collider1.bounds;
        Bounds bounds2 = collider2.bounds;

        // Calculate the overlap area
        float xMin = Mathf.Max(bounds1.min.x, bounds2.min.x);
        float xMax = Mathf.Min(bounds1.max.x, bounds2.max.x);
        float yMin = Mathf.Max(bounds1.min.y, bounds2.min.y);
        float yMax = Mathf.Min(bounds1.max.y, bounds2.max.y);

        //// Check if there is an overlap
        //if (xMin < xMax && yMin < yMax)
        //{
        //    // Calculate the center point of the overlap
        //    float overlapCenterX = (xMin + xMax) / 2;
        //    float overlapCenterY = (yMin + yMax) / 2;
        //    return new Vector2(overlapCenterX, overlapCenterY);
        //}
        //else
        //{
        //    // No overlap
        //    return Vector2.zero;
        //}

        // Calculate the center point of the overlap
        float overlapCenterX = (xMin + xMax) / 2;
        float overlapCenterY = (yMin + yMax) / 2;
        return new Vector2(overlapCenterX, overlapCenterY);
    }
}

