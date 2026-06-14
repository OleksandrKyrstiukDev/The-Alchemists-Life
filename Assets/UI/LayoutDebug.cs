using UnityEngine;
using UnityEngine.UI;

public class LayoutDebug : MonoBehaviour
{
    void Start()
    {
        Debug.Log("FORCE REBUILD LAYOUT");

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            GetComponent<RectTransform>()
        );
    }
}