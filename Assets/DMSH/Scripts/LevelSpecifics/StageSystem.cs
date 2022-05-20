using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO
//Name, Description 
//Need to create add function for stageObjects 
[Serializable]
public class Stage
{
    public List<GameObject> stageObjects = new List<GameObject>();
    public bool isDone = false;
}

//TODO
//Need to check count of stage systems
//Cuz we need only one system 

public class StageSystem : MonoBehaviour
{
    [Header("Stages")]
    public List<Stage> stagesList = new List<Stage>();

    [Header("Current Stage")]
    public Timer timer;
    [SerializeField] private int _stageListID = 0;
    [SerializeField] private int _listPassed = 0;
    public Stage currentStage = null;

    /// <summary>
    /// Custom timer events 
    /// </summary>
    [Header("Events")]
    public List<Action> onTimerStart = new List<Action>();
    public List<Action> onTimerEnd = new List<Action>();
    //TODO
    //It's should be not here
    public List<int> skipStageTimerEvents = new List<int>();



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
            Debug.Log($"[StageSystem] Added pass {currentStage.stageObjects.Count} {_stageListID} {_listPassed}");
            //If all scenario lists passed we are passed this stage
            if (_listPassed == currentStage.stageObjects.Count)
                StagePassed();
        }
    }

    private void StagePassed()
    {
        Debug.Log($"[StageSystem] Stage passed {_stageListID}");

        currentStage.isDone = true;

        foreach (GameObject go in currentStage.stageObjects)
            go.SetActive(false);

        _stageListID++;
        _listPassed = 0;

        timer.ResetTimer();
        timer.StartTimer();
    }

    public void OnTimerStart()
    {
        //Invoke action when timer is runned
        foreach (int skip in skipStageTimerEvents)
            if (_stageListID != skip)
                foreach (Action action in onTimerStart)
                    action?.Invoke();
    }

    public void AddToStage(GameObject go)
    {
        Debug.Log($"[StageSystem] Add {go.name} {currentStage.stageObjects.Count}");
        go.SetActive(true);
        currentStage.stageObjects.Add(go);
        PathSystem ps = go.GetComponentInChildren<PathSystem>();
        ps?.EnableSpawner();
        Debug.Log($"[StageSystem] Added {currentStage.stageObjects.Count}");
    }

    public void OnTimerEnd()
    {      
        Debug.Log($"[StageSystem] Try to start another stage Current: {_stageListID}");
        //Invoke action when timer is ended
        foreach (int skip in skipStageTimerEvents)
            if (_stageListID != skip)
                foreach (Action action in onTimerEnd)
                    action?.Invoke();

        foreach (Stage st in stagesList)
        {
            Debug.Log($"[StageSystem] Index: {stagesList.IndexOf(st)} StageObjects Count:{st.stageObjects.Count}");
            //TODO
            //If last stage is done we are show player the result screen
            //But for now we are need to switch to main menu

            //If current stage is passed we are go to another
            if (st.isDone)
                continue;

            //If it's stage is not passed
            //We are activate objects in pack
            foreach (GameObject go in st.stageObjects)
            {
                if(go == null)
                    continue;
                AddToStage(go);
            }

            currentStage = st;
            //When we are activate everything we are break the loop         
            break;
        }       
    }

}
