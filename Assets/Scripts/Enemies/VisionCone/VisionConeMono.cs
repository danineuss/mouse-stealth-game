using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionConeMono: MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform player = null;
    [SerializeField] private EventsMono eventsMono = null;
    [SerializeField] private LayerMask obstacleMask = new LayerMask();
    [SerializeField, Range(0.01f, 1.0f)] private float DetectionEscalationSpeed = 0.5f;
    [SerializeField, Range(0.01f, 0.5f)] private float DetectionDeescalationSpeed = 0.2f;

    [Header("Control Points")]
    [SerializeField] private VisionConeControlPointsMono controlPointsMono = null;

    [Header("Visualization")]
    [SerializeField] private float kConeRangeMultiplier = 1.5f;
    [SerializeField] private Material greenMaterial = null;
    [SerializeField] private Material blueMaterial = null;
    [SerializeField] private Color kSpotLightGreen = new Color(0f, 183f, 18f, 1f);
    [SerializeField] private Color kSpotLightOrange = new Color(183f, 102f, 0f, 1f);
    [SerializeField] private Color kSpotLightRed = new Color(191, 0f, 10f, 1f);
    [SerializeField] private Color kSpotLightBlue = new Color(0f, 23f, 183f, 1f);

    public IPlayerDetector PlayerDetector => playerDetector;

    private IVisionConeVM visionConeVM;
    private IPlayerDetector playerDetector;

    void Awake()
    {
        var spotLight = GetComponentInChildren<Light>();
        var coneMeshRenderer = GetComponentInChildren<MeshRenderer>();
        var coneOutline = GetComponentInChildren<OutlineMono>();
        var coneScaleParent = GetComponentsInChildren<Transform>()
                                .Where(x => x.CompareTag("ScaleParent"))
                                .First();
        var coneScaleAnchor = GetComponentsInChildren<Transform>()
                            .Where(x => x.CompareTag("ScaleAnchor"))
                            .First();
        var coneVisualizer = new ConeVisualizer(
            transform,
            coneScaleParent,
            coneScaleAnchor,
            kConeRangeMultiplier,
            coneMeshRenderer,
            greenMaterial,
            blueMaterial,
            coneOutline,
            spotLight,
            kSpotLightGreen,
            kSpotLightOrange,
            kSpotLightRed,
            kSpotLightBlue
        );

        visionConeVM = new VisionConeVM(
            controlPointsMono.PatrolPoints,
            controlPointsMono.DistractPoint,
            coneVisualizer, 
            transform,
            player, 
            obstacleMask,
            eventsMono
        );
        playerDetector = new PlayerDetector(
            visionConeVM,
            eventsMono,
            DetectionEscalationSpeed,
            DetectionDeescalationSpeed
        );
    }

    void Update()
    {
        visionConeVM.Update();
        playerDetector.Update();
    }
}