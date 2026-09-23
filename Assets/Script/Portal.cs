using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private GameObject tipPanel;

    private bool playerInRange = false;
    private KeyCode interactionKey = KeyCode.E;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(interactionKey))
            GameEvents.NextLevel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tipPanel.SetActive(true);
            playerInRange = true;
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            tipPanel.SetActive(false);
            playerInRange = false;
        }
            
    }
}
