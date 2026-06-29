using UnityEngine;

public class BoardGridVisual : MonoBehaviour
{
    public Material lineMaterial;

    public int width = 8;
    public int depth = 8;

    public float lineThickness = 0.03f;
    public float yOffset = 0.03f;

    private void Start()
    {
        CreateGridLines();
    }

    private void CreateGridLines()
    {
        for (int z = 0; z <= depth; z++)
        {
            float zPosition = z - 0.5f;

            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "GridLine_Z_" + z;
            line.transform.parent = transform;

            line.transform.position = new Vector3(
                (width - 1) / 2f,
                yOffset,
                zPosition
            );

            line.transform.localScale = new Vector3(
                width + 0.1f,
                lineThickness,
                lineThickness
            );

            if (lineMaterial != null)
            {
                line.GetComponent<Renderer>().material = lineMaterial;
            }
        }

        for (int x = 0; x <= width; x++)
        {
            float xPosition = x - 0.5f;

            GameObject line = GameObject.CreatePrimitive(PrimitiveType.Cube);
            line.name = "GridLine_X_" + x;
            line.transform.parent = transform;

            line.transform.position = new Vector3(
                xPosition,
                yOffset + 0.01f,
                (depth - 1) / 2f
            );

            line.transform.localScale = new Vector3(
                lineThickness,
                lineThickness,
                depth + 0.1f
            );

            if (lineMaterial != null)
            {
                line.GetComponent<Renderer>().material = lineMaterial;
            }
        }
    }
}