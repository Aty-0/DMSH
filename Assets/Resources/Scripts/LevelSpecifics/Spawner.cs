using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Main")]
    public bool isDone = false; 
    public List<GameObject> toSpawn = new List<GameObject>();
    public Timer timer;

    [Header("Misc")]
    [SerializeField] private StageSystem _stageSystem;

    [Tooltip("If we are attach spawner to point")]
    [SerializeField] private PathSystem _pathSystem;

    protected void Start()
    {
        _stageSystem    = FindObjectOfType<StageSystem>();
        timer           = GetComponent<Timer>();
        if(timer)
            timer.EndEvent += Spawn;
        _pathSystem = GetComponentInParent<PathSystem>();

        if(_pathSystem)
            _pathSystem.onMovableObjectsRemoved.Add(Spawn);
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
                if (!go.activeSelf)
                    _stageSystem.AddToStage(go);
            isDone = true;
        }
    }
}
