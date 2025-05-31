using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DistanceLine : MonoBehaviour
{
    public Transform objA;
    public Transform objB;
    public string groundName = "Ground";
    public float heightAboveGround = 3.5f;

    public Material lineMaterial;
    public Font distanceFont;

    private LineRenderer lineRenderer;
    private Transform ground;

    private GameObject distanceTextObj;
    private TextMesh distanceTextMesh;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ground = GameObject.Find(groundName)?.transform;

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;

        distanceTextObj = new GameObject("DistanceText");
        distanceTextMesh = distanceTextObj.AddComponent<TextMesh>();
        distanceTextMesh.fontSize = 10;
        distanceTextMesh.color = Color.yellow;
        distanceTextMesh.alignment = TextAlignment.Center;
        distanceTextMesh.anchor = TextAnchor.MiddleCenter;
        if (distanceFont != null)
            distanceTextMesh.font = distanceFont;

        distanceTextObj.transform.position = Vector3.zero;
    }

    void Update()
    {
        if (objA == null || objB == null || ground == null) return;

        Vector3 posA = objA.position;
        Vector3 posB = objB.position;

        float yA = GetGroundHeightAt(posA);
        float yB = GetGroundHeightAt(posB);

        Vector3 pointA = new Vector3(posA.x, yA + heightAboveGround, posA.z);
        Vector3 pointB = new Vector3(posB.x, yB + heightAboveGround, posB.z);

        lineRenderer.SetPosition(0, pointA);
        lineRenderer.SetPosition(1, pointB);

        Vector3 midPoint = (pointA + pointB) / 2f;
        distanceTextObj.transform.position = midPoint + Vector3.up * 0.2f;

        float distance = Vector3.Distance(posA, posB);
        distanceTextMesh.text = distance.ToString("F2") + " m";

        if (Camera.main != null)
            distanceTextObj.transform.rotation = Quaternion.LookRotation(distanceTextObj.transform.position - Camera.main.transform.position);
    }

    float GetGroundHeightAt(Vector3 pos)
    {
        Ray ray = new Ray(new Vector3(pos.x, pos.y + 10f, pos.z), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f))
        {
            if (hit.collider.gameObject.name == groundName)
            {
                return hit.point.y;
            }
        }
        return ground.position.y;
    }
}