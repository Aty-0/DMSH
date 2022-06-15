using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


#region Globals
public enum PointsToolsCreationMode
{
    OnEnd,
    OnAheadSelectedObject,
    OnBehindSelectedObject
}

public class PointsToolsEditorGlobals
{
    public static PathPoint selectedPoint = null;
    public static PathSystem usedPointSystem = null;
    public static bool makeCurveOnCreate = true;
    public static bool pickObjectOnPosChange = false;
    public static bool createByEdCameraPosition = false;
    public static PointsToolsCreationMode creationMode = PointsToolsCreationMode.OnEnd;
    public static Vector3 createVec = new Vector3(-1, 0, 0);
    public static bool showHandles = true;
    public static bool showOnlyLine = false;
    public static bool showCurveHandle = true;

    public static bool CheckPathSystem()
    {
        if (usedPointSystem == null)
        {
            usedPointSystem = SceneView.FindObjectOfType<PathSystem>();
            Debug.LogWarning("Selected first founded path system because previously non any has been path system selected!");
        }

        return usedPointSystem != null;
    }

    public static void Separator(int i_height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, i_height);
        rect.height = i_height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }

    public static void GoToGameObject()
    {
        if (selectedPoint != null)
            Selection.activeObject = selectedPoint;
    }

    //TODO: Make a better function 
    public static Vector2 CalcCurvePoint(Vector2 p1, Vector2 p2)
    {
        return (p1 - p2);
    }

    public static void Create(Vector3 example_new_position, PathPoint example = null)
    {
        if (selectedPoint == null)
            example = SceneView.FindObjectOfType<PathPoint>();
        else
            example = selectedPoint;


        if (example == null)
        {
            Debug.LogError("No example found for creation");
            return;
        }

        if (usedPointSystem == null)
        {
            usedPointSystem = SceneView.FindObjectOfType<PathSystem>();
            Debug.LogWarning("Selected first founded path system because previously non any has been path system selected!");
        }


        //We are want to create new PathPoint
        PathPoint point = SceneView.Instantiate(example, example_new_position, Quaternion.identity, usedPointSystem == null ? null :
            usedPointSystem.gameObject.transform);

        //Set new name 
        point.name = usedPointSystem.pathPointsList[0].name + "_" + usedPointSystem.pathPointsList.Count;

        point.useCurve = makeCurveOnCreate;

        List<PathPoint> pathlist = usedPointSystem.pathPointsList;

        if (makeCurveOnCreate)
        {
            if (pathlist.Count != 1 && pathlist.IndexOf(selectedPoint) == pathlist.Count)
                point.curvePoint = CalcCurvePoint(pathlist[pathlist.IndexOf(selectedPoint)].transform.position, pathlist[pathlist.IndexOf(selectedPoint) + 1].transform.position);
        }

        switch (creationMode)
        {
            case PointsToolsCreationMode.OnEnd:
                pathlist.Add(point);
                break;
            case PointsToolsCreationMode.OnBehindSelectedObject:
                pathlist.Insert(pathlist.IndexOf(selectedPoint), point);
                break;
            case PointsToolsCreationMode.OnAheadSelectedObject:
                pathlist.Insert(pathlist.IndexOf(selectedPoint) + 1, point);
                break;
        }

    }
}
#endregion

#region Path Point Editor
[CustomEditor(typeof(PathPoint), true), CanEditMultipleObjects]
public class PathPointEditor : Editor
{
    protected virtual void OnSceneGUI()
    {
        PathPoint point = (PathPoint)target;

        //Push object to globals if we are select the point 
        PointsToolsEditorGlobals.selectedPoint = point;
        if (PointsToolsEditorGlobals.usedPointSystem == null)
            PointsToolsEditorGlobals.usedPointSystem = PointsToolsEditorGlobals.selectedPoint.GetComponentInParent<PathSystem>();
    }
}
#endregion

public class PathSystemEditorWindow : EditorWindow
{
    public  Camera      editorCamera = null;

    private GUIStyle    _buttonToolsGUIStyle = null;
    private GUIStyle    _headerTextGUIStyle = null;
    private bool        _foldoutOptionsShow = true;
    private bool        _foldoutStatsShow = true;
    private bool        _foldoutToolsShow = true;

    [MenuItem("Window/DMSH/Path System")]
    static void Init()
    {
        PathSystemEditorWindow window = (PathSystemEditorWindow)EditorWindow.GetWindowWithRect(typeof(PathSystemEditorWindow), new Rect(0, 0, 400, 600));
        GUIContent guiContent = new GUIContent();
        guiContent.text = "Path System";
        //TODO: Icon
        window.titleContent = guiContent;
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }
    private void OnGotoSystemButtonClick()
    {
        PointsToolsEditorGlobals.CheckPathSystem();
        Selection.activeObject = PointsToolsEditorGlobals.usedPointSystem;
    }

    private void OnSwitchCreationModeClick(PointsToolsCreationMode mode)
    {
        PointsToolsEditorGlobals.creationMode = mode;
    }

    private void OnGotoGameObjectButtonClick()
    {
        PointsToolsEditorGlobals.GoToGameObject();
    }

    private void OnDeleteButtonClick()
    {
        EditorGUI.BeginChangeCheck();
        if (PointsToolsEditorGlobals.selectedPoint == null || !PointsToolsEditorGlobals.CheckPathSystem())
            return;

        //Remove object from list
        PointsToolsEditorGlobals.usedPointSystem.pathPointsList.Remove(PointsToolsEditorGlobals.selectedPoint);
        //Remove it from scene
        DestroyImmediate(PointsToolsEditorGlobals.selectedPoint.gameObject);
        //Select previous object from list if we can
        PointsToolsEditorGlobals.selectedPoint = PointsToolsEditorGlobals.usedPointSystem.pathPointsList[PointsToolsEditorGlobals.usedPointSystem.pathPointsList.Count - 1];

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(PointsToolsEditorGlobals.selectedPoint, "Deleted point");
            Undo.RecordObject(PointsToolsEditorGlobals.usedPointSystem, "Changed array in point system");
            EditorUtility.SetDirty(PointsToolsEditorGlobals.selectedPoint);
        }
    }

    private void SearchEditorCamera()
    {
        if (SceneView.currentDrawingSceneView)
            editorCamera = SceneView.currentDrawingSceneView.camera;
    }

    private void OnCreateButtonClick()
    {
        Vector3 position = PointsToolsEditorGlobals.selectedPoint != null ? PointsToolsEditorGlobals.selectedPoint.transform.position + PointsToolsEditorGlobals.createVec : Vector3.zero;

        if (PointsToolsEditorGlobals.createByEdCameraPosition)
        {
            SearchEditorCamera();
            if (editorCamera != null)
                position = new Vector3(editorCamera.transform.position.x, editorCamera.transform.position.y, 0.0f);
            else
                Debug.LogError("editorCamera is null! Can't set position from editor camera");
        }

        PointsToolsEditorGlobals.Create(position);
    }

    public void OnInspectorUpdate()
    {
        //Repaint window
        //OnInspectorUpdate called 10 times per second
        Repaint();
    }

    protected void OnGUI()
    {
        _headerTextGUIStyle = new GUIStyle(GUI.skin.label);
        _headerTextGUIStyle.fontSize = 14;
        
        _buttonToolsGUIStyle = new GUIStyle(GUI.skin.button);
        _buttonToolsGUIStyle.fixedHeight = 22;
        _buttonToolsGUIStyle.fixedWidth = 36;

        PathSystem pathSystem = PointsToolsEditorGlobals.usedPointSystem;
        GUILayout.Label($"Path system:", _headerTextGUIStyle);
        if (pathSystem)
        {
            GUILayout.Label($"Name:{pathSystem.name} ID:{pathSystem.GetInstanceID()}\n" +
                $"Position:{pathSystem.transform.position }");
            PointsToolsEditorGlobals.Separator();
        }
        else
        {
            GUILayout.Label("Non", _headerTextGUIStyle);
            PointsToolsEditorGlobals.Separator();
        }

        PathPoint point = PointsToolsEditorGlobals.selectedPoint;
        GUILayout.Label($"Current point:", _headerTextGUIStyle);
        if (point)
        {
            GUILayout.Label($"Name:{point.name}\n" +
                $"Position:{point.transform.position }");
            GUILayout.Label($"eventOnEndForAll: {point.eventOnEndForAll}");

            if (point.eventSpecial.GetPersistentEventCount() != 0)
                for (int i = 0; i <= point.eventSpecial.GetPersistentEventCount() - 1; i++)
                    GUILayout.Label($"OnEnd:{point.eventSpecial.GetPersistentMethodName(i)}");
            point.eventOnEndForAll = (EnemyScriptedBehavior)EditorGUILayout.EnumPopup(point.eventOnEndForAll);
            point.useCurve = GUILayout.Toggle(point.useCurve, "Use curve");
            PointsToolsEditorGlobals.Separator();
        }
        else
        {
            GUILayout.Label("Non", _headerTextGUIStyle);
            PointsToolsEditorGlobals.Separator();
        }

        _foldoutToolsShow = EditorGUILayout.Foldout(_foldoutToolsShow, $"Tools");
        if (_foldoutToolsShow)
        {
            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button($"C", _buttonToolsGUIStyle))
                    OnCreateButtonClick();
                if (GUILayout.Button($"D", _buttonToolsGUIStyle))
                    OnDeleteButtonClick();
                if (GUILayout.Button($"TS", _buttonToolsGUIStyle))
                    OnGotoSystemButtonClick();
                if (GUILayout.Button($"TGO", _buttonToolsGUIStyle))
                    OnGotoGameObjectButtonClick();
            }
            GUILayout.EndHorizontal();

            PointsToolsEditorGlobals.Separator();

            GUILayout.Label($"Creation Mode:", _headerTextGUIStyle);
            string[] creationToolbarElements = { "End", "Ahead", "Behind" };
            PointsToolsEditorGlobals.creationMode = (PointsToolsCreationMode)GUILayout.Toolbar((int)PointsToolsEditorGlobals.creationMode, creationToolbarElements);
        }

        _foldoutStatsShow = EditorGUILayout.Foldout(_foldoutStatsShow, $"Stats");
        if (_foldoutStatsShow)
        { 
            GUILayout.Label($"Path systems:{ FindObjectsOfType<PathSystem>(true).Length }");
            GUILayout.Label($"Points:{ FindObjectsOfType<PathPoint>(true).Length }");
            GUILayout.Label($"Active path systems:{ FindObjectsOfType<PathSystem>().Length }");
            GUILayout.Label($"Active points:{ FindObjectsOfType<PathPoint>().Length }");
        }

        _foldoutOptionsShow = EditorGUILayout.Foldout(_foldoutOptionsShow, $"Options");
        if(_foldoutOptionsShow)
        {
            PointsToolsEditorGlobals.showOnlyLine = GUILayout.Toggle(PointsToolsEditorGlobals.showOnlyLine, "Show only lines");
            PointsToolsEditorGlobals.showHandles = GUILayout.Toggle(PointsToolsEditorGlobals.showHandles, "Show handles");
            PointsToolsEditorGlobals.showCurveHandle = GUILayout.Toggle(PointsToolsEditorGlobals.showCurveHandle, "Show curve handle");
            PointsToolsEditorGlobals.createByEdCameraPosition = GUILayout.Toggle(PointsToolsEditorGlobals.createByEdCameraPosition, "Create by editor camera position");
            PointsToolsEditorGlobals.pickObjectOnPosChange = GUILayout.Toggle(PointsToolsEditorGlobals.pickObjectOnPosChange, "Pick object when handle is used");
            PointsToolsEditorGlobals.makeCurveOnCreate = GUILayout.Toggle(PointsToolsEditorGlobals.makeCurveOnCreate, "Make curve on create object");
            PointsToolsEditorGlobals.Separator();
            PointsToolsEditorGlobals.createVec = EditorGUILayout.Vector2Field("Addition Vector (Addition to create point)", PointsToolsEditorGlobals.createVec);
        }       
    }
}

#region Path System Editor 
[CustomEditor(typeof(PathSystem), true), CanEditMultipleObjects]
public class PathSystemEditor : Editor
{
    [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
    static void DrawCustomGizmos(PathSystem path, GizmoType gizmoType)
    {        
        PathPoint current_point = null;
        PathPoint prev_point = null;

        foreach (var point in path.pathPointsList.ToArray())
        {
            if (point == null)
                continue;

            current_point = point;

            if (prev_point != null)
            {
                Gizmos.color = path.lineColor;

                if (prev_point.useCurve)
                    PathSystem.LineFollowedByPath(prev_point.transform.position, current_point.curvePoint, current_point.transform.position);                
                else
                    Gizmos.DrawLine(prev_point.transform.position, current_point.transform.position);
            }

            prev_point = current_point;
        }
    }

    public virtual void OnSceneGUI()
    {
        if (target == null)
            return;
        
        PathSystem path = (PathSystem)target;        
        if (!path.pathPointsList.Any())
            return;

        if (!PointsToolsEditorGlobals.showOnlyLine)
        {
            PathPoint current_point = null;
            PathPoint prev_point = null;

            foreach (var point in path.pathPointsList.ToArray())
            {
                if (point == null)
                    continue;

                current_point = point;

                if (prev_point != null)
                {
                    if ((PointsToolsEditorGlobals.selectedPoint == prev_point || PointsToolsEditorGlobals.selectedPoint == point)
                        && prev_point.useCurve && PointsToolsEditorGlobals.showCurveHandle)
                    {
                        var startMatrix = Handles.matrix;
                        
                        Handles.matrix = Matrix4x4.Scale(Vector3.one / 2) * startMatrix;
                        point.curvePoint = Handles.PositionHandle(point.curvePoint * 2, Quaternion.identity) / 2;
                        Handles.matrix = startMatrix;

                        Handles.color = Color.gray;
                        Handles.DrawLine(prev_point.transform.position, point.curvePoint);
                        Handles.DrawLine(point.curvePoint, point.transform.position);
                    }
                }

                EditorGUI.BeginChangeCheck();
                {
                    Vector3 position = Vector3.zero;

                    if (PointsToolsEditorGlobals.showHandles)
                        position = Handles.DoPositionHandle(point.transform.position, Quaternion.identity);
                    else
                        position = point.transform.position;

                    //FIXME: Don't show if we are hold shift key                    
                    Handles.color = PointsToolsEditorGlobals.selectedPoint != point ? Color.white : Color.red;
                    float zoom = SceneView.currentDrawingSceneView.camera.orthographicSize;
                    if (Handles.Button(position, Quaternion.identity, 0.05f * zoom, 0.07f * zoom, Handles.RectangleHandleCap))
                    {
                        //Get point and path system
                        PointsToolsEditorGlobals.selectedPoint = point;
                        PointsToolsEditorGlobals.usedPointSystem = point.GetComponentInParent<PathSystem>();

                        //Repaint editor window
                        PathSystemEditorWindow psewindow = (PathSystemEditorWindow)EditorWindow.GetWindow(typeof(PathSystemEditorWindow));
                        psewindow?.Repaint();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        //TODO: Working Undo Redo
                        //      With working hot keys
                        Undo.RecordObject(point.gameObject, "Changed point position");
                        EditorUtility.SetDirty(point);

                        //TODO: Recalculate position for curve
                        point.transform.position = position;

                        //Set selected object
                        if (PointsToolsEditorGlobals.pickObjectOnPosChange)
                            PointsToolsEditorGlobals.selectedPoint = point;
                    }
                }

                prev_point = current_point;

            }
        }
    }
}
#endregion

