using System.Collections.Generic;
using UnityEngine;

using DMSH.LevelSpecifics.Stage;
using DMSH.Path;
using DMSH.Misc;

namespace DMSH.LevelSpecifics
{
    public class Spawner : MonoBehaviour
    {
        [Header("Main")]
        public bool isDone = false;
        public List<GameObject> toSpawn = new List<GameObject>();
        public Timer timer;

        [Header("Misc")]
        [SerializeField] private StageSystem _stageSystem;

        [Tooltip("If spawner attached to point")]
        [SerializeField] private PathSystem _pathSystem;

        protected void Start()
        {
            _stageSystem = FindObjectOfType<StageSystem>();
            timer = GetComponent<Timer>();
            if (timer)
                timer.EndEvent += Spawn;

            _pathSystem = GetComponentInParent<PathSystem>();

            if (_pathSystem)
            {
                PointAction pa = new PointAction();
                pa.action += Spawn;
                pa.pathPoint = gameObject.GetComponent<PathPoint>();
                // When last movable object is reached needed point we are activate spawner
                _pathSystem.onLastMovableObjectReached.Add(pa);
            }

        }

        public void StartTimer()
        {
            timer?.StartTimer();
        }

        public void Spawn()
        {
            if (!isDone)
            {
                foreach (GameObject go in toSpawn)
                {
                    if (go)
                    {
                        if (go.scene.name == null) // this is prefab
                            _stageSystem.AddToStage(Instantiate(go));
                        else
                             if (!go.activeSelf)
                                _stageSystem.AddToStage(go);
                    }
                }

                isDone = true;
            }
        }
    }
}