using UnityEngine;

public class GranularController : MonoBehaviour
{
    public ComputeShader computeShader;

    public int particleCount = 16384;

    ComputeBuffer particleBuffer;
    int kernel;

    void Start()
    {
        if (computeShader == null)
        {
            Debug.LogError("Assign SandCompute first.");
            enabled = false;
            return;
        }

        kernel = computeShader.FindKernel("UpdateParticles");

        // Buffer layout must match the HLSL Particle struct.
        particleBuffer = new ComputeBuffer(
            particleCount, 24
        );

        computeShader.SetBuffer(
            kernel, "particles", particleBuffer
        );
    }

    void Update()
    {
        computeShader.Dispatch(
            kernel,
            Mathf.CeilToInt(particleCount / 64f),
            1, 1
        );
    }

    void OnDestroy()
    {
        particleBuffer?.Release();
    }
}