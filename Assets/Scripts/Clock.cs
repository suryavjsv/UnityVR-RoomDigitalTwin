using UnityEngine;
using System;

public class Clock : MonoBehaviour {

    public GameObject pointerSeconds;
    public GameObject pointerMinutes;
    public GameObject pointerHours;

    [Header("Alignment")]
    [Tooltip("Adjust this if 12:00:00 doesn't point UP. Try 90, -90, or 180.")]
    public float rotationOffset = 0f;

    void Update() {
        DateTime now = DateTime.Now;

        // 1. Get 0-1 progress of the time
        float secPercent = (now.Second + (now.Millisecond / 1000f)) / 60f;
        float minPercent = (now.Minute + secPercent) / 60f;
        float hourPercent = ((now.Hour % 12) + minPercent) / 12f;

        // 2. Calculate Clockwise Rotation
        // We use (1.0 - percent) so that as time INCREASES, 
        // the angle value mathematically DECREASES to stay clockwise.
        float rotS = (1.0f - secPercent) * 360f;
        float rotM = (1.0f - minPercent) * 360f;
        float rotH = (1.0f - hourPercent) * 360f;

        // 3. Apply to Pointers
        pointerSeconds.transform.localEulerAngles = new Vector3(0, 0, rotS + rotationOffset);
        pointerMinutes.transform.localEulerAngles = new Vector3(0, 0, rotM + rotationOffset);
        pointerHours.transform.localEulerAngles   = new Vector3(0, 0, rotH + rotationOffset);
    }
}