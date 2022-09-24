using UnityEngine;

namespace DMSH.Effects
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class BasicBlur : MonoBehaviour
    {
        public Material shaderMaterial;
        [Range(0.00f, 10.0f)] public float quality = 4;
        [Range(0.00f, 100.0f)] public float size = 20;
        [Range(0.00f, 100.0f)] public float directions = 16;

        protected void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (shaderMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            shaderMaterial.SetFloat("_Quality", quality);
            shaderMaterial.SetFloat("_Directions", directions);
            shaderMaterial.SetFloat("_Size", size);

            Graphics.Blit(source, destination, shaderMaterial);
        }
    }
}