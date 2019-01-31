using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HorseGame
{
    [RequireComponent(typeof(Collider2D))]
    public class RopeCircle : MonoBehaviour
    {
        public Rope rope;
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
            }
            else if (collision.gameObject.GetComponentInParent<Player>() != null)
            {

            }
        }

    }

}