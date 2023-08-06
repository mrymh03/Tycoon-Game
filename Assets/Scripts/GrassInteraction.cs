using UnityEngine;

public class GrassInteraction : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_PositionMoving", transform.position);
    }
}