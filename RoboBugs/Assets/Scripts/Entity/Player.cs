using System.Collections;
using System.Collections.Generic;

//using UnityEditor.Animations;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Run,
        Jumpsquat,
        Jump,
        Landing,
        Hitstun,
        SideAttack,
        UpAttack,
        DownAttack,
        Menuing
    }

    //Animation info fields
    public AnimatorStateInfo animStateInfo;
    public AnimatorClipInfo[] currentClipInfo;
    private int currentFrame;
    private int frameCount;
    private int hitstunVal = 0;

    //weapon and color swapping support fields
    public string weaponName = "sword";
    public Animator animator;
    public RuntimeAnimatorController baseAnimController;
    //public List<AnimatorOverrideController> otherWeaponAnimControllers;
    //public List<Texture2D> colorPalletes;
    public JSONReader characterJSON;
    public JSONReader.FrameDataContainer frameData;
    public JSONReader.HitboxDataContainer hitboxData;
    public JSONReader.HurtboxDataContainer hurtboxData;
    public JSONReader.ImpulseFrameData impulseFrames;
    public JSONReader.ImpulseDataContainer impulseData;
    public int maxHitboxes = 2;
    private JSONReader.WeaponDataList weaponData;
    //private int currentAnimControllerIndex = 0;
    //public int currentColorIndex = 0;
    public int characterId = 0;
    public bool characterSwapFlag = false;

    //Player fields
    public int runSpeed = 3;
    public int jumpForce = 10;
    public int gravity = 1;
    private int gravityModCounter = 0;
    public int gravityModifier = 1;
    public int health = 100;
    //public int stockCount = 4;
    public float hspd = 0;
    public float vspd = 0;
    public int maxHspd = 10;
    public int maxVspd = 1;
    public PlayerState state = PlayerState.Idle;
    public bool facingRight = true;
    public InputHandler inputHandler;
    public LayerMask groundLayer; // Layer mask to specify what is considered ground
    public float rayLength = 0.1f; // Length of the ray
    public Vector2 rayOffset = new Vector2(8f, 8f); // Offset for the rays
    public RaycastHit2D grounded;
    public RaycastHit2D touchingWall;
    public RaycastHit2D collidedCeiling;
    public GameObject hitboxReference;
    public GameObject hurtboxReference;
    private List<GameObject> hitboxes = new List<GameObject>();
    private GameObject hurtbox;
    public bool isAlive = true;
    public float invincibilityTime = 1f;
    public float invincibilityCounter = 0;



    private float tempHspd = 0;
    public int hitstopVal = 0;
    private PlayerState prevState;
    private int lerpDelay = 0;
    private BoxCollider2D boxCollider;// Reference to the BoxCollider2D component
    private Dictionary<InputHandler.Inputs, InputHandler.InputState> inputs;

    //entity stuff
    public List<GameObject> entityList = new List<GameObject>();
    public Dictionary<string, GameObject> entities = new Dictionary<string, GameObject>();



    public BoxCollider2D PlayerCollider
    {
        get => boxCollider;
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        inputs = inputHandler.keyBindings;

    }
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        if (characterJSON == null)
        {
            characterJSON = gameObject.GetComponent<JSONReader>();
        }

        characterJSON.GetWeaponStats();
        weaponData = characterJSON.weaponDataList;
        InitWeapon();

        //set up the player's initial color ( the -1 set is so the cycle color +1 into index 0)
        //currentColorIndex = -1;
        //CycleColor();
        gameObject.transform.position = new Vector3(0, -64, 0);
        ResetBoxCollider();

    }


    void FixedUpdate()
    {
        //this is a method to just make sure the player never gets stuck out of bounds
        //DetectOutOfBounds();

        //update this frames inputs
        inputs = inputHandler.keyBindings;

        if (hitstopVal > 0)
        {
            hitstopVal--;
            return;
        }
        //else if (PauseManager.instance.isPaused)
        //{
        //    animator.enabled = false;
        //    return;
        //}
        else
        {
            animator.enabled = true;
        }

        if (invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;
            gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Brightness", 2f);
        }
        else
        {
            invincibilityCounter = 0;
            gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Brightness", 1f);
        }


        //update animator info things to get current frame and frame count
        animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        frameCount = (int)(currentClipInfo[0].clip.length * currentClipInfo[0].clip.frameRate);
        currentFrame = ((int)(animStateInfo.normalizedTime * frameCount)) % frameCount;

        grounded = IsGrounded();
        touchingWall = IsTouchingWall();



        //re-update weapon stats when pause is pressed
        if (Input.GetKey(KeyCode.F1))
        {
            characterJSON.GetWeaponStats();
            weaponData = characterJSON.weaponDataList;
            InitWeapon();
        }

        switch (state)
        {
            case PlayerState.Idle:

                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Held && characterId == 0)
                {
                    //check for turnaround inputs
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        facingRight = false;
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        facingRight = true;
                    }

                    //check for which attack is pressed
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }
                }
                //check for movement input
                if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                {
                    facingRight = false;
                    SetState(PlayerState.Run);

                }
                else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                {
                    facingRight = true;
                    SetState(PlayerState.Run);

                }

                //check for jump input
                if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held)
                {
                    SetState(PlayerState.Jumpsquat);
                }


                //check for shield input
                //if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Pressed)
                //{
                //    SetState(PlayerState.Shield);
                //}

                //check for menu input
                if (inputs[InputHandler.Inputs.Menu] == InputHandler.InputState.Held)
                {
                    SwapCharacter();

                }

                LerpHspd(0, 1);
                //check for ground
                if (grounded.collider == null)
                {
                    SetState(PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.Run:

                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Held && characterId == 0)
                {
                    //check for turnaround inputs
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        facingRight = false;
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        facingRight = true;
                    }

                    //check for which attack is pressed
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }
                }

                //run logic
                if (facingRight)
                {
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        //hspd = hspd < runSpeed ? runSpeed : hspd;
                        if (hspd > runSpeed)
                        {
                            LerpHspd(runSpeed, 5);
                        }
                        else
                        {
                            hspd = runSpeed;
                        }
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                        SetState(PlayerState.Idle);
                        break;
                    }
                }
                else
                {
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        if (hspd < -runSpeed)
                        {
                            LerpHspd(-runSpeed, 5);
                        }
                        else
                        {
                            hspd = -runSpeed;
                        }
                    }
                    else if (inputHandler.keyBindings[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                        SetState(PlayerState.Idle);
                    }
                }
                //check for jump input
                if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held)
                {
                    //check for turnaround inputs
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        facingRight = false;
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        facingRight = true;
                    }

                    SetState(PlayerState.Jumpsquat);
                }
                //check for shield input
                //if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Pressed)
                //{
                //    SetState(PlayerState.Shield);
                //}
                //check for collision
                if (grounded.collider == null)
                {
                    //if not grounded
                    SetState(PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.Jumpsquat:


                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Held && characterId == 0)
                {
                    if (tempHspd != 0)
                    {
                        hspd = tempHspd;
                        tempHspd = 0;
                    }

                    vspd = jumpForce;
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }
                }
                //check for shield input
                //if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Pressed)
                //{
                //    if (tempHspd != 0)
                //    {
                //        hspd = tempHspd;
                //        tempHspd = 0;
                //    }
                //    vspd = jumpForce;
                //    SetState(PlayerState.Shield);
                //}
                if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                {
                    facingRight = true;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                {
                    facingRight = false;
                }
                if (currentFrame == frameCount - 1 && grounded.collider != null)
                {
                    if (tempHspd != 0)
                    {
                        hspd = tempHspd;
                        tempHspd = 0;
                    }
                    vspd = (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held ? jumpForce : jumpForce / 2);
                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(PlayerState.Jump);

                }

                break;
            case PlayerState.Jump:



                //check for ground collision
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    SetState(PlayerState.Landing);
                    break;
                }
                //check for wall collision
                if (touchingWall.collider != null && touchingWall.collider.gameObject.tag != "slope")
                {
                    SnapToWall(touchingWall);
                    hspd = -hspd;
                }

                //check for attack input
                if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Held && characterId == 0)
                {
                    //check for turnaround inputs
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        facingRight = false;
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        facingRight = true;
                    }

                    //check for which attack is pressed
                    if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.UpAttack);
                        break;
                    }
                    else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.DownAttack);
                        break;
                    }
                    else
                    {
                        SetState(PlayerState.SideAttack);
                        break;
                    }
                }
                //allow for horizontal movement
                if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                {
                    facingRight = true;
                    hspd = hspd < runSpeed ? runSpeed : hspd;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                {
                    facingRight = false;
                    hspd = hspd > -runSpeed ? -runSpeed : hspd;
                }
                else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                {
                    LerpHspd(0, 5);
                }
                //check for shield input
                //if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Pressed)
                //{
                //    SetState(PlayerState.Shield);
                //}

                break;
            case PlayerState.Landing:
                vspd = 0;
                LerpHspd(0, 5);
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.Run);
                        facingRight = false;
                    }
                    else if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        SetState(PlayerState.Run);
                        facingRight = true;
                    }
                    else
                    {
                        SetState(PlayerState.Idle);
                    }

                }

                break;
            case PlayerState.Hitstun:
                if (grounded.collider != null && vspd < 0)
                {
                    SnapToSurface(grounded);
                    vspd = -vspd;
                    //LerpHspd(0, 3);
                }
                //check for wall collision
                if (touchingWall.collider != null && touchingWall.collider.gameObject.tag != "slope")
                {
                    SnapToWall(touchingWall);
                    hspd = -hspd;
                }
                if (hitstunVal > 0)
                {
                    hitstunVal--;
                }
                else
                {
                    //if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.Held)
                    //{
                    //    SetState(PlayerState.Shield);
                    //}
                    //else
                    //{
                    //    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    //}
                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                }
                break;
            //case PlayerState.Shield:
            //    //check for attack input
            //    if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Pressed)
            //    {
            //        if (inputs[InputHandler.Inputs.Up] == InputHandler.InputState.Held)
            //        {
            //            SetState(PlayerState.UpAttack);
            //            break;
            //        }
            //        else if (inputs[InputHandler.Inputs.Down] == InputHandler.InputState.Held)
            //        {
            //            SetState(PlayerState.DownAttack);
            //            break;
            //        }
            //        else
            //        {
            //            SetState(PlayerState.SideAttack);
            //            break;
            //        }
            //    }
            //    //check for ground collision
            //    if (grounded.collider != null)
            //    {
            //        SnapToSurface(grounded);
            //        vspd = 0;
            //        //check for Jump input
            //        if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
            //        {
            //            SetState(PlayerState.Jumpsquat);
            //        }
            //    }
            //    //check for wall collision
            //    if (touchingWall.collider != null && touchingWall.collider.gameObject.tag != "slope")
            //    {
            //        SnapToWall(touchingWall);
            //        hspd = -hspd;
            //    }
            //    //check for shield release
            //    if (inputs[InputHandler.Inputs.Shield] == InputHandler.InputState.UnPressed)
            //    {
            //        SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
            //    }

            //    //check for horizontal inputs
            //    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
            //    {
            //        facingRight = true;
            //    }
            //    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
            //    {
            //        facingRight = false;
            //    }


            //    LerpHspd(0, 6);
            //    break;
            case PlayerState.SideAttack:

                //handle hitbox activation
                for (int i = 0; i < frameData.sideAttackFrames.startFrames.Count; i++)
                {
                    if (currentFrame == frameData.sideAttackFrames.endFrames[i])
                    {
                        hitboxes[0].SetActive(false);

                    }
                    else if (currentFrame == frameData.sideAttackFrames.startFrames[i])
                    {
                        if (hitboxes[0].activeSelf == false)
                        {
                            hitboxes[0].GetComponent<Hitbox>().ignorePlayers.Clear();
                        }
                        hitboxes[0].SetActive(true);
                        hitboxes[0].GetComponent<Hitbox>().updateHitbox(
                            1,
                            hitboxData.sideAttackHitboxes[i].xOffset,
                            hitboxData.sideAttackHitboxes[i].yOffset,
                            hitboxData.sideAttackHitboxes[i].width,
                            hitboxData.sideAttackHitboxes[i].height,
                            hitboxData.sideAttackHitboxes[i].xKnockback,
                            hitboxData.sideAttackHitboxes[i].yKnockback,
                            hitboxData.sideAttackHitboxes[i].hitstun
                        );
                    }


                }




                //if grounded
                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    if (prevState == PlayerState.Jump || prevState == PlayerState.Jumpsquat)
                    {

                        SetState(PlayerState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 3);
                        //jump canceling logic on ground only
                        if (hitboxes[0].GetComponent<Hitbox>().canCancel)
                        {
                            //check for Jump input
                            if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held)
                            {
                                SetState(PlayerState.Jumpsquat);
                            }
                        }
                    }

                }
                else
                {
                    //allow for horizontal movement
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = hspd < runSpeed ? runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = hspd > -runSpeed ? -runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                    }
                }
                //handle impulse activation
                for (int i = 0; i < impulseFrames.sideAttackImpulseFrames.Count; i++)
                {
                    if (currentFrame == impulseFrames.sideAttackImpulseFrames[i])
                    {

                        vspd = impulseData.sideAttackImpulseData[i].yImpulse;
                        hspd = impulseData.sideAttackImpulseData[i].xImpulse * (facingRight ? 1 : -1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {
                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.UpAttack:

                //handle hitbox activation
                for (int i = 0; i < frameData.upAttackFrames.startFrames.Count; i++)
                {
                    if (currentFrame == frameData.upAttackFrames.endFrames[i])
                    {
                        hitboxes[0].SetActive(false);
                    }
                    else if (currentFrame == frameData.upAttackFrames.startFrames[i])
                    {
                        if (hitboxes[0].activeSelf == false)
                        {
                            hitboxes[0].GetComponent<Hitbox>().ignorePlayers.Clear();
                        }
                        hitboxes[0].SetActive(true);
                        hitboxes[0].GetComponent<Hitbox>().updateHitbox(
                            1,
                            hitboxData.upAttackHitboxes[i].xOffset,
                            hitboxData.upAttackHitboxes[i].yOffset,
                            hitboxData.upAttackHitboxes[i].width,
                            hitboxData.upAttackHitboxes[i].height,
                            hitboxData.upAttackHitboxes[i].xKnockback,
                            hitboxData.upAttackHitboxes[i].yKnockback,
                            hitboxData.upAttackHitboxes[i].hitstun
                        );
                    }


                }





                if (grounded.collider != null)
                {

                    SnapToSurface(grounded);
                    vspd = 0;
                    if (prevState == PlayerState.Jump || prevState == PlayerState.Jumpsquat)
                    {

                        SetState(PlayerState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 1);

                        //jump canceling logic on ground only
                        if (hitboxes[0].GetComponent<Hitbox>().canCancel)
                        {
                            //check for Jump input
                            if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held)
                            {
                                SetState(PlayerState.Jumpsquat);
                            }
                        }
                    }

                }
                else
                {
                    //allow for horizontal movement
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = hspd < runSpeed ? runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = hspd > -runSpeed ? -runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                    }
                }
                //handle impulse activation
                for (int i = 0; i < impulseFrames.upAttackImpulseFrames.Count; i++)
                {
                    if (currentFrame == impulseFrames.upAttackImpulseFrames[i])
                    {

                        vspd = impulseData.upAttackImpulseData[i].yImpulse;
                        hspd = impulseData.upAttackImpulseData[i].xImpulse * (facingRight ? 1 : -1);
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    break;
                }
                break;
            case PlayerState.DownAttack:

                //handle hitbox activation
                for (int i = 0; i < frameData.downAttackFrames.startFrames.Count; i++)
                {
                    if (currentFrame == frameData.downAttackFrames.endFrames[i])
                    {
                        hitboxes[0].SetActive(false);
                        hitboxes[0].GetComponent<Hitbox>().ignorePlayers.Clear();
                    }
                    else if (currentFrame == frameData.downAttackFrames.startFrames[i])
                    {
                        if (hitboxes[0].activeSelf == false)
                        {
                            hitboxes[0].GetComponent<Hitbox>().ignorePlayers.Clear();
                        }
                        hitboxes[0].SetActive(true);
                        hitboxes[0].GetComponent<Hitbox>().updateHitbox(
                            1,
                            hitboxData.downAttackHitboxes[i].xOffset,
                            hitboxData.downAttackHitboxes[i].yOffset,
                            hitboxData.downAttackHitboxes[i].width,
                            hitboxData.downAttackHitboxes[i].height,
                            hitboxData.downAttackHitboxes[i].xKnockback,
                            hitboxData.downAttackHitboxes[i].yKnockback,
                            hitboxData.downAttackHitboxes[i].hitstun
                        );
                    }


                }


                if (grounded.collider != null)
                {
                    SnapToSurface(grounded);
                    vspd = 0;
                    if (prevState == PlayerState.Jump || prevState == PlayerState.Jumpsquat)
                    {

                        SetState(PlayerState.Landing);
                        break;
                    }
                    else
                    {
                        LerpHspd(0, 2);
                        //jump canceling logic on ground only
                        if (hitboxes[0].GetComponent<Hitbox>().canCancel)
                        {
                            //check for Jump input
                            if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Held)
                            {
                                SetState(PlayerState.Jumpsquat);
                            }
                        }
                    }

                }
                else
                {
                    //allow for horizontal movement
                    if (inputs[InputHandler.Inputs.Right] == InputHandler.InputState.Held)
                    {
                        hspd = hspd < runSpeed ? runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.Held)
                    {
                        hspd = hspd > -runSpeed ? -runSpeed : hspd;
                    }
                    else if (inputs[InputHandler.Inputs.Left] == InputHandler.InputState.UnPressed && inputs[InputHandler.Inputs.Right] == InputHandler.InputState.UnPressed)
                    {
                        LerpHspd(0, 5);
                    }
                }
                //handle impulse activation
                for (int i = 0; i < impulseFrames.downAttackImpulseFrames.Count; i++)
                {
                    if (currentFrame == impulseFrames.downAttackImpulseFrames[i])
                    {

                        vspd = impulseData.downAttackImpulseData[i].yImpulse;
                        hspd = impulseData.downAttackImpulseData[i].xImpulse * (facingRight ? 1 : -1);
                    }
                }
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                {

                    SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
                    break;
                }
                break;
            //case PlayerState.Menuing:
            //    hspd = 0;
            //    vspd = 0;
            //    if (inputs[InputHandler.Inputs.Menu] == InputHandler.InputState.Pressed)
            //    {
            //        SetState(grounded.collider != null ? PlayerState.Idle : PlayerState.Jump);
            //    }

            //    //change weapon when attack is pressed
            //    if (inputs[InputHandler.Inputs.Attack] == InputHandler.InputState.Pressed)
            //    {
            //        CycleWeapon();
            //    }
            //    //change color when jump is pressed
            //    if (inputs[InputHandler.Inputs.Jump] == InputHandler.InputState.Pressed)
            //    {
            //        CycleColor();
            //    }

            //    break;



        }

        

        //check horizontal collision
        RaycastHit2D hitWallRay = IsTouchingWall();
        if (hitWallRay.collider != null && hitWallRay.collider.gameObject.tag != "slope")
        {
            //if (hitWallRay.point.x < gameObject.transform.position.x)
            //{
            //    if (hspd < 0)
            //    {
            //        hspd = 0;
            //    }
            //}
            //else
            //{
            //    if (hspd > 0)
            //    {
            //        hspd = 0;
            //    }
            //}
            SnapToWall(hitWallRay);
            hspd = 0;
        }
        // check for ceiling
        collidedCeiling = IsTouchingCeiling();
        if (collidedCeiling.collider != null)
        {
            vspd = vspd > 0 ? 0 : vspd;
            SnapToCeiling(collidedCeiling);
        }
        //clamp hspd and vspd to max hspd and vspd
        hspd = Mathf.Clamp(hspd, -maxHspd, maxHspd);
        vspd = Mathf.Clamp(vspd, -maxVspd, maxVspd);

        gameObject.transform.position += new Vector3(hspd, vspd, 0);
        gameObject.GetComponent<SpriteRenderer>().flipX = facingRight ? false : true;
        animator.SetInteger("characterID", characterId);

        //check for ground collision
        if (grounded.collider != null && vspd <= 0)
        {
            SnapToSurface(grounded);
            vspd = 0;
        }
        else
        {
            //vspd -= gravity;
            if (gravityModCounter < gravityModifier)
            {
                gravityModCounter++;
            }
            else
            {
                gravityModCounter = 0;
                vspd -= gravity;
            }

        }

    }

    private void SetState(PlayerState targetState)
    {
        tempHspd = 0;
        animator.enabled = true;
        animator.SetInteger("state", (int)targetState);
        prevState = state;
        state = targetState;

        //----------------------------any State specific enter and exit logic----------------------
        switch (targetState)
        {
            case PlayerState.Idle:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.idleHurtbox.xOffset,
                hurtboxData.idleHurtbox.yOffset,
                hurtboxData.idleHurtbox.width,
                hurtboxData.idleHurtbox.height
                );
                break;
            case PlayerState.Run:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.runHurtbox.xOffset,
                hurtboxData.runHurtbox.yOffset,
                hurtboxData.runHurtbox.width,
                hurtboxData.runHurtbox.height
                );
                entities["dash_dust"].SetActive(true);
                entities["dash_dust"].GetComponent<Entity>().InitEntity(0, 0);
                //GameManager.audioManager.PlayDashSound();
                break;
            case PlayerState.Jumpsquat:
                //store horizontal speed for jumpsquat
                if (hspd != 0)
                {
                    tempHspd = hspd;
                    hspd = 0;
                }

                //create jump_dust effect
                entities["jump_dust"].SetActive(true);
                entities["jump_dust"].GetComponent<Entity>().InitEntity(0, 0);

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.jumpsquatHurtbox.xOffset,
                hurtboxData.jumpsquatHurtbox.yOffset,
                hurtboxData.jumpsquatHurtbox.width,
                hurtboxData.jumpsquatHurtbox.height
                );
                break;
            case PlayerState.Jump:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.jumpHurtbox.xOffset,
                hurtboxData.jumpHurtbox.yOffset,
                hurtboxData.jumpHurtbox.width,
                hurtboxData.jumpHurtbox.height
                );
                break;
            case PlayerState.Landing:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.landingHurtbox.xOffset,
                hurtboxData.landingHurtbox.yOffset,
                hurtboxData.landingHurtbox.width,
                hurtboxData.landingHurtbox.height
                );
                break;
            case PlayerState.Hitstun:
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.hitstunHurtbox.xOffset,
                hurtboxData.hitstunHurtbox.yOffset,
                hurtboxData.hitstunHurtbox.width,
                hurtboxData.hitstunHurtbox.height
                );
                break;
            //case PlayerState.Shield:


            //    hurtbox.GetComponent<Hurtbox>().updateHurtbox(
            //    hurtboxData.shieldHurtbox.xOffset,
            //    hurtboxData.shieldHurtbox.yOffset,
            //    hurtboxData.shieldHurtbox.width,
            //    hurtboxData.shieldHurtbox.height
            //    );
            //    break;
            case PlayerState.SideAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.sideAttackHurtbox.xOffset,
                hurtboxData.sideAttackHurtbox.yOffset,
                hurtboxData.sideAttackHurtbox.width,
                hurtboxData.sideAttackHurtbox.height
                );
                break;
            case PlayerState.UpAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.upAttackHurtbox.xOffset,
                hurtboxData.upAttackHurtbox.yOffset,
                hurtboxData.upAttackHurtbox.width,
                hurtboxData.upAttackHurtbox.height
                );
                break;
            case PlayerState.DownAttack:
                hitboxes[0].GetComponent<Hitbox>().canCancel = false;

                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.downAttackHurtbox.xOffset,
                hurtboxData.downAttackHurtbox.yOffset,
                hurtboxData.downAttackHurtbox.width,
                hurtboxData.downAttackHurtbox.height
                );
                break;
            case PlayerState.Menuing:
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Scene_MainMenu")
                {
                    SetState(PlayerState.Idle);
                    return;
                }

                //GameManager.audioManager.ClickSound();
                hurtbox.GetComponent<Hurtbox>().updateHurtbox(
                hurtboxData.menuingHurtbox.xOffset,
                hurtboxData.menuingHurtbox.yOffset,
                hurtboxData.menuingHurtbox.width,
                hurtboxData.menuingHurtbox.height
                );
                break;
        }

        //Exit state 
        switch (prevState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Jumpsquat:
                break;
            case PlayerState.Jump:
                break;
            case PlayerState.Landing:
                break;
            case PlayerState.Hitstun:
                break;
            //case PlayerState.Shield:
            //    break;
            case PlayerState.SideAttack:
                DisableAllHitboxes();
                break;
            case PlayerState.UpAttack:
                DisableAllHitboxes();
                break;
            case PlayerState.DownAttack:
                DisableAllHitboxes();
                break;
            case PlayerState.Menuing:
                //GameManager.audioManager.ClickSound();
                break;
        }
    }

    #region Collision Detection

    public RaycastHit2D IsGrounded()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = -(vspd - 6);

        // Calculate the positions for the left and right rays
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.min.y);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.min.y);

        // Cast rays downwards
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.down, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.down, rayLength, groundLayer);
        RaycastHit2D nullHit = new RaycastHit2D();

        // Draw the rays in the editor for debugging
        Debug.DrawRay(leftRayOrigin, Vector2.down * rayLength, Color.red);
        Debug.DrawRay(rightRayOrigin, Vector2.down * rayLength, Color.red);

        // Return the point of collision if either ray hits the ground
        if (leftHit.collider != null && rightHit.collider != null)
        {
            if (leftHit.point.y > rightHit.point.y)
            {
                return leftHit;
            }
            else if (leftHit.point.y < rightHit.point.y)
            {
                return rightHit;
            }
            else
            {
                return facingRight ? rightHit : leftHit;
            }
        }
        if (leftHit.collider != null)
        {
            return leftHit;
        }
        else if (rightHit.collider != null)
        {
            return rightHit;
        }
        return nullHit;
    }

    public RaycastHit2D IsTouchingWall()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = hspd;

        // Calculate the positions for the top and bottom rays on the left and right sides
        Vector2 topLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.max.y - rayOffset.y);
        Vector2 bottomLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.min.y + rayOffset.y);
        Vector2 centerLeftRayOrigin = new Vector2(bounds.min.x + 2, bounds.center.y);
        Vector2 topRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.max.y - rayOffset.y);
        Vector2 bottomRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.min.y + rayOffset.y);
        Vector2 centerRightRayOrigin = new Vector2(bounds.max.x - 2, bounds.center.y);


        // Cast rays to the left and right
        RaycastHit2D topLeftHit = Physics2D.Raycast(topLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D bottomLeftHit = Physics2D.Raycast(bottomLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D centerLeftHit = Physics2D.Raycast(centerLeftRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D topRightHit = Physics2D.Raycast(topRightRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D bottomRightHit = Physics2D.Raycast(bottomRightRayOrigin, Vector2.right, rayLength, groundLayer);
        RaycastHit2D centerRightHit = Physics2D.Raycast(centerRightRayOrigin, Vector2.right, rayLength, groundLayer);

        // Draw the rays in the editor for debugging
        Debug.DrawRay(topLeftRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(bottomLeftRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(centerLeftRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(topRightRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(bottomRightRayOrigin, Vector2.right * rayLength, Color.blue);
        Debug.DrawRay(centerRightRayOrigin, Vector2.right * rayLength, Color.blue);

        // Return true if any of the rays hit a wall
        if (topLeftHit.collider != null)
        {
            return topLeftHit;
        }
        else if (bottomLeftHit.collider != null)
        {
            return bottomLeftHit;
        }
        else if (centerLeftHit.collider != null)
        {
            return centerLeftHit;
        }
        else if (topRightHit.collider != null)
        {
            return topRightHit;
        }
        else if (bottomRightHit.collider != null)
        {
            return bottomRightHit;
        }
        else if (centerRightHit.collider != null)
        {
            return centerRightHit;
        }
        else
        {
            return new RaycastHit2D();
        }
    }

    public RaycastHit2D IsTouchingCeiling()
    {
        // Get the bounds of the BoxCollider2D
        Bounds bounds = boxCollider.bounds;
        rayLength = vspd;

        // Calculate the positions for the left and right rays
        Vector2 leftRayOrigin = new Vector2(bounds.min.x + rayOffset.x, bounds.max.y);
        Vector2 rightRayOrigin = new Vector2(bounds.max.x - rayOffset.x, bounds.max.y);

        // Cast rays upwards
        RaycastHit2D leftHit = Physics2D.Raycast(leftRayOrigin, Vector2.up, rayLength, groundLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rightRayOrigin, Vector2.up, rayLength, groundLayer);

        // Draw the rays in the editor for debugging
        Debug.DrawRay(leftRayOrigin, Vector2.up * rayLength, Color.green);
        Debug.DrawRay(rightRayOrigin, Vector2.up * rayLength, Color.green);

        if (leftHit.collider != null)
        {
            return leftHit;
        }
        else if (rightHit.collider != null)
        {
            return rightHit;
        }
        else
        {
            return new RaycastHit2D();
        }
    }

    public float getColliderSurface(float xValue, Collider2D targetCollider)
    {
        // Define a point above the collider at the given x value
        Vector2 rayOrigin = new Vector2(xValue, boxCollider.bounds.max.y - 1); // Adjust the y value as needed

        // Cast a ray downwards
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, groundLayer);

        // Draw the ray in the editor for debugging
        Debug.DrawRay(rayOrigin, Vector2.down * 20f, Color.grey);

        // Check if the ray hit a collider
        if (hit.collider != null)
        {
            // Return the y-coordinate of the hit point
            return hit.point.y;
        }

        // If no collider was hit, return a default value (e.g., float.MinValue)
        return 0;
    }

    public float getColliderLeftWallSurface(float yValue, Collider2D targetCollider)
    {
        // Define a point above the collider at the given x value
        Vector2 rayOrigin = new Vector2(boxCollider.bounds.max.x - 1, yValue); // Adjust the y value as needed

        // Cast a ray downwards
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left, Mathf.Infinity, groundLayer);

        // Draw the ray in the editor for debugging
        Debug.DrawRay(rayOrigin, Vector2.left * 20f, Color.grey);

        // Check if the ray hit a collider
        if (hit.collider != null)
        {
            // Return the y-coordinate of the hit point
            return hit.point.x;
        }

        // If no collider was hit, return a default value (e.g., float.MinValue)
        return 0;
    }

    public float getColliderRightWallSurface(float yValue, Collider2D targetCollider)
    {
        // Define a point above the collider at the given x value
        Vector2 rayOrigin = new Vector2(boxCollider.bounds.min.x + 1, yValue); // Adjust the y value as needed

        // Cast a ray downwards
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right, Mathf.Infinity, groundLayer);

        // Draw the ray in the editor for debugging
        Debug.DrawRay(rayOrigin, Vector2.right * 20f, Color.grey);

        // Check if the ray hit a collider
        if (hit.collider != null)
        {
            // Return the y-coordinate of the hit point
            return hit.point.x;
        }

        // If no collider was hit, return a default value (e.g., float.MinValue)
        return 0;
    }

    public float getColliderCeiling(float xValue, Collider2D targetCollider)
    {
        // Define a point above the collider at the given x value
        Vector2 rayOrigin = new Vector2(xValue, boxCollider.bounds.min.y + 1); // Adjust the y value as needed

        // Cast a ray upwards
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, Mathf.Infinity, groundLayer);

        // Draw the ray in the editor for debugging
        Debug.DrawRay(rayOrigin, Vector2.up * 20f, Color.grey);

        // Check if the ray hit a collider
        if (hit.collider != null)
        {
            // Return the y-coordinate of the hit point
            return hit.point.y;
        }

        // If no collider was hit, return a default value (e.g., float.MinValue)
        return 0;
    }

    public void SnapToSurface(RaycastHit2D hitRay)
    {
        float surfaceYVal = getColliderSurface(hitRay.point.x, hitRay.collider);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, surfaceYVal, 0);
    }

    public void SnapToWall(RaycastHit2D hitRay)
    {
        bool isLeftWall = hitRay.point.x < gameObject.transform.position.x;
        float wallXVal = isLeftWall ? getColliderLeftWallSurface(hitRay.point.y, hitRay.collider) : getColliderRightWallSurface(hitRay.point.y, hitRay.collider);

        gameObject.transform.position = new Vector3(isLeftWall ? wallXVal + boxCollider.bounds.extents.x : wallXVal - boxCollider.bounds.extents.x, gameObject.transform.position.y, 0);
    }

    public void SnapToCeiling(RaycastHit2D hitRay)
    {
        float ceilingYVal = getColliderCeiling(hitRay.point.x, hitRay.collider);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, ceilingYVal - boxCollider.bounds.size.y - 1, 0);
    }
    #endregion

    public void DetectOutOfBounds()
    {
        if (gameObject.transform.position.y < -3600 ||
            gameObject.transform.position.y > 3600 ||
            gameObject.transform.position.x < -6400||
            gameObject.transform.position.x > 6400)
        {
            gameObject.transform.position = new Vector3(0, 0, 0);
            ResetBoxCollider();
        }
    }

    public void LerpHspd(int targetHspd, int lerpval)
    {
        if (lerpDelay >= lerpval)
        {
            lerpDelay = 0;
            if (hspd < targetHspd)
            {
                hspd++;
            }
            else if (hspd > targetHspd)
            {
                hspd--;
            }
            if (hspd > -1 && hspd < 1)
            {
                hspd = 0;
            }
        }
        else
        {
            lerpDelay++;
        }

        return;
    }

    void InitWeapon()
    {
        isAlive = true;
        for (int i = 0; i < weaponData.weaponData.Count; i++)
        {
            if (weaponData.weaponData[i].weapon == weaponName)
            {
                runSpeed = weaponData.weaponData[i].runSpeed;
                jumpForce = weaponData.weaponData[i].jumpForce;
                maxHitboxes = weaponData.weaponData[i].maxHitboxes;
                frameData = weaponData.weaponData[i].frameData;
                hitboxData = weaponData.weaponData[i].hitboxData;
                hurtboxData = weaponData.weaponData[i].hurtboxData;
                impulseData = weaponData.weaponData[i].impulseData;
                impulseFrames = weaponData.weaponData[i].impulseFrames;
            }
        }
        InitHitboxes();
        InitHurtbox();
        InitEntities();
    }

    void InitHitboxes()
    {
        if (hitboxes.Count >= maxHitboxes)
        {
            return;
        }
        for (int i = 0; i < maxHitboxes; i++)
        {
            GameObject hitbox = Instantiate(hitboxReference, gameObject.transform);
            hitbox.GetComponent<Hitbox>().owner = gameObject;
            hitbox.GetComponent<Hitbox>().ignorePlayers.Clear();
            hitbox.GetComponent<Hitbox>().damage = 0;
            hitbox.GetComponent<Hitbox>().xoffset = 0;
            hitbox.GetComponent<Hitbox>().yoffset = 0;
            hitbox.GetComponent<Hitbox>().width = 0;
            hitbox.GetComponent<Hitbox>().height = 0;
            hitbox.GetComponent<Hitbox>().xKnockback = 0;
            hitbox.GetComponent<Hitbox>().yKnockback = 0;
            hitbox.GetComponent<Hitbox>().hitstun = 0;
            hitbox.SetActive(false);
            hitboxes.Add(hitbox);
        }

    }
    void InitHurtbox()
    {
        if (hurtbox != null)
        {
            return;
        }
        hurtbox = Instantiate(hurtboxReference, gameObject.transform);
        hurtbox.GetComponent<Hurtbox>().owner = gameObject;
        hurtbox.GetComponent<Hurtbox>().hurtboxActive = true;
        hurtbox.GetComponent<Hurtbox>().xoffset = 0;
        hurtbox.GetComponent<Hurtbox>().yoffset = 0;
        hurtbox.GetComponent<Hurtbox>().width = 0;
        hurtbox.GetComponent<Hurtbox>().height = 0;
        hurtbox.SetActive(true);

    }
    void InitEntities()
    {


        //initialize entity dictionary
        if (entities.Count < 1)
        {
            //TODO: Replace the entity list indicies with the proper entities once they are added
            entities.Add("jump_dust", Instantiate(entityList[0]));
            entities.Add("dash_dust", Instantiate(entityList[1]));
            entities.Add("hit_spark", Instantiate(entityList[2]));
            entities.Add("block_spark", Instantiate(entityList[3]));

            foreach (KeyValuePair<string, GameObject> entity in entities)
            {
                DontDestroyOnLoad(entity.Value);
            }




            entities["jump_dust"].GetComponent<Entity>().owner = gameObject;
            entities["jump_dust"].SetActive(false);

            entities["dash_dust"].GetComponent<Entity>().owner = gameObject;
            entities["dash_dust"].SetActive(false);


            entities["hit_spark"].GetComponent<Entity>().owner = gameObject;
            entities["hit_spark"].SetActive(false);

            entities["block_spark"].GetComponent<Entity>().owner = gameObject;
            entities["block_spark"].SetActive(false);


        }



    }

    void DisableAllHitboxes()
    {
        foreach (GameObject hitbox in hitboxes)
        {
            hitbox.SetActive(false);
            hitbox.GetComponent<Hitbox>().ignorePlayers.Clear();
        }

    }

    //public void TakeDamage(GameObject hitPlayer, int damage, int xKnockback, int yKnockback, int hitstun, Vector2 hitsparkSpawnPoint, Texture targetTexture)
    //{

    //    //If this player is block and facing the right direction
    //    if (state == PlayerState.Shield &&
    //        ((hitPlayer.transform.position.x > gameObject.transform.position.x && facingRight) ||
    //        (hitPlayer.transform.position.x < gameObject.transform.position.x && !facingRight)))
    //    {

    //        GameManager.audioManager.PlayShieldHitSound();
    //        entities["block_spark"].SetActive(true);
    //        entities["block_spark"].GetComponent<Entity>().InitEntity(
    //            (int)(hitsparkSpawnPoint.x - transform.position.x),
    //            (int)(hitsparkSpawnPoint.y - transform.position.y));
    //        hspd = xKnockback / 1.5f;
    //    }
    //    else
    //    {
    //        entities["hit_spark"].SetActive(true);
    //        entities["hit_spark"].GetComponent<Entity>().InitEntity(
    //            (int)(hitsparkSpawnPoint.x - transform.position.x),
    //            (int)(hitsparkSpawnPoint.y - transform.position.y),
    //            targetTexture);
    //        //Play Damage Sound
    //        GameManager.audioManager.PlayDamageSound();

    //        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Scene_MainMenu" &&
    //            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "results")
    //        {
    //            health -= damage;
    //        }
    //        hspd = xKnockback;
    //        vspd = yKnockback;
    //        hitstunVal = hitstun;
    //        SetState(PlayerState.Hitstun);
    //        if (health <= 0)
    //        {
    //            Die();
    //        }
    //    }

    //    //Debug.Log("Player Health: " + health);


    //}

    //private void CycleWeapon()
    //{
    //    if (otherWeaponAnimControllers.Count == 0) return;

    //    currentAnimControllerIndex++;
    //    if (currentAnimControllerIndex > otherWeaponAnimControllers.Count)
    //    {
    //        currentAnimControllerIndex = 0;
    //    }

    //    if (currentAnimControllerIndex == 0)
    //    {
    //        animator.runtimeAnimatorController = baseAnimController;
    //        weaponName = "sword";
    //        animator.SetInteger(name: "player_state", (int)PlayerState.Menuing);
    //    }
    //    else
    //    {
    //        animator.runtimeAnimatorController = otherWeaponAnimControllers[currentAnimControllerIndex - 1];
    //        weaponName = otherWeaponAnimControllers[currentAnimControllerIndex - 1].name;
    //    }
    //    GameManager.audioManager.ClickSound();
    //    InitWeapon();

    //}

    //private void CycleColor()
    //{
    //    currentColorIndex++;
    //    if (currentColorIndex >= GameManager.Instance.colorPalettes.Count)
    //    {
    //        currentColorIndex = 0;
    //    }
    //    while (!GameManager.Instance.unusedPalettes.Contains(GameManager.Instance.colorPalettes[currentColorIndex]))
    //    {
    //        currentColorIndex++;
    //        if (currentColorIndex >= GameManager.Instance.colorPalettes.Count)
    //        {
    //            currentColorIndex = 0;
    //        }
    //    }

    //    GameManager.audioManager.ClickSound();
    //    gameObject.GetComponent<SpriteRenderer>().material.SetTexture("_PaletteTex", GameManager.Instance.colorPalettes[currentColorIndex]);
    //    GameManager.Instance.UpdateUnusedPalettes();

    //}

    //public void Die()
    //{
    //    //disable player
    //    //gameObject.SetActive(false);
    //    gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", .3f);
    //    //gameObject.GetComponent<PlayerInput>().enabled = false;
    //    isAlive = false;
    //    GameManager.audioManager.PlayDeathSound();
    //    //deathParticles.Play();
    //    //this is where we would do death burst animations


    //}

    //public void Respawn()
    //{
    //    if (health <= 0)
    //    {
    //        stockCount--;
    //    }
    //    if (stockCount <= 0)
    //    {
    //        //Debug.Log("DEAD BOY ALERT DEADYDEAD BOY OVER HERE");
    //        //return;
    //    }
    //    else
    //    {
    //        gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Alpha", 1f);
    //        isAlive = true;
    //        health = 8;

    //    }
    //    //reset player
    //    //gameObject.SetActive(true);

    //    //this respawn point should be set based on the map
    //    //gameObject.transform.position = Vector3.zero;
    //    hspd = 0;
    //    vspd = 0;
    //    invincibilityCounter = invincibilityTime;
    //    SetState(PlayerState.Idle);
    //}

    public void ResetBoxCollider()
    {
        boxCollider.enabled = false;
        boxCollider.enabled = true; //this resets the collider to fix a bug where the player wouldn't spawn in the correct position thus being pushed a bit to the right
    }

    //Used in the animation manager to play whiff sounds on specific frames.
    public void WhiffSound()
    {
        GameManager.audioManager.PlaySwoosh();
    }

    //public void GunWhiffSound()
    //{
    //    GameManager.audioManager.PlayGunSound();
    //}

    public void JumpSound()
    {
        GameManager.audioManager.PlayJumpSound();
    }

    public void SwapCharacter()
    {
        if (characterSwapFlag == false)
        {

            characterId = characterId == 0 ? 1 : 0;
            characterSwapFlag = true;
        }
    }
}
