using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIController : MonoBehaviour
{
    public Slider progressBar;
    public Gradient colorGradient;
    public Image fillImage;

    public void UpdateProgress(float target)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateBar(target));
    }

    IEnumerator AnimateBar(float target)
    {
        float start = progressBar.value;
        float time = 0;

        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float t = time / 0.3f;

            float value = Mathf.Lerp(start, target, t);
            progressBar.value = value;
            fillImage.color = colorGradient.Evaluate(value);

            yield return null;
        }
    }
}