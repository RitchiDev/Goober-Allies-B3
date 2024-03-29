using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TextPopUp : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;
    [SerializeField] private GameObject m_Parent;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private UnityEvent m_OnEndPositionReached;

    [Header("Position")]
    [SerializeField] private float m_PositionLerpTime = 5f;
    [SerializeField] private Vector3 m_EndPositionOffset = new Vector3(0f, 1f, 0f);

    [Header("Color")]
    [SerializeField] private List<Color> m_Colors = new List<Color>(0);
    [SerializeField] private float m_TimeColorBeforeChange = 0.2f;
    [SerializeField] private float m_ColorLerpTime = 5f;
    private Color m_PreviousColor;

    private void OnEnable()
    {
        m_PreviousColor = m_Text.color;

        StartCoroutine(PopUpTimer());

        if (m_Colors.Count > 1)
        {
            StartCoroutine(ColorChangeTimer());
        }
    }

    public void DestroyParent()
    {
        Destroy(m_Parent);
    }

    public void SetText(string text)
    {
        m_Text.text = text; 
    }

    public void SetColor(Color color)
    {
        m_Colors = new List<Color>();
        m_Colors.Add(color);

        m_Text.color = color;
    }

    public void SetColors(List<Color> colors)
    {
        m_Colors = colors;
    }

    private IEnumerator PopUpTimer()
    {
        float timer = 0;
        Vector3 endPosition = m_Text.transform.localPosition + m_EndPositionOffset;

        while (timer < m_PositionLerpTime)
        {
            timer += Time.deltaTime;

            m_Text.transform.localPosition = Vector3.Lerp(m_Text.transform.localPosition, endPosition, timer / m_PositionLerpTime);

            m_CanvasGroup.alpha = Mathf.Lerp(m_CanvasGroup.alpha, 0f, timer / m_PositionLerpTime);

            yield return null;
        }

        m_Text.transform.localPosition = endPosition;

        yield return new WaitForEndOfFrame();

        m_OnEndPositionReached?.Invoke();
    }

    private IEnumerator ColorChangeTimer()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(m_TimeColorBeforeChange);

            int rng = Random.Range(0, m_Colors.Count);

            int breakout = 0;
            while (m_PreviousColor == m_Colors[rng])
            {
                rng = Random.Range(0, m_Colors.Count);

                breakout++;
                if (breakout >= 20)
                {
                    Debug.LogWarning("Broke out of the loop!");
                    break;
                }
            }

            float timer = 0;
            while (timer < m_ColorLerpTime)
            {
                timer += Time.deltaTime;
                m_Text.color = Color.Lerp(m_PreviousColor, m_Colors[rng], timer / m_ColorLerpTime);

                yield return null;
            }

            m_Text.color = m_Colors[rng];

            m_PreviousColor = m_Text.color;
        }
    }
}
