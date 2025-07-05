using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UI_ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI tmpText;
    private bool isHovered = false;
    private Coroutine bounceRoutine;

    private void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        tmpText.ForceMeshUpdate(); // Ensure mesh is ready
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        bounceRoutine = StartCoroutine(BounceText());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (bounceRoutine != null)
            StopCoroutine(bounceRoutine);

        tmpText.ForceMeshUpdate();
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    private IEnumerator BounceText()
    {
        TMP_TextInfo textInfo = tmpText.textInfo;
        float time = 0f;

        tmpText.ForceMeshUpdate();

        while (time < 0.6f) // duration of bounce
        {
            tmpText.ForceMeshUpdate();
            textInfo = tmpText.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 offset = new Vector3(0, Mathf.Sin((time * 20f) - i * 0.2f) * 5f, 0);
                for (int j = 0; j < 4; j++)
                    vertices[vertexIndex + j] += offset;
            }

            // Push the modified vertex data back to TMP
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                tmpText.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // Reset vertices
        tmpText.ForceMeshUpdate();
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
}
