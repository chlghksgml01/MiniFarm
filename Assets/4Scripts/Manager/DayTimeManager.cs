using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DayTimeManager : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] int dayStartTime = 6;
    [SerializeField] int dayEndTime = 26;
    [SerializeField] int twilightStartTime = 19;
    [SerializeField] int darkestTime = 22;

    [SerializeField] float timeScale = 60f;
    [SerializeField] float timeInterval = 10f;
    [SerializeField] TextMeshProUGUI hourUIText;
    [SerializeField] TextMeshProUGUI minuteUIText;

    [Header("Light")]
    [SerializeField] private Light2D globalLight;
    [SerializeField] private Color nightLightColor;
    [SerializeField] private Color dayLightColor = Color.white;

    [Header("Slime")]
    [SerializeField] private int slimeSpawnTime = 19;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private SlimeSpawnController spawnController;
    [SerializeField] private bool isSlimeSpawn = true;

    const int secondsPerHour = 3600;

    private float realTimer = 0f;
    private float gameTimer = 0f;
    private int hour = 6;
    private int minute = 0;
    private Coroutine slimeSpawn;

    public event Action SpawnSlime = null;
    public event Action OnDayPassed = null;

    private void Awake()
    {
        gameTimer = dayStartTime * secondsPerHour;
        hourUIText.text = string.Format("{00:00}", dayStartTime);
        minuteUIText.text = string.Format("{00:00}", minute);
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

            if (gameTimer >= slimeSpawnTime * secondsPerHour && slimeSpawn == null && isSlimeSpawn)
            {
                slimeSpawn = StartCoroutine(StartSpawnSlime());
            }

            else if (gameTimer < slimeSpawnTime * secondsPerHour && slimeSpawn != null)
            {
                StopCoroutine(slimeSpawn);
                slimeSpawn = null;
            }

            if (gameTimer >= dayEndTime * secondsPerHour)
            {
                NextDay();
            }

            int hourText = hour;
            if (hourText > 24)
                hourText -= 24;
            hourUIText.text = string.Format("{00:00}", hourText);
            minuteUIText.text = string.Format("{00:00}", minute);

            UpdateLight();
        }
    }

    private void NextDay()
    {
        gameTimer = dayStartTime * secondsPerHour;
        hour = dayStartTime;
        OnDayPassed?.Invoke();
    }

    private void UpdateLight()
    {
        float lightLerpTime = Mathf.InverseLerp(twilightStartTime * secondsPerHour, darkestTime * secondsPerHour, gameTimer);

        Color color = Color.Lerp(dayLightColor, nightLightColor, lightLerpTime);
        globalLight.color = color;
    }

    private IEnumerator StartSpawnSlime()
    {
        while (true)
        {
            SpawnSlime?.Invoke();
            float waitTimeInRealSeconds = spawnInterval * secondsPerHour / timeScale;
            yield return new WaitForSeconds(waitTimeInRealSeconds);
        }
    }
}
