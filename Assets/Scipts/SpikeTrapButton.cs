using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapButton : MonoBehaviour
{
        private Animator anim;
        [SerializeField] private int spikeID;
        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                anim.SetBool("ButtonTriggered", true);
                EvenManager.TriggerSpikeButton(spikeID);
                Invoke(nameof(ResetButton), 5f);
            }
        }

        private void ResetButton()
        {
            anim.SetBool("ButtonTriggered", false);
        }
}
