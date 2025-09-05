using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DayTimeManager : MonoBehaviour
{
    [Header("Time")]
    [SerializeField] private int dayStartTime = 6;
    [SerializeField] private int dayEndTime = 26;
    [SerializeField] private int twilightStartTime = 19;
    [SerializeField] private int darkestTime = 22;
    [SerializeField] private float timeScale = 60f;
    [SerializeField] private float timeInterval = 10f;

    [Header("TimeUI")]
    [SerializeField] public TextMeshProUGUI hourUIText;
    [SerializeField] public TextMeshProUGUI minuteUIText;

    [Header("Light")]
    [SerializeField] public Light2D globalLight;
    [SerializeField] private Color nightLightColor;
    [SerializeField] private Color dayLightColor = Color.white;

    [Header("Slime")]
    [SerializeField] private int slimeSpawnTime = 19;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private SlimeSpawnController spawnController;
    [SerializeField] private bool isSlimeSpawn = true;

    const int secondsPerHour = 3600;

    bool stopTime = false;
    public GameObject stop;

    private float realTimer = 0f;
    private float gameTimer = 0f;
    private int hour = 6;
    private int minute = 0;
    private Coroutine slimeSpawn;

    public event Action SpawnSlime = null;
    public event Action OnDayFinished = null;

    private void Awake()
    {
        gameTimer = dayStartTime * secondsPerHour;
        if (hourUIText == null || minuteUIText == null)
            return;
        hourUIText.text = string.Format("{00:00}", dayStartTime);
        minuteUIText.text = string.Format("{00:00}", minute);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            stopTime = !stopTime;
            if (stopTime)
                stop.SetActive(true);
            else
                stop.SetActive(false);
        }

        if (InGameManager.Instance.player.isDead)
            return;

        if (!stopTime)
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
                InGameManager.Instance.player.Die();
                return;
            }

            int hourText = hour;
            if (hourText > 24)
                hourText -= 24;
            hourUIText.text = string.Format("{00:00}", hourText);
            minuteUIText.text = string.Format("{00:00}", minute);

            UpdateLight();
        }
    }

    public void NextDay()
    {
        SceneLoadManager.Instance.StartLoadScene("House", false, true);
    }

    public IEnumerator StartNewDay()
    {
        yield return null;
        DataManager.instance.SaveData();

        hour = dayStartTime;
        gameTimer = dayStartTime * secondsPerHour;
        globalLight.color = dayLightColor;

        hourUIText.text = string.Format("{00:00}", hour);
        minuteUIText.text = string.Format("{00:00}", minute);
    }

    public void UpdateLight()
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

    public void SetTimeStop(bool stop)
    {
        Image image = hourUIText.GetComponentInParent<Image>();
        if (image == null)
        {
            Debug.Log("이미지 없음");
            return;
        }

        if (stop)
            image.color = new Color(0.8f, 0.8f, 0.8f);
        else
            image.color = Color.white;
    }

    public void StartOnDayFinishedEvent()
    {
        OnDayFinished?.Invoke();
    }
}
