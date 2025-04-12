using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum BugMovementType
{
    WALK_ON_GROUND, //WALK_ON_GROUND_AND_WALLS, FLY
};

enum BugMovementDirection
{ 
    LEFT, RIGHT
};

enum BugState
{
    MOVING, STUNNED
};

public class BugMovement : MonoBehaviour
{
    private BugState currentBugState = BugState.MOVING;
    private float timeToGetUp = 3f;
    private float tempTimeToGetUp = 0f;

    [Header("Bug Movement vars")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BugMovementType bugMovementType = BugMovementType.WALK_ON_GROUND;
    [SerializeField] private BugMovementDirection startingDirection = BugMovementDirection.LEFT;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Hurtbox hurtbox;
    private Vector2 movementDirection = Vector2.left;

    [Header("Bug Sprite vars")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        //flip sprite and movementDirection depending on startingDirection
        if (startingDirection == BugMovementDirection.LEFT)
        {
            spriteRenderer.flipX = true;
            movementDirection = Vector2.left;
        }
        else if(startingDirection == BugMovementDirection.RIGHT)
        {
            spriteRenderer.flipX = false;
            movementDirection = Vector2.right;
        }
    }

    //UPDATE FUNCTION FOR DEBUGGING PURPOSES
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            EnterStunnedState();
        }
    }

    void FixedUpdate()
    {
        //if the bug should be moving,...
        if (currentBugState == BugState.MOVING)
        {
            //apply a force in movementDirection equal to moveSpeed
            rb.AddForce(movementDirection.normalized * moveSpeed, ForceMode2D.Force);
        }
        //else if bug is stunned,...
        else if(currentBugState == BugState.STUNNED)
        {
            //increment tempTimeToGetUp
            tempTimeToGetUp += Time.fixedDeltaTime;

            //if timeToGetUp has elapsed,...
            if (tempTimeToGetUp >= timeToGetUp)
            {
                EnterMovingState();
            }
        }
        hurtbox.updateHurtbox(0,0,1,1);
    }

    public void EnterStunnedState()
    {
        //DEBUG
        spriteRenderer.color = Color.red;

        //enter the stunned state
        currentBugState = BugState.STUNNED;

        //allow bug to spin
        rb.freezeRotation = false;

        //begin to countdown til getting back up
        tempTimeToGetUp = 0;
    }

    public void EnterMovingState()
    {
        //DEBUG
        spriteRenderer.color = Color.yellow;

        //enter the moving state
        currentBugState = BugState.MOVING;

        //reset rotation
        transform.rotation = Quaternion.identity;

        //prevent bug from spinning
        rb.freezeRotation = true;
    }
}
