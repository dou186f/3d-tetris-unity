using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public GameObject blockCubePrefab;
    public Material ghostMaterial;

    public Material singleCubeMaterial;
    public Material lineMaterial;
    public Material lShapeMaterial;
    public Material squareMaterial;
    public Material verticalMaterial;

    private GridManager gridManager;
    private GameManager gameManager;

    private Vector3Int spawnPosition;

    private enum PieceType
    {
        SingleCube,
        Line,
        LShape,
        Square,
        Vertical2,
        Vertical3
    }

    public void Initialize(GridManager gridManager, GameManager gameManager)
    {
        this.gridManager = gridManager;
        this.gameManager = gameManager;

        spawnPosition = new Vector3Int(
            gridManager.width / 2,
            gridManager.height - 1,
            gridManager.depth / 2
        );
    }

    public void SpawnNewPiece()
    {
        if (gameManager.IsGameOver)
        {
            return;
        }

        PieceType pieceType = GetRandomPieceType();

        Vector3Int[] blockOffsets = GetOffsetsForPieceType(pieceType);
        Material pieceMaterial = GetMaterialForPieceType(pieceType);

        if (!gridManager.CanPlaceBlocks(spawnPosition, blockOffsets))
        {
            gameManager.GameOver();
            return;
        }

        GameObject pieceObject = new GameObject("Falling Piece - " + pieceType);

        FallingPiece fallingPiece = pieceObject.AddComponent<FallingPiece>();
        fallingPiece.ghostMaterial = ghostMaterial;
        fallingPiece.fallInterval = gameManager.GetCurrentFallInterval();

        Transform[] blockTransforms = CreateBlocksForPiece(
            pieceObject.transform,
            blockOffsets,
            pieceMaterial
        );

        fallingPiece.Initialize(
            gridManager,
            this,
            gameManager,
            spawnPosition,
            blockOffsets,
            blockTransforms
        );
    }

    private Transform[] CreateBlocksForPiece(
        Transform parent,
        Vector3Int[] blockOffsets,
        Material pieceMaterial
    )
    {
        Transform[] blockTransforms = new Transform[blockOffsets.Length];

        for (int i = 0; i < blockOffsets.Length; i++)
        {
            GameObject blockObject = Instantiate(blockCubePrefab);
            blockObject.name = "Block_" + i;

            blockObject.transform.parent = parent;

            Renderer blockRenderer = blockObject.GetComponent<Renderer>();

            if (blockRenderer != null && pieceMaterial != null)
            {
                blockRenderer.material = pieceMaterial;
            }

            blockTransforms[i] = blockObject.transform;
        }

        return blockTransforms;
    }

    private PieceType GetRandomPieceType()
    {
        int randomIndex = Random.Range(0, 6);

        if (randomIndex == 0)
        {
            return PieceType.SingleCube;
        }
        else if (randomIndex == 1)
        {
            return PieceType.Line;
        }
        else if (randomIndex == 2)
        {
            return PieceType.LShape;
        }
        else if (randomIndex == 3)
        {
            return PieceType.Square;
        }
        else if (randomIndex == 4)
        {
            return PieceType.Vertical2;
        }
        else
        {
            return PieceType.Vertical3;
        }
    }

    private Vector3Int[] GetOffsetsForPieceType(PieceType pieceType)
    {
        if (pieceType == PieceType.SingleCube)
        {
            return CreateSingleCubeOffsets();
        }
        else if (pieceType == PieceType.Line)
        {
            return CreateTwoBlockLineOffsets();
        }
        else if (pieceType == PieceType.LShape)
        {
            return CreateLShapeOffsets();
        }
        else if (pieceType == PieceType.Square)
        {
            return CreateSquareOffsets();
        }
        else if (pieceType == PieceType.Vertical2)
        {
            return CreateVertical2Offsets();
        }
        else
        {
            return CreateVertical3Offsets();
        }
    }

    private Material GetMaterialForPieceType(PieceType pieceType)
    {
        if (pieceType == PieceType.SingleCube)
        {
            return singleCubeMaterial;
        }
        else if (pieceType == PieceType.Line)
        {
            return lineMaterial;
        }
        else if (pieceType == PieceType.LShape)
        {
            return lShapeMaterial;
        }
        else if (pieceType == PieceType.Square)
        {
            return squareMaterial;
        }
        else
        {
            return verticalMaterial;
        }
    }

    private Vector3Int[] CreateSingleCubeOffsets()
    {
        return new Vector3Int[]
        {
            new Vector3Int(0, 0, 0)
        };
    }

    private Vector3Int[] CreateTwoBlockLineOffsets()
    {
        return new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0)
        };
    }

    private Vector3Int[] CreateLShapeOffsets()
    {
        return new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, 1)
        };
    }

    private Vector3Int[] CreateSquareOffsets()
    {
        return new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 1)
        };
    }

    private Vector3Int[] CreateVertical2Offsets()
    {
        return new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, -1, 0)
        };
    }

    private Vector3Int[] CreateVertical3Offsets()
    {
        return new Vector3Int[]
        {
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, -2, 0)
        };
    }
}