using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LocationServicesManager : MonoBehaviour
{
    [SerializeField]
    private string externalIP = "";

    [SerializeField]
    public LocationData locationData;

    private bool hasExternalIP = false;
    public bool hasLocationData = false;

    [Header("Manual Location Entry")]
    public string customCity = "";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Initialising location services...");

        StartCoroutine(GetPublicIP());

        // Do editor / Windows location fetching
        string url = "http://ip-api.com/json/" + externalIP;
        StartCoroutine(GetLocationData(url));

#if UNITY_ANDROID
        
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location services are not enabled on device, or this app does not have permission to access location.");
        }
        
        StartCoroutine(InitMobileLocationServices());

#endif
    }

    #region Desktop Location Services
    IEnumerator GetPublicIP()
    {
        // https://gist.github.com/Raziel619/2636dc4c6aaa7f7076432339fa1f8e62 shout out

        UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error when sending web request: " + www.error);
        }
        else
        {
            string result = www.downloadHandler.text;

            // This results in a string similar to this: <html><head><title>Current IP Check</title></head><body>Current IP Address: 123.123.123.123</body></html>
            // where 123.123.123.123 is your external IP Address.
            //  Debug.Log("" + result);

            string[] a = result.Split(':'); // Split into two substrings -> one before : and one after. 
            string a2 = a[1].Substring(1);  // Get the substring after the :
            string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
            string a4 = a3[0];              // Get the substring before the tag.

            Debug.Log("External IP Address = " + a4);
            externalIP = a4;

            hasExternalIP = true;
        }
    }

    IEnumerator GetLocationData(string url)
    {
        while (!hasExternalIP)
        {
            yield return null;
        }

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

                    locationData = CreateFromJSON(webRequest.downloadHandler.text);
                    Debug.Log("My city is: " + locationData.city);
                    hasLocationData = true;
                    break;
            }
        }
    }
    #endregion

    public static LocationData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<LocationData>(jsonString);
    }

    #region Mobile Location Services
    IEnumerator InitMobileLocationServices()
    {
        // Start location service
        Input.location.Start();

        // Wait for initialisation
        int maxWait = 20;

        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Cancel location service use if it times out
        if (maxWait < 1)
        {
            Debug.Log("Location services timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location.");
            yield break;
        }
        else
        {
            // If there was a successful connection, retrieve the device's current location and log it in console
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

        // Stop the service once the location has been retrieved
        Input.location.Stop();
    }
    #endregion

    public void UseCustomLocation()
    {
        if (customCity == "" || customCity == null)
        {
            Debug.LogError("No custom city declared.");
        }
        else
        {
            locationData.city = customCity;

            WeatherApiManager WAM = FindObjectOfType<WeatherApiManager>();
            WAM.hasWeatherData = false;
            WAM.GetWeatherData();

            WeatherFXManager WFXM = FindObjectOfType<WeatherFXManager>();
            WFXM.SetWeatherConditions();
        }
    }
}

[System.Serializable]
public class LocationData
{
    public string country = "";
    public string countryCode = "";
    public string region = "";
    public string regionName = "";
    public string city = "";
}