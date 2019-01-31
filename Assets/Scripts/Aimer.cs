using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aimer : MonoBehaviour
{
    public GameObject playerObj;
    public Player player;
    //public GameObject arrowL;
    //public GameObject arrowR;
    public float moveSpeed = 2f;
    public float maxChargeTime = 3f;
    public float curChargeTime = 0f;

    protected string aimerHorzontal;
    protected string aimerVertical;

    public float R;
    private readonly float Rmin = 0.3f;
    private readonly float Rmax = 1.1f;
    private readonly float spd = 5;
    private readonly float step = 0.8f; // curSpeed of charging

    [SerializeField]
    protected Animator m_Animator;
    protected readonly int m_HashChargingPara = Animator.StringToHash("Charging");
    protected readonly int m_HashEnablePara = Animator.StringToHash("Enable");

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        player = playerObj.GetComponent<Player>();
        aimerHorzontal = player.aimerHorzontal;
        aimerVertical = player.aimerVertical;
        //arrowR.transform.localScale = new Vector2(-1, 1);
        //ResetAimer();
    }

    // Update is called once per frame
    void Update()
    {
        float aHorizontal, aVertical;
        // PC test
        /***********************************************
        float aHorizontal = Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
        float UD = Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
        ***********************************************/
        // move aimer
        aHorizontal = Input.GetAxis(aimerHorzontal);
        aVertical = Input.GetAxis(aimerVertical);
        //Debug.Log(aHorizontal + ", " + aVertical);
        transform.position += new Vector3(aHorizontal * moveSpeed * Time.deltaTime, -aVertical * moveSpeed * Time.deltaTime, 0);
        // limit aimer distance
        Vector3 ropeDirection = transform.position - player.transform.position;
        float mag = ropeDirection.magnitude;
        if (mag > player.lenRope)
        {
            ropeDirection = ropeDirection * (player.lenRope / ropeDirection.magnitude);
            transform.position = player.transform.position + ropeDirection;
        }
        if (player.playerStatus == Player.PlayerStatus.Normal
            || player.playerStatus == Player.PlayerStatus.Roping
            )
        {
            m_Animator.SetBool(m_HashEnablePara, true);
        }
        else
        {
            m_Animator.SetBool(m_HashEnablePara, false);
        }

        if (player.playerStatus == Player.PlayerStatus.Charging)
        {
            if (curChargeTime > maxChargeTime)
                curChargeTime = maxChargeTime;
            else
                curChargeTime += Time.deltaTime;
        }
        else
        {
            curChargeTime = 0f;
        }
    }

    public void StartCharging()
    {
        m_Animator.SetBool(m_HashChargingPara, true);
    }

    public void EndCharging()
    {
        m_Animator.SetBool(m_HashChargingPara, false);
    }
    /// <summary>
    /// 眩晕打断蓄力
    /// </summary>
    /// <param name="seconds">眩晕秒数</param>
    public void EndCharging(float seconds)
    {
        m_Animator.SetBool(m_HashChargingPara, false);
        StartCoroutine(EnableAimer(seconds, false));
    }

    IEnumerator EnableAimer(float seconds, bool enable)
    {
        m_Animator.SetBool(m_HashEnablePara, enable);
        yield return new WaitForSeconds(seconds);
        m_Animator.SetBool(m_HashEnablePara, !enable);
    }

    public void ResetAimer()
    {
        //Debug.Log("Aimer reset");
        //R = Rmin;
        //Vector3 temp = transform.position - new Vector3(R, 0, 0);
        //arrowL.transform.position = transform.position - new Vector3(R, 0, 0);
        //arrowR.transform.position = transform.position + new Vector3(R, 0, 0);
        //GetComponent<SpriteRenderer>().enabled = true;
        //arrowL.GetComponent<SpriteRenderer>().enabled = true;
        //arrowR.GetComponent<SpriteRenderer>().enabled = true;
    }

    //public void HideAimer()
    //{
    //    GetComponent<SpriteRenderer>().enabled = false;
    //    arrowL.GetComponent<SpriteRenderer>().enabled = false;
    //    arrowR.GetComponent<SpriteRenderer>().enabled = false;
    //}

    //public void AddR()
    //{
    //    R += step*Time.deltaTime;
    //    if (R > Rmax)
    //        R = Rmax;
    //}

    //public float CalDelay()
    //{
    //    // cal ropeDirection flying throwTime by R
    //    return 0.8f - 0.2f * (R - Rmin) / step;
    //}
}
