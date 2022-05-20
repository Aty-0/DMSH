using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool isDone = false; 
    public List<GameObject> toSpawn = new List<GameObject>();
    public Timer timer;

    [SerializeField]
    private StageSystem _stageSystem;

    protected void Start()
    {
        timer = GetComponent<Timer>();

        if(timer != null)
            timer.EndEvent += Spawn;

        _stageSystem = FindObjectOfType<StageSystem>();
        Debug.Assert(_stageSystem != null);
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
