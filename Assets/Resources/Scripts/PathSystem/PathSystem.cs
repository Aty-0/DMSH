using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using DMSH.Misc;
using DMSH.LevelSpecifics.Stage;

//#define PATH_SYSTEM_DEBUG_MOVEMENT

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
        public PathPoint currentPoint = null;

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
                    if (currentPoint.gameObject == pa.pathPoint.gameObject)
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
#if PATH_SYSTEM_DEBUG_MOVEMENT
    [Header("Debug")]
    [SerializeField] private Vector2 _dbgObjectVec;
    [SerializeField] private Vector2 _dbgLineStart;
    [SerializeField] private Vector2 _dbgLineEnd;
    [SerializeField] private Vector2 _dbgCurveLineEnd;
    [SerializeField] private Vector2 _dbgCurvePoint;
    [SerializeField] private bool _dbgUseCurve;

    protected void OnDrawGizmos()
    {
        Gizmos.DrawLine(_dbgObjectVec, _dbgLineEnd);
        Gizmos.color = Color.green;
        if (_dbgUseCurve)
        {
            MathCurve.LineFollowedByPath(_dbgLineStart, _dbgCurvePoint, _dbgLineEnd);
            MathCurve.CubeFollowedByPath(_dbgLineStart, _dbgCurvePoint, _dbgLineEnd);

            Gizmos.DrawCube(_dbgCurvePoint, new Vector3(0.5f, 0.5f, 0.5f));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_dbgObjectVec, _dbgLineEnd);
        }
    }
#endif
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

            MovableObject currentObject = null;
            MovableObject previousObject = null;

            // TODO: Slip movement
            foreach (var move_object in movablePathObjectsList.ToList())
            {
                currentObject = move_object;
                if (move_object == null)
                    continue;

                // Get current point 
                int i = move_object.currentPoint;

                // If we are have excess point
                if (move_object.currentPoint >= pathPointsList.Count)
                {
                    // If loop on we are reset current point num 
                    if (loop)
                        move_object.currentPoint = 0;
                    else
                    {
                        Debug.Log($"Unspawn: {move_object.name}");
                        move_object.Unspawn();
                    }

                    continue; // Move to next object
                }

                // Get current point point 
                currentPoint = pathPointsList[i];

                // Calculate reduce, augment speed for protecting objects from stacking or moving too far from previous 
                var reduceSpeed = 1.0f;
                var augmentSpeed = 1.0f;
                var speed = 0.0f;

                if (previousObject)
                {
                    var distanceBetweenCurrentObjects = Vector3.Distance(currentObject.transform.position, previousObject.transform.position);

                    if (holdDistanceBetweenObjects)
                        reduceSpeed = Mathf.Clamp(reduceSpeed, 1.0f, Mathf.Clamp(distanceBetweenCurrentObjects, 0.0f, 2.0f / distanceBetweenObjects));

                    if (catchUpNextObject)
                        augmentSpeed = Mathf.Clamp(augmentSpeed, distanceBetweenCurrentObjects, distanceBetweenObjects);
                }

                // Calculate final object speed 
                speed = (move_object.speed * augmentSpeed * reduceSpeed * Time.deltaTime) * GlobalSettings.gameActiveAsInt; 

                // What's mode we need to use 
                if (currentPoint.useCurve)
                {
                    // Check current curve point 
                    if (move_object.currentCurvePoint <= MathCurve.PATH_CURVE_LINE_STEPS)
                    {
                        if (CheckPointOnValid(i + 1))
                        {
                            var lineEnd = MathCurve.MakeCurve(currentPoint.transform.position, pathPointsList[i + 1].curvePoint,
                                pathPointsList[i + 1].transform.position, move_object.currentCurvePoint / (float)MathCurve.PATH_CURVE_LINE_STEPS);
                            var curveEndDistance = Vector3.Distance(move_object.transform.position, lineEnd);
                            move_object.transform.position = Vector3.MoveTowards(move_object.transform.position, lineEnd, speed);
                            if (curveEndDistance <= distanceAccuracy)
                                move_object.currentCurvePoint++;

#if PATH_SYSTEM_DEBUG_MOVEMENT
                            _dbgLineEnd = lineEnd;
#endif
                        }
                        else
                        {
                            // Not critical because we can disable this flag 
                            //Debug.LogWarning($"{name} - index: {i} | that point is last in the list and have curve mode!");

                            // Disable useCurve flag because last point can't have a curve mode
                            currentPoint.useCurve = false;
                        }

                    }
                    else
                    {
                        IsReached(currentPoint, move_object);
                        continue;
                    }
                }
                else
                {
                    // Check distance for current object
                    var distance = Vector3.Distance(move_object.transform.position, currentPoint.transform.position);
                    move_object.transform.position = Vector3.MoveTowards(move_object.transform.position, currentPoint.transform.position, speed);
                    // If we are get close for point
                    if (distance <= distanceAccuracy)
                    {
                        IsReached(currentPoint, move_object);
                        continue;
                    }
                }

                previousObject = currentObject;

#if PATH_SYSTEM_DEBUG_MOVEMENT
                _dbgObjectVec = move_object.transform.position;
                _dbgLineStart = point.transform.position;
                _dbgUseCurve = point.useCurve;

                if (point.useCurve)
                {
                    if (CheckPointOnValid(i + 1))
                    {
                        _dbgLineEnd = pathPointsList[i + 1].transform.position;
                        _dbgCurvePoint = pathPointsList[i + 1].curvePoint;
                    }
                }
#endif


            }


        }
        #endregion
    }
}