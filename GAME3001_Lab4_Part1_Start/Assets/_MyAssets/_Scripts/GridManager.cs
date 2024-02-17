using System.Collections.Generic;
using UnityEngine;

public enum TileStatus
{
    UNVISITED,
    OPEN,
    CLOSED,
    IMPASSABLE,
    GOAL,
    START
};

public enum NeighbourTile
{
    TOP_TILE,
    RIGHT_TILE,
    BOTTOM_TILE,
    LEFT_TILE,
    NUM_OF_NEIGHBOUR_TILES
};

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject minePrefab;
    [SerializeField] private GameObject tilePanelPrefab;
    [SerializeField] private GameObject panelParent;
    [SerializeField] private Color[] colors;
    [SerializeField] private bool useManhattanHeuristic = true;
    private GameObject[,] grid;
    private int rows = 12;
    private int columns = 16;
    private float baseTileCost = 1f;

    private List<GameObject> mines = new List<GameObject>();
    public static GridManager Instance { get; private set; } // Static object of the class.

    void Awake()
    {
        if (Instance == null) // If the object/instance doesn't exist yet.
        {
            Instance = this;
            Initialize();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }

    private void Initialize()
    {
        BuildGrid();
        ConnectGrid();
    }

    void Update()
    {
       if(Input.GetKeyDown(KeyCode.G))
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
            panelParent.gameObject.SetActive(!panelParent.gameObject.activeSelf);
        }
       if(Input.GetKeyDown(KeyCode.M))
        {
            Vector2 gridPosition = GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            GameObject mineInst = GameObject.Instantiate(minePrefab, new Vector3(gridPosition.x, gridPosition.y),Quaternion.identity);
            Vector2 mineIndex = mineInst.GetComponent<NavigationObject>().GetGridIndex();
            grid[(int)mineIndex.y, (int)mineIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.IMPASSABLE);
            mines.Add(mineInst);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach(GameObject mine in mines)
            {
                Vector2 mineIndex = mine.GetComponent<NavigationObject>().GetGridIndex();
                grid[(int)mineIndex.y, (int)mineIndex.x].GetComponent<TileScript>().SetStatus(TileStatus.UNVISITED);
                Destroy (mine);
            }
            mines.Clear();
        }
    }

    private void BuildGrid()
    {
        grid = new GameObject[rows, columns];
        int count = 0;
        float rowPos = 5.5f;

        for (int row = 0; row < rows; row++, rowPos--)
        {
            float colPos = -7.5f;
            for (int col = 0; col < columns; col++, colPos++)
            {
                GameObject tileInst = GameObject.Instantiate(tilePrefab,new Vector3(colPos,rowPos,0f),Quaternion.identity);
                TileScript tileScript = tileInst.GetComponent<TileScript>();
                tileScript.SetColor(colors[System.Convert.ToInt32(count++ % 2 == 0)]);
                tileInst.transform.SetParent(transform);
                grid[row,col] = tileInst;

                GameObject panelInst = GameObject.Instantiate(tilePanelPrefab, tilePanelPrefab.transform.position, Quaternion.identity);
                panelInst.transform.SetParent(panelParent.transform);
                RectTransform panelTransform = panelInst.GetComponent<RectTransform>();
                panelTransform.localScale = Vector3.one;
                panelTransform.anchoredPosition = new Vector3(64f * col, -64f * row);
                tileScript.tilePanel = panelInst.GetComponent<TilePanel>();


            }
            count--;
        }
        GameObject ship = GameObject.FindGameObjectWithTag("Ship");
        Vector2 shipIndices = ship.GetComponent<NavigationObject>().GetGridIndex();
        grid[(int)shipIndices.y, (int)shipIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.START);

        GameObject planet = GameObject.FindGameObjectWithTag("Planet");
        Vector2 planetIndices = planet.GetComponent<NavigationObject>().GetGridIndex();
        grid[(int)planetIndices.y, (int)planetIndices.x].GetComponent<TileScript>().SetStatus(TileStatus.GOAL);
    }

    private void ConnectGrid()
    {
        for(int row =0;row<rows;row++)
        {
            for(int col =0; col<columns;col++)
            {
                TileScript tileScript = grid[row,col].GetComponent<TileScript>();
                if(row>0)
                {
                    tileScript.SetNeighbourTile((int)NeighbourTile.TOP_TILE, grid[row - 1, col]);
                }
                if(col<columns-1)
                {
                    tileScript.SetNeighbourTile((int)NeighbourTile.RIGHT_TILE, grid[row, col+1]);
                }
                if(row<rows-1)
                {
                    tileScript.SetNeighbourTile((int)NeighbourTile.BOTTOM_TILE, grid[row + 1, col]);
                }
                if(col>0)
                {
                    tileScript.SetNeighbourTile((int)NeighbourTile.LEFT_TILE, grid[row, col-1]);
                }
            }
        }
    }

    public GameObject[,] GetGrid()
    {
         return grid;
    }

    // The following utility function creates the snapping to the center of a tile.
    public Vector2 GetGridPosition(Vector2 worldPosition)
    {
        float xPos = Mathf.Floor(worldPosition.x) + 0.5f;
        float yPos = Mathf.Floor(worldPosition.y) + 0.5f;
        return new Vector2(xPos, yPos);
    }
    public void SetTileCosts(Vector2 targetIndicies)
    {
        float distance= 0f;
        float dx = 0;
        float dy = 0;

        for(int row = 0; row<rows; row++)
        {
            for(int col = 0; col<columns;col++)
            {
                TileScript tileScript = grid[row,col].GetComponent<TileScript>();

                if(useManhattanHeuristic)
                {
                    dx = Mathf.Abs(col - targetIndicies.x);
                    dy = Mathf.Abs(row - targetIndicies.y);
                    distance = dx+dy;
                }
                else //euclidian
                {
                    dx = targetIndicies.x - col;
                    dy = targetIndicies.y - row;
                    distance = Mathf.Sqrt(dx*dx + dy*dy);
                }
                float adjustedCost = distance * baseTileCost;
                tileScript.cost = adjustedCost;
                tileScript.tilePanel.costText.text = tileScript.cost.ToString("F1");
            }
        }
    }
}
