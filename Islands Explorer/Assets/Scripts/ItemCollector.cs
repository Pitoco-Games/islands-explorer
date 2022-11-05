using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private int coinsCollected = 0;

    private void OnTriggerEnter(Collider collision)
    {
        GameObject colliderGameObject = collision.gameObject;

        if(colliderGameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            coinsCollected++;
            Debug.Log("Coins collected: " + coinsCollected);
        }
    }
}
