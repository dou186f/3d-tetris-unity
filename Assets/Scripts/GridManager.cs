using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 8;
    public int height = 16;
    public int depth = 8;

    private Transform[,,] grid;

    private void Awake()
    {
        grid = new Transform[width, height, depth];
    }

    public bool IsInsideGrid(Vector3Int gridPos)
    {
        bool insideX = gridPos.x >= 0 && gridPos.x < width;
        bool insideY = gridPos.y >= 0 && gridPos.y < height;
        bool insideZ = gridPos.z >= 0 && gridPos.z < depth;

        return insideX && insideY && insideZ;
    }

    public bool IsCellEmpty(Vector3Int gridPos)
    {
        if (!IsInsideGrid(gridPos))
        {
            return false;
        }

        return grid[gridPos.x, gridPos.y, gridPos.z] == null;
    }

    public bool CanPlaceAt(Vector3Int gridPos)
    {
        return IsInsideGrid(gridPos) && IsCellEmpty(gridPos);
    }

    public bool CanPlaceBlocks(Vector3Int originPosition, Vector3Int[] blockOffsets)
    {
        foreach (Vector3Int offset in blockOffsets)
        {
            Vector3Int blockGridPosition = originPosition + offset;

            if (!CanPlaceAt(blockGridPosition))
            {
                return false;
            }
        }

        return true;
    }

    public void RegisterBlock(Vector3Int gridPos, Transform blockTransform)
    {
        if (!IsInsideGrid(gridPos))
        {
            Debug.LogWarning("Tried to register block outside grid: " + gridPos);
            return;
        }

        grid[gridPos.x, gridPos.y, gridPos.z] = blockTransform;
    }

    public void RegisterBlocks(Vector3Int originPosition, Vector3Int[] blockOffsets, Transform[] blockTransforms)
    {
        for (int i = 0; i < blockOffsets.Length; i++)
        {
            Vector3Int blockGridPosition = originPosition + blockOffsets[i];
            RegisterBlock(blockGridPosition, blockTransforms[i]);
        }
    }

    public int ClearFullLayers()
    {
        int clearedLayerCount = 0;

        int y = 0;

        while (y < height)
        {
            if (IsLayerFull(y))
            {
                ClearLayer(y);
                MoveLayersDownAbove(y);

                clearedLayerCount++;

                // Do not increment y here.
                // Because after moving layers down, the new layer at this same y
                // might also be full.
            }
            else
            {
                y++;
            }
        }

        return clearedLayerCount;
    }

    private bool IsLayerFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                if (grid[x, y, z] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void ClearLayer(int y)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Transform blockTransform = grid[x, y, z];

                if (blockTransform != null)
                {
                    Destroy(blockTransform.gameObject);
                    grid[x, y, z] = null;
                }
            }
        }
    }

    private void MoveLayersDownAbove(int clearedY)
    {
        for (int y = clearedY + 1; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Transform blockTransform = grid[x, y, z];

                    grid[x, y - 1, z] = blockTransform;
                    grid[x, y, z] = null;

                    if (blockTransform != null)
                    {
                        Vector3Int newGridPosition = new Vector3Int(x, y - 1, z);
                        blockTransform.position = GridToWorldPosition(newGridPosition);
                    }
                }
            }
        }
    }

    public Vector3 GridToWorldPosition(Vector3Int gridPos)
    {
        return new Vector3(gridPos.x, gridPos.y, gridPos.z);
    }
}