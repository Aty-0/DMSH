//#define PATH_SYSTEM_DEBUG_MOVEMENT

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//TODO
//Curve movement [DONE]
//Run events [DONE]
//Detach object function [DONE]
//Give him god mode or destroy it [DONE] 
//Run events for enemy who reached point [DONE]
//Distance between objects for fix stacking [DONE]
//Dynamic creation by count (Spawner) [DONE]

//Slip movement

//Now:
//Wait timer for enemy

public class PathSystem : MonoBehaviour
{
    //How much we need points for curve
    public static readonly int          PATH_CURVE_LINE_STEPS = 20;

    [Header("Spawner")]
    public int                          objectCount = 0;
    public int                          spawnedObjectCount = 0;
    public MovableObject                objectPrefab = null;
    public Timer                        spawnerTimer = null;

    [Header("PathSystem")]
    public ObservedList<MovableObject>  movablePathObjectsList = new ObservedList<MovableObject>();
    public List<PathPoint>              pathPointsList = new List<PathPoint>();
    public bool                         loop = true;

    [Header("Misc")]
    public bool                         holdDistanceBetweenObjects = true;
    public bool                         catchUpNextObject = false; //At the moment working weird
    public float                        distanceAccuracy = 0.01f;
    public float                        distanceBetweenObjects = 2.0f;
    public StageSystem                  stageSystem = null;

    [Header("Editor")]
    public Color                        lineColor = Color.green;

    [Header("Actions")]
    public List<Action>                 onMovableObjectsAdded   = new List<Action>();
    public List<Action>                 onMovableObjectsChanged   = new List<Action>();
    public List<Action>                 onMovableObjectsRemoved = new List<Action>();

    protected void Start()
    {
        movablePathObjectsList.Updated += UpdateElementChangedCallback;
        movablePathObjectsList.Added += UpdateElementAddedCallback;
        movablePathObjectsList.Removed += OnMovablePathObjectsListElementRemoved;

        stageSystem = FindObjectOfType<StageSystem>();
        Debug.Assert(stageSystem != null, "No stage system in scene");

        //Set to object what's path system he use
        foreach (var move_object in movablePathObjectsList)
            if(move_object != null)
                move_object.pathSystem = this;
    }

    private void SpawnObject()
    {
        spawnedObjectCount++;
        MovableObject movableObject = Instantiate(objectPrefab, pathPointsList[0].transform.position, Quaternion.identity);
        movableObject.pathSystem = this;
        movableObject.name = $"{movableObject.name}{spawnedObjectCount}"; 
        movableObject.transform.parent = transform.parent;
        movablePathObjectsList.Add(movableObject);
        if (spawnedObjectCount == objectCount)
            spawnerTimer.EndTimer();
        else
            spawnerTimer.ResetTimer();
    }

    public void EnableSpawner()
    {
        if (objectCount != 0)
        {
            spawnerTimer = GetComponent<Timer>();
            Debug.Assert(pathPointsList[0] != null);
            Debug.Assert(spawnerTimer != null);
            spawnerTimer.EndEvent += SpawnObject;
            if (objectPrefab != null)
                for (int i = 1; i <= objectCount && !spawnerTimer.isEnded; i++)
                    spawnerTimer.StartTimer();
        }
    }

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
        //If all enemy is dead
        //We are pass this scenario list
        if (!movablePathObjectsList.Any() && ((spawnerTimer == null) ? true : spawnerTimer.isEnded))
        {
            UpdateElementRemovedCallback();
            stageSystem.AddedPass();
        }
    }

    public bool CheckPointOnValid(int index)
    {
        //First we are check points storage 
        //Second we are check index on valid 
        //Then we are try to get object with that index and check it to null
        if (pathPointsList.Any() && (index < pathPointsList.Count))
            if (pathPointsList[index] != null)
                return true;

        return false;
    }

    public static Vector3 MakeCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Vector3.Lerp(Vector3.Lerp(p0, p1, t), Vector3.Lerp(p1, p2, t), t);
    }

    public static void LineFollowedByPath(Vector2 start, Vector2 point, Vector2 end)
    {
        Vector3 lineStart = start;
        for (int i = 1; i <= PATH_CURVE_LINE_STEPS; i++)
        {
            Vector3 lineEnd = MakeCurve(start, point, end, i / (float)PATH_CURVE_LINE_STEPS);
            Gizmos.DrawLine(lineStart, lineEnd);
            lineStart = lineEnd;
        }
    }

    public static void CubeFollowedByPath(Vector2 start, Vector2 point, Vector2 end)
    {
        for (int i = 1; i <= PATH_CURVE_LINE_STEPS; i++)
        {
            Vector3 lineEnd = MakeCurve(start, point, end, i / (float)PATH_CURVE_LINE_STEPS);
            Gizmos.DrawCube(lineEnd, new Vector3(0.2f, 0.2f, 0.2f));
        }
    }
    private void IsReached(PathPoint point, MovableObject move_object)
    {
        if (point.eventSpecial?.GetPersistentEventCount() != 0)
            point.eventSpecial.Invoke();

        if (pathPointsList.IndexOf(point) == pathPointsList.Count - 1)
            move_object.OnReachedLastPoint();
        else if (pathPointsList.IndexOf(point) == 0)
            move_object.OnReachedFirstPoint();
        else
            move_object.OnReachedPoint();

        move_object.OnReachedPointEvent(point.eventOnEndForAll);

        move_object.currentCurvePoint = 1;
        move_object.currentPoint++;
    }
    #endregion

    #region DEBUG_DEFINES
#if PATH_SYSTEM_DEBUG_MOVEMENT
    [Header("Debug")]
    [SerializeField]
    private Vector2 _dbgObjectVec;
    [SerializeField]
    private Vector2 _dbgLineStart;
    [SerializeField]
    private Vector2 _dbgLineEnd;
    [SerializeField]
    private Vector2 _dbgCurveLineEnd;
    [SerializeField]
    private Vector2 _dbgCurvePoint;
    [SerializeField]
    private bool _dbgUseCurve;

    public void OnGUI()
    {

    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(_dbgObjectVec, _dbgLineEnd);
        Gizmos.color = Color.green;
        if (_dbgUseCurve)
        {
            LineFollowedByPath(_dbgLineStart, _dbgCurvePoint, _dbgLineEnd);
            CubeFollowedByPath(_dbgLineStart, _dbgCurvePoint, _dbgLineEnd);

            Gizmos.DrawCube(_dbgCurvePoint, new Vector3(0.5f, 0.5f, 0.5f));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(_dbgObjectVec, _dbgLineEnd);
        }


    }
#endif
    #endregion

    #region Update
    public void Update()
    {
        //Check both lists on object exist
        if (!pathPointsList.Any() || !movablePathObjectsList.Any())
            return;

        //FIXME ?
        //I'm not sure about using this function in update method
        movablePathObjectsList.RemoveAll(o => o == null);

        MovableObject currentObject = null;
        MovableObject previousObject = null;

        foreach (var move_object in movablePathObjectsList.ToList())
        {
            currentObject = move_object;
            if (move_object == null)
                continue;

            //Get current point 
            int i = move_object.currentPoint;

            //If we are have excess point
            if (move_object.currentPoint >= pathPointsList.Count)
            {
                //If loop on we are reset current point num 
                if (loop)
                    move_object.currentPoint = 0;
                continue; //Move to next object
            }

            PathPoint point = pathPointsList[i]; //Get current point point 
            float reduceSpeed = 1.0f;
            float augmentSpeed = 1.0f;
            float speed = 0.0f;
         
            if (previousObject)
            {
                float distanceBetweenCurrentObjects = Vector3.Distance(currentObject.transform.position, previousObject.transform.position);

                if(holdDistanceBetweenObjects)
                    reduceSpeed = Mathf.Clamp(reduceSpeed, 1.0f, Mathf.Clamp(distanceBetweenCurrentObjects, 0.0f, 2.0f / distanceBetweenObjects));

                if(catchUpNextObject)
                    augmentSpeed = Mathf.Clamp(augmentSpeed, distanceBetweenCurrentObjects, distanceBetweenObjects);

                //FIX ME 
                //Not work correctly for object except first and last
                //if (CheckPointOnValid(i + 1))
                //{
                //    float distanceBetweenPoints = Vector3.Distance(point.transform.position, pathPointsList[i + 1].transform.position);
                //    if (augmentSpeed > distanceBetweenPoints) //If object is stuck out of bounds or far away 
                //    {
                //        Debug.LogError($"{move_object.name} {distanceBetweenPoints} {augmentSpeed} augmentSpeed > distanceBetweenPoints");
                //        speed = move_object.speed * augmentSpeed * Time.deltaTime;
                //        float distanceToPreviousObject = Vector3.Distance(move_object.transform.position, previousObject.transform.position);
                //        move_object.transform.position = Vector3.MoveTowards(move_object.transform.position, previousObject.transform.position, speed);
                //        //continue;
                //    }
                //}
            }
            
            speed = move_object.speed * augmentSpeed * reduceSpeed * Time.deltaTime; //Calculate speed 

            //What's mode we need to use 
            if (point.useCurve)
            {
                //Check current curve point 
                if (move_object.currentCurvePoint <= PATH_CURVE_LINE_STEPS)
                {
                    if (CheckPointOnValid(i + 1))
                    {
                        Vector3 lineEnd = MakeCurve(point.transform.position, pathPointsList[i + 1].curvePoint,
                            pathPointsList[i + 1].transform.position, move_object.currentCurvePoint / (float)PATH_CURVE_LINE_STEPS);
                        float curve_end_distance = Vector3.Distance(move_object.transform.position, lineEnd);
                        move_object.transform.position = Vector3.MoveTowards(move_object.transform.position, lineEnd, speed);
                        if (curve_end_distance <= distanceAccuracy)
                            move_object.currentCurvePoint++;

#if PATH_SYSTEM_DEBUG_MOVEMENT
                            _dbgLineEnd = lineEnd;
#endif
                    }
                    else
                    {
                        Debug.LogWarning($"{i} That point is last in the list and have curve mode!");
                        //Disable useCurve Flag in current point
                        point.useCurve = false;
                    }

                }
                else
                {
                    IsReached(point, move_object);
                    continue;
                }
            }
            else
            {
                //Check distance for current object
                float distance = Vector3.Distance(move_object.transform.position, point.transform.position);
                move_object.transform.position = Vector3.MoveTowards(move_object.transform.position, point.transform.position, speed);
                //If we are get close for point
                if (distance <= distanceAccuracy)
                {
                    IsReached(point, move_object);
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
