using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class FirebaseReader : MonoBehaviour
{
    public LiveData liveData;
    private string url = "https://roomdigitaltwin-default-rtdb.asia-southeast1.firebasedatabase.app/room.json";

    void Start()
    {
        StartCoroutine(PollFirebase());
    }

    IEnumerator PollFirebase()
    {
        while (true)
        {
            UnityWebRequest req = UnityWebRequest.Get(url);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                RoomData data = JsonUtility.FromJson<RoomData>(req.downloadHandler.text);
                liveData.temperature = data.temperature;
                liveData.humidity = data.humidity;
                liveData.light = data.light;
                liveData.motion = data.motion;
                Debug.Log("Firebase updated! Temp: " + data.temperature);
            }
            else
            {
                Debug.LogError("Firebase error: " + req.error);
            }

            yield return new WaitForSeconds(2f);
        }
    }
}

[System.Serializable]
public class RoomData
{
    public float temperature;
    public float humidity;
    public int light;
    public int motion;
}