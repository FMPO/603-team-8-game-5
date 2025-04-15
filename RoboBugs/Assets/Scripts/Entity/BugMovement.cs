using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum BugMovementType
{
    WALK_ON_GROUND, //WALK_ON_GROUND_AND_WALLS, FLY
};

public enum BugType {
    RED, YELLOW, BLUE
}

enum BugMovementDirection
{ 
    LEFT, RIGHT
};

public enum BugState
{
    MOVING, STUNNED
};

public class BugMovement : MonoBehaviour
{
    private float timeToGetUp = 3f;
    private float tempTimeToGetUp = 0f;

    [Header("Bug Movement vars")]
	public BugState BugState = BugState.MOVING;
	public BugType BugType = BugType.RED;
	[SerializeField] private float wallDetectionRange = 1f;
    private Vector2 wallDetectionDirection = Vector2.left;
    [SerializeField] private BugMovementType bugMovementType = BugMovementType.WALK_ON_GROUND;
    [SerializeField] private BugMovementDirection startingDirection = BugMovementDirection.LEFT;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Hurtbox hurtbox;
    private Vector2 movementDirection = Vector2.left;
    

    [Header("Bug Physics vars")]
    [SerializeField] private PhysicsMaterial2D slipperyMaterial;
    [SerializeField] private PhysicsMaterial2D bouncyMaterial;
    public Rigidbody2D rb;
    public float damagedForce = 1f;

    [Header("Bug Sprite vars")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        //flip sprite and movementDirection depending on startingDirection
        if (startingDirection == BugMovementDirection.LEFT)
        {
            spriteRenderer.flipX = true;
            movementDirection = Vector2.left;
            wallDetectionDirection = Vector2.left;
        }
        else if(startingDirection == BugMovementDirection.RIGHT)
        {
            spriteRenderer.flipX = false;
            movementDirection = Vector2.right;
            wallDetectionDirection = Vector2.right;
        }

        //enter the moving state
        EnterMovingState();
    }

    //UPDATE FUNCTION FOR DEBUGGING PURPOSES
    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.M))
    //    {
    //        EnterStunnedState();
    //    }
    //}

    void FixedUpdate()
    {
        //if the bug should be moving,...
        if (BugState == BugState.MOVING)
        {
            //cast a ray in wallDetectionDirection direction
            RaycastHit2D hit = Physics2D.Raycast(transform.position, wallDetectionDirection.normalized, wallDetectionRange, LayerMask.GetMask("Solids"));

            //if the afformentioned ray hits something,...
            if (hit)
            {
                //flip moving direction
                FlipMovingDirection();
            }

            //apply a force in movementDirection equal to moveSpeed
            rb.AddForce(movementDirection.normalized * moveSpeed, ForceMode2D.Force);
        }
        //else if bug is stunned,...
        else if(BugState == BugState.STUNNED)
        {
            //increment tempTimeToGetUp
            tempTimeToGetUp += Time.fixedDeltaTime;

            //if timeToGetUp has elapsed,...
            if (tempTimeToGetUp >= timeToGetUp)
            {
                EnterMovingState();
            }
        }

        //reset hurtbox values
        hurtbox.updateHurtbox(0,0,1,1);
    }

    public void EnterStunnedState()
    {
        //enter the stunned state
        BugState = BugState.STUNNED;

        //allow bug to spin
        rb.freezeRotation = false;

        //change to bouncy physics material so stunned bug can bounce around
        rb.sharedMaterial = bouncyMaterial;

        //begin to countdown til getting back up
        tempTimeToGetUp = 0;
    }

    public void EnterMovingState()
    {
        //enter the moving state
        BugState = BugState.MOVING;

        //reset rotation
        transform.rotation = Quaternion.identity;

        //change to slippery physics material so moving bug does NOT bounce around
        rb.sharedMaterial = slipperyMaterial;

        //prevent bug from spinning
        rb.freezeRotation = true;

        //zero out bug's velocities
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f;
    }

    private void FlipMovingDirection()
    {
        //flip sprite and movementDirection depending on the currect direction
        if (movementDirection == Vector2.right)
        {
            spriteRenderer.flipX = true;
            movementDirection = Vector2.left;
            wallDetectionDirection = Vector2.left;
        }
        else if (movementDirection == Vector2.left)
        {
            spriteRenderer.flipX = false;
            movementDirection = Vector2.right;
            wallDetectionDirection = Vector2.right;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, wallDetectionDirection.normalized * wallDetectionRange);
    }
}
