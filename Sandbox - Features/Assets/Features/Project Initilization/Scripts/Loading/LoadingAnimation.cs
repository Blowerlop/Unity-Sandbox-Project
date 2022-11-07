using System;
using UnityEngine;
using DG.Tweening;

public class LoadingAnimation : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform _loadingImage;
    [SerializeField] private float _duration;
    private Tween _loadingImageTween;
    
    #endregion

    
    
    #region Updates

    private void Start()
    { 
        _loadingImageTween = _loadingImage.DORotate(Vector3.forward * -360, _duration, RotateMode.FastBeyond360)
            .SetLoops(-1)
            .SetRelative(true)
            .SetEase(Ease.Linear); 
    }

    private void OnDestroy()
    {
        _loadingImageTween.Kill();
    }

    #endregion
}
