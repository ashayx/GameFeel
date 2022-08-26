using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class AutoQualityManager : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _getDefaultDPR();

    [DllImport("__Internal")]
    private static extern void _setDPR(float float1);
#endif

    public void __setDPR(float float1)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _setDPR(float1);
#endif
    }

    public void getDefaultDPR(float float1)
    {
        dpi = float1;
    }

    public void requestDefaultDPR()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        _getDefaultDPR();
#endif
    }

    void Start()
    {
        requestDefaultDPR();
        //初始化自动质量
        OK_To_Start_Counting = false;
        Current_Time = 0.0f;
        Total_Time = DelayBeforeStarting;
        Currently_CheckEvery = AtStart_CheckEvery;
        ItsTimeToSkipFrames_ToLetTheNewQualityApply = false;
        CurrentFramesSkiped_AfterTheQualityHasChanged = 0;

        //If we want to stay on the most used quality settings
        if (StayOnTheMostUsedQualitySettingsAfter > 0)
        {
            //We keep that information
            StayOnTheMostUsedQualitySettingsAfterTheTimeBelow = true;

            //We create the array used to count how many times a quality settings is used
            SaveHowManyTimesTheCurrentQualityWasUsed_Array = new int[QualitySettings.names.Length];
        }
        else //Continually check FPS and adapt quality
        {
            //We keep that information
            StayOnTheMostUsedQualitySettingsAfterTheTimeBelow = false;
        }

        //Main coroutine
        StartCoroutine(LetsCheckFPS());

        //初始自动DPI

        //初始FPS计算
        m_numbers = new string[NumberBufferSize];
        for (int i = 0; i < NumberBufferSize; i++) m_numbers[i] = i.ToString();

        FpsSamples = new int[SampleSize];
        for (int i = 0; i < FpsSamples.Length; i++) FpsSamples[i] = 1;
        //if (!TargetText) enabled = false;
        if (ShowDebugLog) Debug.Log("AutoQualityManager GetQualityLevel---" + QualitySettings.GetQualityLevel());
    }

    void Update()
    {
        if (GroupSampling)
        {
            Group();
        }
        else
        {
            SingleFrame();
        }

        m_localfps = m_numbers[Framerate];

        SampleIndex = SampleIndex < SampleSize - 1 ? SampleIndex + 1 : 0;
        TextUpdateIndex = TextUpdateIndex > UpdateTextEvery ? 0 : TextUpdateIndex + 1;

        if (TargetText != null)
        {
            if (TextUpdateIndex == UpdateTextEvery) TargetText.text = m_localfps;

            if (UseColors)
            {
                if (Framerate < BadBelow)
                {
                    TargetText.color = Bad;
                }
                else
                {
                    TargetText.color = Framerate < OkayBelow ? Okay : Good;
                }
            }
        }

        if (StartAutoQuality)
        {
            //自动Quality
            UpdateQuality();
        }

        //自动DPI
        UpdateDPI();

        //QualityText.text = string.Format("Quality:{0} DPI:{1}", QualitySettings.GetQualityLevel(), dpi);


    }
    #region Auto DPI
    [Header("-------------Auto DPI")]
    public float dpi = 1;
    public bool dynamicResolutionSystem = true;
    public float dpiDecrease = 0.020f;
    public float dpiIncrease = 0.020f;
    public int fpsMax = 35;
    public int fpsMin = 30;
    public float dpiMin = 0.5f;
    public float dpiMax = 1f;
    public float measurePeriod = 4f;//几秒执行一次


    private float dpr = 0;
    //private float defaultDPR = 0f;
    private float lastDPR;
    private float m_FpsNextPeriod = 4.0f;
    private bool startAutoDPI = true;

#if USING_URP
    private UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset urpAsset;
#endif

    void UpdateDPI()
    {
        if (startAutoDPI && Time.realtimeSinceStartup >= m_FpsNextPeriod)
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + measurePeriod;
            dynamicResolutionSystemMethod();
        }
    }

    private void dynamicResolutionSystemMethod()
    {
        if (dynamicResolutionSystem)
        {
            if (Framerate > fpsMax)
            {
                dpi += dpiIncrease;
            }
            else if (Framerate < fpsMin)
            {
                dpi -= dpiDecrease;
            }
            dpi = Mathf.Clamp(dpi, dpiMin, dpiMax);

            dpr = dpi;

            if (dpr != lastDPR)
            {

#if USING_URP
                urpAsset.renderScale = dpr;
#endif
                __setDPR(dpr);
                if (ShowDebugLog)
                {
                    Debug.Log("AutoQualityManager renderScale----" + dpr);
                }
            }
            lastDPR = dpr;
        }
    }
    #endregion Auto DPI
    #region FPS
    [Header("-------------FPS ")]
    [Tooltip("Instead of reporting a specific framerate, this will sample many frames and report the average.")]
    public bool GroupSampling;

    [Tooltip("If Group Sampling is on, how many frames would you want to sample to report an average on?")]
    [Range(0, 20)]
    public int SampleSize;

    [Tooltip("The Text Component you want the result pushed to. If you're using TMP then you need to change this in the code to a TMP_Text component instead.")]
    // use 'TMP_Text' instead of 'Text' if you want Text Mesh Pro support.
    public TMPro.TMP_Text TargetText;

    [Tooltip("How often (in frames) do you want to update the Text component?")]
    [Range(1, 20)]
    public int UpdateTextEvery;

    [Tooltip("This will smooth out the results so they blend together between updates and are easier to read.")]
    public bool Smoothed;

    [Tooltip("This sets how many numbers are buffered into memory as strings in order to obtain zero allocations at runtime.\n\nAlthough this is trivial in memory usage, realistically, there's no reason to be over 1000.")]
    [Range(0, 1000)]
    public int NumberBufferSize;

    [Tooltip("Would you like to read the System Tick instead of calculating it in this script?\n\nTests show that differences are negligible, but the option remains available to you.")]
    public bool UseSystemTick;

    [Tooltip("Optionally change the color of the TargetText based on FPS performance.")]
    public bool UseColors;
    [Tooltip("If the framerate is above 'OkayBelow' it will be the 'Good' color.")]
    public Color Good;
    [Tooltip("If the framerate is below 'OkayBelow' it will be the 'Okay' color.")]
    public Color Okay;
    [Tooltip("If the framerate is below 'BadBelow' it will be the 'Bad' color.")]
    public Color Bad;
    [Tooltip("Threshold for defining an 'okay' framerate. Below this value is considered okay, but not high enough to be good, and not low enough to be bad.")]
    public int OkayBelow;
    [Tooltip("Threshold for defining an 'bad' framerate. Below this value is considered bad.")]
    public int BadBelow;

    public int Framerate { get; private set; }

    protected int[] FpsSamples;
    protected int SampleIndex;
    protected int TextUpdateIndex;

    private int m_sysLastSysTick;
    private int m_sysLastFrameRate;
    private int m_sysFrameRate;
    private string m_localfps;

    private static string[] m_numbers;

    protected virtual void Reset()
    {
        GroupSampling = true;
        SampleSize = 20;
        UpdateTextEvery = 1;
        Smoothed = true;
        UseColors = true;

        Good = Color.green;
        Okay = Color.yellow;
        Bad = Color.red;

        OkayBelow = 60;
        BadBelow = 30;

        UseSystemTick = false;
        NumberBufferSize = 1000;
    }



    protected virtual void SingleFrame()
    {
        Framerate = Mathf.Clamp(UseSystemTick
            ? GetSystemFramerate()
            : (int)(Smoothed
                ? 1 / Time.smoothDeltaTime
                : 1 / Time.deltaTime),
            0,
            m_numbers.Length - 1);
    }

    protected virtual void Group()
    {
        FpsSamples[SampleIndex] = Mathf.Clamp(UseSystemTick
            ? GetSystemFramerate()
            : (int)(Smoothed
                ? 1 / Time.smoothDeltaTime
                : 1 / Time.deltaTime),
            0,
            m_numbers.Length - 1);

        Framerate = 0;
        bool loop = true;
        int i = 0;
        while (loop)
        {
            if (i == SampleSize - 1) loop = false;
            Framerate += FpsSamples[i];
            i++;
        }
        Framerate /= FpsSamples.Length;
    }

    protected virtual int GetSystemFramerate()
    {
        if (System.Environment.TickCount - m_sysLastSysTick >= 1000)
        {
            m_sysLastFrameRate = m_sysFrameRate;
            m_sysFrameRate = 0;
            m_sysLastSysTick = System.Environment.TickCount;
        }
        m_sysFrameRate++;
        return m_sysLastFrameRate;
    }
    #endregion FPS
    #region Quality
    [Header("-------------Auto Quality ")]
    public bool StartAutoQuality = true;
    /// <summary>
    /// Automatically adapt image quality according to frames per second. Version = 1.0.3
    /// </summary>
    [Tooltip("Do you want to see infos in the console?")]
    public bool ShowDebugLog = true;

    [Tooltip("Below that target (in frames per second), the quality will automatically decrease")]
    public float TargetFPS_min = 30.0f; //in frames per second

    [Tooltip("Over that target (in frames per second), the quality will automatically increase")]
    public float TargetFPS_max = 45.0f; //in frames per second

    [Tooltip("Time to wait before FPS is more stable (in seconds)")]
    public float DelayBeforeStarting = 5.0f; //in seconds

    [Tooltip("While we're under min FPS or over max FPS, check FPS and quality every __ seconds")]
    public float AtStart_CheckEvery = 2.0f; //in seconds

    [Tooltip("If the perfect quality and FPS is reached, we now check FPS and quality every __ seconds")]
    public float Then_WhenItsStable_CheckEvery = 2.0f; //in seconds

    private bool StayOnTheMostUsedQualitySettingsAfterTheTimeBelow; //Do you want to stay on the most used quality settings after some time? (in seconds)

    [Tooltip("If > 0, the most used quality settings will be chosen after __ seconds")]
    public float StayOnTheMostUsedQualitySettingsAfter = 20.0f; //in seconds

    private int[] SaveHowManyTimesTheCurrentQualityWasUsed_Array;

    [Tooltip("Should expensive quality changes be applied? Like anti-aliasing etc")]
    public bool ApplyExpensiveQualityChanges = true; //Should expensive changes be applied? Like anti-aliasing etc

    //public Text QualityText;
    private float Current_Time; //in seconds
    private float Total_Time; //in seconds

    private bool OK_To_Start_Counting;
    private float Currently_CheckEvery; //in seconds


    private int Current_Quality;

    private bool ItsTimeToSkipFrames_ToLetTheNewQualityApply = false;
    private int CurrentFramesSkiped_AfterTheQualityHasChanged = 0;
    private int FramesToSkip_AfterTheQualityHasChanged = 5;

    void UpdateQuality()
    {
        startAutoDPI = false;
        //When FPS is stable
        if (OK_To_Start_Counting)
        {
            //If it's not yet the time to skip frames (to let the new quality apply)
            if (!ItsTimeToSkipFrames_ToLetTheNewQualityApply)
            {
                //We start counting frames over time
                Current_Time += Time.deltaTime;
                //If it's time to check the current fps
                if (Current_Time >= Currently_CheckEvery)
                {

                    //Current Quality
                    Current_Quality = QualitySettings.GetQualityLevel();

                    if (ShowDebugLog) { Debug.Log("AutoQualityManager Current FPS = " + Framerate); }

                    //If fps is below the minimum fps target
                    if (Framerate < TargetFPS_min)
                    {
                        //Decreasing Quality Level
                        QualitySettings.DecreaseLevel(ApplyExpensiveQualityChanges);

                        //If the quality has changed
                        if (Current_Quality != QualitySettings.GetQualityLevel())
                        {
                            ItsTimeToSkipFrames_ToLetTheNewQualityApply = true;

                            //If we want to show some informations in the console
                            if (ShowDebugLog) { Debug.Log("AutoQualityManager Decreasing Quality Level from (" + Current_Quality + ") to (" + QualitySettings.GetQualityLevel() + ")"); }
                        }
                        else //Current_Quality == QualitySettings.GetQualityLevel()
                        {
                            //If we want to show some informations in the console
                            if (ShowDebugLog) { Debug.Log("AutoQualityManager Currently in the lowest quality level (" + Current_Quality + ")"); }
                        }

                        //We count how many times a quality settings is used
                        SaveCurrentQualitySettings();
                    }
                    else
                    {
                        //If fps is over the maximum fps target
                        if (Framerate > TargetFPS_max)
                        {
                            //Increasing Quality Level
                            QualitySettings.IncreaseLevel(ApplyExpensiveQualityChanges);

                            //If the quality has changed
                            if (Current_Quality != QualitySettings.GetQualityLevel())
                            {
                                ItsTimeToSkipFrames_ToLetTheNewQualityApply = true;

                                //If we want to show some informations in the console
                                if (ShowDebugLog) { Debug.Log("AutoQualityManager Increasing Quality Level from (" + Current_Quality + ") to (" + QualitySettings.GetQualityLevel() + ")"); }
                            }
                            else //Current_Quality == QualitySettings.GetQualityLevel()
                            {
                                //If we want to show some informations in the console
                                if (ShowDebugLog) { Debug.Log("AutoQualityManager Currently in the highest quality level (" + Current_Quality + ")"); }
                            }

                            //We count how many times a quality settings is used
                            SaveCurrentQualitySettings();
                        }
                        else //Between min and max fps
                        {
                            if (ShowDebugLog) { Debug.Log("AutoQualityManager Between min and max fps: No changes to the current quality level (" + Current_Quality + ")"); }
                            Currently_CheckEvery = Then_WhenItsStable_CheckEvery;
                            SaveCurrentQualitySettings();
                        }
                    }

                    //Initializing variables
                    Total_Time += Current_Time;
                    Current_Time = 0.0f;
                }

                //If we've reached the total time
                if (StayOnTheMostUsedQualitySettingsAfterTheTimeBelow && Total_Time >= StayOnTheMostUsedQualitySettingsAfter)
                {
                    //We stop counting
                    OK_To_Start_Counting = false;
                    StayOnTheMostUsedQualitySettings();
                }
            }
            else //Now it's time to skip frames (to let the new quality apply)
            {
                //Increase the frame counter
                CurrentFramesSkiped_AfterTheQualityHasChanged++;

                //If enough frames were skipped
                if (CurrentFramesSkiped_AfterTheQualityHasChanged >= FramesToSkip_AfterTheQualityHasChanged)
                {
                    //We can continue counting frames
                    ItsTimeToSkipFrames_ToLetTheNewQualityApply = false;
                    CurrentFramesSkiped_AfterTheQualityHasChanged = 0;
                }
            }
        }
    }

    //We count how many times a quality settings is used
    void SaveCurrentQualitySettings()
    {
        if (StayOnTheMostUsedQualitySettingsAfterTheTimeBelow)
        {
            SaveHowManyTimesTheCurrentQualityWasUsed_Array[QualitySettings.GetQualityLevel()] += 1;
        }
    }

    //We choose the most used quality settings
    void StayOnTheMostUsedQualitySettings()
    {
        int save_cpt = 0;
        int cpt = 1;

        //While the array is not read to the end
        while (cpt < SaveHowManyTimesTheCurrentQualityWasUsed_Array.Length)
        {
            //If the "saved quality" is used less often than the "current quality"
            if (SaveHowManyTimesTheCurrentQualityWasUsed_Array[save_cpt] < SaveHowManyTimesTheCurrentQualityWasUsed_Array[cpt])
            {
                //Then we save the "current quality"
                save_cpt = cpt;
            }

            //Next line
            cpt++;
        }

        if (ShowDebugLog) { Debug.Log("AutoQualityManager The most used quality level is (" + save_cpt + ")."); }

        //If the current quality level is NOT the most used quality level
        if (QualitySettings.GetQualityLevel() != save_cpt)
        {
            if (ShowDebugLog) { Debug.Log("AutoQualityManager Changing quality level from (" + QualitySettings.GetQualityLevel() + ") to (" + save_cpt + ")."); }

            //We change the current quality to the most used quality
            QualitySettings.SetQualityLevel(save_cpt, ApplyExpensiveQualityChanges);

        }
        else //the current quality level is the most used quality level
        {
            if (ShowDebugLog) { Debug.Log("AutoQualityManager And that's the current quality level." + save_cpt); }
        }

        //We disable the script
        //this.enabled = false;

        if (ShowDebugLog) Debug.Log("AutoQualityManager 完成质量设置");
#if USING_URP
        urpAsset = (UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)QualitySettings.renderPipeline;
        dpi = urpAsset.renderScale;
#endif

        startAutoDPI = true;
    }


    //Main coroutine
    IEnumerator LetsCheckFPS()
    {
        //We wait for FPS to be more stable
        yield return new WaitForSeconds(DelayBeforeStarting);

        //It's OK to start counting FPS
        OK_To_Start_Counting = true;
        if (ShowDebugLog) Debug.Log("AutoQualityManager Start Auto Quality");
    }
    #endregion Quality

}
