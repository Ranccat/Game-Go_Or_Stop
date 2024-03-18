using UnityEngine;

public class MapObject : MonoBehaviour
{
    public int row, col;
    public bool blocked, isVertical, isTile;

    public void SetCoordinates(int r, int c)
    {
        row = r;
        col = c;
    }
}
