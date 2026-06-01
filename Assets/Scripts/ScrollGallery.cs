using System.Collections.Generic;
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
        [HideInInspector] public List<RectTransform> imageRects = new List<RectTransform>();
    }

    [Header("Rows")]
    [SerializeField] private GalleryRow[] rows;

    [Header("Image Settings")]
    [SerializeField] private Sprite[] images;
    [SerializeField] private float imageWidth = 300f;
    [SerializeField] private float imageHeight = 180f;
    [SerializeField] private float imagePadding = 20f;

    private float _screenWidth;
    private float _slotWidth;

    private void Awake()
    {
        _screenWidth = GetComponent<RectTransform>().rect.width;
        _slotWidth = imageWidth + imagePadding;
        BuildRows();
    }

    private void BuildRows()
    {
        int imageCount = Mathf.CeilToInt(_screenWidth / _slotWidth) + 2;

        foreach (GalleryRow row in rows)
        {
            row.imageRects.Clear();

            for (int i = 0; i < imageCount; i++)
            {
                Sprite sprite = images[i % images.Length];
                RectTransform rect = CreateImage(row.rowContainer, sprite);
                rect.anchoredPosition = new Vector2(i * _slotWidth, 0f);
                row.imageRects.Add(rect);
            }
        }
    }

    private RectTransform CreateImage(RectTransform parent, Sprite sprite)
    {
        GameObject imageObject = new GameObject("GalleryImage", typeof(RectTransform), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(imageWidth, imageHeight);
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);

        Image img = imageObject.GetComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;

        return rect;
    }

    private void Update()
    {
        foreach (GalleryRow row in rows)
        {
            float delta = row.speed * Time.deltaTime;
            float direction = row.movesLeft ? -1f : 1f;

            foreach (RectTransform rect in row.imageRects)
                rect.anchoredPosition += new Vector2(direction * delta, 0f);

            if (row.movesLeft)
                RecycleLeft(row);
            else
                RecycleRight(row);
        }
    }

    private void RecycleLeft(GalleryRow row)
    {
        // Find the leftmost image that has gone off screen and move it to the right
        RectTransform leftmost = null;
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach (RectTransform rect in row.imageRects)
        {
            if (rect.anchoredPosition.x < minX)
            {
                minX = rect.anchoredPosition.x;
                leftmost = rect;
            }
            if (rect.anchoredPosition.x > maxX)
                maxX = rect.anchoredPosition.x;
        }

        if (leftmost != null && minX < -(imageWidth + imagePadding))
            leftmost.anchoredPosition = new Vector2(maxX + _slotWidth, 0f);
    }

    private void RecycleRight(GalleryRow row)
    {
        // Find the rightmost image that has gone off screen and move it to the left
        RectTransform rightmost = null;
        float maxX = float.MinValue;
        float minX = float.MaxValue;

        foreach (RectTransform rect in row.imageRects)
        {
            if (rect.anchoredPosition.x > maxX)
            {
                maxX = rect.anchoredPosition.x;
                rightmost = rect;
            }
            if (rect.anchoredPosition.x < minX)
                minX = rect.anchoredPosition.x;
        }

        if (rightmost != null && maxX > _screenWidth + imageWidth + imagePadding)
            rightmost.anchoredPosition = new Vector2(minX - _slotWidth, 0f);
    }
}
