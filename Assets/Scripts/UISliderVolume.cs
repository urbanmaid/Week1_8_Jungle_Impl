using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UISliderVolume : MonoBehaviour
{
    private Slider _slider;
    [SerializeField] string audioMixerGroup;

    void Start()
    {
        _slider = transform.GetComponent<Slider>();

        //InitValue(PreferenceManager.Instance.currentPref.audVolBGM);

        if(audioMixerGroup != "" && _slider) _slider.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(float value)
    {
        UIManager.instance.SetAudioValue(audioMixerGroup, value);
    }

    void InitValue(float value)
    {
        _slider.value = value;
    }
}
