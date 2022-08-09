using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField]
    Transform hoursPivot,minutesPivot,secondsPivot;

    const float hoursToDegrees=-30f,minutesToDegrees=-6f,secondsToDegrees=-6f;

    void Update(){
        TimeSpan time=DateTime.Now.TimeOfDay;
        hoursPivot.localRotation=Quaternion.Euler(0f,0f,hoursToDegrees*(float)time.TotalHours);
        minutesPivot.localRotation=Quaternion.Euler(0f,0f,minutesToDegrees*(int)time.TotalMinutes);
        secondsPivot.localRotation=Quaternion.Euler(0f,0f,secondsToDegrees*(int)time.TotalSeconds);
    }
}