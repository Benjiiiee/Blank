
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    #region VARIABLES DEPLACEMENT

    [HideInInspector] public float gravityUpward = 60f;
    [HideInInspector] public float gravityDownward = 75f;
    [HideInInspector] public float gravityCap = -20f;
    [HideInInspector] public float drag = 1f;

    [HideInInspector] public float velocityY, velocityZ;
    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public float vitesse = 30f;
    [HideInInspector] public float impulsionY = 4.5f, impulsionZ = 4.5f, impulsionLongJump = 2f;

    [HideInInspector] public bool isCrouching = false;
    [HideInInspector] public bool canCrouch = true;
    [HideInInspector] public bool canDecrouch = true;

    [HideInInspector] public bool inputDisabled;
    [HideInInspector] public int zapState;
    public bool debug = false;

    //Turns
    [HideInInspector] public float animMouvementSmoothTime = 7f;
    [HideInInspector] public float codeMouvementSmoothTime = 15f;
    [HideInInspector] public float targetRotation;
    [HideInInspector] public Vector3 initialDirection;
    private float turnAngle;
    private float turnSmoothVelocity;
    private float currentRotation;

    //Grounding
    [HideInInspector] public float rayMaxDist = 0.1f;
    [HideInInspector] public float rayCastThreshold = 0.06f;
    [HideInInspector] public bool onGround;
    [HideInInspector] public float rayHitDistance;
    [HideInInspector] public RaycastHit hit;
    [HideInInspector] public RaycastHit ledgeHit;
    private Vector3[] vecteurs = new Vector3[4];
    [HideInInspector] public float vecteursRadius = 0.65f;

    //LedgeGrab
    private RaycastHit ledgeGrabHit;
    [HideInInspector] public float ledgeGrabOffset = 1.5f;
    [HideInInspector] public float ledgeGrabDistance = 1f;
    [HideInInspector] public bool isClimbing = false;

    #endregion

    #region COMPONENTS
    [HideInInspector] public CharacterController controller;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform tr;
    [HideInInspector] public Collider col;
    [HideInInspector] public CharacterState stateMachine;
    [HideInInspector] public Transform currentCameraTr;
    [HideInInspector] public AudioListener audioListener;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public PathFollow currentPlatform;
    #endregion

    #region FX

    [HideInInspector] public Renderer blankHead;
    [HideInInspector] public GameObject falseBlankHead;
    [HideInInspector] public Rigidbody falseBlankHeadRb;
    [HideInInspector] public float expediteHeadForce = 10f;

    [HideInInspector] public GameObject blot;
    private Blot currentBlot;

    [HideInInspector] public Transform leftFoot;
    [HideInInspector] public Transform rightFoot;

    [HideInInspector] public bool vision = true;
    [HideInInspector] public bool canVision = true;
    [HideInInspector] public Transform visionPos;

    #endregion

    #region TrainEvent
    public HeadlessNPC headlessInFront;
    public HeadlessNPC headlessInBack;
    public bool isFirstInLine = false;
    #endregion

    #region Fonctions MonoBehaviour

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Init();
        inputDisabled = false;
        animator.applyRootMotion = true;
        blankHead = blankHead.GetComponent<Renderer>();
    }

    public void Init()
    {
        //initialisation des variables
        tr = transform;
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        audioListener = GetComponentInChildren<AudioListener>();
        audioSource = GetComponentInChildren<AudioSource>();
        stateMachine = GetComponent<CharacterState>();
        currentCameraTr = Camera.main.transform; //GameManager.instance.cameraManager.thirdPersonCam.transform;

        leftFoot = leftFoot.transform;
        rightFoot = rightFoot.transform;

        if (!GameManager.instance.spcLvlMan.hasHead)
            blankHead.enabled = false;
    }

    void Update()
    //Check if grounded
    {
        if (debug)
            Debug.Log(stateMachine.currentState);

        rayHitDistance = 3f; //Reset du rayHitDistance a tous les frames

        //Grounding
        if (!isClimbing)
        {
            CheckGround();
            onGround = (rayHitDistance <= rayCastThreshold);
            if (!onGround)
                CheckLedges();
        }

        if (currentBlot == null)
        {
            currentBlot = Instantiate(blot).GetComponent<Blot>();
        }

        //FX changement de pesrpectives
        if (canVision && InputsManager.instance.triggerLeftDown && GameManager.instance.spcLvlMan.perspSubsUnlocked || canVision && InputsManager.instance.triggerRightDown && GameManager.instance.spcLvlMan.perspAddUnlocked)
        {
            if(Time.timeScale != 0f)
                visionPos = PlayFxVision("FX_Blank_Vision", tr.position, currentCameraTr.eulerAngles, 1f);
        }

        if (vision && visionPos != null)
        {
            visionPos.position = tr.position + Vector3.up * 2f;
            visionPos.eulerAngles = currentCameraTr.eulerAngles;
        }

        //Plateformes mobile
        if (currentPlatform)
        {
            controller.Move(currentPlatform.GetTargetDirection());
        }
    }

    #endregion

    #region Grounding

    /// <summary> Raycast pour savoir si l'avatar touche au sol. </summary>
    public void CheckGround()
    {
        if (Physics.Raycast(tr.position, Vector3.down, out hit, rayMaxDist))
        {
            if (!hit.collider.isTrigger)
            {
                rayHitDistance = hit.distance;
                if (hit.distance <= rayCastThreshold)
                {
                    PathFollow platform = hit.transform.GetComponentInParent<PathFollow>();
                    if (platform)
                    {
                        currentPlatform = platform;
                    }
                    else
                        currentPlatform = null;
                }
            }
        }
    }

    /// <summary> S'il n'est pas au sol, raycast les alentours et translate l'avatar. </summary>
    public void CheckLedges()
    {
        vecteurs[0] = tr.forward;
        vecteurs[1] = -tr.forward;
        vecteurs[2] = tr.right;
        vecteurs[3] = -tr.right;

        bool[] rays = new bool[4];

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(tr.position + vecteurs[i] * vecteursRadius + tr.up * 0.4f, Vector3.down, out ledgeHit, 0.46f))
            {
                if (!ledgeHit.collider.isTrigger)
                    controller.Move((tr.position - ledgeHit.point).normalized * 2f * Time.deltaTime);
                return;
            }
        }
    }

    #endregion

    #region Mouvements

    /// <summary> Anim ou code mouvement fonction selon le grounding </summary>
    public void MoveCharacter(Vector3 input)
    {

        if (onGround)
            AnimDrivenMouvement(input);
        else
            CodeDrivenMouvement(input);

        //Stock la rotation a la fin du frame
        currentRotation = targetRotation;
    }

    /// <summary> Bouge l'avatar en root motion </summary>
    public void AnimDrivenMouvement(Vector3 input)
    {
        if (input != Vector3.zero)
        {
            // Get direction en fonction des inputs et de la cam
            targetRotation = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + currentCameraTr.eulerAngles.y;
            turnAngle = Mathf.DeltaAngle(currentRotation, targetRotation);

            // Compare la nouvelle direction avec la vieille et applique la bonne anim
            animator.SetFloat("directionAngle", turnAngle);

            //Call anim de turn
            if (turnAngle < -120 && stateMachine.currentState == stateMachine.locomotion && stateMachine.locomotion.timeInState > 0.1f)
            {
                stateMachine.turn.turnTime = 0.6f;
                stateMachine.ChangeState(stateMachine.turn);
                return;
            }
            if (turnAngle > 120 && stateMachine.currentState == stateMachine.locomotion && stateMachine.locomotion.timeInState > 0.1f)
            {
                stateMachine.turn.turnTime = 0.6f;
                stateMachine.ChangeState(stateMachine.turn);
                return;
            }

            //Corrige la rotation de l'avatar en fonction de la cam
            tr.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, animMouvementSmoothTime);
        }
    }

    /// <summary> Bouge l'avatar en code driven </summary>
    public void CodeDrivenMouvement(Vector3 input)
    {
        //Gravite
        if (velocityY >= 0)
            velocityY += -Mathf.Abs(gravityUpward);
        else
            velocityY += -Mathf.Abs(gravityDownward);

        if (velocityZ <= 0f)
            velocityZ = 0f;

        //Cap velocite
        if (velocityY <= gravityCap)
            velocityY = gravityCap;

        //Rotation selon les mouvements
        if (input != Vector3.zero)
            targetRotation = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + currentCameraTr.eulerAngles.y;

        if (velocityZ > 0f) // S'il est en long jump
        {
            if (Vector3.Angle(initialDirection, input) < 100f)
                tr.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, codeMouvementSmoothTime * 2f);
        }
        else
            tr.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, codeMouvementSmoothTime);

        velocity = tr.forward * velocityZ + tr.up * velocityY;
        velocity += new Vector3(currentCameraTr.TransformDirection(input).x, 0f, currentCameraTr.TransformDirection(input).z).normalized * vitesse;

        //Implementation du mouvement
        controller.Move(velocity * Time.deltaTime);
    }

    #endregion

    #region Ledge Grab

    /// <summary> Raycast pour detecter un ledge </summary>
    public void CheckLedgeGrab()
    {
        if (Physics.Raycast(tr.position + Vector3.up * ledgeGrabOffset + tr.forward * 0.3f, tr.forward, out ledgeGrabHit, ledgeGrabDistance))
        {
            if (ledgeGrabHit.collider.CompareTag("LedgeGrab"))
            {
                ExecuteLedgeGrab();
            }
        }
    }

    /// <summary> Fait le Ledge Grab si le CheckLedgeGrab revient true </summary>
    private void ExecuteLedgeGrab()
    {
        //Bloque les controles et aligne l'avatar a la bonne position
        controller.enabled = false;
        isClimbing = true;
        velocityY = 0f;
        velocityZ = 0f;

        LedgeGrab ledge = ledgeGrabHit.collider.GetComponentInParent<LedgeGrab>();

        Transform hitTransform = ledgeGrabHit.collider.transform;
        tr.position = ledgeGrabHit.point;
        tr.forward = -hitTransform.forward;
        
        //Enum pour appeler la bonne animation
        if (ledge.type == LedgeGrab.LedgeGrabType.mur)
        {
            tr.position = new Vector3(tr.position.x, hitTransform.position.y - 2.3f, tr.position.z) + hitTransform.forward * 0.95f;
            animator.SetTrigger("ledgeGrabTall");
        }
        else
        {
            tr.position = new Vector3(tr.position.x, hitTransform.position.y - 1.75f, tr.position.z) + hitTransform.forward * 0.95f;
            animator.SetTrigger("ledgeGrabShort");
        }

        stateMachine.ChangeState(stateMachine.climb);
    }

    #endregion

    #region FX
    public void PlayFxCode(string name, Vector3 position, Vector3 rotation, float timeToReturn)
    {
        GameManager.instance.fxPool.GetObjectAutoReturn(name, position, rotation, timeToReturn);
    }

    public void PlayFx(AnimationEvent animation)
    {
        GameManager.instance.fxPool.GetObjectAutoReturn(animation.stringParameter, tr.position, tr.eulerAngles, animation.floatParameter);
    }

    public void PlayFxRightFoot(AnimationEvent animation)
    {
        GameManager.instance.fxPool.GetObjectAutoReturn(animation.stringParameter, rightFoot.position, animation.floatParameter);
    }

    public void PlayFxLeftFoot(AnimationEvent animation)
    {
        GameManager.instance.fxPool.GetObjectAutoReturn(animation.stringParameter, leftFoot.position, animation.floatParameter);
    }

    public Transform PlayFxVision(string name, Vector3 position, Vector3 rotation, float timeToReturn)
    {
        Transform temp = GameManager.instance.fxPool.GetObjectAutoReturn(name, position, rotation, timeToReturn).transform;
        canVision = false;
        vision = true;
        Invoke("ResetVision", 1f);
        return temp;
    }

    public void ResetVision()
    {
        vision = false;
        canVision = true;
    }

    public void InstantiateBlot()
    {
        Instantiate(blot);
    }

    public void ExpediteHead(Transform bouncingBlade)
    {
        blankHead.enabled = false;
        falseBlankHeadRb = Instantiate(falseBlankHead, tr.position, tr.rotation).GetComponent<Rigidbody>();
        falseBlankHeadRb.AddForce(((falseBlankHeadRb.transform.position - bouncingBlade.position).normalized + Vector3.up) * expediteHeadForce);
        falseBlankHeadRb.AddTorque(Vector3.forward);
        Invoke("DeleteHead", 3f);
    }

    public void DeleteHead()
    {
        Destroy(falseBlankHeadRb.gameObject);
    }

    #endregion

    #region Inputs

    public void DisableInput()
    {
        InputsManager.instance.cameraInputsAreDisabled = true;
        stateMachine.ChangeState(stateMachine.disabled);
    }

    public void EnableInput()
    {
        InputsManager.instance.cameraInputsAreDisabled = false;
        stateMachine.ChangeState(stateMachine.idle);
    }

    public void ResetCrouch()
    {
        canCrouch = true;
    }

    #endregion

    #region Train event

    public void NextPosInLine()
    {
        if (isFirstInLine)
        {
            GameManager.instance.LoadLevel1();
            isFirstInLine = false;
        }
        else
        {
            headlessInBack.Invoke("NextPosInLine", 0.5f); //Random.Range marche pas
            animator.SetBool("isWalking", true);
            Invoke("StopWalk", 1.025f);
        }
    }

    public void StopWalk()
    {
        animator.SetBool("isWalking", false);
    }

    public void ResetCanInput()
    {
        stateMachine.train.canInput = true;
    }

    #endregion
}
