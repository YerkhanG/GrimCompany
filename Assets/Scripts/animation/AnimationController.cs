using System;
using System.Collections;
using UnityEngine;

namespace animation
{
    /// <summary>
    /// Manages animation flow and locks combat during animations
    /// </summary>
    public class AnimationController : MonoBehaviour
    {
        public static AnimationController Instance;
        
        [Header("State")]
        private bool isAnimationPlaying = false;
        private Queue animationQueue = new Queue();
        
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
        
        /// <summary>
        /// Plays an animation and invokes callback when complete
        /// </summary>
        public void PlayAnimation(Action animationAction, Action onComplete = null)
        {
            if (debugMode)
                Debug.Log($"[AnimationController] Starting animation");
            
            isAnimationPlaying = true;
            
            animationAction?.Invoke();
            
            // Store callback for when animation completes
            StartCoroutine(WaitForAnimationComplete(onComplete));
        }
        
        private IEnumerator WaitForAnimationComplete(Action onComplete)
        {
            // Wait for animation to signal completion
            yield return new WaitUntil(() => !isAnimationPlaying);
            
            if (debugMode)
                Debug.Log($"[AnimationController] Animation complete, invoking callback");
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// Called by animation events to signal completion
        /// </summary>
        public void SignalAnimationComplete()
        {
            if (debugMode)
                Debug.Log($"[AnimationController] Animation signaled complete");
            
            isAnimationPlaying = false;
        }
        
        /// <summary>
        /// Immediately unlocks animation (use for errors/edge cases)
        /// </summary>
        public void ForceUnlock()
        {
            Debug.LogWarning("[AnimationController] Force unlocking animation");
            isAnimationPlaying = false;
        }
    }
}