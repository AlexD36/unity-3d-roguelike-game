using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDamageScript : MonoBehaviour
{
    private List<Material[]> originalMaterials = new List<Material[]>();
    private List<Color[]> originalColors = new List<Color[]>();
    private Coroutine changeCoroutine;
    private Coroutine revertCoroutine;

    protected virtual void Start()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            originalMaterials.Add(renderer.materials);
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            originalColors.Add(colors.ToArray());
        }
    }

    public void TriggerMaterialChange()
    {
        if (changeCoroutine != null)
        {
            StopCoroutine(changeCoroutine);
        }

        if (revertCoroutine != null)
        {
            StopCoroutine(revertCoroutine);
        }
        changeCoroutine = StartCoroutine(ChangeMaterialsTemporarily());
    }

    private IEnumerator ChangeMaterialsTemporarily()
    {
        ChangeMaterialsToColor(new Color(2, 2, 2)); // 255, 255, 255
        yield return new WaitForSeconds(0.2f);
        revertCoroutine = StartCoroutine(RevertMaterialsSmoothly(1.0f));
    }

    private void ChangeMaterialsToColor(Color color)
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                // Create a new temporary material instance
                newMaterials[i] = new Material(renderer.materials[i]);
                if (newMaterials[i].HasProperty("_Color"))
                {
                    newMaterials[i].color = color;
                }
            }
            renderer.materials = newMaterials;
        }
    }

    private IEnumerator RevertMaterialsSmoothly(float duration)
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

        List<Color[]> currentColors = new List<Color[]>();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            List<Color> colors = new List<Color>();
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                if (renderer.materials[i].HasProperty("_Color"))
                {
                    colors.Add(renderer.materials[i].color);
                }
                else
                {
                    colors.Add(Color.black); // Dummy color for materials without _Color property
                }
            }
            currentColors.Add(colors.ToArray());
        }

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                for (int j = 0; j < meshRenderers[i].materials.Length; j++)
                {
                    if (meshRenderers[i].materials[j].HasProperty("_Color"))
                    {
                        meshRenderers[i].materials[j].color = Color.Lerp(Color.white, originalColors[i][j], t);
                    }
                }
            }

            yield return null;
        }

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].materials = originalMaterials[i];
        }
    }
}
