using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public int numberofCoins = 5;
    public GameObject Coin;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberofCoins; i++) {
            Vector3 size = transform.localScale - new Vector3(0.75f, 0f, 0.75f);
            Vector3 pos = transform.position + new Vector3(Random.Range(-size.x / 2, size.x /2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
            Instantiate(Coin, pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
