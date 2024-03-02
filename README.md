# WeatherManager
WeatherManager provides functionality to add weather and time of day effects to Unity projects via WeatherAPI data. 

![Demo](https://github.com/SavageOranges/WeatherManager/blob/main/Previews/WeatherManagerDemo.gif)

Accesses location via IPV4 and checkip.dyndns.org, and uses WeatherAPI to get your time of day and rough location. 

This can be overriden with a manual 'City' string entry for more accurate results, as IPV4 will usually return the ISP's location. 

IPV6 will give a more accurate location, but no free services to get this data exist that I know of.

## Instructions
All files are in `Assets/WeatherEffects`.

Get your own WeatherAPI key from https://www.weatherapi.com/ and add it to the designated inspector field in the `WeatherApiManager` component on the `WeatherManager` GameObject.

Enter play mode to get your current weather and time of day data.

The `WeatherFXManager` component on the `WeatherManager` GameObject contains dropdowns to alter time of day and active weather effects.

The `LocationServicesManager` component on the `WeatherManager` GameObject allows you to input a city name and update location and weather data manually.

There is a custom ScriptableObject class and entry, `TimeOfDayFx`, that allows you to define timespans and custom skyboxes for each time of day.

## Compatibility
Supports Unity 2021.3.0f1 and above.

Hasn't been tested on all platforms that Unity supports, but it probably works on most. 

Mobile builds may be able to leverage device-level location services as a workaround for IPV6 support, but this has not been tested. Some intial work on this in LocationServicesManager.cs

### Credits
Snowflake particle: https://opengameart.org/content/snow-flake

Gradient skybox shader: https://www.youtube.com/watch?v=AHd5Bh5myVY
