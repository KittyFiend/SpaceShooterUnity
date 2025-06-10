using UnityEngine;

public class MobileOptimization : MonoBehaviour
{
    [Header("Performance Settings")]
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private int vSyncCount = 0;

    [Header("Resolution Settings")]
    [SerializeField] private bool limitResolution = true;
    [SerializeField] private int maxScreenWidth = 1920;
    [SerializeField] private int maxScreenHeight = 1080;

    [Header("Quality Settings")]
    [SerializeField] private bool optimizeQuality = true;
    [SerializeField] private string[] qualityLevels = { "Fastest", "Fast", "Simple", "Good", "Beautiful", "Fantastic" };
    [SerializeField] private int mobileQualityLevel = 1;

    private void Awake()
    {
        OptimizeForMobile();
    }

    private void Start()
    {
        OptimizeRuntime();
        LogPerformanceInfo();
    }

    private void OptimizeForMobile()
    {
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = vSyncCount;

        if (limitResolution)
        {
            int currentWidth = Screen.width;
            int currentHeight = Screen.height;

            if (currentWidth > maxScreenWidth || currentHeight > maxScreenHeight)
            {
                float aspectRatio = (float)currentWidth / currentHeight;

                int newWidth = maxScreenWidth;
                int newHeight = Mathf.RoundToInt(newWidth / aspectRatio);

                if (newHeight > maxScreenHeight)
                {
                    newHeight = maxScreenHeight;
                    newWidth = Mathf.RoundToInt(newHeight * aspectRatio);
                }

                Screen.SetResolution(newWidth, newHeight, Screen.fullScreen);
            }
        }

        if (optimizeQuality)
        {
#if UNITY_ANDROID || UNITY_IOS
            if (mobileQualityLevel >= 0 && mobileQualityLevel < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(mobileQualityLevel, true);

                QualitySettings.pixelLightCount = 1;
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                QualitySettings.antiAliasing = 0;
                QualitySettings.softVegetation = false;
                QualitySettings.realtimeReflectionProbes = false;
                QualitySettings.shadows = ShadowQuality.Disable;

                string qualityName = mobileQualityLevel < qualityLevels.Length ? qualityLevels[mobileQualityLevel] : $"Level {mobileQualityLevel}";
            }
#endif
        }

#if UNITY_ANDROID
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif
    }

    private void OptimizeRuntime()
    {
        Physics2D.queriesHitTriggers = true;
        Physics2D.queriesStartInColliders = false;
    }

    private void LogPerformanceInfo()
    {
        int currentQualityLevel = QualitySettings.GetQualityLevel();
        string qualityName = "Unknown";

        if (currentQualityLevel >= 0 && currentQualityLevel < qualityLevels.Length)
        {
            qualityName = qualityLevels[currentQualityLevel];
        }
        else
        {
            qualityName = $"Level {currentQualityLevel}";
        }
    }

    public void SetTargetFrameRate(int fps)
    {
        Application.targetFrameRate = fps;
    }

    public void ToggleVSync()
    {
        QualitySettings.vSyncCount = QualitySettings.vSyncCount == 0 ? 1 : 0;
    }

    public void SetQualityLevel(int level)
    {
        if (level >= 0 && level < qualityLevels.Length)
        {
            QualitySettings.SetQualityLevel(level, true);
        }
    }
}