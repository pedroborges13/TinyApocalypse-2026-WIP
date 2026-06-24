using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;

    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;

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
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0.65f, z * cellSize);

                Quaternion quadRotation = Quaternion.Euler(90f, 0f, 0f);

                GameObject cell = Instantiate(cellPrefab, pos, quadRotation, transform);

                Debug.Log("Generate Grid");
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
