using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NodusBFS
{
    private static float[] BFS(NodusNet net, int root, bool ignoreDanger)
    {
        float[] distanceFromRoot = new float[net.NetSize];
        for (int i = 0; i < distanceFromRoot.Length; i++) distanceFromRoot[i] = Mathf.Infinity;

        Queue<NodeData> Q = new Queue<NodeData>();
        distanceFromRoot[root] = 0;
        Q.Enqueue(net.GetNodeData(root));

        while (Q.Count > 0)
        {
            NodeData current = Q.Dequeue();

            List<int> connections = current.connections;
            for (int i = 0; i < connections.Count; i++)
            {
                //if !danger {}
                if(ignoreDanger || (net.GetNodeType(connections[i]) != NodeType.Type.Enemy && net.GetNodeType(connections[i]) != NodeType.Type.Hazard))
                {
                    if(distanceFromRoot[connections[i]] == Mathf.Infinity)
                    {
                        float distance = Vector2.Distance(net.GetNodeCoordinates(current.id), net.GetNodeCoordinates(connections[i]));
                        distanceFromRoot[connections[i]] = distanceFromRoot[current.id] + distance;
                        Q.Enqueue(net.GetNodeData(connections[i]));
                    }
                }
                else
                {
                    //Debug.Log(connections[i] + " DANGER NODE");
                    //Debug.Log(ignoreDanger + "   " + net.GetNodeType(connections[i]).ToString());
                }
            }
        }
        return distanceFromRoot;
    }

    public static int[] GetPath(NodusNet net, int from, int to, bool ignoreDanger)
    {
        //float[] bfsDistance = BFS(net, from);
        float[] bfsDistance = BFS(net, to, ignoreDanger);
        float distance = Mathf.Infinity;
        Queue<int> Q = new Queue<int>();
        
        //if(bfsDistance[to] < distance) distance = bfsDistance[to];
        if(bfsDistance[from] < distance) distance = bfsDistance[from];
        //for(int i = 0; i < bfsDistance.Length; i++) Debug.Log("bfsDistance: "+ i "_" + bfsDistance[i]);

        if (distance == Mathf.Infinity)
        {
            //Debug.LogError("No valid path " + Q.ToArray().Length);
            return Q.ToArray();
        }

        else
        {            
            //Q.Enqueue(to);
            //Q.Enqueue(from);
            //while (!Q.Contains(from))
            while(!Q.Contains(to))
            {
                //int aux = to;
                int aux = from;

                //foreach (int n in net.GetNodeConnections(to))
                foreach(int n in net.GetNodeConnections(from))
                {
                    if (bfsDistance[n] <= distance)
                    {
                        distance = bfsDistance[n];
                        aux = n;
                    }
                }
                //to = aux;
                //Q.Enqueue(to);

                from = aux;
                Q.Enqueue(from);
            }
            //Q.Enqueue(to);
            //Quitar el from e invertir el array.
            int[] qArray = Q.ToArray();
            //System.Array.Resize(ref qArray, qArray.Length - 1);/
            //System.Array.Reverse(qArray);

            return qArray;
        } 
        //return Q.ToArray();
    }
}
