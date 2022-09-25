using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using DMSH.Misc;
using DMSH.LevelSpecifics.Stage;


namespace DMSH.Path
{  
    public class PathSystem : MonoBehaviour
    {        
        [Header("Spawner")]
        public int objectCount = 0;
        public int spawnedObjectCount = 0;
        public MovableObject objectPrefab = null;
        public float spawnerTimerTime = 0.5f;
        public float spawnerTimerTick = 0.5f;
        private Timer _spawnerTimer = null;

        [Header("PathSystem")]
        public ObservedList<MovableObject> movablePathObjectsList = new ObservedList<MovableObject>();
        public List<PathPoint> pathPointsList = new List<PathPoint>();
        public bool loop = true;
       
        [Header("Misc")]
        public bool holdDistanceBetweenObjects = true;
        // FIX ME: At this moment this working weird
        public bool catchUpNextObject = false; 
        public float distanceAccuracy = 0.01f;
        public float distanceBetweenObjects = 2.0f;
        public StageSystem stageSystem = null;

        [Header("Editor")]
        public Color lineColor = Color.green;

        [Header("Actions")]
        public List<Action> onMovableObjectsAdded = new List<Action>();
        public List<Action> onMovableObjectsChanged = new List<Action>();
        public List<Action> onMovableObjectsRemoved = new List<Action>();

        public List<PointAction> onLastMovableObjectReached = new List<PointAction>();

        [Header("Lifetime")]
        public float lifeTime = 0.0f;
        private Timer _lifetimeTimer = null;

        [Header("Current")]
        public PathPoint currentPathPoint = null;
        public PathPoint nextPathPoint = null;
        public MovableObject currentPathObject = null;

        protected void Start()
        {
            movablePathObjectsList.Updated += UpdateElementChangedCallback;
            movablePathObjectsList.Added += UpdateElementAddedCallback;
            movablePathObjectsList.Removed += OnMovablePathObjectsListElementRemoved;

            stageSystem = FindObjectOfType<StageSystem>();
            Debug.Assert(stageSystem != null, "No stage system in scene");

            // Set to object what's path system he use
            foreach (var move_object in movablePathObjectsList)
                if (move_object != null)
                    move_object.pathSystem = this;
        }

        public void OnLifetimeEnd()
        {
            // TODO: How we can direct every object to last point immediately ?
            loop = false;
        }

        #region Spawner
        private void SpawnObject()
        {
            spawnedObjectCount++;
            MovableObject movableObject = Instantiate(objectPrefab, pathPointsList[0].transform.position, Quaternion.identity);
            movableObject.pathSystem = this;
            movableObject.name = $"{movableObject.name}{spawnedObjectCount}";
            movableObject.transform.parent = transform.parent;
            movablePathObjectsList.Add(movableObject);
            if (spawnedObjectCount == objectCount)
                _spawnerTimer.EndTimer();
            else
                _spawnerTimer.ResetTimer();
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
                    for (int i = 1; i <= objectCount && !_spawnerTimer.isEnded; i++)
                        _spawnerTimer.StartTimer();
            }

            if (lifeTime > 0.0f)
            {
                _lifetimeTimer = gameObject.AddComponent<Timer>();
                _lifetimeTimer.time = lifeTime;
                _lifetimeTimer.EndEvent += OnLifetimeEnd;
                _lifetimeTimer.StartTimer();
            }
        }
        #endregion

        #region Utils
        public void UpdateElementChangedCallback()
        {
            foreach (Action action in onMovableObjectsChanged)
                action?.Invoke();
        }

        public void UpdateElementRemovedCallback()
        {
            foreach (Action action in onMovableObjectsRemoved)
                action?.Invoke();

            foreach (PointAction pa in onLastMovableObjectReached)
                if (movablePathObjectsList.Count == 0)
                    pa.action?.Invoke();
        }

        public void UpdateElementAddedCallback()
        {
            foreach (Action action in onMovableObjectsAdded)
                action?.Invoke();
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
                point.eventSpecial.Invoke();

            // Invoke custom function only when last object reached needed point 
            if (movablePathObjectsList.IndexOf(move_object) == (movablePathObjectsList.Count - 1))
            {
                foreach (PointAction pa in onLastMovableObjectReached)
                    if (currentPathPoint.gameObject == pa.pathPoint.gameObject)
                        pa.action?.Invoke();
            }
             
            // Invoke MovableObject events 
            move_object.OnReachedPointEvent(point.eventOnEndForAll);

            if (pathPointsList.IndexOf(point) == (pathPointsList.Count - 1))
                move_object.OnReachedLastPoint();
            else if (pathPointsList.IndexOf(point) == 0)
                move_object.OnReachedFirstPoint();
            else
                move_object.OnReachedPoint();

            move_object.currentCurvePoint = 1;
            move_object.currentPoint++;
        }
        #endregion

        #region Debug        
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
                            $"iPnt: {move_object.currentPoint}\n" +
                            $"S: {move_object.speed}\n" +
                            $"fS:{move_object.finalSpeed}\n" +
                            $"aS: {move_object.augmentSpeed}\n" +
                            $"rS: {move_object.reduceSpeed}");
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

#endregion

        #region Update
        protected void Update()
        {
            // Check both lists on object exist
            if (!pathPointsList.Any() || !movablePathObjectsList.Any())
                return;

            // FIXME: I'm not sure about using this function in update method

            // Will protect from unexpected deleted objects 
            movablePathObjectsList.RemoveAll(o => o == null);

            MovableObject previousObject = null;

            // TODO: Slip movement
            foreach (var mO in movablePathObjectsList.ToList())
            {
                currentPathObject = mO;
                if (currentPathObject == null)
                    continue;

                // Get current point 
                int i = currentPathObject.currentPoint;

                // If we are have excess point
                if (currentPathObject.currentPoint >= pathPointsList.Count)
                {
                    // If loop on we are reset current point num 
                    if (loop)
                        currentPathObject.currentPoint = 0;
                    else
                    {
                        Debug.Log($"Unspawn: {currentPathObject.name}");
                        currentPathObject.Unspawn();
                    }

                    continue; // Move to next object
                }

                // Get current point point 
                currentPathPoint = pathPointsList[i];

                if (pathPointsList.IndexOf(currentPathPoint) + 1 != pathPointsList.Count)
                    nextPathPoint = pathPointsList[i + 1];
                else
                    nextPathPoint = null;

                // Calculate final speed 
                currentPathObject.finalSpeed = (currentPathObject.speed * Time.deltaTime) * GlobalSettings.gameActiveAsInt; 

                // Calculate reduce, augment speed for protecting objects from stacking or moving too far from previous                 
                if (previousObject)
                {
                    var distanceBetweenCurrentObjects = Vector3.Distance(currentPathObject.transform.position, previousObject.transform.position);

                    if (holdDistanceBetweenObjects)
                    {
                        currentPathObject.reduceSpeed = Mathf.Clamp(currentPathObject.reduceSpeed, 1.0f, Mathf.Clamp(distanceBetweenCurrentObjects, 0.0f, 2.0f / distanceBetweenObjects));
                        currentPathObject.finalSpeed *= currentPathObject.reduceSpeed;
                    }

                    if (catchUpNextObject)
                    {
                        currentPathObject.augmentSpeed = Mathf.Clamp(currentPathObject.augmentSpeed, distanceBetweenCurrentObjects, distanceBetweenObjects);
                        currentPathObject.finalSpeed *= currentPathObject.augmentSpeed;
                    }
                }

                // What's mode we need to use 
                if (currentPathPoint.useCurve)
                {
                    // Check current curve point 
                    if (currentPathObject.currentCurvePoint <= MathCurve.PATH_CURVE_LINE_STEPS)
                    {
                        if (CheckPointOnValid(i + 1))
                        {
                            var lineEnd = MathCurve.MakeCurve(currentPathPoint.transform.position, pathPointsList[i + 1].curvePoint,
                                pathPointsList[i + 1].transform.position, currentPathObject.currentCurvePoint / (float)MathCurve.PATH_CURVE_LINE_STEPS);
                            var curveEndDistance = Vector3.Distance(currentPathObject.transform.position, lineEnd);
                            currentPathObject.transform.position = Vector3.MoveTowards(currentPathObject.transform.position, lineEnd, currentPathObject.finalSpeed);
                            if (curveEndDistance <= distanceAccuracy)
                                currentPathObject.currentCurvePoint++;
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
                    currentPathObject.transform.position = Vector3.MoveTowards(currentPathObject.transform.position, currentPathPoint.transform.position, currentPathObject.finalSpeed);
                    // If we are get close for point
                    if (distance <= distanceAccuracy)
                    {
                        IsReached(currentPathPoint, currentPathObject);
                        continue;
                    }
                }

                previousObject = currentPathObject;
            }


        }
        #endregion
    }
}