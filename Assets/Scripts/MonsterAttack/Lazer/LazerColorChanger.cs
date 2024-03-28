using System.Collections.Generic;
using UnityEngine;

public class LazerColorChanger : MonoBehaviour
{
    public List<LineRenderer> lineRenderers;
    public List<ParticleSystem> particleSystems;

    public void SetColor(Color color)
    {
        foreach (LineRenderer lr in lineRenderers) { lr.startColor = color; lr.endColor = color; }
        foreach (ParticleSystem ps in particleSystems) { ps.startColor = color; }
    }
}
