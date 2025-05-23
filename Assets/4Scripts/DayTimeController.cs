using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class DayTimeController : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] int dayStartTime = 6;
    [SerializeField] float timeScale = 60f;
    [SerializeField] float timeInterval = 10f;
    [SerializeField] TextMeshProUGUI timeText;

    [Header("Light")]
    [SerializeField] Light2D globalLight;
    [SerializeField] Color nightLightColor;
    [SerializeField] Color dayLightColor = Color.white;
    [SerializeField] AnimationCurve nightTimeCurve;

    private float realTimer = 0f;
    private float gameTimer = 0f;
    private int hour = 6;
    private int minute = 0;

    private void Awake()
    {
        gameTimer = dayStartTime * 3600;
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

            hour = (int)(gameTimer / 3600) % 24;
            minute = (int)(gameTimer / 60) % 60;

            // »õº® 2½Ã 
            if (gameTimer >= 93600f)
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
        gameTimer = dayStartTime * 3600;
        hour = dayStartTime;
    }

    private void UpdateLight()
    {
        float v = nightTimeCurve.Evaluate(gameTimer);
        Color color = Color.Lerp(dayLightColor, nightLightColor, v);
        globalLight.color = color;
    }
}
