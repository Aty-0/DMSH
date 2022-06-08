using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stage
{
    public List<GameObject> stageObjects = new List<GameObject>();
    public bool isDone = false;
    public string name;
}

[DisallowMultipleComponent]
[RequireComponent(typeof(Timer))]
public class StageSystem : MonoBehaviour
{
    public int CurrentStageIndex
    {
        get { return _stageListIndex; }
        private set { _stageListIndex = value; }
    }

    [Header("Stages")]
    public List<Stage> stagesList = new List<Stage>();

    [Header("Current Stage")]
    public Stage currentStage = null;
    public Timer timer;

    [SerializeField] private int _stageListIndex = 0;
    [SerializeField] private int _listPassed = 0;

    [Header("Events")]
    public List<Action> onTimerStart = new List<Action>();
    public List<Action> onTimerEnd = new List<Action>();

    protected void Start()
    {
        Debug.Log($"[StageSystem] Stage system is started...");
        //Disable all scenario packs
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("ScenarioPack"))
            go.SetActive(false);

        timer = GetComponent<Timer>();
        timer.StartEvent += OnTimerStart;
        timer.EndEvent += OnTimerEnd;        
        timer.StartTimer();
    }

    public void AddedPass()
    {
        if (currentStage != null)
        {
            _listPassed++;
            Debug.Log($"[StageSystem] Added pass | Count {currentStage.stageObjects.Count} Index{_stageListIndex} Passes {_listPassed}");
            //If all scenario lists passed we are passed this stage
            if (_listPassed == currentStage.stageObjects.Count)
                StagePassed();
        }
    }

    private void StagePassed()
    {
        Debug.Log($"[StageSystem] Stage passed {_stageListIndex}");

        currentStage.isDone = true;

        foreach (GameObject go in currentStage.stageObjects)
            go.SetActive(false);

        _stageListIndex++;
        _listPassed = 0;

        timer.ResetTimer();
        timer.StartTimer();
    }

    public void OnTimerStart()
    {
        foreach (Stage st in stagesList)
        {
            Debug.Log($"[StageSystem] Index: {stagesList.IndexOf(st)} StageObjects Count:{st.stageObjects.Count}");
            //If current stage is passed we are go to another
            if (st.isDone)
                continue;            
            currentStage = st;   
            break;
        }

        //Invoke action when timer is runned
        foreach (Action action in onTimerStart)
            action?.Invoke();
    }

    //Dynamicly add pack and activate it
    public void AddToStage(GameObject go)
    {
        go.SetActive(true);
        currentStage.stageObjects.Add(go);
        PathSystem ps = go.GetComponentInChildren<PathSystem>();
        ps?.EnableSpawner();
        Debug.Log($"[StageSystem] Added | Last Index:{currentStage.stageObjects.Count}");
    }

    //Activate existing pack
    public void Activate(GameObject go)
    {
        go.SetActive(true);
        PathSystem ps = go.GetComponentInChildren<PathSystem>();
        ps?.EnableSpawner();
        Debug.Log($"[StageSystem] Activate | Index:{currentStage.stageObjects.IndexOf(go)}");
    }

    //TODO: If last stage is done we are show player the result screen or open main menu
    public void OnTimerEnd()
    {
        //Invoke action when timer is ended
        foreach (Action action in onTimerEnd)
            action?.Invoke();

        if (currentStage != null)
        {
            if (currentStage.isDone)
                return;

            Debug.Log($"[StageSystem] Try to start another stage Current: {_stageListIndex}");
            //If it's stage is not passed
            //We are activate objects in pack
            foreach (GameObject go in currentStage.stageObjects.ToList()) 
                if (go != null)
                    Activate(go);
            
            //If we don't have any objects 
            //We are skip this
            if (currentStage.stageObjects.Count == 0)
                StagePassed();
        }
        else
        {
            Debug.LogError($"Current stage is null | Stage list index:{_stageListIndex}");
        }
    }

}
