using DMSH.Characters;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

using UnityEngine;

using DMSH.Misc;
using DMSH.Gameplay;
using DMSH.LevelSpecifics.Stage;

using Scripts.Utils;
using Scripts.Utils.Pools;

namespace DMSH.Path
{  
    public class PathSystem : MonoBehaviourWithUniqueID
    {        
        [Header("Main")]
        [HideInInspector]
        [SerializeField]
        private StageSystem stageSystem = null;

        public ObservedList<MovableObject> movablePathObjectsList = new ObservedList<MovableObject>();
        public List<PathPoint> pathPointsList = new List<PathPoint>();
        public bool loop = true;
       
        [Header("Spawner")]
        public int objectCount = 0;
        public int spawnedObjectCount = 0;
        public Enemy objectPrefab = null;
        public float spawnerTimerTime = 0.5f;
        public float spawnerTimerTick = 0.5f;
        private Timer _spawnerTimer = null;

        private Coroutine _spawnCoroutine;

        [Header("Misc")]
        public float distanceBetweenObjects = 2.0f;
        public float maxAugmentSpeed = 5.0f;

        [Tooltip("Distance accuracy for reach point")]
        public float distanceAccuracy = 0.01f;

        [Header("Editor")]
        public Color lineColor = Color.green;

        [Header("Lifetime")]
        public float lifeTime = 0.0f;
        [SerializeField]
        private Timer _lifetimeTimer = null;

        //[Header("Current")]
        [HideInInspector]
        public PathPoint currentPathPoint = null;
        [HideInInspector] 
        public PathPoint nextPathPoint = null;
        [HideInInspector] 
        public MovableObject currentPathObject = null;

        [Header("Callbacks")]
        public List<Action> onMovableObjectsAdded = new List<Action>();
        public List<Action> onMovableObjectsChanged = new List<Action>();
        public List<Action> onMovableObjectsRemoved = new List<Action>();
        public List<PointAction> onLastMovableObjectReached = new List<PointAction>();

        protected void Start()
        {
            movablePathObjectsList.Updated += UpdateElementChangedCallback;
            movablePathObjectsList.Added += UpdateElementAddedCallback;
            movablePathObjectsList.Removed += OnMovablePathObjectsListElementRemoved;

            stageSystem = StageSystem.Get;
            if (stageSystem == null)
                Debug.LogError("No stage system in scene");

            // Set to object what's path system he use
            foreach (var move_object in movablePathObjectsList)
            {
                if (move_object != null)
                {
                    move_object.PathSystem = this;
                }
            }
        }

        public void OnLifetimeEnd()
        {
            loop = false;

            foreach (var mO in movablePathObjectsList.ToList())
            {
                mO.CurrentPoint = pathPointsList.Count - 1;
            }
        }

        private void SpawnObject()
        {
            spawnedObjectCount++;
            var spawnedEnemy = EnemyPool.GetOrCreate();
            objectPrefab.CopyTo(spawnedEnemy);
            spawnedEnemy.transform.SetPositionAndRotation(pathPointsList[0].transform.position, Quaternion.identity);
            spawnedEnemy.PathSystem = this;
            spawnedEnemy.name = $"{spawnedEnemy.name}{spawnedObjectCount}";
            // ? spawnedEnemy.transform.parent = transform.parent;
            
            movablePathObjectsList.Add(spawnedEnemy);

            if (spawnedObjectCount == objectCount)
            {
                StopCoroutine(_spawnCoroutine);
                Destroy(_spawnerTimer);
            }
            else
            {
                _spawnerTimer.ResetTimer();
            }
        }
        private IEnumerator StartSpawning()
        {
            while (spawnedObjectCount <= objectCount)
            {
                if (_spawnerTimer.isEnded == true ||
                    _spawnerTimer.isStarted == false)
                {
                    _spawnerTimer.StartTimer();                    
                }
      
                yield return null;
            }
        }

        public void EnableSpawner()
        {
            if (objectCount != 0)
            {
                _spawnerTimer = gameObject.AddComponent<Timer>();
                _spawnerTimer.tick = spawnerTimerTick;
                _spawnerTimer.time = spawnerTimerTime;
                Debug.Assert(pathPointsList[0] != null);
                Debug.Assert(objectPrefab != null);
                _spawnerTimer.EndEvent += SpawnObject;
                if (objectPrefab != null)
                {
                    _spawnCoroutine = StartCoroutine(StartSpawning());
                }
            }

            if (lifeTime > 0.0f)
            {
                _lifetimeTimer = gameObject.AddComponent<Timer>();
                _lifetimeTimer.time = lifeTime;
                _lifetimeTimer.EndEvent += OnLifetimeEnd;
                _lifetimeTimer.StartTimer();
            }
        }

        public void UpdateElementChangedCallback()
        {
            foreach (Action action in onMovableObjectsChanged)
            {
                action?.Invoke();
            }
        }

        public void UpdateElementRemovedCallback()
        {
            foreach (Action action in onMovableObjectsRemoved)
            {
                action?.Invoke();
            }

            foreach (PointAction pa in onLastMovableObjectReached)
            {
                if (movablePathObjectsList.Count == 0)
                {
                    pa.action?.Invoke();
                }
            }
        }

        public void UpdateElementAddedCallback()
        {
            foreach (Action action in onMovableObjectsAdded)
            {
                action?.Invoke();
            }
        }

        public void DetachObject(MovableObject movableObject)
        {
            movablePathObjectsList.Remove(movableObject);
        }

        public void OnMovablePathObjectsListElementRemoved()
        {
            // If all enemy is dead pass current scenario list
            if (!movablePathObjectsList.Any() && ((_spawnerTimer == null) ? true : _spawnerTimer.isEnded))
            {
                UpdateElementRemovedCallback();
                stageSystem.AddedPass();
            }
        }

        public bool CheckPointOnValid(int index)
        {
            // First we are check points storage 
            // Second we are check index on valid 
            // Then we are try to get object with that index and check it to null
            if (pathPointsList.Any() && (index < pathPointsList.Count))
                if (pathPointsList[index] != null)
                    return true;

            return false;
        }
       
        private void IsReached(PathPoint point, MovableObject move_object)
        {
            //Debug.Log($"[PathSystem] IsReached {pathPointsList.IndexOf(point)} {movablePathObjectsList.IndexOf(move_object)} {movablePathObjectsList.Count}");

            // Invoke points events 
            if (point.eventSpecial?.GetPersistentEventCount() != 0)
            {
                point.eventSpecial.Invoke();
            }

            // Invoke custom function only when last object reached needed point 
            if (movablePathObjectsList.IndexOf(move_object) == (movablePathObjectsList.Count - 1))
            {
                foreach (PointAction pa in onLastMovableObjectReached)
                {
                    if (currentPathPoint.gameObject == pa.pathPoint.gameObject)
                    {
                        pa.action?.Invoke();
                    }
                }
            }
             
            // Invoke MovableObject events 
            move_object.OnReachedPointEvent(point.eventOnEndForAll);

            if (pathPointsList.IndexOf(point) == (pathPointsList.Count - 1))
            {
                move_object.OnReachedLastPoint();
            }
            else if (pathPointsList.IndexOf(point) == 0)
            {
                move_object.OnReachedFirstPoint();
            }
            else
            {
                move_object.OnReachedPoint();
            }

            move_object.CurrentCurvePoint = 1;
            move_object.CurrentPoint++;
        }

#if UNITY_EDITOR 
        protected void OnGUI()
        {
            if (GlobalSettings.debugDrawPSObjectInfo)
            {
                foreach (var move_object in movablePathObjectsList.ToList())
                {
                    if (move_object != null)
                    {
                        Vector3 convertedPos = Camera.main.WorldToScreenPoint(new Vector3(move_object.transform.position.x, -move_object.transform.position.y, 0));
                        GUI.Label(new Rect(convertedPos.x, convertedPos.y, 400, 400),
                            $"iObj: {movablePathObjectsList.IndexOf(move_object)}\n" +
                            $"iPnt: {move_object.CurrentPoint}\n" +
                            $"S: {move_object.speed}\n" +
                            $"fS:{move_object.FinalSpeed}\n" +
                            $"aS: {move_object.AugmentSpeed}\n" +
                            $"rS: {move_object.ReduceSpeed}");
                    }
                }
            }
        }

        protected void OnDrawGizmos()
        {
            if (GlobalSettings.debugDrawPSCurrentMovement)
            {
                if (nextPathPoint != null && currentPathObject != null)
                {
                    var useCurve = currentPathPoint.useCurve;

                    if (useCurve)
                    {
                        Gizmos.color = Color.green;
                        MathCurve.LineFollowedByPath(currentPathPoint.transform.position, nextPathPoint.curvePoint, nextPathPoint.transform.position);
                        MathCurve.CubeFollowedByPath(currentPathPoint.transform.position, nextPathPoint.curvePoint, nextPathPoint.transform.position);

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawCube(currentPathPoint.curvePoint, new Vector3(0.5f, 0.5f, 0.5f));
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(currentPathPoint.transform.position, nextPathPoint.transform.position);
                    }
                    
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(currentPathObject.transform.position, nextPathPoint.transform.position);
                }
            }

            if (GlobalSettings.debugDrawPSAllPoints)
            {
                PathPoint prev = null;

                if (movablePathObjectsList.Count != 0)
                {
                    foreach (var point in pathPointsList.ToList())
                    {
                        if (prev)
                        {
                            var useCurve = point.useCurve;
                            var currentPos = point.transform.position;
                            Gizmos.color = new Color(1, 1, 1, 0.4f);

                            if (useCurve)
                            {
                                MathCurve.LineFollowedByPath(prev.transform.position, point.curvePoint, point.transform.position);
                                MathCurve.CubeFollowedByPath(prev.transform.position, point.curvePoint, point.transform.position);
                                Gizmos.DrawCube(point.curvePoint, new Vector3(0.2f, 0.2f, 0.2f));
                            }
                            else
                            {
                                Gizmos.DrawLine(prev.transform.position, point.transform.position);
                            }
                        }

                        Gizmos.DrawLine(pathPointsList[pathPointsList.Count - 1].transform.position, pathPointsList[0].transform.position);

                        prev = point;
                    }
                }
            }

        }
#endif

        protected void Update()
        {
            // Check both lists on object exist
            if (pathPointsList.Count == 0 || movablePathObjectsList.Count == 0)
                return;
          
            MovableObject futurePathObject = null;
            // TODO: Slip movement
            foreach (var mO in movablePathObjectsList.ToList())
            {
                currentPathObject = mO;

                // If current path object is null we are skip that object and remove it from list
                if (currentPathObject == null)
                {
                    movablePathObjectsList.Remove(currentPathObject);
                    continue;
                }

                // If we are have excess point
                if (currentPathObject.CurrentPoint >= pathPointsList.Count)
                {
                    // If loop on we are reset current point num 
                    if (loop)
                    {
                        currentPathObject.CurrentPoint = 0;
                    }
                    else
                    {
                        Debug.Log($"Unspawn: {currentPathObject.name}");
                        currentPathObject.UnSpawn();
                        movablePathObjectsList.Remove(currentPathObject);
                    }

                    continue; // Move to next object
                }

                // Get future path object
                int fi = movablePathObjectsList.IndexOf(currentPathObject) + 1;
                futurePathObject = fi < movablePathObjectsList.Count ? movablePathObjectsList[fi] : movablePathObjectsList[0];
                
                // Get current point 
                int i =  currentPathObject.CurrentPoint;
                currentPathPoint = pathPointsList[i];

                // Get next point 
                int npi = i + 1; 
                nextPathPoint = npi < pathPointsList.Count ? pathPointsList[npi] : pathPointsList[0];
                                        
                var distanceBetweenCurrentObjects = Vector3.Distance(currentPathObject.transform.position, futurePathObject.transform.position);

                // Will protect objects from stucking 
                if (distanceBetweenCurrentObjects < 1)
                {
                    currentPathObject.ReduceSpeed = Mathf.Clamp(Mathf.Clamp(currentPathObject.ReduceSpeed, distanceBetweenObjects / distanceBetweenCurrentObjects,
                        Mathf.Clamp(distanceBetweenCurrentObjects, 0.0f, 1.0f / distanceBetweenObjects)), 0.001f, 1.0f);
                }
                else
                {
                    currentPathObject.ReduceSpeed = 1;
                }

                // Will boost up speed when object is stand behind 
                if (distanceBetweenCurrentObjects > distanceBetweenObjects)
                {
                    currentPathObject.AugmentSpeed = Mathf.Clamp(Mathf.Clamp(distanceBetweenCurrentObjects, 1.0f, distanceBetweenCurrentObjects) / distanceBetweenObjects, 0.01f, maxAugmentSpeed);
                }
                else
                {
                    currentPathObject.AugmentSpeed = 1;
                }

                // Calculate final speed 
                currentPathObject.FinalSpeed = (currentPathObject.speed * Time.deltaTime * currentPathObject.ReduceSpeed * currentPathObject.AugmentSpeed) * GlobalSettings.GameActiveAsInt;

            
                // What's mode we need to use 
                if (currentPathPoint.useCurve)
                {
                    // Check current curve point 
                    if (currentPathObject.CurrentCurvePoint <= MathCurve.PATH_CURVE_LINE_STEPS)
                    {
                        if (nextPathPoint != pathPointsList[0])
                        {
                            var lineEnd = MathCurve.MakeCurve(currentPathPoint.transform.position, nextPathPoint.curvePoint,
                                nextPathPoint.transform.position, currentPathObject.CurrentCurvePoint / (float)MathCurve.PATH_CURVE_LINE_STEPS);
                            var curveEndDistance = Vector3.Distance(currentPathObject.transform.position, lineEnd);
                            var currentObjectPosition = currentPathObject.transform.position;
                            var newPosition = Vector3.MoveTowards(currentObjectPosition, lineEnd, currentPathObject.FinalSpeed);
                            currentPathObject.MoveDirection = (newPosition - currentObjectPosition).normalized;
                            currentPathObject.transform.position = newPosition;
                            if (curveEndDistance <= distanceAccuracy)
                            {
                                currentPathObject.CurrentCurvePoint++;
                            }
                        }
                        else
                        {
                            // Not critical because we can disable this flag 
                            //Debug.LogWarning($"{name} - index: {i} | that point is last in the list and have curve mode!");

                            // Disable useCurve flag because last point can't have a curve mode
                            currentPathPoint.useCurve = false;
                        }

                    }
                    else
                    {
                        IsReached(currentPathPoint, currentPathObject);
                        continue;
                    }
                }
                else
                {
                    // Check distance for current object
                    var distance = Vector3.Distance(currentPathObject.transform.position, currentPathPoint.transform.position);
                    var currentObjectPosition = currentPathObject.transform.position;
                    var newPosition = Vector3.MoveTowards(currentObjectPosition, currentObjectPosition, currentPathObject.FinalSpeed);
                    currentPathObject.MoveDirection = (newPosition - currentObjectPosition).normalized;
                    currentPathObject.transform.position = newPosition;
                    // If we are get close for point
                    if (distance <= distanceAccuracy)
                    {
                        IsReached(currentPathPoint, currentPathObject);
                        continue;
                    }
                }
            }
        }
    }
}