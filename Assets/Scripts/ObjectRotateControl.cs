using UnityEngine;

public class ObjectRotateControl : MonoBehaviour
{
    public bool isRotate = false;
    public Transform selectedObject = null;
    private Vector3 previousMousePosition;

    static ObjectRotateControl _instance;

    void Awake()
    {
        _instance = this;
    }

    public static ObjectRotateControl GetInstance()
    {
        return _instance;
    }

    void Update()
    {
        if (!ObjectManipulateControl.GetInstance().isManipulate && Input.GetKeyDown(KeyCode.E))
        {
            CameraControl cameraControl = CameraControl.GetInstance();

            cameraControl.isControl = !cameraControl.isControl;
            cameraControl.SetStatusMessage("Rotate Mode");
            isRotate = !cameraControl.isControl;

            if (cameraControl.isControl)
                WorldTree.MouseHidden();
            else
                WorldTree.MouseShow();
        }

        if (!isRotate)
        {
            if (selectedObject != null)
            {
                selectedObject = null;
            }
            return;
        }

        HandleRotation();
    }

    void HandleRotation()
    {
        Camera cam = GetComponent<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (selectedObject == null)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.transform.CompareTag("Interactable") && Input.GetMouseButtonDown(0))
                {
                    selectedObject = hit.transform;
                    previousMousePosition = Input.mousePosition;
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mouseDelta = Input.mousePosition - previousMousePosition;
                previousMousePosition = Input.mousePosition;

                float rotateSpeed = 0.2f;
                float scrollRotateSpeed = 5f;

                selectedObject.Rotate(0f, mouseDelta.x * -rotateSpeed, 0f, Space.Self);
                
                float scrollDelta = Input.mouseScrollDelta.y;
                if (Mathf.Abs(scrollDelta) > 0.01f)
                {
                    selectedObject.Rotate(scrollDelta * scrollRotateSpeed, 0f, 0f, Space.Self);
                }
            }
            else
            {
                selectedObject = null;
            }
        }
    }
}
