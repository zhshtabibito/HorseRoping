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

        public float raycastDistance = 0.1f;
        public LayerMask borderLayerMask;

        [SerializeField]
        protected Vector3 m_MoveVector;

        protected CharacterController2D m_CharacterController2D;
        protected CapsuleCollider2D m_CapsuleCollider2D;
        protected ContactFilter2D m_ContactFilter;
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
        public int previousDirc = 0;

        public Vector2[] MoveDiretion
        {
            get { return m_MoveDiretion; }
        }
        public ContactFilter2D ContactFilter { get { return m_ContactFilter; } }
        private void Awake()
        {
            m_CharacterController2D = GetComponent<CharacterController2D>();
            m_CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
            m_ContactFilter.layerMask = borderLayerMask;
            m_ContactFilter.useLayerMask = true;
            m_ContactFilter.useTriggers = false;
        }
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void FixedUpdate()
        {
            m_CharacterController2D.Move(m_MoveVector * Time.deltaTime);
        }

        public void SetMoveVector(Vector2 newMoveVector)
        {
            m_MoveVector = newMoveVector;
        }

        public void MoveRandom()
        {
            if (BorderCheck())
            {
                previousDirc = (previousDirc + 4) % 8;
                m_MoveVector = m_MoveDiretion[previousDirc] * moveSpeed;
            }
            else
            {
                previousDirc = Random.Range(0, m_MoveDiretion.Length);
                m_MoveVector = m_MoveDiretion[previousDirc] * moveSpeed;
            }
        }

        public void SetIdle()
        {
            m_MoveVector = Vector2.zero;
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
            raycastDirection[0] = new Vector2(0, -1);
            raycastDirection[0] = new Vector2(-1, 0);
            raycastDirection[0] = new Vector2(1, 0);

            for (int i = 0; i < raycastStart.Length; i++)
            {
                if (Physics2D.Raycast(raycastStart[i], raycastDirection[i], raycastDistance, borderLayerMask))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
