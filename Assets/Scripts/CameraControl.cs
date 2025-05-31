using UnityEngine;

[RequireComponent(typeof(ObjectManipulateControl))]
public class CameraControl : MonoBehaviour
{
    static CameraControl _instance;

    GUIStyle style;

    float moveSpeed = 15f;
    float sprintMultiplier = 3f;
    float verticalSpeed = 15f;
    float rotateSpeedX = 3f;
    float rotateSpeedY = 5f;
    float limitMinX = -80f;
    float limitMaxX = 50f;

    public bool isControl = true;

    float eulerAngleX;
    float eulerAngleY;

    string statusMessage = "";

    bool isSprinting = false;

    float lastWPressTime = -1f;
    float doubleTapThreshold = 0.3f;
    bool sprintKeyHeld = false;

    float deltaTime = 0f;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.red;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        if (!isControl)
            return;

        statusMessage = "";

        HandleMouseLook();
        HandleSprint();

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        MoveOnXZ(moveX, moveZ);
        MoveVertical();
    }

    public static CameraControl GetInstance()
    {
        return _instance;
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        eulerAngleY += mouseX * rotateSpeedX;
        eulerAngleX -= mouseY * rotateSpeedY;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    void HandleSprint()
    {
        // W 키 두 번 연타 감지
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastWPressTime < doubleTapThreshold)
            {
                sprintKeyHeld = true;
                isSprinting = true;
            }
            else
            {
                sprintKeyHeld = false;
                isSprinting = false;
            }
            lastWPressTime = Time.time;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            sprintKeyHeld = false;
            isSprinting = false;
        }
    }

    void MoveOnXZ(float moveX, float moveZ)
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 move = right * moveX + forward * moveZ;

        if (move.magnitude > 0.01f)
        {
            transform.position += move * moveSpeed * (isSprinting ? sprintMultiplier : 1f) * Time.deltaTime;
            if (isSprinting)
                AppendStatusMessage("Running");
            else
                AppendStatusMessage("Moving");
        }
    }

    void MoveVertical()
    {
        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.Space))
        {
            move += Vector3.up * verticalSpeed * Time.deltaTime;
            AppendStatusMessage("Ascending");
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            move += Vector3.down * verticalSpeed * Time.deltaTime;
            AppendStatusMessage("Descending");
        }

        transform.position += move;
    }

    void AppendStatusMessage(string message)
    {
        if (string.IsNullOrEmpty(statusMessage))
            statusMessage = message;
        else
            statusMessage += " / " + message;
    }

    public void SetStatusMessage(string message)
    {
        statusMessage = message;
    }

    void OnGUI()
    {
        Vector3 pos = transform.position;

        float ms = deltaTime * 1000f;
        float fps = 1.0f / deltaTime;

        string fullText = $"X: {pos.x:F2}  Y: {pos.y:F2}  Z: {pos.z:F2}  FPS: {fps:0.} ({ms:0.0} ms)\n{statusMessage}";
        GUI.Label(new Rect(10, 10, 500, 120), fullText, style);

        float x = 10;
        float y = Screen.height - 200;

        string instructions =
            "Controls:\n" +
            "W/A/S/D: Move horizontally\n" +
            "Space: Move up\n" +
            "Left Shift: Move down\n" +
            "Mouse Left Click + Drag: Move object\n" +
            "Q: Toggle Manipulation Mode\n" +
            "E: Toggle Rotation Mode (Mouse Left&Right: X Rotate, Mouse Scroll: Y Rotate)";

        GUI.Label(new Rect(x, y, 400, 110), instructions, style);
    }
}