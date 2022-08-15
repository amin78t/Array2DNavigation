using System.Collections.Generic;
using UnityEngine;
using System;

namespace Array2DNavigation
{
    public class Tile : MonoBehaviour
    {
        #region Variables
        public Action OnClick;
        [HideInInspector] public bool CanLand = true;
        [HideInInspector] public TileType Type;
        [HideInInspector] public TilePosition Position;
        public HashSet<Agent> Agents = new HashSet<Agent>();

        [SerializeField] private GameObject indicator;
        private SpriteRenderer indicatorRenderer;
        #endregion

        #region Monobehaviour Callbacks
        private void Awake()
        {
            indicatorRenderer = indicator.GetComponent<SpriteRenderer>();
        }
        private void OnMouseDown()
        {
            OnClick?.Invoke();
        }
        #endregion

        #region Functions
        public void OnIndicator(bool active)
        {
            indicator.SetActive(active);
        }
        public void SetIndicatorColor(Color color)
        {
            indicatorRenderer.color = color;
        }
        #endregion

        #region Inspector Buttons
        [NaughtyAttributes.Button]
        private void EMPTY()
        {
            Type = TileType.Empty;
            GetComponent<SpriteRenderer>().color = Resources.Load<GridData>("GridData").DefaultColor;
        }
        [NaughtyAttributes.Button]
        private void ICE()
        {
            Type = TileType.Ice;
            GetComponent<SpriteRenderer>().color = Resources.Load<GridData>("GridData").IceColor;
        }
        [NaughtyAttributes.Button]
        private void OBSTACLE()
        {
            Type = TileType.Obstacle;
            GetComponent<SpriteRenderer>().color = Resources.Load<GridData>("GridData").ObstacleColor;
        }
        #endregion

        #region Serializable Classes
        [Serializable]
        public class TilePosition
        {
            public int X;
            public int Y;

            public int GetMagnitudeDistanceTo(TilePosition b)
            {
                return Mathf.Abs(b.X - X) + Mathf.Abs(b.Y - Y);
            }
            public TilePosition GetNext(TilePosition previous)
            {
                var result = new TilePosition();
                result.X = X + (X - previous.X);
                result.Y = Y + (Y - previous.Y);
                return result;
            }
        }
        #endregion

        #region Enums
        public enum TileType
        {
            Empty,
            Ice,
            Obstacle
        }
        #endregion
    }
}