using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WeatherApiManager : MonoBehaviour
{
	[Header("WeatherAPI URL Constructors")]

	[SerializeField]
	private string weatherApiKey = ""; // your WeatherAPI key here
	private string weatherApiBaseUrl = "";

	[Header("WeatherAPI Data")]
	[SerializeField]
	public WeatherData weatherData;
	public Sprite weatherConditionSprite;

	[Header("Managers")]
	private LocationServicesManager LSM;
	public bool hasWeatherData = false;

	private void Awake()
	{
		if (weatherApiKey == "" || weatherApiKey == null)
		{
			Debug.LogError("No WeatherAPI key provided.");
		}
		else
		{
			weatherApiBaseUrl = "http://api.weatherapi.com/v1/current.json?key=" + weatherApiKey + "&q=";
		}

		LSM = FindObjectOfType<LocationServicesManager>();
	}

	private void Start()
	{
		GetWeatherData();
	}

	public void GetWeatherData()
	{
		StartCoroutine(GetWeatherDataFromJSON());
	}

	IEnumerator GetWeatherDataFromJSON()
	{
		while (!LSM.hasLocationData)
		{
			yield return null;
		}

		string url = weatherApiBaseUrl + LSM.locationData.city + "&aqi=no";
		using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
		{
			// Request and wait for the desired page
			yield return webRequest.SendWebRequest();

			string[] pages = url.Split('/');
			int page = pages.Length - 1;

			switch (webRequest.result)
			{
				case UnityWebRequest.Result.ConnectionError:
				case UnityWebRequest.Result.DataProcessingError:
					Debug.LogError(pages[page] + ": Error: " + webRequest.error);
					break;
				case UnityWebRequest.Result.ProtocolError:
					Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
					break;
				case UnityWebRequest.Result.Success:
					Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
					weatherData = JsonUtility.FromJson<WeatherData>(webRequest.downloadHandler.text);
					hasWeatherData = true;
					StartCoroutine(GetWeatherConditionIcon());
					break;
			}
		}
	}

	IEnumerator GetWeatherConditionIcon()
	{
		UnityWebRequest www = UnityWebRequestTexture.GetTexture(weatherData.current.condition.icon);

		yield return www.SendWebRequest();

		if (www.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.LogError(www.error);
		}
		else
		{
			Debug.Log("Successfully downloaded image.");
			var texture = DownloadHandlerTexture.GetContent(www);
			weatherConditionSprite = TextureToSprite.ConvertToSprite(texture);
			//weatherIcon.sprite = weatherConditionSprite;
		}
	}

	public static WeatherData CreateFromJSON(string jsonString)
	{
		return JsonUtility.FromJson<WeatherData>(jsonString);
	}
}

[System.Serializable]
public class WeatherData
{
	public WeatherDataLocation location;
	public WeatherDataCurrent current;
}

[System.Serializable]
public class WeatherDataLocation
{
	public string name;
	public string region;
	public string country;
	public string lat;
	public string lon;
	public string tz_id;
	public string localtime_epoch;
	public string localtime;
}

[System.Serializable]
public class WeatherDataCurrent
{
	public double last_updated_epoch;
	public string last_updated;
	public float temp_c;
	public float temp_f;
	public int isDay;
	public WeatherDataCurrentCondition condition;
	public float wind_mph;
	public float wind_kph;
	public float wind_degree;
	public string wind_dir;
	public float pressure_mb;
	public float pressure_in;
	public float precip_mm;
	public float precip_in;
	public float humidity;
	public float cloud;
	public float feelslike_c;
	public float feelslike_f;
	public float vis_km;
	public float vis_miles;
	public float uv;
	public float gust_mph;
	public float gust_kph;
}

[System.Serializable]
public class WeatherDataCurrentCondition
{
	public string text;
	public string icon;
	public int code;
}


