using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameManagerEx
{
    [Header("Prefabs")]
    private GameObject _player1Prefab;
    private GameObject _player2Prefab;
    private GameObject _tilePrefab;
    private GameObject _wallVPrefab;
    private GameObject _wallHPrefab;
    private GameObject _goalPrefab;
    private GameObject _seamPrefab;

    [Header("Map")]
    private GameObject[,] _objMap;
    private const int MapSize = 7;
    private const int MaxSize = MapSize * 2 - 1;

    [Header("Algorithm")]
    private readonly int[] _dr = { 1, -1, 0, 0 };
    private readonly int[] _dc = { 0, 0, 1, -1 };
    private readonly int[] _vdr = { 1, -1, 2, -2, 1, -1 };
    private readonly int[] _vdc = { 1, 1, 0, 0, -1, -1 };
    private readonly int[] _hdr = { 0, 0, 1, 1, -1, -1 };
    private readonly int[] _hdc = { 2, -2, 1, -1, 1, -1 };

    [Header("Others")]
    private GameObject _player1;
    private GameObject _player2;
    private int _moveCount;
    private int _wallCount;
    private bool _overlapped;
    private List<MapObject> _wallList;
    
    private enum Mode
    {
        None = 0,
        Wall = 1,
        Move = 2
    }

    private int _turn;
    private Mode _mode;

    public void Init()
    {
        LoadAssets();
        MapSetting();
        PlayerSetting();
        Managers.UI.Init();
        StartGame();

        _turn = 1;
        _mode = Mode.None;

        _wallCount = 0;
        _moveCount = 0;

        _overlapped = false;
    }

    public void LoadAssets()
    {
        _player1Prefab = (GameObject)Resources.Load("Prefabs/Player1");
        _player2Prefab = (GameObject)Resources.Load("Prefabs/Player2");
        
        _tilePrefab = (GameObject)Resources.Load("Prefabs/Tile");
        _wallHPrefab = (GameObject)Resources.Load("Prefabs/Wall_V");
        _wallVPrefab = (GameObject)Resources.Load("Prefabs/Wall_H");
        _seamPrefab = (GameObject)Resources.Load("Prefabs/Seam");
        _goalPrefab = (GameObject)Resources.Load("Prefabs/Goal");
    }

    public void MapSetting()
    {
        _objMap = new GameObject[MaxSize, MaxSize];

        _objMap[0, 0] = Object.Instantiate(_tilePrefab, new Vector2(-6, 4), Quaternion.identity);
        _objMap[0, 0].GetComponent<MapObject>().SetCoordinates(0, 0);
        for (int row = 1; row < MaxSize; row++)
        {
            Vector2 prevPos = _objMap[row - 1, 0].transform.position;
            Vector2 nextPos = prevPos + (Vector2.down * 0.6f);
            if (row % 2 != 0)
            {
                _objMap[row, 0] = Object.Instantiate(_wallHPrefab, nextPos, Quaternion.identity);
                _objMap[row, 0].GetComponent<MapObject>().SetCoordinates(row, 0);
            }
            else
            {
                _objMap[row, 0] = Object.Instantiate(_tilePrefab, nextPos, Quaternion.identity);
                _objMap[row, 0].GetComponent<MapObject>().SetCoordinates(row, 0);
            }
        }
        
        for (int row = 0; row < MaxSize; row++)
        {
            Vector2 prevPos = _objMap[row, 0].transform.position;
            for (int col = 1; col < MaxSize; col++)
            {
                Vector2 nextPos = prevPos + (Vector2.right * 0.6f);
                
                if (row == 0 && col == MapSize - 1)
                {
                    _objMap[row, col] = Object.Instantiate(_goalPrefab, nextPos, Quaternion.identity);
                    _objMap[row, col].GetComponent<MapObject>().SetCoordinates(row, col);
                    prevPos = nextPos;
                    continue;
                }
                
                if (row % 2 == 0)
                {
                    if (col % 2 != 0)
                    {
                        _objMap[row, col] = Object.Instantiate(_wallVPrefab, nextPos, Quaternion.identity);
                        _objMap[row, col].GetComponent<MapObject>().SetCoordinates(row, col);
                    }
                    else
                    {
                        _objMap[row, col] = Object.Instantiate(_tilePrefab, nextPos, Quaternion.identity);
                        _objMap[row, col].GetComponent<MapObject>().SetCoordinates(row, col);
                    }
                }
                else
                {
                    if (col % 2 != 0)
                    {
                        _objMap[row, col] = Object.Instantiate(_seamPrefab, nextPos, Quaternion.identity);
                        _objMap[row, col].GetComponent<MapObject>().SetCoordinates(row, col);
                    }
                    else
                    {
                        _objMap[row, col] = Object.Instantiate(_wallHPrefab, nextPos, Quaternion.identity);
                        _objMap[row, col].GetComponent<MapObject>().SetCoordinates(row, col);
                    }
                }
                prevPos = nextPos;
            }
        }
    }

    public void PlayerSetting()
    {
        _player1 = Object.Instantiate(_player1Prefab, _objMap[MaxSize - 1, 0].transform.position, Quaternion.identity);
        _player1.GetComponent<MapObject>().row = MaxSize - 1;
        _player1.GetComponent<MapObject>().col = 0;
        _player2 = Object.Instantiate(_player2Prefab, _objMap[MaxSize - 1, MaxSize - 1].transform.position, Quaternion.identity);
        _player2.GetComponent<MapObject>().row = MaxSize - 1;
        _player2.GetComponent<MapObject>().col = MaxSize - 1;
        
        Camera.main.transform.position = new Vector3(0.8f, 0.3f, -10f);
    }
    
    public void StartGame()
    {
        GameObject go = new GameObject { name = "@PlayerController" };
        go.AddComponent<PlayerController>();
    }
    
    public void WallMode()
    {
        _mode = Mode.Wall;
        
        _wallList = new List<MapObject>();
        
        Managers.UI.WallActionLeft(3);
        Managers.UI.WallMode();
    }

    public void MoveMode()
    {
        _mode = Mode.Move;
        
        Managers.UI.MoveActionLeft(3);
        Managers.UI.MoveMode();
    }

    public void NextTurn()
    {
        _mode = Mode.None;
        _turn = _turn == 1 ? 2 : 1;
        _moveCount = 0;
        _wallCount = 0;
        
        Managers.UI.ResetAction();
        Managers.UI.TurnChanged(_turn);
    }

    public void Clicked(GameObject go)
    {
        if (CheckMode(go) != _mode) return;
        
        switch (_mode)
        {
            case Mode.None:
                break;
            case Mode.Move:
                if (_moveCount == 3) break;
                if (CanMove(go) == false) break;
                Move(go);
                break;
            case Mode.Wall:
                if (_wallCount == 3) break;
                if (CanBuild(go) == false) break;
                MakeWall(go);
                break;
            default:
                Console.WriteLine("Mouse Input Error");
                break;
        }
    }

    private Mode CheckMode(GameObject go)
    {
        MapObject mo = go.GetComponent<MapObject>();
        return (mo.row + mo.col) % 2 == 0 ? Mode.Move : Mode.Wall;
    }

    public bool CanBuild(GameObject go)
    {
        if (IsAdjacent(go) == false) return false;

        var mo = go.GetComponent<MapObject>();
        if (mo.blocked) return false;
        
        _objMap[mo.row, mo.col].GetComponent<MapObject>().blocked = true;

        bool flag = BFS(_player1) && BFS(_player2);

        _objMap[mo.row, mo.col].GetComponent<MapObject>().blocked = false;
        
        return flag;
    }

    public bool BFS(GameObject go)
    {
        Queue<Coord> q = new Queue<Coord>();
        bool[,] v = new bool[MaxSize, MaxSize];

        MapObject mo = go.GetComponent<MapObject>();
        q.Enqueue(new Coord(mo.row, mo.col));
        v[mo.row, mo.col] = true;

        int cnt = 0;
        
        while (q.Count > 0)
        {
            cnt++;
            if (cnt > 1000)
            {
                Debug.Log("Error in BFS");
                return false;
            }
            
            var current = q.Dequeue();
            int cr = current.Row;
            int cc = current.Col;

            if (cr == 0 && cc == MapSize - 1)
            {
                return true;
            }

            for (int dir = 0; dir < 4; dir++)
            {
                int nr = cr + _dr[dir];
                int nc = cc + _dc[dir];
                
                if (nr < 0 || nc < 0 || nr >= MaxSize || nc >= MaxSize) continue;
                if (v[nr, nc]) continue;
                if (_objMap[nr, nc].GetComponent<MapObject>().blocked) continue;

                v[nr, nc] = true;
                q.Enqueue(new Coord(nr, nc));
            }
        }
        
        return false;
    }

    public bool IsAdjacent(GameObject go)
    {
        if (_wallCount == 0)
        {
            return true;
        }

        MapObject mo = go.GetComponent<MapObject>();
        int cr = mo.row;
        int cc = mo.col;
        
        for (int dir = 0; dir < 6; dir++)
        {
            int nr, nc;
            if (mo.isVertical)
            {
                nr = cr + _vdr[dir];
                nc = cc + _vdc[dir];
            }
            else
            {
                nr = cr + _hdr[dir];
                nc = cc + _hdc[dir];
            }

            foreach (var wall in _wallList)
            {
                if (wall.row == nr && wall.col == nc) return true;
            }
        }

        return false;
    }

    public bool CanMove(GameObject go)
    {
        GameObject currentPlayer;
        if (_turn == 1) currentPlayer = _player1;
        else currentPlayer = _player2;
        
        // check if adjacent
        float distance = Vector2.Distance(currentPlayer.transform.position, go.transform.position);
        if (distance >= 1.3f || distance <= 0.1f) return false;
        
        // check if wall exists
        MapObject mop = currentPlayer.GetComponent<MapObject>();
        MapObject tile = go.GetComponent<MapObject>();

        int r = (mop.row + tile.row) / 2;
        int c = (mop.col + tile.col) / 2;
        if (_objMap[r, c].GetComponent<MapObject>().blocked) return false;
        
        return true;
    }

    public void MakeWall(GameObject go)
    {
        go.GetComponent<SpriteRenderer>().color = Color.cyan;
        go.GetComponent<MapObject>().blocked = true;
        _wallCount++;
        _wallList.Add(go.GetComponent<MapObject>());
        
        Managers.UI.WallActionLeft(3 - _wallCount);
    }

    public void Move(GameObject go)
    {
        MapObject mo1 = _player1.GetComponent<MapObject>();
        MapObject mo2 = _player2.GetComponent<MapObject>();

        if (_overlapped)
        {
            ChangeColor(false);
            _overlapped = false;
        }
        
        if (_turn == 1)
        {
            _player1.transform.position = go.transform.position;
            _player1.GetComponent<MapObject>().row = go.GetComponent<MapObject>().row;
            _player1.GetComponent<MapObject>().col = go.GetComponent<MapObject>().col;
            _moveCount++;
        }
        else
        {
            _player2.transform.position = go.transform.position;
            _player2.GetComponent<MapObject>().row = go.GetComponent<MapObject>().row;
            _player2.GetComponent<MapObject>().col = go.GetComponent<MapObject>().col;
            _moveCount++;
        }

        if (mo1.row == mo2.row && mo1.col == mo2.col)
        {
            _overlapped = true;
            ChangeColor(true);
        }
        
        
        Managers.UI.MoveActionLeft(3 - _moveCount);
        
        MapObject mo = go.GetComponent<MapObject>();
        if (mo.row == 0 && mo.col == MapSize - 1)
        {
            EndGame();
        }
    }

    public void ChangeColor(bool overlapped)
    {
        if (overlapped) // To Purple
        {
            _player2.GetComponent<SpriteRenderer>().color = Color.magenta;
        }
        else
        {
            _player2.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void EndGame()
    {
        Managers.UI.GameEnd(_turn);
    }

    public void BackToLobby()
    {
        Managers.Scene.LoadLobby();
    }
}
