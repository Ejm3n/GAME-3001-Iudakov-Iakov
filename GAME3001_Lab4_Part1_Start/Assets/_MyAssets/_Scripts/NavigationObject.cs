using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationObject : MonoBehaviour
{
    public Vector2 gridIndex;

    private void Awake()
    {
        gridIndex = new Vector2();
        SetGridIndex();
    }
    public Vector2 GetGridIndex()
    {
        return gridIndex;
    }
    public void SetGridIndex()
    {
        float originalX = Mathf.Floor(transform.position.x) + .5f;
        gridIndex.x = ((int)Mathf.Floor(originalX + 7.5f));
        float originalY = Mathf.Floor(transform.position.y) + .5f;
        gridIndex.y = 11 - ((int)Mathf.Floor(originalY + 5.5f));
    }
}
