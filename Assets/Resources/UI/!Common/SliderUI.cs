using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SliderUI : MonoBehaviour
{
    public void SetSliderValueText() => GetComponentsInChildren<TMP_Text>().Single(x => x.name == "SliderValueText").text = (Mathf.Round(GetComponent<Slider>().value * 100)/100).ToString();
}
