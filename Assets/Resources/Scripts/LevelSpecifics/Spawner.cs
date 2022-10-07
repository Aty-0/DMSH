using System.Collections.Generic;
using UnityEngine;

using DMSH.LevelSpecifics.Stage;
using DMSH.Path;
using DMSH.Gameplay;

namespace DMSH.LevelSpecifics
{
    public class Spawner : MonoBehaviour
    {
        [Header("Main")]
        public bool isDone = false;
        public List<GameObject> toSpawn = new List<GameObject>();
        public Timer timer;

        [Tooltip("If spawner attached to point")]
        [SerializeField]
        private PathSystem _pathSystem;

        [HideInInspector] 
        [SerializeField] 
        private StageSystem _stageSystem;


        protected void Start()
        {
            _stageSystem = StageSystem.Get;

            timer = GetComponent<Timer>();

            if (timer != null)
            {
                timer.EndEvent += Spawn;
            }

            _pathSystem = GetComponentInParent<PathSystem>();

            if (_pathSystem != null)
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
            if (timer != null)
            {
                timer.StartTimer();
            }
        }

        public void Spawn()
        {
            if (!isDone)
            {
                foreach (var go in toSpawn)
                {
                    if (go)
                    {
                        // If it's prefab we are create it
                        if (go.scene.name == null) 
                        {
                            _stageSystem.AddToStage(Instantiate(go));
                        }
                        else
                        {
                            if (!go.activeSelf)
                            {
                                _stageSystem.AddToStage(go);
                            }
                        }
                    }
                }

                isDone = true;
            }
        }
    }
}