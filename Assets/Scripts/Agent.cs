using UnityEngine;
using System;

namespace Array2DNavigation
{
    public class Agent : MonoBehaviour
    {
        #region Variables
        public Action OnClick;
        public Color AgentColor { get { return GetComponent<SpriteRenderer>().color; } }
        [HideInInspector] public AgentState State = AgentState.Standing;

        private Tile currentTile;
        private int maxMoves;
        private int remainingMoves;
        #endregion

        #region Properties
        public Tile CurrentTile
        {
            get { return currentTile; }
            set
            {
                if (currentTile) currentTile.Agents.Remove(this);
                currentTile = value;
                currentTile.Agents.Add(this);
            }
        }
        #endregion

        #region Monobehaviour Callbacks
        private void OnMouseDown()
        {
            OnClick?.Invoke();
        }
        #endregion

        #region Functions
        public void Initialize(string name, Color color, int maxMoves, Tile started)
        {
            gameObject.name = name;
            GetComponent<SpriteRenderer>().color = color;
            this.maxMoves = maxMoves;
            Charge();
            currentTile = started;
        }

        public void Charge()
        {
            remainingMoves = maxMoves;
        }
        public void OnSelected(bool active)
        {
            if (active) transform.localScale = Vector3.one * 2.5f;
            else transform.localScale = Vector3.one * 1.5f;
        }
        public void MoveTo(Tile target,bool lastStep=false)
        { 
            CurrentTile = target;
            currentTile.CanLand = !lastStep;
            transform.position = target.transform.position;
            remainingMoves--;
            if (remainingMoves == 0)
                State = AgentState.Standing;
        }
        public void JumpTo(Tile target)
        {
            remainingMoves = 0;
            State = AgentState.Standing;
            CurrentTile = target;
            currentTile.CanLand = false;
            transform.position = target.transform.position;
        }
        #endregion

        #region Enums
        public enum AgentState
        {
            Moving,
            Standing
        }
        #endregion
    }
}