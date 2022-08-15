using UnityEngine;
namespace Array2DNavigation
{
    [CreateAssetMenu(fileName = "GridData", menuName = "Data/GridData")]
    public class GridData : ScriptableObject
    {
        public GameObject TilePrefab;
        public Color DefaultColor;
        public Color IceColor;
        public Color ObstacleColor;
    }
}