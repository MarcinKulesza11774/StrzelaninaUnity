using UnityEngine;
using System.Collections.Generic;

public class CameraOcclusion : MonoBehaviour
{
    [Header("Referencje")]
    public Transform player;

    [Header("Ustawienia")]
    [Range(0f, 1f)] public float hiddenAlpha = 0.15f;
    [Range(1f, 20f)] public float fadeSpeed = 8f;
    public LayerMask occlusionLayers = ~0;

    private Dictionary<Renderer, Material[]> hiddenRenderers
        = new Dictionary<Renderer, Material[]>();
    private HashSet<Renderer> currentlyHidden = new HashSet<Renderer>();

    void LateUpdate()
    {
        if (player == null) return;

        currentlyHidden.Clear();

        Vector3 direction = transform.position - player.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(player.position, direction.normalized, distance, occlusionLayers);

        foreach (var hit in hits)
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend == null) continue;

            currentlyHidden.Add(rend);

            if (!hiddenRenderers.ContainsKey(rend))
            {
                hiddenRenderers[rend] = rend.materials;
                Material[] transparentMats = new Material[rend.materials.Length];
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    transparentMats[i] = new Material(rend.materials[i]);
                    SetMaterialTransparent(transparentMats[i]);
                }
                rend.materials = transparentMats;
            }

            foreach (var mat in rend.materials)
            {
                Color c = mat.GetColor("_BaseColor");
                c.a = Mathf.Lerp(c.a, hiddenAlpha, fadeSpeed * Time.deltaTime);
                mat.SetColor("_BaseColor", c);
            }
        }

        var toRestore = new List<Renderer>();
        foreach (var kvp in hiddenRenderers)
        {
            if (currentlyHidden.Contains(kvp.Key)) continue;

            bool fullyRestored = true;
            foreach (var mat in kvp.Key.materials)
            {
                Color c = mat.GetColor("_BaseColor");
                c.a = Mathf.Lerp(c.a, 1f, fadeSpeed * Time.deltaTime);
                mat.SetColor("_BaseColor", c);
                if (c.a < 0.99f) fullyRestored = false;
            }

            if (fullyRestored)
            {
                kvp.Key.materials = kvp.Value;
                toRestore.Add(kvp.Key);
            }
        }

        foreach (var rend in toRestore)
            hiddenRenderers.Remove(rend);
    }

    void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Surface", 1f);
        mat.SetFloat("_Blend", 0f);
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    void OnDisable()
    {
        foreach (var kvp in hiddenRenderers)
            if (kvp.Key != null) kvp.Key.materials = kvp.Value;
        hiddenRenderers.Clear();
    }
}