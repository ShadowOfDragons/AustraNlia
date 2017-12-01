using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeNode : MonoBehaviour
{
    [Header("Properties")]
    public NodeType.Type type;
    public NodusNet _nodusNet;
    public bool Xconstrain;
    public bool Yconstrain;
    public bool free;
    public bool initializeAtStart = false;
    [Header("State")]    
    private int _index;
    private bool _connectionsHasChanged = false;
    private Vector2 _lastPosition;
    private int _lastFirstConnection, _lastSecondConnection;
    private int _firstConnection, _secondConnection;
    [Header("Target")]
    private FreeNode _targetNode;
    private int _target = -1;
    [Header("Path")]
    public int[] path;
    public int indexPath;

    private void Start()
    {
        if(initializeAtStart)Initialize();
    }

    #region Initialize
    public void Initialize()
    {
        _target = -1;
        _targetNode = null;

       // FindTerrainNodusNet();
        InitializeFreeNode();
    }
    /*void FindTerrainNodusNet()
    {
        if(_nodusNet == null) _nodusNet = GameObject.FindGameObjectWithTag("TerrainNodusNet").GetComponent<NodusNet>();
    }*/
    void InitializeFreeNode()
    {        
        if (_nodusNet != null)
        {
            Create();
            FindNearConnections();
            ConnectToNet();
        }
        else Debug.LogError("Terrain Nodus Net not initialized");
    }
    void Create()
    {
        _index = _nodusNet.Node(transform.position);
        _nodusNet.SetNodeType(_index, type);

        _lastFirstConnection = -1;
        _lastSecondConnection = -1;
        _lastPosition = transform.position;
    }
    #endregion
    #region Net connection
    void ConnectToNet()
    {
        if(_nodusNet.NetSize > 0)
        { 
            if((_firstConnection != _lastFirstConnection) || (_secondConnection != _lastSecondConnection))
            {
                if((_firstConnection != _lastSecondConnection) ||(_secondConnection != _lastFirstConnection))
                {
                    _nodusNet.DisconnectNodes(_index, _lastFirstConnection);
                    _nodusNet.DisconnectNodes(_index, _lastSecondConnection);

                    _nodusNet.ConnectNodes(_index, _firstConnection);
                    _nodusNet.ConnectNodes(_index, _secondConnection);

                    _lastFirstConnection = _firstConnection;
                    _lastSecondConnection = _secondConnection;

                    _connectionsHasChanged = true;
                }
            }     
        }
    }
    void FindNearConnections()
    {
        if(_nodusNet.NetSize > 0)
        {
            _firstConnection = FindFirstConnection();
            _secondConnection = FindSecondConnection();
        }
    }
    int FindFirstConnection()
    {
        int nearestNode = -1;
        float minDistance = Mathf.Infinity;

        for(int i = 0; i < _nodusNet.NetSize; i++)
        {
            NodeType.Type type = _nodusNet.GetNodeType(i);
            if(i != _index && type != NodeType.Type.Enemy)
            {                
                Vector3 nodeCoords = _nodusNet.GetNodeCoordinates(i);               
                if(transform.position.y >= nodeCoords.y)
                {
                    float distance = Vector2.Distance(transform.position, nodeCoords);
                    
                    if(distance < minDistance)
                    {
                        minDistance = distance;
                        nearestNode = i;
                    }
                }
            }
        }
        return nearestNode;
    }
    int FindSecondConnection()
    {
        int nearestNode = -1;

        if (_nodusNet.HasConnections(_firstConnection))
        {
            List<int> connections = _nodusNet.GetNodeConnections(_firstConnection);
            float minDistance = Mathf.Infinity;

            Vector2 nodeCoord = transform.position;
            Vector2 firstCoord = _nodusNet.GetNodeCoordinates(_firstConnection);

            List<int> exceptionsNodes = new List<int>();

            for (int i = 0; i < connections.Count; i++)
            {
                NodeType.Type type = _nodusNet.GetNodeType(connections[i]);
                if(connections[i] != _index && type != NodeType.Type.Enemy)
                {
                    Vector2 secondCoord = _nodusNet.GetNodeCoordinates(connections[i]);

                    float distance = Vector2.Distance(transform.position, secondCoord);

                    bool clampX = false;
                    bool clampY = false;

                    if (Xconstrain)
                    {
                        if ((firstCoord.x <= nodeCoord.x && nodeCoord.x <= secondCoord.x) || (firstCoord.x >= nodeCoord.x && nodeCoord.x >= secondCoord.x)) clampX = true;   
                    }
                    else clampX = true;

                    if (Yconstrain)
                    {
                        if ((firstCoord.y <= nodeCoord.y && nodeCoord.y <= secondCoord.y) || (firstCoord.y >= nodeCoord.y && nodeCoord.y >= secondCoord.y)) clampY = true;
                    }
                    else clampY = true;
                    
                    if (clampX && clampY && distance < minDistance)
                    {  
                        minDistance = distance;
                        nearestNode = connections[i];
                    }
                }
            }
        }
        return nearestNode;
    }
    #endregion
    #region Target
    void FindTarget()
    {
        _target = -1;
        GameObject[] freeNodes = GameObject.FindGameObjectsWithTag("FreeNode");
        foreach (GameObject fn in freeNodes)
        {
            if (fn != this.gameObject)
            {
                _targetNode = fn.GetComponent<FreeNode>();
                _target = _targetNode.GetFreeNodeIndex();
                return;
            }
        }
    }    
    bool TargetConnectionChanged()
    {
        if (_targetNode != null)
        {
            return _targetNode.ConnectionsChanged();
        }
        else return false;
    }
    public void SetTargetIndex(int index)
    {
        _target = index;
    }
    public int SetNodeTarget(FreeNode n)
    {
        _targetNode = n;
        _target = _targetNode.GetFreeNodeIndex();

        return _target;
    }
    public bool ScanTarget()
    {
        if(TargetConnectionChanged() || ConnectionsChanged())
        {
            if(indexPath >= path.Length - 1)
            {
                //Debug.Log("ScanTarget: " + transform.parent.parent.parent.name);
                UpdateConnections();
            }
            FindPath();
            return true;
        }
        return false;
    }
    public bool HasTarget()
    {
        if(_target != -1) return true;
        else return false;
    }
    public bool HasConnectionsChanged()
    {
        if(TargetConnectionChanged() || ConnectionsChanged()) return true;
        else return false;
    }
    #endregion
    #region Path
    public void FindPath()
    {
        if(type == NodeType.Type.Enemy) path = NodusBFS.GetPath(_nodusNet, _index, _target, true);
        else path = NodusBFS.GetPath(_nodusNet, _index, _target, false);
        indexPath = 0;
    }
    public void GoToNextNode()
    {
        if(path.Length != 0)
        {            
            if (indexPath < path.Length - 1)
            {
                _firstConnection = path[indexPath];
                indexPath++;
                _secondConnection = path[indexPath];

                ConnectToNet();
            }
            else indexPath = path.Length - 1;
            
        }
    }
    public void SkipNode()
    {
        if(path.Length != 0)
        {
            if(indexPath < path.Length - 2)
            {
                indexPath++;
                _firstConnection = path[indexPath];
                indexPath++;
                _secondConnection = path[indexPath];                
                ConnectToNet();
            }
            else
            {
                GoToNextNode();
            }
        }
    }
    public void ResetPath()
    {
        path = new int[0];
    }
    public Vector3 GetNextNodeCoordinates()
    {
        if (path.Length > 0) return (Vector3)_nodusNet.GetNodeCoordinates(path[0]);
        else return new Vector3(0, 0, -1);
    }
    public Vector3 GetStepDistances()
    {
        Vector3 d = Vector3.zero;
        d.z = -1;
        if(path.Length > 2)
        {
            Vector3 a = (Vector3)_nodusNet.GetNodeCoordinates(path[0]);
            Vector3 b = (Vector3)_nodusNet.GetNodeCoordinates(path[1]);
            d = new Vector3(b.x - a.x, b.y - a.y, 0);
        }
        return d;
    }
    public Vector3 GetConnectionsDistance()
    {
        Vector3 d = Vector3.zero;
        d.z = -1;

        if(_firstConnection != -1 && _secondConnection != -1)
        {
            Vector3 a = _nodusNet.GetNodeCoordinates(_firstConnection);
            Vector3 b = _nodusNet.GetNodeCoordinates(_secondConnection);

            d = new Vector3(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y), 0);
            
        }

        return d;
    }
    public NodeType.Type GetNextNodeType()
    {
        if(path.Length > 0) return _nodusNet.GetNodeType(path[0]);
        else return NodeType.Type.Path;
    }
    #endregion
    #region GetSet
    public int GetFreeNodeIndex()
    {
        return _index;
    }
    public bool ConnectionsChanged()
    {
        bool changed = _connectionsHasChanged;
        _connectionsHasChanged = false;
        return changed;
    }
    public Vector2 GetCoordinates()
    {
        return _nodusNet.GetNodeCoordinates(_index);
    }
    public void SetConstrain(bool x, bool y)
    {
        Xconstrain = x;
        Yconstrain = y;
    }
    public void SetFree(bool f)
    {
        free = f;
    }
    #endregion
    public void UpdateConnections()
    {
        //Debug.Log("UpdateConnections: " + transform.parent.parent.parent.name);
        if(_nodusNet == null) return;
        _nodusNet.SetNodeCoordinates(_index, transform.position);
        _lastPosition = transform.position;
        FindNearConnections();
        ConnectToNet();
    }
    void Update()
    {
        //if(_nodusNet == null) Initialize();

        //Test functions
        //if (_lastPosition != (Vector2)transform.position) UpdateConnections();

        /*if(Input.GetKeyDown(KeyCode.P))
        {
            path = NodusBFS.GetPath(_nodusNet, _index, _target);
        }*/
        if(_nodusNet != null) _nodusNet.SetNodeCoordinates(_index, transform.position);
        if(!free)
        {
            /*if(CheckIfTargetConnectionsHasChanged())
            {
                Debug.Log("Recalculate path");
                if(indexPath <= 1)UpdateConnections();
                FindPath();
            }*/

            //if (Input.GetKeyDown(KeyCode.N)) GoToNextNode();
            //else if (Input.GetKeyDown(KeyCode.M)) SkipNode();
        }
        if(free)
        {
            if (_lastPosition != (Vector2)transform.position) UpdateConnections();
        }

    }
}
