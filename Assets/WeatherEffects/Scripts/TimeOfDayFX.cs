using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeOfDayFX", menuName = "WeatherFX/TimeOfDayFX", order = 1)]
public class TimeOfDayFX : ScriptableObject
{
    public TimeOfDayFXObject[] timeOfDayFx;
}

[System.Serializable]
public class TimeOfDayFXObject
{
    public string timeOfDay = "";
    public string startTime = "";
    public string endTime = "";
    public Material skyboxMaterial;
}
