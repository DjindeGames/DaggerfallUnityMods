using DaggerfallWorkshop.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestMarkerComponent : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_DistanceText;
    [SerializeField]
    private Image m_Icon;

    private RectTransform m_RectTransform;

    private void Awake()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public void Display()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetWorldPosition(Vector3 worldPosition)
    {
        m_RectTransform.position = ToScreenPosition(worldPosition);
    }

    public void SetDistance(int distance)
    {
        m_DistanceText.text = distance.ToString() + "m";
    }

    public void SetColor(Color32 color)
    {
        m_Icon.color = color;
    }

    public void SetIconSize(float size)
    {
        m_Icon.rectTransform.sizeDelta = new Vector2(m_Icon.rectTransform.sizeDelta.x * size, m_Icon.rectTransform.sizeDelta.y * size);
    }
    public void SetFontSize(int size)
    {
        m_DistanceText.fontSize = size;
    }

    private Vector2 ToScreenPosition(Vector3 worldPosition)
    {

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        float markerWidth = m_RectTransform.sizeDelta.x;
        float markerHeight = m_RectTransform.sizeDelta.y;

        float minX = markerWidth / 2;
        float maxX = Camera.main.pixelWidth - minX;
        float minY = markerHeight / 2;
        float maxY = Camera.main.pixelHeight - minY;

        if (Vector3.Dot((worldPosition - GameManager.Instance.PlayerObject.transform.position), Camera.main.transform.forward) <= 0)
        {
            // Check if the target is on the left side of the screen
            if (screenPosition.x < Screen.width / 2)
            {
                // Place it on the right (Since it's behind the player, it's the opposite)
                screenPosition.x = maxX;
            }
            else
            {
                // Place it on the left side
                screenPosition.x = minX;
            }
            if (screenPosition.y < Screen.height / 2)
            {
                // Place it on the right (Since it's behind the player, it's the opposite)
                screenPosition.y = maxY;
            }
            else
            {
                // Place it on the left side
                screenPosition.y = minY;
            }
        }

        screenPosition.x = Mathf.Clamp(screenPosition.x, minX, maxX);
        screenPosition.y = Mathf.Clamp(screenPosition.y, minY, maxY);
        return screenPosition;
    }
}
