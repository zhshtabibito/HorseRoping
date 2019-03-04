using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorseGame
{
    public class Aimer : MonoBehaviour
    {
        public GameObject playerObj;
        public Player player;
        //public GameObject arrowL;
        //public GameObject arrowR;
        public float moveSpeed = 2f;
        public float maxChargeTime = 3f;
        public float curChargeTime = 0f;

        [SerializeField]
        protected Animator m_Animator;
        protected readonly int m_HashChargingPara = Animator.StringToHash("Charging");
        protected readonly int m_HashEnablePara = Animator.StringToHash("Enable");
        protected readonly int m_HashResetPara = Animator.StringToHash("Reset");

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }
        // Start is called before the first frame update
        void Start()
        {
            player = playerObj.GetComponent<Player>();
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
            aHorizontal = /*Input.GetAxis("J1_Horizontal_R");*/player.playerInput.AimerHorizontal.Value;
            aVertical = /*Input.GetAxis("J1_Vertical_R");*/player.playerInput.AimerVertical.Value;
            //Debug.Log(aHorizontal + ", " + aVertical);
            transform.position += new Vector3(aHorizontal, aVertical, 0).normalized * moveSpeed * Time.deltaTime;
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
            curChargeTime = 0f;
            m_Animator.SetTrigger(m_HashResetPara);
        }
    }
}