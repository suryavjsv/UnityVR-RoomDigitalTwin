using UnityEngine;

public class RoomTwin : MonoBehaviour
{
    public LiveData liveData;

    [Header("Temperature - Wall Color")]
    public Renderer[] walls;
    public Color coldColor = new Color(0.6f, 0.8f, 1f);
    public Color hotColor = new Color(1f, 0.3f, 0.1f);
    public float minTemp = 20f;
    public float maxTemp = 40f;

    [Header("Light - Point Light + Tube Light")]
    public Light roomLight;
    public Renderer[] tubeLights;
    public Color tubeLightOff = new Color(0.2f, 0.2f, 0.2f);
    public Color tubeLightOn = new Color(1f, 1f, 0.8f);
    public float minLightValue = 0f;
    public float maxLightValue = 4095f;

    [Header("Motion - Floor Glow")]
    public Renderer floor;
    public Color normalFloor = new Color(0.3f, 0.3f, 0.3f);
    public Color motionFloor = new Color(1f, 0.9f, 0.3f);

    void Update()
    {
        // Temperature → wall color
        float tempRatio = Mathf.InverseLerp(minTemp, maxTemp, liveData.temperature);
        Color wallColor = Color.Lerp(coldColor, hotColor, tempRatio);
        foreach (Renderer wall in walls)
        {
            wall.material.color = wallColor;
        }

        // LDR → point light intensity + tube light emission
        // LDR 4095 = dark room, 0 = bright room — so we invert it!
        float lightRatio = Mathf.InverseLerp(minLightValue, maxLightValue, liveData.light);

        if (roomLight != null)
        {
            roomLight.intensity = Mathf.Lerp(0f, 3f, lightRatio);
        }

        // Tube light - Element 1 is the glowing part!
        foreach (Renderer tube in tubeLights)
        {
            Color tubeColor = Color.Lerp(tubeLightOff, tubeLightOn, lightRatio);
            Material[] mats = tube.materials;
            if (mats.Length > 1)
            {
                mats[1].SetColor("_BaseColor", tubeColor);
            }
        }

        // Motion → floor color
        if (floor != null)
        {
            floor.material.color = liveData.motion == 1 ? motionFloor : normalFloor;
        }
    }
}