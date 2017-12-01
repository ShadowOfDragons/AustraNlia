using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class NodusNet : MonoBehaviour
{
    [SerializeField] private List<NodeData> net;
    public int NetSize { get { return net.Count; } }
    public Color gizmoColor = Color.red;

    //Node list
    public bool EmptyNet()
    {
        if (net == null) return true;
        else return false;
    }
    public void CreateNet()
    {
        net = new List<NodeData>();
    }
    public void EraseNet()
    {
        net.Clear();
        CreateNet();
    }
    //Create node
    public int Node()
    {
        return Node(Vector2.zero);
    }
    public int Node(Vector2 position)
    {
        if (EmptyNet()) CreateNet();
        
        NodeData nodeData = new NodeData(net.Count, position);
        net.Add(nodeData);

        return nodeData.id;
    }
    //Delete node
    public void DeleteNode(int a)
    {
        if(net[a].HasConnections()) Breakconnections(a);
        net.RemoveAt(a);
        UpdateNodeListConnections(a); 

        for(int i = 0; i < NetSize; i++) net[i].id = i;
    }
    //Connections
    private bool AreConnected(int a, int b)
    {
        if (net[a].IsConnectWith(b) || net[b].IsConnectWith(a)) return true;
        return false;
    }
    public void ConnectNodes(int a, int b)
    {
        if (a < 0 || b < 0 || a > NetSize || b > NetSize) return;
        if (!AreConnected(a,b) && a != b)
        {
            net[a].ConnectWith(b);
            net[b].ConnectWith(a);
        }
    }
    public void DisconnectNodes(int a, int b)
    {
        if (a < 0 || b < 0 || a > NetSize || b > NetSize) return;
        if (AreConnected(a, b) && a != b)
        {
            net[a].DisconnectFrom(b);
            net[b].DisconnectFrom(a);
        }
    }
    public void Breakconnections(int a)
    {
        if(net[a].HasConnections())
        {
            for (int i = 0; i < NetSize; i++) DisconnectNodes(a, i);
        }
    }
    public bool HasConnections(int a)
    {
        return net[a].HasConnections();
    }
    //Update nodelist data
    public void UpdateRelativePosition()
    {
        Vector2 netPosition = transform.position;
        transform.position = Vector3.zero;

        for(int i = 0; i < NetSize; i++)
        {
            SetNodeRelativeCoordinates(i, netPosition);
        }
    }
    public void UpdateNodeListConnections(int a)
    {
        for (int i = 0; i < NetSize; i++)
        { 
            net[i].UpdateNodeConnections(a);
        }
    }
    public void UpdateNodeCoordinates(int a, Vector2 newCoordinate)
    {
        net[a].SetCoordinates(newCoordinate);
    }
    //Get data
    public Vector2 GetNodeCoordinates(int a)
    {
        return net[a].coordinates;
    }
    public List<int> GetNodeConnections(int a)
    {
        return net[a].connections;
    }
    public int GetNodeConnectionsCount(int a)
    {
        return net[a].ConnectionsCount;
    }
    public int GetNodeID(int a)
    {
        return net[a].id;
    }
    public NodeType.Type GetNodeType(int a)
    {
        return net[a].type;
    }
    public List<NodeData> GetNetList()
    {
        return net;
    }
    public NodeData[] GetNetArray()
    {
        return net.ToArray();
    }    
    public NodeData GetNodeData(int a)
    {
        return net[a];
    }
    //Set data
    public void SetNodeCoordinates(int a, Vector2 coordinates)
    {
        net[a].SetCoordinates(coordinates);
    }
    public void SetNodeRelativeCoordinates(int a, Vector2 relativeCoordinates)
    {
        net[a].SetRelativeCoordinates(relativeCoordinates);
    }
    public void SetNodeType(int a, NodeType.Type newType)
    {
        net[a].type = newType;
    }


    //Draw gizmos
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (Array.IndexOf(UnityEditor.Selection.gameObjects, this.gameObject) >= 0) return;

        Gizmos.color = gizmoColor;
        Matrix4x4 mat = transform.localToWorldMatrix;

        for(int i = 0; i < NetSize; i++)
        {
            Vector3 nodePosition = mat.MultiplyPoint3x4(GetNodeCoordinates(i));

            List<int> connections = GetNodeConnections(i);         
            for(int j = 0; j < connections.Count; j++)
            {
                Vector3 connectionPosition = mat.MultiplyPoint3x4(GetNodeCoordinates(connections[j]));
                Gizmos.DrawLine(nodePosition, connectionPosition);
            }

            //Gizmos.DrawIcon(nodePosition, "node_normal.png", true);
        }
    }
#endif

}
