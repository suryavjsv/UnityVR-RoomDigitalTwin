using UnityEngine;

[CreateAssetMenu(fileName = "LiveData", menuName = "RoomTwin/LiveData")]
public class LiveData : ScriptableObject
{
    public float temperature;
    public float humidity;
    public int light;
    public int motion;
}