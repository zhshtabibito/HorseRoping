using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HorseGame;

public class Rope : MonoBehaviour
{
    public Vector3 throwDirection;
    public float throwTime;
    public float throwSpeed;
    public float maxLength = 3f;

    public Player player;
    public Transform throwPosition;
    public GameObject ropeCircle;
    public GameObject ropeLine;
    public bool isThrowing;
    public bool isCatching;

    protected Animator m_Animator;

    protected readonly int m_HashChargePara = Animator.StringToHash("Charge");
    protected readonly int m_HashThrowPara = Animator.StringToHash("Throw");
    protected readonly int m_HashEnablePara = Animator.StringToHash("Enable");
    // Start is called before the first frame update

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        isThrowing = false;
        isCatching = false;
    }
    void Start()
    {
        //spriteRender = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isThrowing && !isCatching)
        {
            throwTime -= Time.deltaTime;
            if (throwTime > 0)
            {
                m_Animator.SetBool(m_HashEnablePara, false);
                m_Animator.SetBool(m_HashThrowPara, true);
                ropeCircle.GetComponent<SpriteRenderer>().enabled = true;
                ropeCircle.GetComponent<BoxCollider2D>().enabled = true;
                ropeCircle.transform.position = ropeCircle.transform.position + throwSpeed * throwDirection * Time.deltaTime/* / throwTime*/;
                ropeLine.GetComponent<LineRenderer>().enabled = true;
                ropeLine.GetComponent<LineRenderer>().SetPosition(0, throwPosition.position);
                ropeLine.GetComponent<LineRenderer>().SetPosition(1, ropeCircle.transform.position);
            }
            else
            {
                throwTime = 0;
                ropeCircle.GetComponent<SpriteRenderer>().enabled = false;
                ropeCircle.GetComponent<BoxCollider2D>().enabled = false;
                ropeLine.GetComponent<LineRenderer>().enabled = false;
                isThrowing = false;
            }
        }
        else if(!isThrowing && !isCatching)
        {
            ropeCircle.transform.position = player.throwPosition.position;
            m_Animator.SetBool(m_HashEnablePara, true);
            ropeLine.GetComponent<LineRenderer>().enabled = false;
            ropeCircle.GetComponent<BoxCollider2D>().enabled = false;
            ropeCircle.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (isThrowing && isCatching)
        {
            ropeCircle.GetComponent<SpriteRenderer>().enabled = false;
            ropeCircle.GetComponent<BoxCollider2D>().enabled = false;
            ropeLine.GetComponent<LineRenderer>().SetPosition(0, throwPosition.position);
            ropeLine.GetComponent<LineRenderer>().SetPosition(1, GameController.Instance.horse.transform.position);
        }
    }

    //public void Throw(float time, Vector3 vec)
    //{
    //    this.throwTime = time;
    //    this.throwDirection = vec;
    //    transform.position = GetComponentInParent<Player>().transform.position;
    //    ropeLine.GetComponent<LineRenderer>().enabled = true;
    //    ropeLine.GetComponent<LineRenderer>().SetPosition(0, transform.position);
    //    ropeLine.GetComponent<LineRenderer>().SetPosition(1, transform.position);
    //    StartCoroutine("TimingAndThrow");
    //}

    //IEnumerator TimingAndThrow()
    //{
    //    isThrowing = true;
    //    //spriteRender.enabled = true;
    //    yield return new WaitForSeconds(throwTime);
    //    isThrowing = false;
    //    //spriteRender.enabled = false;

    //}

    public void BreakLine()
    {
        ropeLine.GetComponent<LineRenderer>().enabled = false;
        ropeCircle.GetComponent<SpriteRenderer>().enabled = false;
        isThrowing = false;
        isCatching = false;
    }

    public void TryThrowRope(float time, Vector3 direction)
    {
        this.throwTime = time;
        this.throwDirection = direction.normalized;

        isThrowing = true;
    }
}
