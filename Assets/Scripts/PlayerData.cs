using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int currentCoins;
    public List<GameObject> backpack = new List<GameObject>(); 

    public void CollectCoin(GameObject gotCoin)
    {
        currentCoins += 1;
        backpack.Add(gotCoin);
    }

    IEnumerator LerpPosition(GameObject gotCoin, Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = gotCoin.transform.position;
        while (time < duration)
        {
            gotCoin.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        gotCoin.transform.position = targetPosition;
        Destroy(gotCoin);
    }

    public void SpendCoin(Vector3 targetPosition)
    {
        if (currentCoins > 0) {
            GameObject gotCoin = backpack[currentCoins-1];
            StartCoroutine(LerpPosition(gotCoin, targetPosition, 0.1f));
            currentCoins -= 1;
            backpack.Remove(gotCoin);
        }
    }
}
