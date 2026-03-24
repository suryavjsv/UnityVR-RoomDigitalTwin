using UnityEngine;
using TMPro;

public class SensorUI : MonoBehaviour
{
    public LiveData liveData;
    public TextMeshProUGUI temperatureText;
    public TextMeshProUGUI humidityText;
    public TextMeshProUGUI lightText;
    public TextMeshProUGUI motionText;

    [Header("AC Board Display")]
    public TextMeshPro acTempDisplay;

    void Update()
    {
        temperatureText.text = "Temp: " + liveData.temperature.ToString("F1") + " C";
        humidityText.text = "Humidity: " + liveData.humidity.ToString("F1") + " %";
        lightText.text = "Light: " + liveData.light.ToString();
        motionText.text = "Motion: " + (liveData.motion == 1 ? "DETECTED!" : "None");

        // AC Board shows live temperature with degree symbol
        if (acTempDisplay != null)
        {
            acTempDisplay.text = liveData.temperature.ToString("F1") + " C";
        }
    }
}