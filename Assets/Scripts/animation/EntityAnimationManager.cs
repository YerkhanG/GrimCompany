using System;
using System.Collections;
using events;
using UnityEngine;

namespace animation
{
    public class EntityAnimationManager: MonoBehaviour
    {
        [Header("Death animations")]
        [SerializeField] private float deathFadeDuration = 0.5f;

        public void OnEnable()
        {
            CombatEvents.OnEntityDeathAnimation += HandleDeathAnimation;
        }

        public void OnDisable()
        {
            CombatEvents.OnEntityDeathAnimation -= HandleDeathAnimation;
        }
        private void HandleDeathAnimation(GameObject deadEntity)
        {
            SpriteRenderer spriteRenderer = deadEntity.GetComponentInChildren<SpriteRenderer>();
            StartCoroutine(PlayDeathAnimation(spriteRenderer, deadEntity));
        }

        private IEnumerator PlayDeathAnimation(SpriteRenderer spriteRenderer, GameObject entityTransform)
        {
            if (spriteRenderer)
            {
                float elapsedTime = 0f;
                Color originalColor = spriteRenderer.color;
                while (elapsedTime < deathFadeDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float alpha = Mathf.Lerp(originalColor.a, 0, elapsedTime / deathFadeDuration);
                    spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(deathFadeDuration);
            }
            entityTransform.gameObject.SetActive(false);
        }
    }
}