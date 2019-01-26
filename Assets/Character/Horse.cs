using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorseGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class Horse : MonoBehaviour
    {
        public float moveSpeed = 1.0f;
        /// <summary>
        /// 0 = free
        /// 1 - 4 = player
        /// </summary>
        public int state;
        public enum MoveType
        {
            Idle = 0,
            MoveRandom,
            Pulled
        }
        public MoveType enumMoveType;
        /// <summary>
        /// 加速衰减：突然给马加一个冲刺的之后的速度衰减程度
        /// </summary>
        public float attenuation;
        public float raycastDistance = 0.1f;
        public float pulledSpeed = 2f;
        public LayerMask borderLayerMask;
        public bool isOriginLeft = true;

        [SerializeField]
        protected Vector2 m_MoveVector;

        protected CharacterController2D m_CharacterController2D;
        protected CapsuleCollider2D m_CapsuleCollider2D;
        protected ContactFilter2D m_ContactFilter;
        protected Rigidbody2D m_Rigidbody2D;
        protected SpriteRenderer m_SpriteRenderer;
        protected Animator m_Animator;
        protected HorseAudio m_HorseAutio;

        protected Vector2[] m_MoveDiretion = new Vector2[8]
        {
            new Vector2(1, 0),
            new Vector2((float)1.414, (float)1.414),
            new Vector2(0, 1),
            new Vector2((float)1.414, -(float)1.414),
            new Vector2(-1, 0),
            new Vector2(-(float)1.414, -(float)1.414),
            new Vector2(0, -1),
            new Vector2(-(float)1.414, (float)1.414),
        };
        /// <summary>
        /// 当射线检测到边缘时，需要获取当前的移动方向previousDirc，然后根据该数值将方向置反
        /// </summary>
        protected int previousDirc = 0;
        /// <summary>
        /// 强制导航状态：当射线检测到边缘时，设置为强制导航状态
        /// </summary>
        [SerializeField] protected bool isBorderNavigation = false;
        /// <summary>
        /// 强制导航持续时间
        /// </summary>
        [SerializeField] protected float navTime = 2.0f;
        /// <summary>
        /// 强制导航已持续时间
        /// </summary>
        [SerializeField] protected float navTimeLast = 0f;

        protected readonly int m_HashSpeedPara = Animator.StringToHash("Speed");
        protected readonly int m_HashCatchingPara = Animator.StringToHash("Catching");
        protected readonly int m_HashCatchedPara = Animator.StringToHash("Catched");
        public Vector2[] MoveDiretion
        {
            get { return m_MoveDiretion; }
        }
        public ContactFilter2D ContactFilter { get { return m_ContactFilter; } }
        private void Awake()
        {
            m_CharacterController2D = GetComponent<CharacterController2D>();
            m_CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Animator = GetComponent<Animator>();
            m_HorseAutio = GetComponent<HorseAudio>();
            m_ContactFilter.layerMask = borderLayerMask;
            m_ContactFilter.useLayerMask = true;
            m_ContactFilter.useTriggers = false;
        }
        void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            UpdateFace();
            m_Animator.SetFloat(m_HashSpeedPara, m_MoveVector.magnitude);
            if (BorderCheck() && isBorderNavigation == false)
            {
                previousDirc = (previousDirc + 4) % 8;
                m_MoveVector = m_MoveDiretion[previousDirc] * moveSpeed;
                isBorderNavigation = true;
            }
            else if (isBorderNavigation)
            {
                if (navTime - navTimeLast < 0)
                {
                    navTimeLast = 0;
                    isBorderNavigation = false;
                }
                else
                {
                    navTimeLast += Time.deltaTime;
                }
            }
            else
            {
                navTimeLast = 0;
            }
            if (enumMoveType == MoveType.Pulled)
            {
                //float speed = Mathf.MoveTowards(m_MoveVector.magnitude, moveSpeed, attenuation * Time.deltaTime);
                m_MoveVector = m_MoveVector * Mathf.Max((m_MoveVector.magnitude - attenuation) / m_MoveVector.magnitude, moveSpeed / m_MoveVector.magnitude);
            }
        }
        private void FixedUpdate()
        {
            m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
        }

        public void UpdateFace()
        {
            if (isOriginLeft)
                m_SpriteRenderer.flipX = m_MoveVector.x > 0 ? true : false;
            else
                m_SpriteRenderer.flipX = m_MoveVector.x > 0 ? false : true;
        }

        public void SetMoveVector(Vector2 newMoveVector)
        {
            m_MoveVector = newMoveVector;
        }

        public void UpdateMoveMode()
        {
            switch (enumMoveType)
            {
                case MoveType.Idle:
                    {
                        m_MoveVector = Vector2.zero;
                    }
                    break;
                case MoveType.MoveRandom:
                    {
                        //当不处于强制导航状态时，随机选择方向
                        if (!isBorderNavigation)
                        {
                            previousDirc = Random.Range(0, m_MoveDiretion.Length);
                            m_MoveVector = m_MoveDiretion[previousDirc] * moveSpeed;
                            //Debug.Log(m_MoveDiretion[previousDirc]);
                        }
                    }
                    break;
                case MoveType.Pulled:
                    {

                    }
                    break;
            }
        }

        public void SetPulled()
        {
            Vector2 direction = MoveDiretion[Random.Range(0, 8)];
            SetPulled(direction, pulledSpeed);
        }

        public void SetPulled(Vector2 direction, float speed)
        {
            enumMoveType = MoveType.Pulled;
            m_MoveVector = direction.normalized * speed;
        }

        public void SetIdle()
        {
            //m_MoveVector = Vector2.zero;
            enumMoveType = MoveType.Idle;
        }

        public void SetMoveRandom()
        {
            enumMoveType = MoveType.MoveRandom;
        }

        public bool BorderCheck()
        {
            Vector2[] raycastStart = new Vector2[4];
            Vector2[] raycastDirection = new Vector2[4];
            raycastStart[0] = m_CapsuleCollider2D.bounds.center + new Vector3(0, m_CapsuleCollider2D.bounds.extents.y, 0);
            raycastStart[1] = m_CapsuleCollider2D.bounds.center + new Vector3(0, -m_CapsuleCollider2D.bounds.extents.y, 0);
            raycastStart[2] = m_CapsuleCollider2D.bounds.center + new Vector3(-m_CapsuleCollider2D.bounds.extents.x, 0, 0);
            raycastStart[3] = m_CapsuleCollider2D.bounds.center + new Vector3(m_CapsuleCollider2D.bounds.extents.x, 0, 0);
            raycastDirection[0] = new Vector2(0, 1);
            raycastDirection[1] = new Vector2(0, -1);
            raycastDirection[2] = new Vector2(-1, 0);
            raycastDirection[3] = new Vector2(1, 0);

            for (int i = 0; i < raycastStart.Length; i++)
            {
                Debug.DrawLine(raycastStart[i], raycastStart[i] + raycastDirection[i] * raycastDistance);
                //Debug.DrawRay(raycastStart[i], raycastDirection[i], Color.green);
            }
            for (int i = 0; i < raycastStart.Length; i++)
            {
                if (Physics2D.Raycast(raycastStart[i], raycastDirection[i], raycastDistance, borderLayerMask))
                {
                    return true;
                }
            }
            return false;
        }

        public Vector2 GetPosition()
        {
            return m_Rigidbody2D.position;
        }

        public void TryCatch(int id = 0)
        {
            if (id == state)
                return;
            state = id;
            m_Animator.SetBool(m_HashCatchingPara, true);
            m_Animator.SetBool(m_HashCatchedPara, true);
            m_HorseAutio.PlayCatching();
            //if (state == 0)
            //{
            //    m_Animator.SetBool(m_HashCatchingPara, true);
            //    m_Animator.SetBool(m_HashCatchedPara, true);
            //    return true;
            //}
            //else
            //    return false;
        }

        public void TryStruggle()
        {
            state = 0;
            m_Animator.SetBool(m_HashCatchedPara, false);
            m_Animator.SetBool(m_HashCatchingPara, false);
            m_HorseAutio.PlayRelease();
        }
    }
}
