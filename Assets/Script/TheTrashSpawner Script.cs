using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
  
    public GameObject trashPrefab;   // Drag your 'paper wad' or 'trash' prefab here

    public void SpawnTrash()
    {
        Instantiate(trashPrefab, new Vector3(5,1,-1), Quaternion.identity);
    }
}