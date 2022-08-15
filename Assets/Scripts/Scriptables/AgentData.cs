using UnityEngine;
namespace Array2DNavigation
{
    [CreateAssetMenu(fileName = "AgentData", menuName = "Data/AgentData")]
    public class AgentData : ScriptableObject
    {
        public string Name;
        public int MaxMoves;
        public Color Color;
        public Tile.TilePosition StartPosition;
        public GameObject AgentPrefab;
    }
}