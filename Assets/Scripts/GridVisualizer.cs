using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.Rendering;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;

    [SerializeField] private int width = 20;
    [SerializeField] private int height = 20;
    [SerializeField] private float posY;

    [SerializeField] private float cellSize = 1f;

    private GridCell[,] cells;  //Stores references to every generated GridCell. This allows changing colours later without searching the scene.

    public float CellSize => cellSize;

    void Start()
    {
        GenerateGrid();
        HideGrid();
    }

    void GenerateGrid()
    {
        //Creates a 2D array with the same dimensions as the visual grid.
        cells = new GridCell[width, height];

        float offsetX = (width * cellSize) / 2f; 
        float offsetZ = (height * cellSize) / 2f;

        //Rotate the Quad so it lies flat on the ground.
        Quaternion quadRotation = Quaternion.Euler(90f, 0f, 0f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Vector3 pos = GetCellPosition(x, z);
                Vector3 pos = new Vector3(x * cellSize - offsetX, posY, z * cellSize - offsetZ);

                GameObject cell = Instantiate(cellPrefab, pos, quadRotation, transform);

                //Store its GridCell component.
                cells[x, z] = cell.GetComponent<GridCell>();
            }    
        }
    }

    public void UpdateGrid(BuildManager buildManager)
    {
        float offsetX = (width * cellSize) / 2f; 
        float offsetZ = (height * cellSize) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                //Vector3 pos = GetCellPosition(x, z);
                Vector3 pos = new Vector3(x * cellSize - offsetX, posY, z * cellSize - offsetZ);

                //Ask BuildManager if this location is valid.
                bool valid = buildManager.IsValidPosition(pos);

                if (valid) cells[x, z].SetColor(new Color(0, 1, 0, 0.2f));
                else cells[x, z].SetColor(new Color(1, 0, 0, 0.2f));
            }
        }
    }

    private Vector3 GetCellPosition(int x, int z)
    {
        float offsetX = (width * cellSize) / 2f;
        float offsetZ = (height * cellSize) / 2f;

        //Calculate this cell's position.
        return new Vector3(x * cellSize - offsetX, posY,  z * cellSize - offsetZ);
    }

    public void SetColor(int x, int z, Color color)
    {
        //Prevent index out of range errors.
        if (x < 0 || x >= width) return;
        if (z < 0 || z >= height) return;

        cells[x, z].SetColor(color);
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
