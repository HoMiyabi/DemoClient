using System;
using UnityEngine;

[Serializable]
public class AnimState
{
    public bool isInertia;
    public float _startPos;
    public float _endPos;
    public float _time;
    public bool IsComplete { get; private set; } = true;
    private Action _onComplete;

    public float _duration;

    public float _dampingRatio;
    public bool _outOfContent;

    public void Kill()
    {
        if (!IsComplete)
        {
            IsComplete = true;
            _onComplete?.Invoke();
        }
    }

    public void Set(float startPos, float endPos, float duration, Action onComplete = null)
    {
        isInertia = false;

        _startPos = startPos;
        _endPos = endPos;
        _time = 0f;
        IsComplete = false;
        _onComplete = onComplete;

        _duration = duration;
        _outOfContent = false;
    }

    public void SetInertiaV0DampingRatio(float startPos, float v0, float dampingRatio)
    {
        CalcInertiaEndPos(startPos, out float endPos, v0, dampingRatio);
        SetInertiaDampingRatio(startPos, endPos, dampingRatio);
    }

    public void SetInertiaDampingRatio(float startPos, float endPos, float dampingRatio)
    {
        isInertia = true;
        _startPos = startPos;
        _endPos = endPos;
        _time = 0f;
        IsComplete = false;
        _onComplete = null;
        _dampingRatio = dampingRatio;
        _outOfContent = false;
    }

    public void SetInertia(float startPos, float endPos, float v0, float dampingRatio)
    {
        CalcInertiaDampingRatio(startPos, endPos, v0, out float dr);
        if (dr < dampingRatio / 3f) // 速度太小，位移量太大，速度与位移反向
        {
            // 保持阻尼率不变
            SetInertiaDampingRatio(startPos, endPos, dampingRatio);
        }
        else
        {
            // 保持初速度不变
            SetInertiaDampingRatio(startPos, endPos, dr);
        }
    }

    public float Update(float dt)
    {
        if (IsComplete) return _endPos;

        _time += dt;
        float pos;
        if (isInertia)
        {
            pos = _startPos + (_endPos - _startPos) * (1f - Mathf.Exp(-_dampingRatio * _time));
            if (Mathf.Abs(pos - _endPos) < 1f)
            {
                IsComplete = true;
                _onComplete?.Invoke();
            }
        }
        else
        {
            pos = Mathf.Lerp(_startPos, _endPos, EaseOutCubic(Mathf.Clamp01(_time / _duration)));
            if (_time >= _duration)
            {
                IsComplete = true;
                _onComplete?.Invoke();
            }
        }
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

    // x = v0 / c
    public static void CalcInertiaEndPos(float startPos, out float endPos, float v0, float dampingRatio)
    {
        endPos = startPos + v0 / dampingRatio;
    }

    public static void CalcInertiaDampingRatio(float startPos, float endPos, float v0, out float dampingRatio)
    {
        dampingRatio = v0 / (endPos - startPos);
    }
}