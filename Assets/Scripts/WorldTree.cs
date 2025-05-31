using UnityEngine;

public static class WorldTree
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Creation()
    {
        Application.targetFrameRate = 60;
        MouseHidden();
    }

    public static void MouseHidden()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void MouseShow()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
