using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashingButton : MonoBehaviour
{
    public Button button; // Reference to the Button component
    public Color startColor = Color.white; // Start color
    public Color endColor = Color.red; // End color
    public float flashDuration = 1.0f; // Duration of one flash cycle

    private void OnEnable()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        // Start the flashing coroutine
        StartCoroutine(FlashButton());
    }

    IEnumerator FlashButton()
    {
        while (true)
        {
            
            // Lerp from startColor to endColor
            float timer = 0;
            //Debug.Log(timer);
            while (timer <= flashDuration)
            {
                button.image.color = Color.Lerp(startColor, endColor, timer);
                timer += Time.deltaTime;
                yield return null;
            }

            // Lerp from endColor to startColor
            timer = 0;
            while (timer <= flashDuration)
            {
                button.image.color = Color.Lerp(endColor, startColor, timer);
                timer += Time.deltaTime;
                yield return null;
            }
        }
    }
}
