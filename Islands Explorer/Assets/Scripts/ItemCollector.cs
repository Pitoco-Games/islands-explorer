using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int coinsCollected = 0;

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            coinsCollected++;
            Debug.Log("Coins collected: " + coinsCollected);
        }
    }
}
