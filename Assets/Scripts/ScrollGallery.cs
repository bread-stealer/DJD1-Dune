using UnityEngine;
using UnityEngine.UI;

public class ScrollGallery : MonoBehaviour
{
    [System.Serializable]
    public class GalleryRow
    {
        public RectTransform rowContainer;
        public float speed = 50f;
        public bool movesLeft = true;
    }

    [Header("Rows")]
    [SerializeField] private GalleryRow[] rows;

    [Header("Image Settings")]
    [SerializeField] private Sprite[] images;
    [SerializeField] private float imageWidth = 300f;
    [SerializeField] private float imageHeight = 180f;
    [SerializeField] private float imagePadding = 20f;

    private float _totalRowWidth;

    private void Awake()
    {
        _totalRowWidth = (imageWidth + imagePadding) * images.Length;
        BuildRows();
    }

    private void BuildRows()
    {
        foreach (GalleryRow row in rows)
        {
            // Duplicate the images to create a seamless loop
            for (int i = 0; i < images.Length * 2; i++)
            {
                Sprite sprite = images[i % images.Length];
                GameObject imageObject = new GameObject($"Image_{i}", typeof(RectTransform), typeof(Image));
                imageObject.transform.SetParent(row.rowContainer, false);

                RectTransform rect = imageObject.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(imageWidth, imageHeight);
                rect.anchoredPosition = new Vector2(i * (imageWidth + imagePadding), 0f);

                Image img = imageObject.GetComponent<Image>();
                img.sprite = sprite;
                img.preserveAspect = true;
            }
        }
    }

    private void Update()
    {
        foreach (GalleryRow row in rows)
        {
            float direction = row.movesLeft ? -1f : 1f;
            row.rowContainer.anchoredPosition += new Vector2(direction * row.speed * Time.deltaTime, 0f);

            // Reset position to create seamless loop
            if (row.movesLeft && row.rowContainer.anchoredPosition.x <= -_totalRowWidth)
                row.rowContainer.anchoredPosition = new Vector2(0f, row.rowContainer.anchoredPosition.y);
            else if (!row.movesLeft && row.rowContainer.anchoredPosition.x >= _totalRowWidth)
                row.rowContainer.anchoredPosition = new Vector2(0f, row.rowContainer.anchoredPosition.y);
        }
    }
}
