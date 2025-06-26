using System;
using UnityEngine;

[Serializable]
public struct AnimState
{
    public float startPos;
    public float endPos;
    public float time;
    public float duration;
    public Action onComplete;

    public void Set(float startPos, float endPos, float duration, Action onComplete = null)
    {
        this.startPos = startPos;
        this.endPos = endPos;
        time = 0f;
        this.duration = duration;
        this.onComplete = onComplete;
    }

    public float Update(float pos, float dt, out bool isComplete)
    {
        time += dt;
        if (time >= duration)
        {
            isComplete = true;
            onComplete?.Invoke();
            return endPos;
        }
        pos = Mathf.Lerp(startPos, endPos, EaseOutCubic(Mathf.Clamp01(time / duration)));
        isComplete = false;
        return pos;
    }

    public static float EaseOutCubic(float x)
    {
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    public static float EaseOutExpo(float x)
    {
        return x == 1f ? 1f : 1f - Mathf.Pow(2, -10 * x);
    }
}