using System;
using System.Collections;
using UnityEngine;

namespace animation
{

    public class AnimationController : MonoBehaviour
    {
        public static AnimationController Instance;
        
        [Header("State")]
        private bool isAnimationPlaying = false;
        [Header("Settings")]
        [SerializeField] private bool debugMode = true;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        public bool IsAnimationPlaying() => isAnimationPlaying;
        

        public void PlayAnimation(Action animationAction, Action onComplete = null)
        {
            if (debugMode)
                Debug.Log($"[AnimationController] Starting animation");
            
            isAnimationPlaying = true;
            
            animationAction?.Invoke();
            
          
            StartCoroutine(WaitForAnimationComplete(onComplete));
        }
        
        private IEnumerator WaitForAnimationComplete(Action onComplete)
        {
           
            yield return new WaitUntil(() => !isAnimationPlaying);
            
            if (debugMode)
                Debug.Log($"[AnimationController] Animation complete, invoking callback");
            
            onComplete?.Invoke();
        }
        
        public void SignalAnimationComplete()
        {
            if (debugMode)
                Debug.Log($"[AnimationController] Animation signaled complete");
            
            isAnimationPlaying = false;
        }
        

        public void ForceUnlock()
        {
            Debug.LogWarning("[AnimationController] Force unlocking animation");
            isAnimationPlaying = false;
        }
    }
}