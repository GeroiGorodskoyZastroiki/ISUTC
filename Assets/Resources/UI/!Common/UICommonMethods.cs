using UnityEngine;

public static class UICommonMethods
{
    public static void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public static void Settings()
    {
        UIManager.Open(UIManager.Settings);
    }
}
