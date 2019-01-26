using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;

public class Player : MonoBehaviour
{
    public Sprite image;

    public enum PlayerStatus : int
    {
        Normal = 0,
        Charging,
        Roping,
        Dizzy,
        Throwing,
    }

    //private static int FREE = 0;
    //private static int CHARGING = 1;
    //public static int ROPING = 2;
    //private static int DIZZY = 3;
    //private static int THROWING = 4;

    public int playerID;
    public int score = 0;
    //public int state = 0;
    public PlayerStatus playerStatus = PlayerStatus.Normal;

    #region InputSetting
    [Header("Input Setting")]
    public string Horizontal = "Horizontal_P1L";
    public string Vertical = "Vertical_P1L";
    /// <summary>
    /// charging and rope
    /// </summary>
    public KeyCode keyRope = KeyCode.Joystick2Button15;

    #endregion

    /// <summary>
    /// current speed
    /// </summary>
    [SerializeField] private float curSpeed;
    [SerializeField] private float speed = 3;
    public float lenRope;

    public GameObject AimerObj;
    public GameObject Rope;
    private Aimer aimer;

    [Header("Dash Setting")]
    private bool canDash = true;
    [SerializeField] private float cdDash = 8f;
    [SerializeField] private float rateDash = 3f;
    [SerializeField] private float pullRate = 1.5f;
    //private float timeDash = 1f;
    /// <summary>
    /// speed attenuation
    /// </summary>
    public float attenuation = 1;

    private bool canPull = true;
    private float cdPull = 8f;
    // private float ratePull = 1 / 3;

    private bool canRope = true;
    private float cdRope = 1f;

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
        curSpeed = speed;
        aimer = AimerObj.GetComponent<Aimer>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
        m_Animator.SetFloat(m_HashDirectionPara, 1);
        m_PlayerAudio = GetComponent<PlayerAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        // speed down after dash
        //if (curSpeed > speed)
        //    curSpeed -= speed * (rateDash - 1) * Time.deltaTime / timeDash;
        curSpeed = curSpeed - attenuation * Time.deltaTime > speed ? curSpeed - attenuation * Time.deltaTime : speed;
        //if (curSpeed < speed)
        //    curSpeed = speed;

        Vector2 v = Vector2.zero;
        if (playerStatus != PlayerStatus.Dizzy)
        {
            float LR, UD;
            // for test on PC
            /***********************************************
            float LR = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
            float UD = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            **************************************************/

            // iuput move
            if (playerID == 1)
            {
                LR = Input.GetAxis("Horizontal_P1L");
                UD = -Input.GetAxis("Vertical_P1L");
            }
            else // playerID == 2
            {
                LR = Input.GetAxis("Horizontal_P2L");
                UD = -Input.GetAxis("Vertical_P2L");
            }
            m_MoveVector = new Vector2(LR, UD) * curSpeed;

            // charging and rope
            if (((playerID == 1) ? // ZR press charge
                Input.GetKey(KeyCode.Joystick2Button15) :
                Input.GetKey(KeyCode.Joystick4Button15)) &&
                (playerStatus == PlayerStatus.Normal || playerStatus == PlayerStatus.Charging))
            {
                // play rope charge anime
                if (playerStatus == PlayerStatus.Normal)
                {
                    Rope.GetComponent<Animator>().enabled = true;
                    Rope.GetComponent<Animator>().SetBool(Animator.StringToHash("throw"), false);
                    Rope.GetComponent<Animator>().SetBool(Animator.StringToHash("rerope"), true);
                    playerStatus = PlayerStatus.Charging;
                    curSpeed = speed * 0.5f; // slow down while charging
                }
                aimer.AddR();
            }
            else if (((playerID == 1) ? // ZR release rope
                Input.GetKeyUp(KeyCode.Joystick2Button15) :
                Input.GetKeyUp(KeyCode.Joystick4Button15)) &&
                playerStatus == PlayerStatus.Charging && canRope)
            {
                curSpeed = speed; // speed normal after charging
                // rope anime
                Rope.GetComponent<Animator>().SetBool(Animator.StringToHash("rerope"), false);
                Rope.GetComponent<Animator>().SetBool(Animator.StringToHash("throw"), true);
                // process the result of roping
                StartCoroutine(RopeHorse());
            }

            // skill dash and pull
            if (((playerID == 1) ? // X dash
                Input.GetKeyDown(KeyCode.Joystick2Button1) :
                Input.GetKeyDown(KeyCode.Joystick4Button1)) &&
                canDash /*&& playerStatus == PlayerStatus.Roping*/)
            {
                StartCoroutine(DashCD());
                m_Animator.SetBool(m_HashDashPara, true);
                curSpeed *= rateDash;
            }
            else if (((playerID == 1) ? // Y pull
                Input.GetKeyDown(KeyCode.Joystick2Button3) :
                Input.GetKeyDown(KeyCode.Joystick4Button3)) &&
                canPull && playerStatus == PlayerStatus.Roping)
            {
                World.WorldInstance.horse.SetPulled(m_Rigidbody2D.position - World.WorldInstance.horse.GetPosition(), curSpeed * pullRate);
                StartCoroutine(PullCD());
                m_Animator.SetBool(m_HashPullPara, true);
                // Vector3 rope = World.horse.transform.position - transform.position;
                // World.horse.SetMoveVector(-rope.normalized * lenRope * ratePull);
            }

        }

        // horse too far while roping
        if (playerStatus == PlayerStatus.Roping)
        {
            if ((World.WorldInstance.horse.transform.position - transform.position).magnitude > aimer.R + lenRope)
            {
                playerStatus = PlayerStatus.Normal;
                Debug.Log("Rope broken");
                GetComponentInChildren<Rope>().BreakLine();
                GetComponentInChildren<LineRenderer>().GetComponent<Animator>().enabled = false;
                GetComponentInChildren<LineRenderer>().GetComponent<SpriteRenderer>().enabled = false;
                World.WorldInstance.horse.TryStruggle();
                World.WorldInstance.HandleHorseState();
                StartCoroutine("RopeCD");

            }
            else if ((World.WorldInstance.horse.transform.position - transform.position).magnitude > 0.6f * (aimer.R + lenRope))
            {
                // warning
                Debug.Log("Warning!");
                GetComponentInChildren<LineRenderer>().GetComponent<Animator>().enabled = true;
                GetComponentInChildren<LineRenderer>().GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                GetComponentInChildren<LineRenderer>().GetComponent<Animator>().enabled = false;
                GetComponentInChildren<LineRenderer>().GetComponent<SpriteRenderer>().enabled = false;
            }

        }

        m_Animator.SetFloat(m_HashSpeedXPara, m_MoveVector.x);
        m_Animator.SetFloat(m_HashSpeedYPara, m_MoveVector.y);

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
        aimer.HideAimer();
        playerStatus = PlayerStatus.Dizzy;
        m_Animator.SetBool(m_HashDizzyPara, true);
        StartCoroutine(Dizzy());
    }

    IEnumerator Dizzy()
    {
        // anime
        yield return new WaitForSeconds(2);
        aimer.ResetAimer();
        m_Animator.SetBool(m_HashDizzyPara, false);
        playerStatus = PlayerStatus.Normal;
    }

    IEnumerator DashCD()
    {
        canDash = false;
        yield return new WaitForSeconds(cdDash);
        canDash = true;
    }

    IEnumerator PullCD()
    {
        canPull = false;
        yield return new WaitForSeconds(cdPull);
        canPull = true;
    }

    IEnumerator RopeCD()
    {
        canRope = false;
        yield return new WaitForSeconds(cdRope);
        aimer.ResetAimer();
        canRope = true;
    }

    // process result of roping
    IEnumerator RopeHorse()
    {
        playerStatus = PlayerStatus.Throwing;
        aimer.HideAimer();
        // play anime
        m_PlayerAudio.PlayThrow();
        GetComponentInChildren<Rope>().Throw(aimer.CalDelay(), aimer.transform.position - transform.position);
        yield return new WaitForSeconds(aimer.CalDelay());

        Player enemy = World.WorldInstance.players[2 - playerID];
        if ((enemy.transform.position - aimer.transform.position).magnitude <= aimer.R)
        {
            // rope player
            playerStatus = PlayerStatus.Normal;
            Debug.Log("Player roped!");
            enemy.BeDizzy();
            enemy.m_Animator.SetBool(m_HashDizzyPara, true);
            enemy.m_PlayerAudio.PlayDize();
            GetComponentInChildren<Rope>().BreakLine();
            enemy.GetComponentInChildren<Rope>().BreakLine();
            World.WorldInstance.horse.state = 0;
            World.WorldInstance.HandleHorseState();
            StartCoroutine("RopeCD");
        }
        else if ((World.WorldInstance.horse.transform.position - aimer.transform.position).magnitude <= aimer.R)
        {
            // rope horse
            if (World.WorldInstance.horse.state == 0) // horse free
            {
                playerStatus = PlayerStatus.Roping;
                Debug.Log("Free Horse roped!");
                World.WorldInstance.horse.TryCatch(playerID);
            }
            else // horse roped
            {
                // Player enemy = World.players[World.horse.state - 1];
                playerStatus = PlayerStatus.Normal;
                Debug.Log("Roped Horse roped!");
                enemy.playerStatus = PlayerStatus.Normal;
                enemy.aimer.ResetAimer();
                enemy.GetComponentInChildren<Rope>().BreakLine();
                World.WorldInstance.horse.TryStruggle();
                World.WorldInstance.HandleHorseState();
                StartCoroutine("RopeCD");
            }
        }
        else // empty
        {
            Debug.Log("Nothing roped!");
            playerStatus = PlayerStatus.Normal;
            GetComponentInChildren<Rope>().BreakLine();
            StartCoroutine("RopeCD");
        }

    }
}
