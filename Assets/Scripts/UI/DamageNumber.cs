using System.Collections;
using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float fadeDuration = 0.8f;

    public void Setup(float damage, bool isHeavy)
    {
        if (text == null)
            text = GetComponent<TextMeshPro>();

        text.text = Mathf.RoundToInt(damage).ToString();
        // Yellow for light, red for heavy
        text.color = isHeavy ? new Color(1f, 0.2f, 0.2f) : new Color(1f, 0.9f, 0.2f);

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float elapsed = 0f;
        Vector3 startPosition = transform.position;
        Color startColor = text.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            // Float upward
            transform.position = startPosition + Vector3.up * (floatSpeed * t);

            // Fade out
            text.color = new Color(startColor.r, startColor.g, startColor.b, 1f - t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
