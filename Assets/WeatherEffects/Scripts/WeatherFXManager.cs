using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http.Headers;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(WeatherApiManager))]
public class WeatherFXManager : MonoBehaviour
{
    WeatherApiManager WAM;

    public enum WeatherConditions { Dry, Rain, Snow, Fog };
    public enum TimesOfDay { Dawn, Morning, Afternoon, Evening, Dusk, Night, Overnight };

    [Header("Weather and Day Conditions")]
    [SerializeField] private WeatherConditions _weatherCondition;
    [SerializeField] private TimesOfDay _timeOfDay;

    // Get Set _weatherCondition
    public WeatherConditions WeatherCondition
    {
        get
        {
            return _weatherCondition;
        }
        set
        {
            if (_weatherCondition != value)
            {
                _weatherCondition = value;
                Debug.Log("WeatherCondition got changed to: " + _weatherCondition);
            }

        }
    }

    // Get Set _timeOfDay
    public TimesOfDay TimeOfDay
    {
        get
        {
            return _timeOfDay;
        }
        set
        {
            if (_timeOfDay != value)
            {
                _timeOfDay = value;
                Debug.Log("TimeOfDay got changed to: " + _timeOfDay);
            }
        }
    }

    public event Action<TimesOfDay> OnTimeOfDayChanged;

    public TimeOfDayFX timeOfDayFxData;
    public List<TimeSpan> timeSpans = new List<TimeSpan>();

    [Header("WeatherFX Objects")]
    public GameObject rainfallFx;
    public GameObject snowFx;
    public GameObject fogFx;

    [Header("OnGUI Display")]
    public bool showWeatherOnGui = false;
    private GUIStyle currentStyle = null;

    private void Awake()
    {
        WAM = GetComponent<WeatherApiManager>();
    }

    private void Start()
    {
        StartCoroutine(SetWeatherConditionsCoroutine());
    }

    public void SetWeatherConditions()
    {
        StartCoroutine(SetWeatherConditionsCoroutine());
    }

    private IEnumerator SetWeatherConditionsCoroutine()
    {
        while (!WAM.hasWeatherData)
        {
            yield return null;
        }

        Debug.Log("WeatherApiManager has data.");
        ParseCurrentTime();
        ParseCurrentWeather();
    }

    #region Weather and Time Handlers
    public void HandleTime()
    {
        var timeState = TimeOfDay;

        switch (timeState)
        {
            case TimesOfDay.Dawn:
                RenderSettings.skybox = timeOfDayFxData.timeOfDayFx[0].skyboxMaterial;
                break;
            case TimesOfDay.Morning:
                RenderSettings.skybox = timeOfDayFxData.timeOfDayFx[1].skyboxMaterial;
                break;
            case TimesOfDay.Afternoon:
                RenderSettings.skybox = timeOfDayFxData.timeOfDayFx[2].skyboxMaterial;
                break;
            case TimesOfDay.Evening:
                RenderSettings.skybox = timeOfDayFxData.timeOfDayFx[3].skyboxMaterial;
                break;
            case TimesOfDay.Dusk:
                RenderSettings.skybox = timeOfDayFxData.timeOfDayFx[4].skyboxMaterial;
                break;
            case TimesOfDay.Night:
                RenderSettings.skybox = timeOfDayFxData.timeOfDayFx[5].skyboxMaterial;
                break;
            case TimesOfDay.Overnight:
                RenderSettings.skybox = timeOfDayFxData.timeOfDayFx[5].skyboxMaterial;
                break;
        }
    }

    public void HandleWeatherCondition()
    {
        //Disable all weather FX objects
        rainfallFx.SetActive(false);
        snowFx.SetActive(false);
        fogFx.SetActive(false);

        var weatherState = _weatherCondition;

        switch (weatherState)
        {
            case WeatherConditions.Dry:
                break;
            case WeatherConditions.Rain:
                rainfallFx.SetActive(true);
                break;
            case WeatherConditions.Snow:
                snowFx.SetActive(true);
                break;
            case WeatherConditions.Fog:
                fogFx.SetActive(true);
                break;
        }
    }

    [ContextMenu("Parse time")]
    public void ParseCurrentTime()
    {
        // Create DateTime from WeatherAPIManager's localTime
        DateTime dtLocalTime = Convert.ToDateTime(WAM.weatherData.location.localtime);
        TimeSpan localTime = dtLocalTime.TimeOfDay;

        // Build TimeSpan list
        for (int i = 0; i < timeOfDayFxData.timeOfDayFx.Length; i++)
        {
            // Split the startTime and endTime strings in timeOfDayFx into hours and minutes
            string[] splitStartTime = timeOfDayFxData.timeOfDayFx[i].startTime.Split(":");
            int startHour = int.Parse(splitStartTime[0]);
            int startMinute = int.Parse(splitStartTime[1]);

            string[] splitEndTime = timeOfDayFxData.timeOfDayFx[i].endTime.Split(":");
            int endHour = int.Parse(splitEndTime[0]);
            int endMinute = int.Parse(splitEndTime[1]); // Corrected index to parse minutes

            // Create start and end TimeSpans
            TimeSpan startTime = new TimeSpan(startHour, startMinute, 0);
            TimeSpan endTime = new TimeSpan(endHour, endMinute, 0);

            // Check which timeOfDayFx localTime is in range of
            if (localTime >= startTime && localTime <= endTime)
            {
                Debug.Log(localTime + " is within range of " + timeOfDayFxData.timeOfDayFx[i].timeOfDay);
                TimeOfDay = (TimesOfDay)i;
                HandleTime();
                break;
            }
            else
            {
                Debug.Log(localTime + " is not within range of " + timeOfDayFxData.timeOfDayFx[i].timeOfDay);
            }
        }
    }

    public void ParseCurrentWeather()
    {
        string condition = WAM.weatherData.current.condition.text.ToLower();

        switch (condition)
        {
            case var weather when condition.ToLower().Contains("rain"):
                WeatherCondition = WeatherConditions.Rain;
                break;
            case var weather when condition.ToLower().Contains("snow"):
                WeatherCondition = WeatherConditions.Snow;
                break;
            case var weather when condition.ToLower().Contains("fog"):
                WeatherCondition = WeatherConditions.Fog;
                break;
            default:
                Debug.Log("There is no FX set up for " + condition);
                WeatherCondition = WeatherConditions.Dry;
                break;
        }

        HandleWeatherCondition();
    }
    #endregion

    #region Dev Controls
    private void OnValidate()
    {
        HandleTime();
        HandleWeatherCondition();
        Debug.Log("Time of day is: " + TimeOfDay + ", weather condition is: " + WeatherCondition);
    }

    public void ToggleDebugWeatherUI()
    {
        showWeatherOnGui = !showWeatherOnGui;
    }
    #endregion

    #region OnGUI
    public void OnGUI()
    {
        InitStyles();

        // Draw current weather
        if (showWeatherOnGui)
        {
            GUI.backgroundColor = Color.black;
            GUI.Box(new Rect(10, 10, 300, 110), "", currentStyle);
            GUI.Label(new Rect(100, 10, 150, 20), "City: " + WAM.weatherData.location.region);
            GUI.Label(new Rect(100, 40, 150, 20), "Weather: " + WAM.weatherData.current.condition.text);
            GUI.Label(new Rect(100, 70, 150, 20), "Temperature: " + WAM.weatherData.current.temp_c + "°");
            GUI.Label(new Rect(100, 100, 200, 20), "Wind direction: " + WAM.weatherData.current.wind_dir + ", " + +WAM.weatherData.current.wind_degree + "°");

            if (WAM.weatherConditionSprite)
            {
                GUI.DrawTexture(new Rect(10, 10, 90, 90), WAM.weatherConditionSprite.texture);
            }
        }
    }
    #endregion

    #region OnGUI Styling
    private void InitStyles()
    {
        if (currentStyle == null)
        {
            currentStyle = new GUIStyle(GUI.skin.box);
            currentStyle.normal.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.5f));
        }
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
    #endregion

}