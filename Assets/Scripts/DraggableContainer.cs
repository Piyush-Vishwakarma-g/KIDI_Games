using UnityEngine;

public class DraggableContainer : MonoBehaviour
{
    public enum ChemicalType { Vinegar, BakingSoda }
    public ChemicalType chemicalType;

    public Transform flaskTarget;
    public float pourDistance = 1.5f;

    private Vector3 startPosition;
    private Vector3 screenPoint;
    private Vector3 offset;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void OnMouseDown()
    {
        if (SafetyManager.Instance != null && (!SafetyManager.Instance.hasGoggles || !SafetyManager.Instance.hasGloves)) return;
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    private void OnMouseDrag()
    {
        if (SafetyManager.Instance != null && (!SafetyManager.Instance.hasGoggles || !SafetyManager.Instance.hasGloves)) return;
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        transform.position = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
    }

    private void OnMouseUp()
    {
        if (flaskTarget == null) return;
        float distance = Vector3.Distance(transform.position, flaskTarget.position);
        if (distance <= pourDistance)
        {
            ChemicalReaction reaction = flaskTarget.GetComponent<ChemicalReaction>();
            if (reaction != null)
            {
                reaction.AddIngredient(chemicalType.ToString());
            }
        }
        else
        {
            transform.position = startPosition;
        }
    }
}