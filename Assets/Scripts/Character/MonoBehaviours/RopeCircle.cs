using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorseGame
{
    [RequireComponent(typeof(Collider2D))]
    public class RopeCircle : MonoBehaviour
    {
        public Rope rope;
        public Player player;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.GetComponentInParent<Horse>() != null)
            {
                collision.gameObject.GetComponentInParent<Horse>().TryCatch(rope.player.playerID);
                rope.isCatching = true;
                for (int i = 0; i < GameController.Instance.players.Count; i++)
                {
                    if (GameController.Instance.players[i].playerID == player.playerID)
                        continue;
                    else if (GameController.Instance.players[i].playerStatus == Player.PlayerStatus.Roping)
                    {
                        GameController.Instance.players[i].BreakRope();
                    }
                }
            }
            else if (collision.gameObject.GetComponent<Player>() != null)
            {
                collision.gameObject.GetComponent<Player>().HitByRope(player.gameObject);
            }
        }

    }

}