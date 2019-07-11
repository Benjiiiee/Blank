using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeadHunterNME : MonoBehaviour
{
    #region Variables

    //Components
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public Transform tr;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform targetPlayerTr;
    [HideInInspector] public SoundRelay soundRelay;

    //Pathing
    [Header("Drag the Path script here, no path = idle")]
    public PathObject pathObject;
    [HideInInspector] public float distanceToTurn = 1f;
    [HideInInspector] public Transform[] path;
    [HideInInspector] public int pathCpt = 0;
    [HideInInspector] public Vector3 searchDestination;
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;

    //States
    [HideInInspector] public HeadHunterIdle idle;
    [HideInInspector] public HeadHunterPatrol patrol;
    [HideInInspector] public HeadHunterChase chase;
    [HideInInspector] public HeadHunterSearch search;
    [HideInInspector] public HeadHunterStunned stunned;
    [HideInInspector] public HeadHunterSearchInPlace searchInPlace;
    [HideInInspector] public HeadHunterDoor door;
    [HideInInspector] public HeadHunterTurnInPlace turn;

    [HideInInspector] public Transform langueTr;
    [Header("Illusion de trick?")]
    public bool isIllusion = false;
    public bool debug = false;

    //Detection
    [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
    private LayerMask playerMask;
    private LayerMask obstaclesMask;
    [HideInInspector] [Range(2.0f, 20.0f)] public float fovRadius = 5.0f;
    [HideInInspector] [Range(0.0f, 360.0f)] public float viewAngle = 70.0f;
    [HideInInspector] public bool playerDetected = false;
    [HideInInspector] public float killDistance = 3f;
    [HideInInspector] public float turnThreshold = 80f;
    [HideInInspector] RaycastHit hit;
    private Collider[] targetsInRadius;
    private Transform target;

    //Fxs
    public List<Material> headHunterMats = new List<Material>();

    #endregion

    #region Fonctions Monobehaviour

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<StateMachine>();
        tr = transform;
        animator = GetComponent<Animator>();
        playerMask = LayerMask.GetMask("Player");
        obstaclesMask = LayerMask.GetMask("Obstacles");
        soundRelay = GetComponent<SoundRelay>();
        startPos = tr.position;
        startRot = tr.rotation;
        langueTr = langueTr.transform;

        idle = new HeadHunterIdle(this);
        patrol = new HeadHunterPatrol(this);
        chase = new HeadHunterChase(this);
        search = new HeadHunterSearch(this);
        stunned = new HeadHunterStunned(this);
        searchInPlace = new HeadHunterSearchInPlace(this);
        door = new HeadHunterDoor(this);
        turn = new HeadHunterTurnInPlace(this);

        if (pathObject != null)
            GetPath();

        stateMachine.currentState = idle;

        animator.applyRootMotion = false;
    }

    private void Update()
    {
        //Update la state machine
        if (Time.timeScale != 0f)
            stateMachine.CurrentStateUpdate();

        if (debug)
            Debug.Log(stateMachine.currentState);
    }

    #endregion

    #region  Pathing

    ///<summary> Update le path à la prochaine destination, loop du dernier au premier point. </summary>
    public void UpdatePath()
    {
        pathCpt++;
        if (pathCpt == path.Length)
            pathCpt = 0;
    }

    ///<summary> Regarde la destination la plus proche pour retourner en patrol. </summary>
    public int CheckClosestDestination()
    {
        int closestDistance = 0;
        for (int i = 0; i < path.Length; i++)
        {
            if (Vector3.Distance(tr.position, path[i].position) < Vector3.Distance(tr.position, path[closestDistance].position))
                closestDistance = i;
        }
        return closestDistance;
    }

    ///<summary> Convertis le Path tool en destinations pour le navMesh. </summary>
    public void GetPath()
    {
        path = new Transform[pathObject.wayPoints.Count];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = pathObject.wayPoints[i].transform;
        }
        if(agent.enabled)
            agent.SetDestination(path[CheckClosestDestination()].position);
    }

    ///<summary> Ajuste la vitesse de l'agent en fonction de l'animation </summary>
    
    private void OnAnimatorMove()
    {
        if (agent.enabled)
        {
            if (Time.deltaTime != 0f) // Bugfix when unpausing de game
                agent.speed = animator.deltaPosition.magnitude / Time.deltaTime;
            else
                agent.speed = animator.deltaPosition.magnitude;
        }
        else
            tr.position = animator.rootPosition;

        tr.rotation = animator.rootRotation;
    }

    public void ResetPos()
    {
        tr.position = startPos;
    }

    #endregion

    #region  Detection

    ///<summary> Detection du player </summary>
    public void LookForPlayer()
    {
        if (GameManager.instance.currentAvatar != null)
        {
            if (GameManager.instance.currentAvatar.stateMachine.currentState != GameManager.instance.currentAvatar.stateMachine.death)
            {
                visibleTargets.Clear();
                targetsInRadius = Physics.OverlapSphere(tr.position, fovRadius, playerMask);
                for (int i = 0; i < targetsInRadius.Length; i++)
                {
                    target = targetsInRadius[i].transform;
                    if (Vector3.Angle(tr.forward, (target.position - tr.position).normalized) < viewAngle / 2)
                    {
                        Debug.DrawLine(tr.position + Vector3.up * 1.5f, target.position + Vector3.up * (GameManager.instance.currentAvatar.controller.height - 0.5f), Color.red);
                        if (Physics.Linecast(tr.position + Vector3.up * 1.5f, target.position + Vector3.up * (GameManager.instance.currentAvatar.controller.height - 0.5f), out hit))
                        {
                            if (hit.collider.gameObject.CompareTag("Player") && GameManager.instance.currentAvatar.tr.position.y - tr.position.y <= 2.5f)
                            {
                                visibleTargets.Add(target);
                                targetPlayerTr = target;

                                playerDetected = true;
                                CancelInvoke();
                                Invoke("PlayerBecomesUndetected", 2.0f);
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary> Détecte Bank s'il s'approche trop </summary>
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.instance.currentAvatar.stateMachine.currentState != GameManager.instance.currentAvatar.stateMachine.death)
        {
            Vector3 dirToTarget = (other.transform.position - tr.position).normalized;
            float dstToTarget = Vector3.Distance(tr.position, other.transform.position);
            if (Physics.Linecast(tr.position + Vector3.up * 1.5f, target.position + Vector3.up * (GameManager.instance.currentAvatar.controller.height - 0.5f), out hit))
            {
                if (hit.collider.gameObject.CompareTag("Player") && GameManager.instance.currentAvatar.tr.position.y - tr.position.y <= 2.5f)
                {
                    visibleTargets.Add(other.transform);
                    targetPlayerTr = other.transform;

                    playerDetected = true;
                    CancelInvoke();
                    Invoke("PlayerBecomesUndetected", 2.0f);
                }
            }
        }
    }

    public void PlayerBecomesUndetected()
    {
        playerDetected = false;
    }

    #endregion

    #region  FX

    public void PlayFx(AnimationEvent animation)
    {
        GameManager.instance.fxPool.GetObjectAutoReturn(animation.stringParameter, tr.position, tr.eulerAngles, animation.floatParameter);
    }

    public void PlayFx(string name, float time)
    {
        GameManager.instance.fxPool.GetObjectAutoReturn(name, tr.position, tr.eulerAngles, time);
    }

    public void PlayFxLangue(AnimationEvent animation)
    {
        GameManager.instance.fxPool.GetObjectAutoReturn(animation.stringParameter, langueTr.position, tr.eulerAngles, animation.floatParameter);
    }

    public void PlayFootStepSound()
    {
        soundRelay.PlayAudioClip(1);
    }
    #endregion

}
