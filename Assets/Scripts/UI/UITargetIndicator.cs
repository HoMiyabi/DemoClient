using Kirara.Indicator;
using Kirara.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 目标指示器
/// </summary>
public class UITargetIndicator : MonoBehaviour
{
    private RectTransform RotTra;
    private Image ArrowImg;
    private TextMeshProUGUI DistanceText;

    private void InitUI()
    {
        var c = GetComponent<KiraraRuntimeComponents>();
        c.Init();
        RotTra = c.dict["RotTra"] as RectTransform;
        ArrowImg = c.dict["ArrowImg"] as Image;
        DistanceText = c.dict["DistanceText"] as TextMeshProUGUI;
    }

    private void Awake()
    {
        InitUI();
    }

    // 视口空间圆半径
    [Range(0f, 0.5f), SerializeField]
    private float radius = 0.375f;

    private Transform cur;

    private bool usePos;
    private Transform targetTra;
    private Vector3 targetPos;

    private Vector3 showOffset;
    private bool hideInCircle;

    public UITargetIndicator Set(Transform cur, Transform target, Vector3 showOffset = default, bool hideInCircle = false)
    {
        this.cur = cur;
        usePos = false;
        targetTra = target;
        this.showOffset = showOffset;
        this.hideInCircle = hideInCircle;
        return this;
    }

    public UITargetIndicator Set(Transform cur, Vector3 target, Vector3 showOffset = default, bool hideInCircle = false)
    {
        this.cur = cur;
        usePos = true;
        targetPos = target;
        this.showOffset = showOffset;
        this.hideInCircle = hideInCircle;
        return this;
    }

    private void UpdateShowAndPos(Vector3 targetShowWorldPos)
    {
        var pos = Camera.main.WorldToViewportPoint(targetShowWorldPos);
        var posFromCenter = new Vector2(pos.x - 0.5f, pos.y - 0.5f);

        if (hideInCircle && pos.z > 0f && posFromCenter.sqrMagnitude < radius * radius)
        {
            // 在摄像机前方，且在视口空间圆内
            transform.localScale = Vector3.zero;
            return;
        }
        transform.localScale = Vector3.one;

        if (pos.z <= 0f)
        {
            // z为负，坐标似乎反转了
            posFromCenter = -posFromCenter;
        }

        bool showArrow;
        if (pos.z > 0f && posFromCenter.sqrMagnitude < radius * radius)
        {
            // 在摄像机前方，且在视口圆内
            showArrow = false;
        }
        else
        {
            showArrow = true;
            // 移动到视口圆上
            posFromCenter = posFromCenter.normalized * radius;
        }
        ArrowImg.gameObject.SetActive(showArrow);

        // Debug.Log($"视口{pos} 中心{centerPos} 圆上{circlePos}");
        var posVP = new Vector2(posFromCenter.x + 0.5f, posFromCenter.y + 0.5f);
        var screenPoint = Camera.main.ViewportToScreenPoint(posVP);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent as RectTransform,
            screenPoint, null, out var localPoint);
        transform.localPosition = localPoint;

        var dirFromScreenCenter = (posFromCenter * new Vector2(Screen.width, Screen.height)).normalized;
        RotTra.rotation = Quaternion.LookRotation(Vector3.forward, dirFromScreenCenter);
        DistanceText.transform.rotation = Quaternion.identity;
    }

    private void UpdateTextDistance(Vector3 targetWorldPos)
    {
        float dist = Vector3.Distance(cur.position, targetWorldPos);
        DistanceText.text = $"{dist:F1}m";
    }

    private void Update()
    {
        if (!usePos && targetTra == null)
        {
            Debug.Log("目标为null");
            IndicatorSystem.RemoveIndicator(this);
            return;
        }
        Vector3 targetShowWorldPos;
        Vector3 targetWorldPos;
        if (usePos)
        {
            targetShowWorldPos = targetPos + showOffset;
            targetWorldPos = targetPos;
        }
        else
        {
            targetShowWorldPos = targetTra.TransformPoint(showOffset);
            targetWorldPos = targetTra.position;
        }
        UpdateShowAndPos(targetShowWorldPos);
        UpdateTextDistance(targetWorldPos);
    }
}