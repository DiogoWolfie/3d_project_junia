using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    public float cellSize = 2f;

    public int gridCount = 20;

    public float spawnHeight = 0.25f;


    public bool placeOnStart = true;

    void Start()
    {
        if (placeOnStart) PlaceRandom();
    }


    public void PlaceRandom()
    {
        int ix = Random.Range(0, gridCount); // 0..gridCount-1
        int iz = Random.Range(0, gridCount);

        float x = ix * cellSize; // centros: 0, 2, 4, ...
        float z = iz * cellSize;
        float y = spawnHeight;

        Vector3 newPos = new Vector3(x, y, z);
        //Vector3 newPos = new Vector3(0f, 0.2f, 0f);
        transform.position = newPos;

        Debug.Log($"SimpleKeyPlacer: colocado em célula ({ix},{iz}) -> posição {newPos}");
    }
}
