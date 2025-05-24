using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayTimeController : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] int dayStartTime = 6;
    [SerializeField] int dayEndTime = 26;
    [SerializeField] int twilightStartTime = 19;
    [SerializeField] int darkestTime = 22;

    [SerializeField] float timeScale = 60f;
    [SerializeField] float timeInterval = 10f;
    [SerializeField] TextMeshProUGUI timeText;

    [Header("Light")]
    [SerializeField] Light2D globalLight;
    [SerializeField] Color nightLightColor;
    [SerializeField] Color dayLightColor = Color.white;

    const int secondsPerHour = 3600;

    private float realTimer = 0f;
    private float gameTimer = 0f;
    private int hour = 6;
    private int minute = 0;

    private void Awake()
    {
        gameTimer = dayStartTime * secondsPerHour;
        timeText.text = string.Format("{0:00}:{1:00}", dayStartTime, minute);
    }
    
    private void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        realTimer += Time.deltaTime;
        if (realTimer >= timeInterval)
        {
            realTimer = 0f;
            gameTimer += timeInterval * timeScale;

            hour = (int)(gameTimer / secondsPerHour) % 24;
            minute = (int)(gameTimer / 60) % 60;

            if (gameTimer >= dayEndTime * secondsPerHour)
            {
                NextDay();
            }

            int hourText = hour;
            if (hourText > 24)
                hourText -= 24;
            timeText.text = string.Format("{0:00}:{1:00}", hourText, minute);

            UpdateLight();
        }
    }

    private void NextDay()
    {
        gameTimer = dayStartTime * secondsPerHour;
        hour = dayStartTime;
    }

    private void UpdateLight()
    {
        float lightLerpTime = Mathf.InverseLerp(twilightStartTime * secondsPerHour, darkestTime * secondsPerHour, gameTimer);

        Color color = Color.Lerp(dayLightColor, nightLightColor, lightLerpTime);
        globalLight.color = color;
    }
}
