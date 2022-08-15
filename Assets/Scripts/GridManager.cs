using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Array2DNavigation
{
    public class GridManager : MonoBehaviour
    {
        #region Variables
        public Tile[,] TilesArray;

        private Tile selectedTile;
        private Agent selectedAgent;
        private Dictionary<Agent, List<Tile>> SelectedPaths = new Dictionary<Agent, List<Tile>>();
        private bool onSimulate = false;
        #endregion

        #region Monobehaviour Callbacks
        private void Start()
        {
            mGenerateAgents();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                Simulate();
        }
        #endregion

        #region Functions
        public void ClickOnTile(Tile tile)
        {
            if (onSimulate || !selectedAgent || tile == selectedAgent.CurrentTile) return;

            selectedTile = tile;
            selectedAgent.OnSelected(false);
            mFindPath();
            selectedAgent = null;
        }
        public void ClickOnAgent(Agent agent)
        {
            if (onSimulate) return;

            if (selectedAgent) selectedAgent.OnSelected(false);
            selectedAgent = agent;
            if (selectedAgent) selectedAgent.OnSelected(true);

            if (SelectedPaths.ContainsKey(selectedAgent))
                foreach (var t in SelectedPaths[selectedAgent])
                    t.OnIndicator(false);
        }
        private void mFindPath()
        {
            if (!SelectedPaths.ContainsKey(selectedAgent)) SelectedPaths.Add(selectedAgent, new List<Tile>());
            SelectedPaths[selectedAgent] = Navigator.GetPath(TilesArray, selectedAgent.CurrentTile, selectedTile);
            foreach (var t in SelectedPaths[selectedAgent])
            {
                t.SetIndicatorColor(selectedAgent.AgentColor);
                t.OnIndicator(true);
            }
        }
        private void mGenerateAgents()
        {
            var agents = Resources.LoadAll<AgentData>("Agents");
            foreach (var a in agents)
            {
                var tile = TilesArray[a.StartPosition.X, a.StartPosition.Y];
                if (tile.Type == Tile.TileType.Empty)
                {
                    var agent = Instantiate(a.AgentPrefab, tile.transform.position, Quaternion.identity).GetComponent<Agent>();
                    agent.Initialize(a.Name, a.Color, a.MaxMoves, tile);
                    agent.OnClick += () => { ClickOnAgent(agent); };
                }
            }
        }

        #endregion

        #region Coroutines
        private IEnumerator mSimulating()
        {
            var wait = new WaitForSeconds(1f);
            foreach (var a in SelectedPaths)
                a.Key.State = Agent.AgentState.Moving;
            var index = 0;
            while (true)
            {
                var standingCount = 0;
                foreach (var a in SelectedPaths)
                {
                    if (a.Key.State == Agent.AgentState.Standing)
                    {
                        standingCount++;
                        continue;
                    }
                    if (index >= a.Value.Count)
                    {
                        a.Key.State = Agent.AgentState.Standing;
                        continue;
                    }
                    if (a.Value[index].Type == Tile.TileType.Ice)
                    {
                        var pretarget = a.Value[index];
                        var prevePosition = index == 0 ? a.Key.CurrentTile.Position : a.Value[index - 1].Position;
                        var newPos = pretarget.Position.GetNext(prevePosition);
                        if (newPos.X >= TilesArray.GetLongLength(0) || newPos.X < 0 || newPos.Y >= TilesArray.GetLongLength(1) || newPos.Y < 0)
                        {
                            a.Key.JumpTo(pretarget);
                            continue;
                        }
                        var target = TilesArray[newPos.X, newPos.Y];
                        if (target.Type == Tile.TileType.Obstacle || !target.CanLand)
                        {
                            a.Key.JumpTo(pretarget);
                            continue;
                        }
                        while (target.Type == Tile.TileType.Ice)
                        {
                            newPos = target.Position.GetNext(pretarget.Position);
                            pretarget = target;
                            if (newPos.X >= TilesArray.GetLongLength(0) || newPos.X < 0 || newPos.Y >= TilesArray.GetLongLength(1) || newPos.Y < 0)
                            {
                                a.Key.JumpTo(pretarget);
                                continue;
                            }
                            target = TilesArray[newPos.X, newPos.Y];
                            if (target.Type == Tile.TileType.Obstacle || !target.CanLand)
                            {
                                a.Key.JumpTo(pretarget);
                                break;
                            }
                        }
                        if (a.Key.State == Agent.AgentState.Moving)
                            a.Key.JumpTo(target);
                        continue;
                    }

                    if (index == a.Value.Count - 1 && a.Value[index].CanLand)
                    {
                        a.Key.MoveTo(a.Value[index],true);
                        continue;
                    }
                    if (index == a.Value.Count - 1 && !a.Value[index].CanLand)
                    {
                        a.Key.JumpTo(a.Key.CurrentTile);
                        continue;
                    }

                    a.Key.MoveTo(a.Value[index]);
                }
                if (standingCount == SelectedPaths.Count) break;
                index++;
                yield return wait;
            }
            onSimulate = false;
            foreach (var a in SelectedPaths)
            {
                a.Key.Charge();
                foreach (var t in a.Value)
                {
                    t.OnIndicator(false);
                    t.CanLand = true;
                }
                a.Value.Clear();
            }
        }
        #endregion

        #region Inspector Buttons
        [NaughtyAttributes.Button]
        private void Simulate()
        {
            if (onSimulate) return;
            onSimulate = true;
            StartCoroutine(mSimulating());
        }
        [NaughtyAttributes.Button]
        private void Stop()
        {
            StopCoroutine(mSimulating());
            onSimulate = false;
            foreach (var a in SelectedPaths)
            {
                a.Key.Charge();
                foreach (var t in a.Value)
                {
                    t.OnIndicator(false);
                    t.CanLand = true;
                }
                a.Value.Clear();
            }
        }
        #endregion
    }
}
