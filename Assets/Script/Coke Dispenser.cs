using UnityEngine;

public class CokeDispenser : MonoBehaviour
{
    [Header("Settings")]
    public GameObject cokePrefab;   // Drag your 'coke' prefab here
    public Transform spawnPoint;    // Drag the empty 'spawnpoint' object here
    public float throwForce = 2.0f; // How hard the can pops out

    // This is the function you will link to your Canvas Button
    public void SpawnCoke()
    {
        if (cokePrefab != null && spawnPoint != null)
        {
            // 1. Create the coke can
            GameObject newCoke = Instantiate(cokePrefab, spawnPoint.position, spawnPoint.rotation);

            // 2. Make it fall and move
            Rigidbody rb = newCoke.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Pushes the can forward (blue arrow of the spawnPoint)
                rb.AddForce(spawnPoint.forward * throwForce, ForceMode.Impulse);
            }
        }
        else
        {
            Debug.LogError("Hey! You forgot to assign the Coke Prefab or Spawn Point in the Inspector.");
        }
    }
}
