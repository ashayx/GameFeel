using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Tilemaps;
using UnityEngine.SceneManagement;
using UnityEditor;

[CreateAssetMenu(fileName = "Prefab Brush")]
[CustomGridBrush(false, true, false, "Prefab Brush")]
public class PrefabBrush : GameObjectBrush
{
    //public GameObject GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    //{
    //    int childCount;
    //    GameObject[] sceneChildren = null;
    //    if (parent == null)
    //    {
    //        var scene = SceneManager.GetActiveScene();
    //        sceneChildren = scene.GetRootGameObjects();
    //        childCount = scene.rootCount;
    //    }
    //    else
    //    {
    //        childCount = parent.childCount;
    //    }
    //    var anchorCellOffset = Vector3Int.FloorToInt(m_Anchor);
    //    var cellSize = grid.cellSize;
    //    anchorCellOffset.x = cellSize.x == 0 ? 0 : anchorCellOffset.x;
    //    anchorCellOffset.y = cellSize.y == 0 ? 0 : anchorCellOffset.y;
    //    anchorCellOffset.z = cellSize.z == 0 ? 0 : anchorCellOffset.z;

    //    //Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));
    //    //Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + cellSize));
    //    //Bounds bounds = new Bounds((min + max) * 0.5f, cellSize);
    //    Debug.Log(GetCellIndex(position));
    //    //for (var i = 0; i < childCount; i++)
    //    //{
    //    //    var child = sceneChildren == null ? parent.GetChild(i) : sceneChildren[i].transform;

    //    //    if (bounds.Contains(child.position))
    //    //    {
    //    //        return child.gameObject;
    //    //    }
    //    //}
    //    for (var i = 0; i < childCount; i++)
    //    {
    //        var child = sceneChildren == null ? parent.GetChild(i) : sceneChildren[i].transform;
    //        Debug.Log(grid.WorldToCell(child.position) - anchorCellOffset);
    //        if (position == grid.WorldToCell(child.position) - anchorCellOffset)
    //            return child.gameObject;
    //    }
    //    return null;
    //}

    //public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
    //{
    //    Reset();
    //    UpdateSizeAndPivot(new Vector3Int(position.size.x, position.size.y, 1), new Vector3Int(pivot.x, pivot.y, 0));

    //    GetGrid(ref gridLayout, ref brushTarget);

    //    foreach (Vector3Int pos in position.allPositionsWithin)
    //    {
    //        Vector3Int brushPosition = new Vector3Int(pos.x - position.x, pos.y - position.y, 0);
    //        PickCell(pos, brushPosition, gridLayout, brushTarget != null ? brushTarget.transform : null);
    //    }
    //}

    //private void GetGrid(ref GridLayout gridLayout, ref GameObject brushTarget)
    //{
    //    if (brushTarget == hiddenGrid)
    //        brushTarget = null;
    //    if (brushTarget != null)
    //    {
    //        var targetGridLayout = brushTarget.GetComponent<GridLayout>();
    //        if (targetGridLayout != null)
    //            gridLayout = targetGridLayout;
    //    }
    //}

    //private void PickCell(Vector3Int position, Vector3Int brushPosition, GridLayout grid, Transform parent)
    //{
    //    Vector3 cellCenter = grid.LocalToWorld(grid.CellToLocalInterpolated(position + m_Anchor));
    //    GameObject go = GetObjectInCell(grid, parent, position);

    //    if (go != null)
    //    {
    //        Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
    //        if (prefab)
    //        {
    //            SetGameObject(brushPosition, (GameObject)prefab);
    //        }
    //        else
    //        {
    //            GameObject newInstance = Instantiate(go);
    //            newInstance.hideFlags = HideFlags.HideAndDontSave;
    //            newInstance.SetActive(false);
    //            SetGameObject(brushPosition, newInstance);
    //        }

    //        SetOffset(brushPosition, go.transform.position - cellCenter);
    //        SetScale(brushPosition, go.transform.localScale);
    //        SetOrientation(brushPosition, go.transform.localRotation);
    //    }
    //}


}
