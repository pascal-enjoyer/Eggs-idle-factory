using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;

    private void OnEnable()
    {
        EffectManager.RegisterEffect();
        Invoke(nameof(Deactivate), lifetime);
    }

    private void Deactivate()
    {
        EffectManager.UnregisterEffect();
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}