using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuyButton : MonoBehaviour
{
    bool isTriggered = false;
    float lastRegisteredTime;
    public float delayedSecondsInBetween = 0.25f;
    public int coinsNeeded = 10;
    public int currentCoinsSpent = 0;
    float buildDelay = 0.35f;
    float buildProgress = 0;
    public int buildNeeded = 5;
    float height;
    float time;
    public GameObject gotObject;
    public TMPro.TextMeshProUGUI gotText;
    bool building = false;
    Collider gotPlayer;
    PlayerData gotPlayerData;
    GameObject clonedObject;
    // Start is called before the first frame update
    void Start()
    {
        gotText.text = "Buy a " + gotObject.name + " for $" + coinsNeeded.ToString();
        Transform mychildtransform = transform.Find("Display");
        Destroy(mychildtransform.gameObject);
        Mesh mesh = gotObject.GetComponent<MeshFilter>().sharedMesh;
        Bounds bounds = mesh.bounds;
        height = bounds.size.y;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if it was a player that stepped onto the pad itself
        if (other.tag == "Player"){
            gotPlayer = other;
            // Getting the player's data so we can manipulate it
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

    void hideButton()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();
        rend.enabled = false;
        Transform mychildtransform = transform.Find("Canvas");
        Destroy(mychildtransform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // This handles the coins being spent
        if (isTriggered && (Time.time - lastRegisteredTime >= delayedSecondsInBetween)) {
            lastRegisteredTime = Time.time;
            if (currentCoinsSpent < coinsNeeded && gotPlayerData.currentCoins > 0) {
                // Player data is in charge of removing the coin from the backpack
                gotPlayerData.SpendCoin(transform.position);
                currentCoinsSpent += 1;
            // If they spent enough coins we begin the building process
            } else if (currentCoinsSpent == coinsNeeded && !building) {
                building = true;
                Vector3 targetPosition = transform.position;
                Vector3 originPosition = targetPosition + Vector3.down * height;
                clonedObject = Instantiate(gotObject, originPosition, Quaternion.identity);
                hideButton();
            }
        }
        // Does the fun tweening from the ground up
        if (building) {
            Vector3 targetPosition = transform.position;
            Vector3 originPosition = targetPosition + Vector3.down * height;
            if (Time.time - time >= buildDelay && buildProgress < buildNeeded) 
            {
                time = Time.time;
                buildProgress += 0.5f;
                clonedObject.transform.position = Vector3.Lerp(originPosition, targetPosition, (float)buildProgress / buildNeeded);
                clonedObject.transform.DORewind();
                clonedObject.transform.DOShakeScale(.2f, .2f, 5, 90, true);
            } else if (buildProgress == buildNeeded) {
                clonedObject.transform.position = targetPosition;
                Destroy(gameObject);
            }
            
        }
    }
}
