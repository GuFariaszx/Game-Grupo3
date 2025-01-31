using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{

    public float threshold = -10f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            transform.position = new Vector3(360.89f, 0f, -99.6f);
        }
    }
}
