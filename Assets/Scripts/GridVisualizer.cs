using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;

    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;

    [SerializeField] private float cellSize = 1f;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0.01f, z * cellSize);

                Instantiate(cellPrefab, pos, Quaternion.identity, transform);
            }    

        }
    }
}
