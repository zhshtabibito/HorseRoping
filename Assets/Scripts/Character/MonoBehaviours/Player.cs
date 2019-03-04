using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;

public class Player : MonoBehaviour
{
    public Sprite image;
    public GameObject aimerObj;
    public GameObject ropeObj;
    public Aimer aimer;
    public Rope rope;
    public Transform throwPosition;
    public enum PlayerStatus : int
    {
        Normal = 0,
        Charging,
        Roping,
        Dizzy,
        Throwing,
    }

    [Header("General Setting")]
    public int playerID;
    public float score = 0;
    //public int state = 0;
    public PlayerStatus playerStatus = PlayerStatus.Normal;
    /// <summary>
    /// current maxSpeed
    /// </summary>
    [SerializeField] private float curSpeed;
    [SerializeField] private float maxSpeed = 3;
    public float lenRope;

    #region Timer
    [Header("Timer Setting")]
    public float dashTimer = 0f;
    public bool canDash = true;

    public float pullTimer = 0f;
    public bool canPull = true;

    public float dizzyTimer = 0f;
    #endregion

    [Header("Dash Setting")]
    /// <summary>
    /// maxSpeed attenuation
    /// </summary>
    public float dashSpeed;
    public float dashAttenuation = 1;
    public float dashCD = 1f;
    [Header("Pull Setting")]
    public float pullSpeed;
    public float pullAttenuation = 1;
    public float pullCD = 1f;
    [Header("Buff&Debuff Setting")]
    public float dizzyTime = 1f;
    public bool isDizzy;
    [Header("ropeObj Setting")]
    [Tooltip("绳索飞行时间 = 比率 * 蓄力时间")]
    public float chargeRatio = 1f;
    [SerializeField] private bool canRope = true;
    [SerializeField] private float cdRope = 1f;

    protected Animator m_Animator;
    protected PlayerAudio m_PlayerAudio;
    protected Rigidbody2D m_Rigidbody2D;
    protected readonly int m_HashSpeedXPara = Animator.StringToHash("Speed_X");
    protected readonly int m_HashSpeedYPara = Animator.StringToHash("Speed_Y");
    protected readonly int m_HashDirectionPara = Animator.StringToHash("Direction");
    protected readonly int m_HashDashPara = Animator.StringToHash("Dash");
    protected readonly int m_HashPullPara = Animator.StringToHash("Pull");
    protected readonly int m_HashDizzyPara = Animator.StringToHash("Dizzy");
    protected readonly int m_HashRespawnPara = Animator.StringToHash("Respawn");

    [SerializeField] protected Vector2 m_MoveVector;
    protected CharacterController2D m_CharacterController2D;
    public PlayerInput playerInput;

    // Start is called before the first frame update
    void Awake()
    {
        lenRope = 1.8f;
        curSpeed = maxSpeed;
        aimer = aimerObj.GetComponent<Aimer>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
        playerInput = GetComponent<PlayerInput>();
        m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat(m_HashDirectionPara, 1);
        m_PlayerAudio = GetComponent<PlayerAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.Pause.Down)
        {
            GameController.Instance.PauseGame();
        }

        #region Player
        curSpeed = curSpeed - dashAttenuation * Time.deltaTime > maxSpeed ? curSpeed - dashAttenuation * Time.deltaTime : maxSpeed;

        Vector2 v = Vector2.zero;
        if (playerStatus != PlayerStatus.Dizzy)
        {
            float mHorizontal, mVertical;
            mHorizontal = playerInput.PlayerHorizontal.Value;
            mVertical = playerInput.PlayerVertical.Value;
            //Debug.Log(playerInput.PlayerHorizontal.Value + "," + playerInput.PlayerVertical.Value);
            m_MoveVector = new Vector2(mHorizontal, mVertical).normalized * curSpeed;

            if (playerInput.ThrowRope.Held &&
               (playerStatus == PlayerStatus.Normal || playerStatus == PlayerStatus.Charging))
            {
                if (playerStatus == PlayerStatus.Normal)
                {
                    playerStatus = PlayerStatus.Charging;
                    curSpeed = maxSpeed * 0.5f; // slow down while charging
                    aimer.StartCharging();
                }
            }
            if (playerInput.ThrowRope.Up && playerStatus == PlayerStatus.Charging && canRope)
            {
                playerStatus = PlayerStatus.Throwing;
                curSpeed = maxSpeed;
                aimer.EndCharging();
                ropeObj.GetComponent<Rope>().TryThrowRope(aimer.curChargeTime * chargeRatio, aimer.transform.position - throwPosition.position);
            }

            // skill dash and pull
            if ((playerInput.Dash.Down && canDash))
            {
                canDash = false;
                m_Animator.SetBool(m_HashDashPara, true);
                curSpeed *= dashSpeed;
            }
            if ((playerInput.Pull.Down && canPull && playerStatus == PlayerStatus.Roping))
            {
                canPull = false;
                GameController.Instance.horse.SetPulled(m_Rigidbody2D.position - GameController.Instance.horse.GetPosition(), pullSpeed);
                m_Animator.SetBool(m_HashPullPara, true);
            }

        }

        // horse too far while roping
        if (playerStatus == PlayerStatus.Roping)
        {
            if ((GameController.Instance.horse.transform.position - transform.position).magnitude > rope.maxLength)
            {
                BreakRope();
                //StartCoroutine("RopeCD");
            }
            //else if ((GameController.Instance.horse.transform.position - transform.position).magnitude > 0.6f * (aimer.R + lenRope))
            //{
            //    // warning
            //    Debug.Log("Warning!");
            //    GetComponentInChildren<LineRenderer>().GetComponent<Animator>().enabled = true;
            //    GetComponentInChildren<LineRenderer>().GetComponent<SpriteRenderer>().enabled = true;
            //}
            //else
            //{
            //    GetComponentInChildren<LineRenderer>().GetComponent<Animator>().enabled = false;
            //    GetComponentInChildren<LineRenderer>().GetComponent<SpriteRenderer>().enabled = false;
            //}

        }
        m_Animator.SetFloat(m_HashSpeedXPara, m_MoveVector.x);
        m_Animator.SetFloat(m_HashSpeedYPara, m_MoveVector.y);
        #endregion

        #region Rope
        if (!ropeObj.GetComponent<Rope>().isThrowing && playerStatus == PlayerStatus.Throwing)
        {
            playerStatus = PlayerStatus.Normal;
        }
        if (ropeObj.GetComponent<Rope>().isCatching)
        {
            playerStatus = PlayerStatus.Roping;
        }
        #endregion

        TimerTick();
    }

    public void TimerTick()
    {
        if (!canDash)
        {
            if (dashTimer < dashCD)
            {
                canDash = false;
                dashTimer += Time.deltaTime;
            }
            else
            {
                dashTimer = 0f;
                canDash = true;
            }
        }
        if (!canPull)
        {
            if (pullTimer < pullCD)
            {
                canPull = false;
                pullTimer += Time.deltaTime;
            }
            else
            {
                pullTimer = 0f;
                canPull = true;
            }
        }
        if (isDizzy)
        {
            if (dizzyTimer < dizzyTime)
            {
                isDizzy = true;
                dizzyTimer += Time.deltaTime;
            }
            else
            {
                dizzyTimer = 0;
                isDizzy = false;
                m_Animator.SetBool(m_HashDizzyPara, false);
                playerStatus = PlayerStatus.Normal;
                playerInput.GainControl();
            }
        }
    }

    public void ResetCD()
    {
        dashTimer = 0f;
        pullTimer = 0f;
    }

    public void CleanAllBuff()
    {
        ResetCD();
    }

    private void FixedUpdate()
    {
        float dir = (m_MoveVector.x < 0) ? (2f / 3f) :
                (m_MoveVector.x > 0) ? 1 :
                (m_MoveVector.y > 0) ? 0 :
                (m_MoveVector.y < 0) ? (1f / 3f) : -1;
        if (dir >= 0)
            m_Animator.SetFloat(m_HashDirectionPara, dir);
        m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
    }


    public void BeDizzy()
    {
        //aimer.HideAimer();
        if (isDizzy)
        {
            dizzyTimer -= dizzyTime;
        }
        else
        {
            isDizzy = true;
            playerStatus = PlayerStatus.Dizzy;
            m_Animator.SetBool(m_HashDizzyPara, true);
            playerInput.ReleaseControl();
            m_PlayerAudio.PlayDizzy();
            BreakRope();
        }
    }

    //Call by event
    public void HitByRope(GameObject gameObject)
    {
        //Debug.Log(gameObject.GetComponent<Player>().playerID + "," + this.playerID);
        if (gameObject.GetComponent<Player>().playerID == this.playerID)
            return;
        if (playerStatus == PlayerStatus.Roping)
        {
            GameController.Instance.horse.TryStruggle();
        }
        BeDizzy();
    }

    public void Respawn(bool cleanScore)
    {
        if (cleanScore)
            score = 0;
        curSpeed = maxSpeed;
        ResetCD();
        CleanAllBuff();
        m_Animator.SetTrigger(m_HashRespawnPara);
        m_MoveVector = Vector2.zero;
        playerStatus = PlayerStatus.Normal;
        rope.ResetRope();
    }
    public void Respawn(bool cleanScore, Vector2 resetPos)
    {
        if (cleanScore)
            score = 0;
        ResetCD();
        CleanAllBuff();
        m_Animator.SetTrigger(m_HashRespawnPara);
        m_MoveVector = Vector2.zero;
        playerStatus = PlayerStatus.Normal;
        this.gameObject.transform.position = resetPos;
        aimer.transform.position = resetPos;
        rope.ResetRope();
        aimer.ResetAimer();
    }

    public void BreakRope()
    {
        playerStatus = PlayerStatus.Normal;
        GetComponentInChildren<Rope>().BreakLine();
        if (GameController.Instance.horse.state == playerID)
            GameController.Instance.horse.TryStruggle();
    }
}
