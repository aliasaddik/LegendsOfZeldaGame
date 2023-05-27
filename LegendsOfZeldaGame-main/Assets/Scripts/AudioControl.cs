using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    public Slider BG;
    public Slider SFX;
    public string parameterName = "SFX";
    public string parameterNameMusic = "Music";

    public AudioMixer mixer;

    // Start is called before the first frame update
    void Start()
    {
        BG.value = PlayerPrefs.GetFloat("Music", 0.75f);
        SFX.value = PlayerPrefs.GetFloat("SFX", 0.75f);


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetLevel(float sliderValue)
    {
        mixer.SetFloat("Music", Mathf.Log10(BG.value) * 20);
        mixer.SetFloat("SFX", Mathf.Log10(SFX.value) * 20);
    }


}



