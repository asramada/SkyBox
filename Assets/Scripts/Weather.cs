using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class Weather : MonoBehaviour {

    public ParticleSystem sunnyParticleSystem;                          // Particle System set in the editor for sunny weather
    public ParticleSystem rainyParticleSystem;                          // Particle System set in the editor for rainy weather
    public ParticleSystem snowyParticleSystem;                          // Particle System set in the editor for snowy weather

    public float audioFadeTime = 0.25f;
    public AudioClip sunnyAudio;
    public AudioClip rainyAudio;
    public AudioClip snowyAudio;

    public float lightDimTime = 0.1f;
    public float sunnylightIntensity = 1.0f;
    public float rainylightIntensity = 0.25f;
    public float snowylightIntensity = 0.75f;

    public float weatherSwitchTimer = 10.0f;                            // Time between each Switch

    public float weatherheight = 15.0f;                                 // Height of the weather GameObject


    // SkyBox Blending Public Variables
    public float skyboxBlendTime = 0.15f;
    //SkyBox Blending Variables - End

    public bool bSwitchWeather = true;

    public Color sunnyFogColor;
    public Color rainyFogColor;
    public Color snowyFogColor;
    public float fogChangeSpeed = 0.1f;

    private float currentSwitchTimer = 0.0f;                            // Current Timer Counter

    private ParticleSystem.EmissionModule sunnyModule;
    private ParticleSystem.EmissionModule rainyModule;
    private ParticleSystem.EmissionModule snowyModule;

    private Transform player;                                           // Player Transform
    private Transform weatherTransform;                                 // Weather Transform

    // SkyBox Blending Private Variables
    private float skyboxBlendValue = 0.1f;
    private bool sunnySkyState;
    private bool rainySkyState;
    private bool snowySkyState;
    //SkyBox Blending Variables - End

    private enum WeatherStates                                          // Different Types of Weather
    {
        WeatherCirculation,
        SunnyWeather,
        RainyWeather,
        SnowyWeather
    }

    private WeatherStates currentWeatherState;


	// Use this for initialization
	void Start ()
    {
        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
        player = playerGameObject.transform;

        GameObject weatherGameObject = GameObject.FindGameObjectWithTag("Weather");
        weatherTransform = weatherGameObject.transform;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.01f;

        RenderSettings.skybox.SetFloat("_Blend", 0);

        sunnyModule = sunnyParticleSystem.emission;
        rainyModule = rainyParticleSystem.emission;
        snowyModule = snowyParticleSystem.emission;

        StartCoroutine(WeatherStateMachine());
	}
	
	// Update is called once per frame
	void Update ()
    {
        WeatherUpdate();

        SkyBoxBlendManager();

        weatherTransform.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + weatherheight, player.transform.position.z);

    }
    
    void WeatherUpdate()
    {
        currentSwitchTimer -= Time.deltaTime;

        if(currentSwitchTimer <= 0.0f)
        {
            currentSwitchTimer = weatherSwitchTimer;
            currentWeatherState = Weather.WeatherStates.WeatherCirculation;
        }
    }

    IEnumerator WeatherStateMachine()
    {
        while (bSwitchWeather)
        {
            switch (currentWeatherState)
            {
                case WeatherStates.WeatherCirculation:
                    WeatherCirculation();
                    break;

                case WeatherStates.SunnyWeather:
                    SunnyWeather();
                    break;

                case WeatherStates.RainyWeather:
                    RainyWeather();
                    break;

                case WeatherStates.SnowyWeather:
                    SnowyWeather();
                    break;

            }
            yield return null;
        }       
    }

    void WeatherCirculation()
    {
        int switchWeather = Random.Range(0, 4);

        sunnyModule.enabled = false;
        rainyModule.enabled = false;
        snowyModule.enabled = false;

        sunnySkyState = false;
        rainySkyState = false;
        snowySkyState = false;

        switch (switchWeather)
        {
            case 0:
                currentWeatherState = Weather.WeatherStates.WeatherCirculation;
                break;

            case 1:
                currentWeatherState = Weather.WeatherStates.SunnyWeather;
                break;

            case 2:
                currentWeatherState = Weather.WeatherStates.RainyWeather;
                break;

            case 3:
                currentWeatherState = Weather.WeatherStates.SnowyWeather;
                break;
        }
    }

    void SunnyWeather()
    {
        sunnyModule.enabled = true;

        sunnySkyState = true;

        // Set Light Intensity
        if(GetComponent<Light>().intensity > sunnylightIntensity)
        {
            GetComponent<Light>().intensity -= Time.deltaTime * lightDimTime;
        }

        if(GetComponent<Light>().intensity < sunnylightIntensity)
        {
            GetComponent<Light>().intensity += Time.deltaTime * lightDimTime;
        }

        // Reduce Volume if its a different Audio Clip
        if ((GetComponent<AudioSource>().volume > 0) && (GetComponent<AudioSource>().clip != sunnyAudio))
            GetComponent<AudioSource>().volume -= Time.deltaTime * audioFadeTime;

        // Stop and Reassign Audio Source to the Current Weather
        if (GetComponent<AudioSource>().volume == 0)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = sunnyAudio;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        // Turn up the Volume if its the right audio clip
        if ((GetComponent<AudioSource>().volume < 1) && (GetComponent<AudioSource>().clip == sunnyAudio))
            GetComponent<AudioSource>().volume += Time.deltaTime * audioFadeTime;

        Color currentColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(currentColor, sunnyFogColor, fogChangeSpeed * Time.deltaTime);
    }

    void RainyWeather()
    {
        rainyModule.enabled = true;

        rainySkyState = true;

        // Set Light Intensity
        if (GetComponent<Light>().intensity > rainylightIntensity)
        {
            GetComponent<Light>().intensity -= Time.deltaTime * lightDimTime;
        }

        if (GetComponent<Light>().intensity < rainylightIntensity)
        {
            GetComponent<Light>().intensity += Time.deltaTime * lightDimTime;
        }

        // Reduce Volume if its a different Audio Clip
        if ((GetComponent<AudioSource>().volume > 0) && (GetComponent<AudioSource>().clip != rainyAudio))
            GetComponent<AudioSource>().volume -= Time.deltaTime * audioFadeTime;

        // Stop and Reassign Audio Source to the Current Weather
        if (GetComponent<AudioSource>().volume == 0)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = rainyAudio;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        // Turn up the Volume if its the right audio clip
        if ((GetComponent<AudioSource>().volume < 1) && (GetComponent<AudioSource>().clip == rainyAudio))
            GetComponent<AudioSource>().volume += Time.deltaTime * audioFadeTime;


        Color currentColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(currentColor, rainyFogColor, fogChangeSpeed * Time.deltaTime);
    }

    void SnowyWeather()
    {
        snowyModule.enabled = true;

        snowySkyState = true;

        // Set Light Intensity
        if (GetComponent<Light>().intensity > snowylightIntensity)
        {
            GetComponent<Light>().intensity -= Time.deltaTime * lightDimTime;
        }

        if (GetComponent<Light>().intensity < snowylightIntensity)
        {
            GetComponent<Light>().intensity += Time.deltaTime * lightDimTime;
        }

        // Reduce Volume if its a different Audio Clip
        if ((GetComponent<AudioSource>().volume > 0) && (GetComponent<AudioSource>().clip != snowyAudio))
            GetComponent<AudioSource>().volume -= Time.deltaTime * audioFadeTime;

        // Stop and Reassign Audio Source to the Current Weather
        if (GetComponent<AudioSource>().volume == 0)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = snowyAudio;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        // Turn up the Volume if its the right audio clip
        if ((GetComponent<AudioSource>().volume < 1) && (GetComponent<AudioSource>().clip == snowyAudio))
            GetComponent<AudioSource>().volume += Time.deltaTime * audioFadeTime;


        Color currentColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(currentColor, snowyFogColor, fogChangeSpeed * Time.deltaTime);
    }

    void SkyBoxBlendManager()
    {
        if (sunnySkyState == true)
        {
            if (skyboxBlendValue == 0.0f)
                return;

            if (skyboxBlendValue > 0.0f)
                skyboxBlendValue -= skyboxBlendTime * Time.deltaTime;

            if (skyboxBlendValue < 0.0f)
                skyboxBlendValue = 0.0f;
        }
        else if (rainySkyState == true)
        {
            if (skyboxBlendValue == 1.0f)
                return;

            if (skyboxBlendValue < 1.0f)
                skyboxBlendValue += skyboxBlendTime * Time.deltaTime;

            if (skyboxBlendValue > 1.0f)
                skyboxBlendValue = 1.0f;
        }
        else if(snowySkyState == true)
        {
            if (skyboxBlendValue == 0.5f)
                return;

            if (skyboxBlendValue < 0.5f)
            {
                skyboxBlendValue += skyboxBlendTime * Time.deltaTime;

                if (skyboxBlendValue > 0.5f)
                    skyboxBlendValue = 0.5f;

            }

            else if (skyboxBlendValue > 0.5f)
            {
                skyboxBlendValue -= skyboxBlendTime * Time.deltaTime;

                if (skyboxBlendValue < 0.5f)
                    skyboxBlendValue = 0.5f;

            }

        }

        RenderSettings.skybox.SetFloat("_Blend", skyboxBlendValue);
    }
}
