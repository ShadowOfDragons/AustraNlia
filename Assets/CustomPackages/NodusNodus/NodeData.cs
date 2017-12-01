using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class NodeData
{
    [SerializeField] public int id;
    [SerializeField] public List<int> connections;
    [SerializeField] public Vector2 coordinates;
    [SerializeField] public NodeType.Type type;
    public int ConnectionsCount { get { return connections.Count; } }

    public NodeData()
    {
        new NodeData(0, Vector2.zero);
    }
    public NodeData(int id, Vector2 coordinates)
    {
        this.id = id;
        this.coordinates = coordinates;
        connections = new List<int>();
        type = NodeType.Type.Path;
    }

    public bool HasConnections()
    {
        if (connections.Count != 0) return true;
        return false;
    }
    public bool IsConnectWith(int num)
    {
        if (connections.Contains(num)) return true;
        return false;
    }
    public void ConnectWith(int num)
    {
        if (!connections.Contains(num)) connections.Add(num);
        else Debug.Log("Node " + id + " contains node " + num + " already.");
    }
    public void DisconnectFrom(int num)
    {
        if(connections.Contains(num)) connections.Remove(num);
        else Debug.Log("Node " + id + " doesn't contains node " + num);
    }
    public void UpdateNodeConnections(int num)
    {
        for(int i = 0; i < ConnectionsCount; i++)
        {
            if (connections[i] > num) connections[i]--;
        }
    }
    public void SetCoordinates(Vector2 coordinates)
    {
        this.coordinates = coordinates;
    }
    public void SetRelativeCoordinates(Vector2 relativeCoordinates)
    {
        coordinates += relativeCoordinates;
    }
}
