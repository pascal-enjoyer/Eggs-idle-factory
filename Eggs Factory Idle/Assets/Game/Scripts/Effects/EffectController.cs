using UnityEngine;

public class EffectController : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f; // Длительность эффекта (настройте под префаб)

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