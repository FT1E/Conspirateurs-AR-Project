using UnityEngine;

public class BoardGenerator : MonoBehaviour
{

    // prefab for tile
    public GameObject tilePrefab;

    // materials/textures for tiles
    public Material whiteTile, blackTile;
    public Material starterTile1, starterTile2;
    public Material finishTile;
    // end materials/textures for tiles

    // variables for options
    public int startX, startY, endX, endY;
    public int spacing;
    public int offsetX, offsetY;
    // end variables for options

    // GameObject references
    public GameObject[,] tiles; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tiles = new GameObject[7, 7];
        GenerateBoard();
    }

    private void GenerateBoard() 
    {

        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
            {
                Vector3 pos = new Vector3(x * spacing - offsetX, transform.position.y, y * spacing - offsetY);
                tiles[x, y] = Instantiate(tilePrefab, pos , Quaternion.identity, this.transform);
                tiles[x, y].name = $"Tile_{x}_{y}";
                tiles[x, y].tag = "board_tile";
                SpotScript script = tiles[x, y].GetComponent<SpotScript>();
                script.Init(x, y);

                Renderer renderer = tiles[x, y].GetComponent<Renderer>();

                if (IsStarterTile(x, y))
                {
                    renderer.material = (x + y) % 2 == 0 ? starterTile1 : starterTile2;
                }
                else if (IsFinishTile(x, y))
                {
                    renderer.material = finishTile;
                }
                else 
                {
                    renderer.material = (x + y) % 2 == 0 ? whiteTile : blackTile;
                }

            }
        }
    }

    public static bool IsStarterTile(int x, int y)
    {
        return (x >= 2 && y >= 2 && x <= 4 && y <= 5);
    }
    public static bool IsFinishTile(int x, int y)
    {
        if (x == 0)
        {
            switch (y)
            {
                case 0:
                case 2:
                case 4:
                case 6:
                    return true;
                default:
                    break;
            }
        }
        else if (x == 6)
        {
            switch (y)
            {
                case 0:
                case 4:
                case 6:
                    return true;
                default:
                    break;
            }
        }
        else if (y == 0)
        {
            return (x == 2 || x == 4);
        }
        return false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
