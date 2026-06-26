using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;

    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;
    [SerializeField] private float posY;

    [SerializeField] private float cellSize = 1f;

    public float CellSize => cellSize;

    void Start()
    {
        GenerateGrid();
    }

    public void SetCellSize(float size)
    {
        cellSize = size;
    }

    void GenerateGrid()
    {
        float offsetX = (width * cellSize) / 2f;
        float offsetZ = (height * cellSize) / 2f;

        Quaternion quadRotation = Quaternion.Euler(90f, 0f, 0f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize - offsetX, posY, z * cellSize - offsetZ);

                GameObject cell = Instantiate(cellPrefab, pos, quadRotation, transform);
            }    
        }
    }   

    public void ShowGrid()
    {
        gameObject.SetActive(true); 
    }

    public void HideGrid()
    {
        gameObject.SetActive(false);
    }
}
