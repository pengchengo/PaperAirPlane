
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PaperAirPlane : MonoBehaviour
{

    public int xSize = 100, ySize = 100;
    public float paperWidth = 4.5f, paperHeight = 8;
    public Material material;
    public MainWindow mainUI;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Mesh mesh;

    Vector3[] vertices;
    Vector3[] orginVertexs;
    Color[] colors;

    bool isFinishedMark = true; //是否开始标记
    List<Vector3> linePoints = new List<Vector3>();

    Vector3 startCamPos;
    Quaternion startRotation;

    void Start()
    {

        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshFilter = gameObject.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        orginVertexs = mesh.vertices;
        vertices = mesh.vertices;
        colors = new Color[orginVertexs.Length];
        startCamPos = Camera.main.transform.position;
        startRotation = Camera.main.transform.rotation;

    }

    private void CreateBaseMesh()
    {
        mesh.name = "planeGrid";
        float widthPerCell = (float)paperWidth / xSize;
        float heightPerCell = (float)paperHeight / ySize;

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        colors = new Color[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x * widthPerCell, y * heightPerCell);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.tangents = tangents;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public void OnAngleSliderValueChanged(float value)
    {

        if(value == 0) return;

        Vector3[] newVertexs = new Vector3[orginVertexs.Length];

        Vector3 lstart = linePoints[0];
        Vector3 lend = linePoints[1];
        //原点到折线的垂向量
        Vector3 offset = Point2LineVec(Vector3.zero, lstart, lend);
        Vector3 axis = (lend - lstart).normalized;

        for (int i = 0; i < orginVertexs.Length; i++)
        {
            Vector3 worldPoint = transform.TransformPoint(orginVertexs[i]);
            if (IsVertexOnMarkLineRight(worldPoint, lstart, lend))
            {
                //将顶点先进行偏移
                Vector3 temp = worldPoint + offset;
                //旋转完后再将偏移恢复
                Vector3 newWorldPoint = Quaternion.AngleAxis(value * 57,axis) * temp - offset;
                newVertexs[i] = transform.InverseTransformPoint(newWorldPoint);
            }
            else
            {
                newVertexs[i] = orginVertexs[i];
            }
        }

        mesh.vertices = newVertexs;
        mesh.RecalculateNormals();

    }

    //三维空间中，点到直线的垂向量
    //targetPoint:指定点 ；lstart,lend：直线上的两点
    private Vector3 Point2LineVec(Vector3 targetPoint, Vector3 lstart, Vector3 lend)
    {
        Vector3 p = targetPoint - lstart;
        Vector3 l = lend - lstart;
        //网格顶点在折线上的投影向量
        Vector3 projectVec = Vector3.Dot(p, l) * l.normalized + lstart;
        //点在折线上的垂向量d1A
        Vector3 p2lineVec = p - projectVec;
        return p2lineVec;
    }

    private bool IsVertexOnMarkLineRight(Vector3 vertex, Vector3 lstart, Vector3 lend)
    {
        Vector3 point2LineVec = Point2LineVec(vertex, lstart, lend);
        Vector3 lineVec = lend - lstart;
        return Vector3.Cross(lineVec, point2LineVec).z < 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isFinishedMark)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100))
                {
                    Vector3 hitPaperHitPoint = new Vector3(hit.point.x, hit.point.y, transform.position.z);
                    linePoints.Add(hitPaperHitPoint);

                    if (linePoints.Count >= 2)
                    {
                        isFinishedMark = false;
                        mainUI.rotateSlider.gameObject.SetActive(true);

                        for (int i = 0; i < orginVertexs.Length; i++)
                        {
                            Vector3 lstart = linePoints[0];
                            Vector3 lend = linePoints[1];
                            Vector3 vertexWorldPos = transform.TransformPoint(vertices[i]);
                            // colors[i] = IsVertexOnMarkLineRight(vertexWorldPos, lstart, lend) ? Color.red : Color.green;
                        }
                        // mesh.colors = colors;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            isFinishedMark = true;
            linePoints.Clear();

            orginVertexs = mesh.vertices;

            mainUI.rotateSlider.value = 0;

        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Camera.main.transform.position = startCamPos;
            Camera.main.transform.rotation = startRotation;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("SampleScene");
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (linePoints.Count > 1)
        {
            Gizmos.DrawLine(linePoints[0], linePoints[1]);
        }

        if (linePoints.Count > 0)
        {
            linePoints.ForEach(p =>
            {
                Gizmos.DrawSphere(p, 0.1f);
            });
        }
    }
}
