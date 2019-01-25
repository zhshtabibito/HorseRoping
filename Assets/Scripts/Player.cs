using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;

public class Player : MonoBehaviour
{
    private static int FREE = 0;
    private static int CHARGING = 1;
    private static int ROPING = 2;
    private static int DIZZY = 3;
    private static int THROWING = 4;

    public int id;
    public int score = 0;
    public int state = 0;

    public float spd0;
    private float spd;
    public float lenRope;

    private Rigidbody2D rb;

    public GameObject AimerObj;
    private Aimer aimer;

    private bool canDash = true;
    private float cdDash = 8f;
    private float rateDash = 3f;
    private float timeDash = 1f;

    private bool canPull = true;
    private float cdPull = 8f;
    // private float ratePull = 1 / 3;

    private bool canRope = true;
    private float cdRope = 1f;
    
    protected Animator m_Animator;
    protected PlayerAudio m_PlayerAudio;
    protected readonly int m_HashSpeedXPara = Animator.StringToHash("Speed_X");
    protected readonly int m_HashSpeedYPara = Animator.StringToHash("Speed_Y");
    protected readonly int m_HashDirectionPara = Animator.StringToHash("Direction");
    protected readonly int m_HashDashPara = Animator.StringToHash("Dash");
    protected readonly int m_HashPullPara = Animator.StringToHash("Pull");
    protected readonly int m_HashDizzyPara = Animator.StringToHash("Dizzy");
    protected Vector2 m_MoveVector;
    protected CharacterController2D m_CharacterController2D;

    // Start is called before the first frame update
    void Awake()
    {
        spd = spd0;
        aimer = AimerObj.GetComponent<Aimer>();
        rb = GetComponent<Rigidbody2D>();
        m_CharacterController2D = GetComponent<CharacterController2D>();
        m_Animator = GetComponent<Animator>();
        m_PlayerAudio = GetComponent<PlayerAudio>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spd > spd0)
            spd -= spd0 * (rateDash - 1) * Time.deltaTime / timeDash;
        if (spd < spd0)
            spd = spd0;

        Vector2 v = Vector2.zero;
        if (state != DIZZY)
        {
            float LR, UD;
            // for test on PC
            /***********************************************
            float LR = Input.GetKey(KeyCode.D) ? 1 : Input.GetKey(KeyCode.A) ? -1 : 0;
            float UD = Input.GetKey(KeyCode.W) ? 1 : Input.GetKey(KeyCode.S) ? -1 : 0;
            **************************************************/
            if (id == 1)
            {
                LR = Input.GetAxis("Horizontal_P1L");
                UD = -Input.GetAxis("Vertical_P1L");
            }
            else // id == 2
            {
                LR = Input.GetAxis("Horizontal_P2L");
                UD = -Input.GetAxis("Vertical_P2L");
            }
            m_MoveVector = new Vector2(LR, UD) * spd;

            // charging and rope
            if (((id==1) ? // ZR press charge
                Input.GetKey(KeyCode.Joystick2Button15) :
                Input.GetKey(KeyCode.Joystick4Button15)) &&
                (state == FREE || state == CHARGING))
            {
                state = CHARGING;
                aimer.AddR();
            }
            else if (((id==1) ? // ZR release rope
                Input.GetKeyUp(KeyCode.Joystick2Button15) :
                Input.GetKeyUp(KeyCode.Joystick4Button15)) &&
                state == CHARGING && canRope)
            {
                StartCoroutine("RopeHorse");
            }

            // skill dash n pull
            if (((id==1)? // X dash
                Input.GetKey(KeyCode.Joystick2Button1) :
                Input.GetKey(KeyCode.Joystick4Button1)) &&
                canDash && state==ROPING)
            {
                DashCD();
                m_Animator.SetBool(m_HashDashPara, true);
                spd *= rateDash;
            }
            else if (((id == 1) ? // Y pull
                Input.GetKey(KeyCode.Joystick2Button3) :
                Input.GetKey(KeyCode.Joystick4Button3)) &&
                canPull && state == ROPING)
            {
                World.horse.SetPulled(rb.position - World.horse.GetPosition(), spd * rateDash * 2);
                PullCD();
                m_Animator.SetBool(m_HashPullPara, true);
                // Vector3 rope = World.horse.transform.position - transform.position;
                // World.horse.SetMoveVector(-rope.normalized * lenRope * ratePull);
            }

        }

        // horse too far while roping
        if (state == ROPING)
        {
            if ((World.horse.transform.position - transform.position).magnitude > aimer.R + lenRope)
            {
                state = FREE;
                Debug.Log("Rope broken");
                World.horse.state = 0;
                World.HandleHorseState();
                StartCoroutine("RopeCD");

            }
            else if((World.horse.transform.position - transform.position).magnitude > 0.8f*(aimer.R + lenRope))
            {
                // warning

            }

        }

        m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
        m_Animator.SetFloat(m_HashSpeedXPara, m_MoveVector.x);
        m_Animator.SetFloat(m_HashSpeedYPara, m_MoveVector.y);
        m_Animator.SetFloat(m_HashDirectionPara,
            (m_MoveVector.y > 0 && m_MoveVector.y >= m_MoveVector.x && m_MoveVector.y >= -m_MoveVector.x) ? 0 :
            (m_MoveVector.y < 0 && -m_MoveVector.y >= m_MoveVector.x && -m_MoveVector.y >= -m_MoveVector.x) ? (1 / 3) :
            (m_MoveVector.x < 0 && -m_MoveVector.x >= m_MoveVector.y && -m_MoveVector.x >= -m_MoveVector.y) ? (2 / 3) : 1);

    }

    public void BeDizzy()
    {
        aimer.HideAimer();
        state = DIZZY;
        StartCoroutine("Dizzy");
    }

    IEnumerator Dizzy()
    {
        // anime
        yield return new WaitForSeconds(2);
        aimer.ResetAimer();
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

    IEnumerator RopeHorse()
    {
        state = THROWING;
        aimer.HideAimer();
        // play anime
        m_PlayerAudio.PlayThrow();
 
        yield return new WaitForSeconds(aimer.CalDelay());
        Player enemy = World.players[2 - id];
        if ((enemy.transform.position - aimer.transform.position).magnitude <= aimer.R)
        {
            // rope player
            state = FREE;
            Debug.Log("Player roped!");
            enemy.BeDizzy();
            enemy.m_Animator.SetBool(m_HashDizzyPara, true);
            enemy.m_PlayerAudio.PlayDize();
            World.horse.state = 0;
            World.HandleHorseState();
            StartCoroutine("RopeCD");
        }
        else if ((World.horse.transform.position - aimer.transform.position).magnitude <= aimer.R)
        {
            // rope horse
            if(World.horse.state == 0) // horse free
            {
                state = ROPING;
                Debug.Log("Free Horse roped!");
                World.horse.TryCatch(id);
            }
            else // horse roped
            {
                // Player enemy = World.players[World.horse.state - 1];
                state = FREE;
                Debug.Log("Roped Horse roped!");
                enemy.state = 0;
                enemy.aimer.ResetAimer();
                World.horse.TryStruggle();
                World.HandleHorseState();
                StartCoroutine("RopeCD");
            }
        }
        else // empty
        {
            Debug.Log("Nothing roped!");
            state = FREE;
            StartCoroutine("RopeCD");
        }
        
    }
}
