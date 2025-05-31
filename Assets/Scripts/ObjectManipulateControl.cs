using UnityEngine;

public class ObjectManipulateControl : MonoBehaviour
{
    public bool isManipulate = false;
    public Transform selectedObject = null;
    public Vector3 offset;
    public float fixedY;

    static ObjectManipulateControl _instance;

    void Awake()
    {
        _instance = this;
    }

    public static ObjectManipulateControl GetInstance()
    {
        return _instance;
    }

    void Update()
    {
        if (!ObjectRotateControl.GetInstance().isRotate && Input.GetKeyDown(KeyCode.Q))
        {
            CameraControl cameraControl = CameraControl.GetInstance();

            cameraControl.isControl = !cameraControl.isControl;
            cameraControl.SetStatusMessage("Manipulate Mode");
            isManipulate = !cameraControl.isControl;

            if (cameraControl.isControl)
                WorldTree.MouseHidden();
            else
                WorldTree.MouseShow();
        }

        if (!isManipulate)
        {
            if (selectedObject != null)
            {
                selectedObject = null;
            }
            return;
        }

        HandleManipulation();
    }

    void HandleManipulation()
    {
        Camera cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (selectedObject == null)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Interactable")) 
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        selectedObject = hit.transform;
                        fixedY = selectedObject.position.y;

                        Vector3 hitPoint = hit.point;
                        offset = selectedObject.position - new Vector3(hitPoint.x, fixedY, hitPoint.z);
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Plane horizontalPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));

                float distanceToPlane;
                if (horizontalPlane.Raycast(ray, out distanceToPlane))
                {
                    Vector3 pointOnPlane = ray.GetPoint(distanceToPlane);
                    selectedObject.position = pointOnPlane + offset;
                }
            }
            else
            {
                selectedObject = null;
            }
        }
    }

}
