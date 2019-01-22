using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;

namespace HorseGame
{
    public class HorseAI : MonoBehaviour
    {
        Animator m_Animator;
        Root m_Ai = BT.Root();
        //EnemyBehaviour m_EnemyBehaviour;
        Horse m_Horse;

        private void OnEnable()
        {
            //m_EnemyBehaviour = GetComponent<EnemyBehaviour>();
            m_Horse = GetComponent<Horse>();

            //m_Animator = GetComponent<Animator>();
            m_Ai.OpenBranch(
                BT.RandomSequence().OpenBranch
                (
                    BT.Sequence().OpenBranch(
                        BT.Call(m_Horse.MoveRandom),
                        BT.Wait(Random.Range(1, 3))
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Call(m_Horse.SetIdle),
                        BT.Wait(Random.Range(1, 3))
                    )
                )
            //BT.If(() => { return m_Horse.BorderCheck() == false; }).OpenBranch(
            //    BT.Call(m_Horse.MoveRandom)
            //    //BT.Wait(Random.Range(1, 3))
            //    )

            //BT.If(() => { return m_EnemyBehaviour.Target != null; }).OpenBranch(
            //    BT.Call(m_EnemyBehaviour.CheckTargetStillVisible),
            //    BT.Call(m_EnemyBehaviour.OrientToTarget),
            //    BT.Trigger(m_Animator, "Shooting"),
            //    BT.Call(m_EnemyBehaviour.RememberTargetPos),
            //    BT.WaitForAnimatorState(m_Animator, "Attack")
            //),

            //BT.If(() => { return m_EnemyBehaviour.Target == null; }).OpenBranch(
            //    BT.Call(m_EnemyBehaviour.ScanForPlayer)
            //)
            );
        }

        private void Update()
        {
            m_Ai.Tick();
        }
    }
}