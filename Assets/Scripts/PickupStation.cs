using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupStation : MonoBehaviour
{
    bool isTriggered = false;
    float lastRegisteredTime;
    List<GameObject> coinsinQueue = new List<GameObject>();
    public float delayedSecondsInBetween = 0.25f;
    public int numberofCoins = 10;
    int currentCoinCollected = 0;
    public GameObject Coin;
    public TMPro.TextMeshProUGUI gotText;
    Collider gotPlayer;
    PlayerData gotPlayerData;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberofCoins; i++) {
            Vector3 size = transform.localScale - new Vector3(0.75f, 0f, 0.75f);
            Vector3 pos = transform.position + new Vector3(Random.Range(-size.x / 2, size.x /2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
            GameObject newCoin = Instantiate(Coin, pos, Quaternion.identity);
            newCoin.transform.parent = transform.parent;
            coinsinQueue.Add(newCoin);
        }
        gotText.text = (numberofCoins - currentCoinCollected).ToString() + "/" + (numberofCoins).ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if it was a player that stepped onto the pad itself
        if (other.tag == "Player"){
            gotPlayer = other;
            gotPlayerData = gotPlayer.GetComponent<PlayerData>();
            isTriggered = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if its a player that stepped off and not just some random part
         if (other.tag == "Player"){
            gotPlayer = other;
            isTriggered = false;
        }
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
        gotCoin.transform.position = gotPlayer.transform.position - (gotPlayer.transform.forward/2) + new Vector3(0f, 0.15f * currentCoinCollected, 0f);
        gotCoin.transform.SetParent(gotPlayer.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTriggered && (Time.time - lastRegisteredTime >= delayedSecondsInBetween)) {
            lastRegisteredTime = Time.time;
            if (currentCoinCollected < numberofCoins) {
                GameObject gotCoin = coinsinQueue[0];
                gotCoin.transform.localScale = new Vector3(30f, 30f, 30f);
                Vector3 targetPosition = gotPlayer.transform.position - (gotPlayer.transform.forward/2) + new Vector3(0f, 0.15f * currentCoinCollected, 0f);
                StartCoroutine(LerpPosition(gotCoin, targetPosition, 0.1f));
                currentCoinCollected = currentCoinCollected + 1;
                gotText.text = (numberofCoins - currentCoinCollected).ToString() + "/" + (numberofCoins).ToString();
                gotPlayerData.CollectCoin(gotCoin);
                coinsinQueue.Remove(gotCoin);
            }
        }
    }
}
