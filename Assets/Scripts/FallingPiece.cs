using UnityEngine;

public class FallingPiece : MonoBehaviour
{
    public float fallInterval = 0.35f;

    public Material ghostMaterial;

    private GridManager gridManager;
    private PieceSpawner spawner;
    private GameManager gameManager;

    private Vector3Int originPosition;
    private Vector3Int[] blockOffsets;
    private Transform[] blockTransforms;

    private Transform[] ghostTransforms;

    private float fallTimer;
    private bool isLocked;

    public void Initialize(
        GridManager gridManager,
        PieceSpawner spawner,
        GameManager gameManager,
        Vector3Int startOriginPosition,
        Vector3Int[] blockOffsets,
        Transform[] blockTransforms
    )
    {
        this.gridManager = gridManager;
        this.spawner = spawner;
        this.gameManager = gameManager;

        this.originPosition = startOriginPosition;
        this.blockOffsets = blockOffsets;
        this.blockTransforms = blockTransforms;

        CreateGhostBlocks();

        UpdateBlockWorldPositions();
        UpdateGhostPositions();
    }

    private void Update()
    {
        if (isLocked || gameManager.IsGameOver || !gameManager.IsGameStarted)
        {
            return;
        }

        HandleInput();
        HandleFalling();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TryMove(new Vector3Int(-1, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            TryMove(new Vector3Int(1, 0, 0));
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            TryMove(new Vector3Int(0, 0, 1));
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            TryMove(new Vector3Int(0, 0, -1));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TryRotateCounterClockwise();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryRotateClockwise();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
    }

    private void TryRotateClockwise()
    {
        Vector3Int[] rotatedOffsets = new Vector3Int[blockOffsets.Length];

        for (int i = 0; i < blockOffsets.Length; i++)
        {
            Vector3Int offset = blockOffsets[i];

            rotatedOffsets[i] = new Vector3Int(
                offset.z,
                offset.y,
                -offset.x
            );
        }

        TryApplyRotation(rotatedOffsets);
    }

    private void TryRotateCounterClockwise()
    {
        Vector3Int[] rotatedOffsets = new Vector3Int[blockOffsets.Length];

        for (int i = 0; i < blockOffsets.Length; i++)
        {
            Vector3Int offset = blockOffsets[i];

            rotatedOffsets[i] = new Vector3Int(
                -offset.z,
                offset.y,
                offset.x
            );
        }

        TryApplyRotation(rotatedOffsets);
    }

    private void TryApplyRotation(Vector3Int[] rotatedOffsets)
    {
        if (!gridManager.CanPlaceBlocks(originPosition, rotatedOffsets))
        {
            return;
        }

        blockOffsets = rotatedOffsets;

        UpdateBlockWorldPositions();
        UpdateGhostPositions();
    }

    private void HandleFalling()
    {
        fallTimer += Time.deltaTime;

        if (fallTimer >= fallInterval)
        {
            fallTimer = 0f;
            TryFallOneStep();
        }
    }

    private void TryFallOneStep()
    {
        bool movedDown = TryMove(new Vector3Int(0, -1, 0));

        if (!movedDown)
        {
            LockPiece();
        }
    }

    private bool TryMove(Vector3Int direction)
    {
        Vector3Int targetOriginPosition = originPosition + direction;

        if (!gridManager.CanPlaceBlocks(targetOriginPosition, blockOffsets))
        {
            return false;
        }

        originPosition = targetOriginPosition;

        UpdateBlockWorldPositions();
        UpdateGhostPositions();

        return true;
    }

    private void HardDrop()
    {
        gameManager.PlayHardDropSound();

        while (TryMove(new Vector3Int(0, -1, 0)))
        {
            // Keep moving down until blocked.
        }

        LockPiece();
    }

    private void LockPiece()
    {
        if (isLocked)
        {
            return;
        }

        isLocked = true;

        DestroyGhostBlocks();

        gridManager.RegisterBlocks(originPosition, blockOffsets, blockTransforms);

        DetachBlocksFromPieceParent();

        int clearedLayers = gridManager.ClearFullLayers();

        if (clearedLayers > 0)
        {
            gameManager.PlayClearLayerSound();
        }

        int scoreToAdd = 10;

        if (clearedLayers > 0)
        {
            scoreToAdd += clearedLayers * 100;
        }

        gameManager.AddScore(scoreToAdd);

        spawner.SpawnNewPiece();

        Destroy(gameObject);
    }

    private void DetachBlocksFromPieceParent()
    {
        for (int i = 0; i < blockTransforms.Length; i++)
        {
            if (blockTransforms[i] != null)
            {
                blockTransforms[i].SetParent(null);
            }
        }
    }

    private void UpdateBlockWorldPositions()
    {
        for (int i = 0; i < blockOffsets.Length; i++)
        {
            Vector3Int blockGridPosition = originPosition + blockOffsets[i];
            blockTransforms[i].position = gridManager.GridToWorldPosition(blockGridPosition);
        }
    }

    private void CreateGhostBlocks()
    {
        ghostTransforms = new Transform[blockOffsets.Length];

        for (int i = 0; i < blockOffsets.Length; i++)
        {
            GameObject ghostObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ghostObject.name = "GhostBlock_" + i;

            ghostObject.transform.localScale = new Vector3(0.82f, 0.82f, 0.82f);

            Collider ghostCollider = ghostObject.GetComponent<Collider>();
            if (ghostCollider != null)
            {
                Destroy(ghostCollider);
            }

            Renderer renderer = ghostObject.GetComponent<Renderer>();
            if (renderer != null && ghostMaterial != null)
            {
                renderer.material = ghostMaterial;
            }

            ghostTransforms[i] = ghostObject.transform;
        }
    }

    private void UpdateGhostPositions()
    {
        if (ghostTransforms == null)
        {
            return;
        }

        Vector3Int landingOrigin = FindLandingOriginPosition();

        for (int i = 0; i < blockOffsets.Length; i++)
        {
            Vector3Int ghostGridPosition = landingOrigin + blockOffsets[i];
            ghostTransforms[i].position = gridManager.GridToWorldPosition(ghostGridPosition);
        }
    }

    private Vector3Int FindLandingOriginPosition()
    {
        Vector3Int testOrigin = originPosition;

        while (gridManager.CanPlaceBlocks(testOrigin + new Vector3Int(0, -1, 0), blockOffsets))
        {
            testOrigin += new Vector3Int(0, -1, 0);
        }

        return testOrigin;
    }

    private void DestroyGhostBlocks()
    {
        if (ghostTransforms == null)
        {
            return;
        }

        for (int i = 0; i < ghostTransforms.Length; i++)
        {
            if (ghostTransforms[i] != null)
            {
                Destroy(ghostTransforms[i].gameObject);
            }
        }
    }
}