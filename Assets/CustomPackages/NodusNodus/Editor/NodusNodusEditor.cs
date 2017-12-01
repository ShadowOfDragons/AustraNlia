using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CanEditMultipleObjects]
[CustomEditor(typeof(NodusNet))]
public class NodusNetInspector : Editor
{   
    enum HandleType { Free, Position };
    static HandleType handleType = HandleType.Free;    

    #region Bools
    private static bool showPosition = false;
    private static bool drawIndex = false;
    private static bool smartSnap = false;
    #endregion
    #region Resources
    static Texture2D logo;
    static Texture2D nodeTexture;
    static Texture2D nodeSelectedTexture;
    static Texture2D deleteTexture;
    static Texture2D deleteSelectedTexture;
    static Texture2D addTexture;
    static Texture2D addSelectedTexture;
    static Texture2D connectTexture;
    static Texture2D connectSelectedTexture;
    static Texture2D multiselectedTexture;
    static Texture2D enemyNodeTexture;
    static Texture2D playerNodeTexture;

    private void Dot(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, nodeTexture); }
    private void DotDelete(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, deleteTexture); }
    private void DotAdd(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, addTexture); }
    private void DotConnect(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, connectTexture); }
    private void DotSel(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, nodeSelectedTexture); }
    private void DotDeleteSel(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, deleteSelectedTexture); }
    private void DotAddSel(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, addSelectedTexture); }
    private void DotConnectSel(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, connectSelectedTexture); }
    private void DotMultiSel(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, multiselectedTexture); }
    private void DotEnemy(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, enemyNodeTexture); }
    private void DotFriend(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) { CW.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, playerNodeTexture); }


    #endregion
    #region Drag
    bool drag = false;
    Vector2 iniDragPos;
    #endregion
    #region Parameters
    NodusNet nodusNet;
    public static List<int> selectedNodes = new List<int>();
    private static NodeType.Type type;
    Vector3 snapVector = Vector3.one;
    #endregion
    void LoadTextures()
    {
        if (nodeTexture != null) return;
        logo = (Texture2D)Resources.Load("Gizmos/nnlogo");
        nodeTexture = (Texture2D)Resources.Load("Gizmos/node_normal");
        deleteTexture = (Texture2D)Resources.Load("Gizmos/node_delete");
        addTexture = (Texture2D)Resources.Load("Gizmos/node_add");
        connectTexture = (Texture2D)Resources.Load("Gizmos/node_connect");
        nodeSelectedTexture = (Texture2D)Resources.Load("Gizmos/node_normal_selected");
        deleteSelectedTexture = (Texture2D)Resources.Load("Gizmos/node_delete_selected");
        addSelectedTexture = (Texture2D)Resources.Load("Gizmos/node_add_selected");
        connectSelectedTexture = (Texture2D)Resources.Load("Gizmos/node_connect_selected");
        multiselectedTexture = (Texture2D)Resources.Load("Gizmos/node_multiSelect");
        enemyNodeTexture = (Texture2D)Resources.Load("Gizmos/node_enemy");
        playerNodeTexture = (Texture2D)Resources.Load("Gizmos/node_player");
    }
    void OnEnable()
    {
        nodusNet = (target as NodusNet);
        LoadTextures();
        selectedNodes.Clear();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        GUILayout.Label(logo);
        NodusNet nodusNet = (target as NodusNet);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Selected nodes count " + selectedNodes.Count.ToString());
        #region Node Data
        EditorGUI.indentLevel = 0;
        //showPosition = EditorGUILayout.Foldout(showPosition, "Positions", true, EditorStyles.toolbarDropDown);
        if(true)
        {
            nodusNet.gizmoColor = EditorGUILayout.ColorField(nodusNet.gizmoColor);

            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            if (!nodusNet.EmptyNet())
            {
                EditorGUI.indentLevel = 0;
                int netSize = nodusNet.NetSize;                
                if (!Application.isPlaying)
                {
                    netSize = EditorGUILayout.IntField("Net size: ", nodusNet.NetSize);
                    while (nodusNet.NetSize > netSize) nodusNet.DeleteNode(nodusNet.NetSize - 1);
                    while (nodusNet.NetSize < netSize)
                    {
                        int numNode = nodusNet.Node();
                        nodusNet.SetNodeType(numNode, type);
                    }
                }
                else
                {
                   EditorGUILayout.LabelField("Net size: ", nodusNet.NetSize.ToString());
                }
                EditorGUI.indentLevel = 2;
                for (int i = 0; i < netSize; i++)
                {
                    Vector2 coordinates = nodusNet.GetNodeCoordinates(i);
                    
                    if (!Application.isPlaying)
                    {
                        Vector2 fieldCoordinates = EditorGUILayout.Vector2Field("#N" + i, nodusNet.GetNodeCoordinates(i));
                        if(coordinates != fieldCoordinates)
                        {
                            nodusNet.SetNodeCoordinates(i, fieldCoordinates);
                            EditorUtility.SetDirty(target);
                        }

                        NodeType.Type nodeType = nodusNet.GetNodeType(i);
                        nodeType = (NodeType.Type)EditorGUILayout.EnumPopup("Type", nodeType);
                        nodusNet.SetNodeType(i,nodeType);
                    }
                    else
                    {
                       EditorGUILayout.Vector2Field("#N" + i, nodusNet.GetNodeCoordinates(i));
                        EditorGUILayout.EnumPopup("Type", nodusNet.GetNodeType(i));
                    }
                }
            }
            else nodusNet.CreateNet();
            EditorGUILayout.EndVertical();
        }
        #endregion        
        #region Buttons
        /*
        EditorGUILayout.Space();
        EditorGUI.indentLevel = 0;
        if (GUILayout.Button("Center position", EditorStyles.miniButtonMid)) CenterPosition(nodusNet);
        if (GUILayout.Button("Merge", EditorStyles.miniButtonMid)) MergeNodes(nodusNet);
        if (GUILayout.Button("Reset", EditorStyles.miniButtonMid)) ResetNodusNet(nodusNet);*/
        #endregion
        
        if (EditorGUI.EndChangeCheck()) Undo.RecordObject(target, "Modified Nodes"); 
        //base.OnInspectorGUI();
    }
    void OnSceneGUI()
    {
        NodusNet nodusNet = (target as NodusNet);
        snapVector = new Vector3(EditorPrefs.GetFloat("MoveSnapX", 1), EditorPrefs.GetFloat("MoveSnapY", 1), EditorPrefs.GetFloat("MoveSnapZ", 1));

        EditorGUI.BeginChangeCheck();

        if (Event.current.commandName == "UndoRedoPerformed") selectedNodes.Clear();
        //if (!Application.isPlaying)
        {
            if (Event.current.button == 1 && Event.current.type == EventType.mouseDown) selectedNodes.Clear();
            else if (Event.current.keyCode == KeyCode.M && Event.current.type == EventType.KeyDown) MergeNodes();
            else if (Event.current.keyCode == KeyCode.R && Event.current.type == EventType.KeyDown) ResetNodusNet();
            else if (Event.current.keyCode == KeyCode.A && Event.current.type == EventType.KeyDown) SelectAllNodes();
            else if (Event.current.keyCode == KeyCode.I && Event.current.type == EventType.KeyDown) InvertSelection();
            else if (Event.current.shift && Event.current.control && !Event.current.alt) DragMultiSelect(); 
            else if (Event.current.shift && !Event.current.control && !Event.current.alt) AddHandles();
            
            NetHandles();
            DrawConnections();
            if (selectedNodes.Count != 0)
            {
                if (selectedNodes.Count == 1) type = nodusNet.GetNodeType(selectedNodes[0]);
                else type = NodeType.Type.undefined;
            }
            
            NodusNetSceneOverlay.OnGUI(nodusNet, this);
        }

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Modified Nodes");
        }

        if(GUI.changed) EditorUtility.SetDirty(target);
    }
    
    void NetHandles()
    {
        Handles.color = Color.white;
        Transform transform = nodusNet.transform;
        Matrix4x4 mat = transform.localToWorldMatrix;
        Matrix4x4 invMat = transform.worldToLocalMatrix;

        Transform cameraTransform = SceneView.lastActiveSceneView.camera.transform;

        Vector3 mousePosition = transform.InverseTransformPoint(CW.EditorTools.GetMousePos(Event.current.mousePosition, transform));

        //Draw caps
        for (int i = 0; i < nodusNet.NetSize; i++)
        {
            Vector3 position = mat.MultiplyPoint3x4(nodusNet.GetNodeCoordinates(i));
            Vector3 iniPosition = position;
            float handleScale = CW.EditorTools.HandleScale(iniPosition);

            bool isSelected = false;
            if (selectedNodes != null) isSelected = selectedNodes.Contains(i);

            #region Delete
            if (Event.current.alt && !Event.current.shift && !Event.current.control)
            {
                Handles.DrawCapFunction cap = (isSelected) ? (Handles.DrawCapFunction)DotDeleteSel : (Handles.DrawCapFunction)DotDelete;
                if (Handles.Button(iniPosition, cameraTransform.rotation, handleScale, handleScale, cap))
                {
                    if (!selectedNodes.Contains(i))
                    {
                        selectedNodes.Clear();
                        selectedNodes.Add(i);
                    }
                    for (int j = 0; j < selectedNodes.Count; j++)
                    {
                        nodusNet.DeleteNode(selectedNodes[j]);
                        if (selectedNodes[j] <= i) i--;

                        for (int k = 0; k < selectedNodes.Count; k++)
                        {
                            if (selectedNodes[k] > selectedNodes[j]) selectedNodes[k] -= 1;
                        }
                    }
                    selectedNodes.Clear();
                }

                #region Disconnect
                cap = DotDelete;
                for (int j = 0; j < nodusNet.NetSize; j++)
                {
                    List<int> nodeConnections = nodusNet.GetNodeConnections(j);
                    for (int k = 0; k < nodeConnections.Count; k++)
                    {
                        int next = nodeConnections[k];
                        Vector3 pos1 = mat.MultiplyPoint3x4(nodusNet.GetNodeCoordinates(j));
                        Vector3 pos2 = mat.MultiplyPoint3x4(nodusNet.GetNodeCoordinates(next));
                        Vector3 middPos = Vector3.Lerp(pos1, pos2, 0.5f);
                        float miniHandleScale = handleScale * 0.6f;
                        if (Handles.Button(middPos, cameraTransform.rotation, miniHandleScale, miniHandleScale, cap))
                        {
                            nodusNet.DisconnectNodes(j, next);
                        }
                    }
                }
                #endregion
            }
            #endregion
            #region Connect
            else if (Event.current.shift && !Event.current.alt && !Event.current.control)
            {
                Handles.DrawCapFunction cap = (isSelected) ? (Handles.DrawCapFunction)DotConnectSel : (Handles.DrawCapFunction)DotConnect;
                if (selectedNodes.Count == 0) cap = Dot;
                if(Handles.Button(iniPosition, cameraTransform.rotation, handleScale, handleScale, cap))
                {
                    if(selectedNodes.Count != 0)
                    {
                        for (int j = 0; j < selectedNodes.Count; j++) nodusNet.ConnectNodes(i, selectedNodes[j]);
                    }
                    selectedNodes.Clear();
                    selectedNodes.Add(i);
                }                
            }
            #endregion
            #region Select
            else if (Event.current.control && !Event.current.shift && !Event.current.alt)
            {
                Handles.DrawCapFunction cap = (isSelected) ? (Handles.DrawCapFunction)DotSel : (Handles.DrawCapFunction)Dot;
                if (Handles.Button(iniPosition, cameraTransform.rotation, handleScale, handleScale, cap))
                {
                    if (!selectedNodes.Contains(i)) selectedNodes.Add(i);
                    else selectedNodes.Remove(i);
                }
            }
            #endregion
            #region Move and middadd
            else
            {
                #region Middle Add
                Handles.DrawCapFunction cap = DotAdd;
                for (int j = 0; j < nodusNet.NetSize; j++)
                {
                    List<int> nodeConnections = nodusNet.GetNodeConnections(j);
                    for (int k = 0; k < nodeConnections.Count; k++)
                    {
                        int next = nodeConnections[k];

                        Vector3 pos1 = mat.MultiplyPoint3x4(nodusNet.GetNodeCoordinates(j));
                        Vector3 pos2 = mat.MultiplyPoint3x4(nodusNet.GetNodeCoordinates(next));
                        Vector3 middPos = Vector3.Lerp(pos1, pos2, 0.5f);
                        float miniHandleScale = handleScale *0.6f;
                        if (Handles.Button(middPos, cameraTransform.rotation, miniHandleScale, miniHandleScale, cap))
                        {
                            int newNode = nodusNet.Node(middPos);
                            nodusNet.SetNodeType(newNode, type);

                            nodusNet.DisconnectNodes(j, next);
                            nodusNet.ConnectNodes(j, newNode);
                            nodusNet.ConnectNodes(next, newNode);
                        }
                    }
                }
                #endregion

                if(nodusNet.GetNodeType(i) == NodeType.Type.Enemy) cap = (Handles.DrawCapFunction)DotEnemy;
                else if (nodusNet.GetNodeType(i) == NodeType.Type.Hazard) cap = (Handles.DrawCapFunction)DotEnemy;
                else if(nodusNet.GetNodeType(i) == NodeType.Type.Player) cap = (Handles.DrawCapFunction)DotFriend;
                else cap = (isSelected) ? (Handles.DrawCapFunction)DotSel : (Handles.DrawCapFunction)Dot;

                Vector3 result = Vector3.zero;
                if (handleType == HandleType.Free)
                {
                    result = Handles.FreeMoveHandle(iniPosition, cameraTransform.rotation, handleScale, snapVector, cap);
                }
                else
                {
                    Handles.FreeMoveHandle(iniPosition, cameraTransform.rotation, handleScale, snapVector, cap);
                    result = Handles.PositionHandle(iniPosition, cameraTransform.rotation);
                }

                if (result != iniPosition)
                {
                    if (!selectedNodes.Contains(i))
                    {
                        selectedNodes.Clear();
                        selectedNodes.Add(i);
                    }
                    
                    Vector3 local = invMat.MultiplyPoint3x4(result);
                    if(smartSnap) local = SmartSnap(local, nodusNet, selectedNodes, 1);

                    Vector3 relative = new Vector2(local.x, local.y) - nodusNet.GetNodeCoordinates(i);

                    for (int j = 0; j < selectedNodes.Count; j++) nodusNet.SetNodeRelativeCoordinates(selectedNodes[j], relative);
                }                
            }

            #region Draw Index
            // Draw Index
            if (drawIndex && !Event.current.alt)
            {
                //style.fontSize = 16 + (int)HandleUtility.GetHandleSize(iniPosition);
                GUIStyle style = EditorStyles.boldLabel;
                style.normal.textColor = Color.gray;
                Handles.Label(iniPosition + new Vector3(0.2f, 0.2f, 0), nodusNet.GetNodeID(i).ToString(), style);
            }
            #endregion
            #endregion
        }
    }
    void AddHandles()
    {
        Transform transform = nodusNet.transform;
        Transform cameraTransform = SceneView.lastActiveSceneView.camera.transform;
        Vector3 mousePosition = transform.InverseTransformPoint(CW.EditorTools.GetMousePos(Event.current.mousePosition, transform));
        Vector3 handlePos = transform.TransformPoint(mousePosition);
        float handleScale = CW.EditorTools.HandleScale(handlePos);

        Handles.color = Color.white;
        Handles.DrawCapFunction cap = DotAdd;

        if (Event.current.button == 0 && Handles.Button(handlePos, cameraTransform.rotation, handleScale, handleScale, cap))
        {
            int newNode = nodusNet.Node(mousePosition);
            //nodusNet.SetNodeType(newNode, type);

            if (selectedNodes.Count > 0)
            {
                for (int i = 0; i < selectedNodes.Count; i++) nodusNet.ConnectNodes(newNode, selectedNodes[i]);
            }
            selectedNodes.Clear();
            selectedNodes.Add(nodusNet.NetSize - 1);
        }

        if (selectedNodes.Count > 0)
        {
            Handles.color = Color.green;
            for (int j = 0; j < selectedNodes.Count; j++)
            {
                Handles.DrawLine(handlePos, transform.TransformPoint(nodusNet.GetNodeCoordinates(selectedNodes[j])));
            }
        }

    }
    void DrawConnections()
    {
        Handles.color = Color.white;
        Matrix4x4 mat = nodusNet.transform.localToWorldMatrix;

        for(int i = 0; i < nodusNet.NetSize; i++)
        {
            Vector3 pos1 = mat.MultiplyPoint3x4(nodusNet.GetNodeCoordinates(i));
            List<int> nodeConnections = new List<int>(nodusNet.GetNodeConnections(i));

            for(int j = 0; j < nodeConnections.Count; j++)
            {
                Vector3 pos2 = mat.MultiplyPoint3x4(nodusNet.GetNodeCoordinates(nodeConnections[j]));
                Handles.DrawLine(pos1, pos2);   
                
                if(drawIndex && !Event.current.alt)
                {
                    float distance = Vector3.Distance(pos1, pos2);
                    Vector3 textCoord = (pos1 + pos2) / 2;
                    GUIStyle style = EditorStyles.textField;
                    Handles.Label(textCoord + new Vector3(0, 0,0), distance.ToString("0.000"), style);
                }             
            }
        }

      //  Handles.Label(iniPosition + new Vector3(0.1f, 0.4f, 0), nodusNet.GetNodeID(i).ToString(), EditorStyles.boldLabel);

    }
    void DragMultiSelect()
    {
        Transform transform = nodusNet.transform;
        Transform camereTransform = SceneView.lastActiveSceneView.camera.transform;
        Vector3 mousePosition = transform.InverseTransformPoint(CW.EditorTools.GetMousePos(Event.current.mousePosition, transform));

        Vector3 handlePos = transform.TransformPoint(mousePosition);
        float handleScale = CW.EditorTools.HandleScale(handlePos);

        Handles.color = Color.white;
        Handles.DrawCapFunction cap = DotMultiSel;

        if (Event.current.type == EventType.repaint)
        {
            if (drag)
            {
                Vector3 pt1 = HandleUtility.GUIPointToWorldRay(iniDragPos).GetPoint(0.2f);
                Vector3 pt2 = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).GetPoint(0.2f);
                Vector3 pt3 = HandleUtility.GUIPointToWorldRay(new Vector2(iniDragPos.x, Event.current.mousePosition.y)).GetPoint(0.2f);
                Vector3 pt4 = HandleUtility.GUIPointToWorldRay(new Vector2(Event.current.mousePosition.x, iniDragPos.y)).GetPoint(0.2f);
                Handles.DrawSolidRectangleWithOutline(new Vector3[] { pt1, pt3, pt2, pt4 }, new Color(0, 0, 0, 0.1f), new Color(1, 1, 1, 0.5f));
            }
        }
        
        switch (Event.current.type)
        {
            case EventType.mouseDrag:
                SceneView.RepaintAll();
                break;
            case EventType.mouseMove:
                SceneView.RepaintAll();
                break;
            case EventType.mouseDown:
                iniDragPos = Event.current.mousePosition;
                drag = true;
                break;
            case EventType.mouseUp:
                Vector2 dragEnd = Event.current.mousePosition;
                selectedNodes.Clear();
                for (int i = 0; i < nodusNet.NetSize; i += 1)
                {
                    float left = Mathf.Min(iniDragPos.x, iniDragPos.x + (dragEnd.x - iniDragPos.x));
                    float right = Mathf.Max(iniDragPos.x, iniDragPos.x + (dragEnd.x - iniDragPos.x));
                    float top = Mathf.Min(iniDragPos.y, iniDragPos.y + (dragEnd.y - iniDragPos.y));
                    float bottom = Mathf.Max(iniDragPos.y, iniDragPos.y + (dragEnd.y - iniDragPos.y));

                    Rect r = new Rect(left, top, right - left, bottom - top);
                    if (r.Contains(HandleUtility.WorldToGUIPoint(transform.TransformPoint(nodusNet.GetNodeCoordinates(i)))))
                    {
                        selectedNodes.Add(i);
                    }
                }
                HandleUtility.AddDefaultControl(0);
                drag = false;
                Repaint();
                break;
            case EventType.layout:
                HandleUtility.AddDefaultControl(GetHashCode());
                break;


        }
        Handles.Button(handlePos, camereTransform.rotation, handleScale, handleScale, cap);
        
        /*else if (drag == true)
        {
            drag = false;
            Repaint();
        }*/
    }
    Vector3 SmartSnap(Vector3 aPoint, NodusNet net, List<int> aIgnore, float aSnapDist)
    {
        float minXDist = aSnapDist;
        float minYDist = aSnapDist;
        Vector3 result = aPoint;

        for (int i = 0; i < net.NetSize; ++i)
        {
            if (aIgnore.Contains(i)) continue;

            float xDist = Mathf.Abs(aPoint.x - net.GetNodeCoordinates(i).x);
            float yDist = Mathf.Abs(aPoint.y - net.GetNodeCoordinates(i).y);

            if (xDist < minXDist)
            {
                minXDist = xDist;
                result.x = net.GetNodeCoordinates(i).x;
            }

            if (yDist < minYDist)
            {
                minYDist = yDist;
                result.y = net.GetNodeCoordinates(i).y;
            }
        }
        return result;
    }

    public void CenterPosition()
    {
        nodusNet.UpdateRelativePosition();
        /*Vector2 netPosition = nodusNet.transform.position;
        nodusNet.transform.position = Vector3.zero;
        for (int i = 0; i < nodusNet.NetSize; i++) nodusNet.SetNodeRelativeCoordinates(i, netPosition);*/
    }
    public void MergeNodes()
    {
        if (selectedNodes.Count > 1)
        {
            // Calculate coordinates
            Vector2 coordinates = new Vector2(0, 0);
            for (int i = 0; i < selectedNodes.Count; i++)
            {
                coordinates += nodusNet.GetNodeCoordinates(selectedNodes[i]);
            }
            coordinates.x /= selectedNodes.Count;
            coordinates.y /= selectedNodes.Count;
            // Create new node
            int newNode = nodusNet.Node(coordinates);
            // Connect new node
            for (int i = 0; i < selectedNodes.Count; i++)
            {
                List<int> connections = nodusNet.GetNodeConnections(selectedNodes[i]);
                for (int j = 0; j < connections.Count; j++)
                {
                    nodusNet.ConnectNodes(newNode, connections[j]);
                }
            }
            // Delete merge nodes
            for (int i = 0; i < nodusNet.NetSize; i++)
            {
                for (int j = 0; j < selectedNodes.Count; j++)
                {
                    nodusNet.DeleteNode(selectedNodes[j]);
                    if (selectedNodes[j] <= i) i--;
                    for (int k = 0; k < selectedNodes.Count; k++)
                    {
                        if (selectedNodes[k] > selectedNodes[j]) selectedNodes[k] -= 1;
                    }
                }
                selectedNodes.Clear();
            }
        }
    }
    public void ResetNodusNet()
    {
        selectedNodes.Clear();
        nodusNet.EraseNet();
    }
    public void ChangeHandleType()
    {
        if (handleType == HandleType.Free) handleType = HandleType.Position;
        else handleType = HandleType.Free;
    }
    void SelectAllNodes()
    {
        Debug.Log("Select All nodes");
        selectedNodes.Clear();
        for (int i = 0; i < nodusNet.NetSize; i++)
        {
            selectedNodes.Add(i);
        }
    }
    void InvertSelection()
    {
        Debug.Log("Invert selection");
        for (int i = 0; i < nodusNet.NetSize; i++)
        {
            if (selectedNodes.Contains(i)) selectedNodes.Remove(i);
            else selectedNodes.Add(i);
        }
    }

    public NodeType.Type GetDefaultType()
    {
        return type;
    }
    public void SetDefaultType(NodeType.Type defaultType)
    {
        if (type == defaultType || defaultType == NodeType.Type.undefined) return;

        type = defaultType;
        if(selectedNodes.Count > 0)
        {
            for(int i = 0; i < selectedNodes.Count; i++)
            {
                nodusNet.SetNodeType(selectedNodes[i], type);
            }
        }
    }
    public bool IsDrawingIndex() { return drawIndex; }
    public bool IsSmartSnap() { return smartSnap; }
    public void SetDrawIndex(bool draw) { drawIndex = draw; }
    public void SetSmartSnap(bool snap) { smartSnap = snap; }

    public List<int> GetSelected()
    {
        return selectedNodes;
    }
}
public class NodusNetMenuButton
{
    [MenuItem("CinnamonWorks/NodusNodus/Create new.../Nodus Net", false, 0)]
    static void MenuCreateNodusNetGameObject()
    {
        GameObject obj = new GameObject();
        obj.name = "NodusNet";

        NodusNet nodusNet = obj.AddComponent<NodusNet>();

        Vector3 sceneViewPos = SceneView.lastActiveSceneView.camera.transform.position;
        sceneViewPos.z = 0;

        nodusNet.Node(sceneViewPos);

        Selection.activeGameObject = obj;
    }
    [MenuItem("CinnamonWorks/NodusNodus/Create new.../Free Node", false, 0)]
    static void MenuCreateFreeNodeGameObject()
    {
        GameObject obj = new GameObject();
        obj.name = "FreeNode";

        FreeNode freeNode = obj.AddComponent<FreeNode>();

        GameObject selection = Selection.activeGameObject;
        if (selection != null)
        {
            freeNode.transform.SetParent(selection.transform);
            freeNode.transform.localPosition = Vector3.zero;
        }
        else
        {
            Vector3 sceneViewPos = SceneView.lastActiveSceneView.camera.transform.position;
            sceneViewPos.z = 0;
            freeNode.transform.position = sceneViewPos;
        }

        Selection.activeGameObject = obj;
    }
}
public class NodusNetSceneOverlay
{
    static Texture2D logoTexture = (Texture2D)Resources.Load("Gizmos/node_multiSelect");

    static int top = 0;
    static int down = 0;
    static bool helpBox = false;

    static string helpText = "\nSHIFT:\tAdd\n" + "ALT:\tRemove\n\n" + "CTRL: Select\n" 
        + "CTRL + SHIFT: Drag & Select\n" + "CTRL + SHIFT + A: Select all\n" 
        + "CTRL + SHIFT + I: Invert selection\n" + "RIGHT CLICK: Deselect";

    static string handleTypeText = "Free";

    public static void OnGUI(NodusNet nodusNet, NodusNetInspector editor)
    {
        Handles.BeginGUI();

        int size = 18;
        int posX = 3;        

        down = (int)Screen.height - size * 3 - 2;
        GUI.Box(new Rect(0, top, Screen.width, size), "", EditorStyles.toolbar);
        GUI.Box(new Rect(0, down, Screen.width, size), "", EditorStyles.toolbar);

        /*-------------- TOP BAR --------------*/
        GUI.Label(new Rect(posX, top+2, size, size), logoTexture);

        posX += size/2;
        if (GUI.Button(new Rect(Screen.width - size * 4, top, size * 4, size), "HELP!", EditorStyles.toolbarButton)) helpBox = !helpBox;
        if (helpBox)
        {
            EditorGUI.HelpBox(new Rect(Screen.width - size * 12, top + size, size * 11, size * 6), helpText, MessageType.None);
        }
        posX += size;
        if (GUI.Button(new Rect(posX, top, size * 4, size), "New Node", EditorStyles.toolbarButton))
        {
            int nodeNum = nodusNet.Node(SceneView.lastActiveSceneView.camera.transform.position);
            nodusNet.SetNodeType(nodeNum, editor.GetDefaultType());
        }

        posX += size * 4;
        NodeType.Type type = editor.GetDefaultType();
        type = (NodeType.Type)EditorGUI.EnumPopup(new Rect(posX, top, size * 3, size), type, EditorStyles.toolbarPopup);
        editor.SetDefaultType(type);

        posX += size *3;
        bool snap = editor.IsSmartSnap();
        snap = GUI.Toggle(new Rect(posX, top, size * 4, size), snap, "SmartSnap", EditorStyles.toolbarButton);
        editor.SetSmartSnap(snap);

        posX += size * 4 + 6;
        if (GUI.Button(new Rect(posX, top, size * 3, size), "Center", EditorStyles.toolbarButton)) { editor.CenterPosition(); }

        posX += size * 3;
        if (GUI.Button(new Rect(posX, top, size * 3, size), "Merge", EditorStyles.toolbarButton)) { editor.MergeNodes(); }

        posX += size * 3;
        if (GUI.Button(new Rect(posX, top, size * 3, size), "Reset", EditorStyles.toolbarButton)) { editor.ResetNodusNet(); }

        posX += size * 3 + 6;
        bool draw = editor.IsDrawingIndex();
        draw = GUI.Toggle(new Rect(posX, top, size * 2, size), draw, "123", EditorStyles.toolbarButton);
        editor.SetDrawIndex(draw);

        posX += size * 2;
        if (GUI.Button(new Rect(posX, top, size * 2, size), handleTypeText, EditorStyles.toolbarButton))
        {
            editor.ChangeHandleType();
            if (handleTypeText == "Free") handleTypeText = "Pos";
            else handleTypeText = "Free";
        }
        /*-------------------------------------*/

        /*-------------- DOWN BAR -------------*/
        posX = 3;
        string infotext = "";

        if (Event.current.alt && !Event.current.shift && !Event.current.control) infotext = "Remove node";
        if (!Event.current.alt && Event.current.shift && !Event.current.control) infotext = "Add node";
        if (!Event.current.alt && !Event.current.shift && Event.current.control) infotext = "Select node";
        if (!Event.current.alt && Event.current.shift && Event.current.control) infotext = "Drag & select";

        GUI.Label(new Rect(posX, down, Screen.width, size), infotext, EditorStyles.label);
        /*-------------------------------------*/


        Handles.EndGUI();
    }
}