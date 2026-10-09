using UnityEngine;

public class FingerGuide : MonoBehaviour
{
    public Transform target;
    public Transform fingerVisual;
    public float distanceAhead = 0.45f;
    public float followSpeed = 8f;
    public float pulseSpeed = 4f;
    public float pulseAmount = 0.08f;

    private Vector3 baseScale = Vector3.one;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (fingerVisual != null)
            baseScale = fingerVisual.localScale;
    }

    private void Update()
    {
        if (target == null || fingerVisual == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 desired = target.position - direction * distanceAhead;
        desired.z = -0.3f;

        fingerVisual.position = Vector3.Lerp(
            fingerVisual.position,
            desired,
            followSpeed * Time.deltaTime);

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        fingerVisual.localScale = baseScale * pulse;
    }
}
