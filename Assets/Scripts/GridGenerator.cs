using UnityEngine;

namespace Array2DNavigation
{
    public class GridGenerator : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int Rows;
        [SerializeField] private int Columns;
        [SerializeField] private float Scale;
        [SerializeField] private float Spacing;
        [SerializeField] private Vector2 LeftButtomPosition = Vector2.zero;
        [SerializeField] private GridManager Manager;
        #endregion

        #region Monobehaviour Callbacks
        private void Awake()
        {
            mManagerInit();
        }
        #endregion

        #region Functions
        private void mManagerInit()
        {
            if (!Manager) return;

            var grid = GetComponentsInChildren<Tile>();
            Manager.TilesArray = new Tile[grid.Length / Rows, grid.Length / Columns];

            for (int i = 0; i < grid.Length; i++)
            {
                var g = grid[i];
                Manager.TilesArray[i / Rows, i % Rows] = g;
                grid[i].OnClick += () => { Manager.ClickOnTile(g); };
            }
        }
        #endregion

        #region Inspector Buttons
        [NaughtyAttributes.Button]
        private void Generate()
        {
            var data = Resources.Load<GridData>("GridData");
            var grid = GetComponentsInChildren<Tile>();
            foreach (var t in grid)
                DestroyImmediate(t.gameObject);

            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    var tileGo = Instantiate(data.TilePrefab, new Vector2(LeftButtomPosition.x + (Spacing + Scale) * i, LeftButtomPosition.y + (Spacing + Scale) * j)
                        , Quaternion.identity, transform);
                    tileGo.name = i.ToString() + ", " + j.ToString();
                    tileGo.transform.localScale = new Vector3(Scale, Scale, Scale);
                    var tile = tileGo.GetComponent<Tile>();
                    tile.Position.X = i;
                    tile.Position.Y = j;
                }
            }
        }
        #endregion
    }
}