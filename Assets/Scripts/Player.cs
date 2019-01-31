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

    public int playerID;
    public int score = 0;
    //public int state = 0;
    public PlayerStatus playerStatus = PlayerStatus.Normal;

    #region InputSetting
    [Header("Input Setting")]
    public string moveHorizontal = "Horizontal_P1L";
    public string moveVertical = "Vertical_P1L";
    public string aimerHorzontal = "Horizontal_P1R";
    public string aimerVertical = "Vertical_P1R";
    //public string chargeButton = "RT_P1";
    public KeyCode ropeButton = KeyCode.Joystick1Button5;
    public KeyCode dashButton = KeyCode.Joystick1Button2;
    public KeyCode pullButton = KeyCode.Joystick1Button3;
    /// <summary>
    /// charging and rope
    /// </summary>
    //public KeyCode keyRope = KeyCode.Joystick2Button15;
    #endregion

    #region Timer
    public float dashCD = 1f;
    public float dashTimer = 0f;
    public bool canDash = true;

    public float pullCD = 1f;
    public float pullTimer = 0f;
    public bool canPull = true;

    public float dizzyTime = 1f;
    public float dizzyTimer = 0f;
    #endregion

    /// <summary>
    /// current maxSpeed
    /// </summary>
    [SerializeField] private float curSpeed;
    [SerializeField] private float maxSpeed = 3;
    public float lenRope;

    [Header("Dash Setting")]
    /// <summary>
    /// maxSpeed attenuation
    /// </summary>
    public float dashSpeed;
    public float dashAttenuation = 1;
    [Header("Pull Setting")]
    public float pullSpeed;
    public float pullAttenuation = 1;
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

    [SerializeField] protected Vector2 m_MoveVector;
    protected CharacterController2D m_CharacterController2D;

    // Start is called before the first frame update
    void Awake()
    {
        lenRope = 1.8f;
        curSpeed = maxSpeed;
        aimer = aimerObj.GetComponent<Aimer>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat(m_HashDirectionPara, 1);
        m_PlayerAudio = GetComponent<PlayerAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        #region Player
        curSpeed = curSpeed - dashAttenuation * Time.deltaTime > maxSpeed ? curSpeed - dashAttenuation * Time.deltaTime : maxSpeed;

        Vector2 v = Vector2.zero;
        if (playerStatus != PlayerStatus.Dizzy)
        {
            float mHorizontal, mVertical;
            mHorizontal = Input.GetAxis(moveHorizontal);
            mVertical = -Input.GetAxis(moveVertical);
            m_MoveVector = new Vector2(mHorizontal, mVertical) * curSpeed;

            if (Input.GetKey(ropeButton) &&
               (playerStatus == PlayerStatus.Normal || playerStatus == PlayerStatus.Charging))
            {
                if (playerStatus == PlayerStatus.Normal)
                {
                    playerStatus = PlayerStatus.Charging;
                    curSpeed = maxSpeed * 0.5f; // slow down while charging
                    aimer.StartCharging();
                }
            }
            if (Input.GetKeyUp(ropeButton) && playerStatus == PlayerStatus.Charging && canRope)
            {
                playerStatus = PlayerStatus.Throwing;
                curSpeed = maxSpeed;
                aimer.EndCharging();
                ropeObj.GetComponent<Rope>().TryThrowRope(aimer.curChargeTime * chargeRatio, aimer.transform.position - throwPosition.position);
            }

            // skill dash and pull
            if ((Input.GetKeyDown(dashButton) && canDash))
            {
                canDash = false;
                m_Animator.SetBool(m_HashDashPara, true);
                curSpeed *= dashSpeed;
            }
            if ((Input.GetKeyDown(pullButton) && canPull && playerStatus == PlayerStatus.Roping))
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
                playerStatus = PlayerStatus.Normal;
                Debug.Log("ropeObj broken");
                GetComponentInChildren<Rope>().BreakLine();
                GameController.Instance.horse.TryStruggle();
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
    }

    public void ResetCD()
    {
        dashTimer = 0f;
        pullTimer = 0f;
    }

    public void CleanDebuff()
    {
        dizzyTimer = 0f;
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
        playerStatus = PlayerStatus.Dizzy;
        m_Animator.SetBool(m_HashDizzyPara, true);
        StartCoroutine(Dizzy());
    }

    IEnumerator Dizzy()
    {
        // anime
        yield return new WaitForSeconds(2);
        //aimer.ResetAimer();
        m_Animator.SetBool(m_HashDizzyPara, false);
        playerStatus = PlayerStatus.Normal;
    }

    //IEnumerator DashCD()
    //{
    //    canDash = false;
    //    yield return new WaitForSeconds(cdDash);
    //    canDash = true;
    //}

    //IEnumerator PullCD()
    //{
    //    canPull = false;
    //    yield return new WaitForSeconds(cdPull);
    //    canPull = true;
    //}

    //IEnumerator RopeCD()
    //{
    //    canRope = false;
    //    yield return new WaitForSeconds(cdRope);
    //    aimer.ResetAimer();
    //    canRope = true;
    //}

    //Call by event
    public void HitByRope(GameObject gameObject)
    {
        BeDizzy();
    }

    //process result of roping
    //IEnumerator RopeHorse()
    //{
    //    playerStatus = PlayerStatus.Throwing;
    //    aimer.HideAimer();
    //    // play anime
    //    m_PlayerAudio.PlayThrow();
    //    GetComponentInChildren<ropeObj>().Throw(aimer.CalDelay(), aimer.transform.position - transform.position);
    //    yield return new WaitForSeconds(aimer.CalDelay());

    //    Player enemy = GameController.Instance.players[2 - playerID];
    //    if ((enemy.transform.position - aimer.transform.position).magnitude <= aimer.R)
    //    {
    //        // rope player
    //        playerStatus = PlayerStatus.Normal;
    //        Debug.Log("Player roped!");
    //        enemy.BeDizzy();
    //        enemy.m_Animator.SetBool(m_HashDizzyPara, true);
    //        enemy.m_PlayerAudio.PlayDize();
    //        GetComponentInChildren<ropeObj>().BreakLine();
    //        enemy.GetComponentInChildren<ropeObj>().BreakLine();
    //        GameController.Instance.horse.state = 0;
    //        GameController.Instance.HandleHorseState();
    //        StartCoroutine("RopeCD");
    //    }
    //    else if ((GameController.Instance.horse.transform.position - aimer.transform.position).magnitude <= aimer.R)
    //    {
    //        // rope horse
    //        if (GameController.Instance.horse.state == 0) // horse free
    //        {
    //            playerStatus = PlayerStatus.Roping;
    //            Debug.Log("Free Horse roped!");
    //            GameController.Instance.horse.TryCatch(playerID);
    //        }
    //        else // horse roped
    //        {
    //            // Player enemy = GameController.players[GameController.horse.state - 1];
    //            playerStatus = PlayerStatus.Normal;
    //            Debug.Log("Roped Horse roped!");
    //            enemy.playerStatus = PlayerStatus.Normal;
    //            enemy.aimer.ResetAimer();
    //            enemy.GetComponentInChildren<ropeObj>().BreakLine();
    //            GameController.Instance.horse.TryStruggle();
    //            GameController.Instance.HandleHorseState();
    //            StartCoroutine("RopeCD");
    //        }
    //    }
    //    else // empty
    //    {
    //        Debug.Log("Nothing roped!");
    //        playerStatus = PlayerStatus.Normal;
    //        GetComponentInChildren<ropeObj>().BreakLine();
    //        StartCoroutine("RopeCD");
    //    }

    //}
}
