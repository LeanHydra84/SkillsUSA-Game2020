using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class VolumeSlider : MonoBehaviour
{
    public Text label;

    Slider slide;
    
    private void Start()
    {
        slide = GetComponent<Slider>();
    }

    // Update is called once per frame
    private void OnEnable()
    {
        if(slide == null) slide = GetComponent<Slider>();
        slide.value = Settings.Volume;
        label.text = Settings.Volume + "%";
    }

    void SliderChanged()
    {
        Settings.Volume = (int)slide.value;
        label.text = Settings.Volume + "%";
        AudioSourceHandler.AudioDelegate();
    }

}
