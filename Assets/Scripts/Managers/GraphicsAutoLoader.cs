using UnityEngine;

public class GraphicsAutoLoader : MonoBehaviour
{
    private void Start()
    {
        LoadAndApplyGraphicsSettings();
    }

    private void LoadAndApplyGraphicsSettings()
    {
        float maxFps = PlayerPrefs.GetFloat("MaxFPS", 60f);
        int fpsCounterMode = PlayerPrefs.GetInt("FpsCounterMode", 0);

        Application.targetFrameRate = (int)maxFps;

        FpsCounter fpsCounter = FindObjectOfType<FpsCounter>();
        if (fpsCounter != null)
        {
            bool showFpsCounter = fpsCounterMode == 1;
            fpsCounter.SetFpsCounterActive(showFpsCounter);
        }
    }

    public void RefreshGraphicsSettings()
    {
        LoadAndApplyGraphicsSettings();
    }
}