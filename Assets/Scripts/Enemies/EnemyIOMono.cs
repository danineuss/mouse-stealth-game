using System.Linq;
using UnityEngine;

public class EnemyIOMono : MonoBehaviour
{
    [SerializeField] private EventsMono eventsMono = null;
    [SerializeField] private Color kActiveTextColor = new Color(0, 71, 188, 255);
    [SerializeField] private Color kInactiveTextColor = new Color(0, 75, 75, 75);

    public IEnemyIO EnemyIO { get; private set; }

    private TextMesh textMesh;
    private Transform cooldownScaleParent;
    // private OutlineMono enemyOutline;
    
    void Awake()
    {
        textMesh = GetComponentInChildren<TextMesh>();
        cooldownScaleParent = GetComponentsInChildren<Transform>()
                                .Where(x => x.CompareTag("ScaleParent"))
                                .First();
        // enemyOutline = GetComponentInParent<EnemyMono>()
        //                 .GetComponentsInChildren<Transform>()
        //                 .Where(x => x.CompareTag("Model"))
        //                 .First()
        //                 .GetComponent<OutlineMono>();
    }

    void Start()
    {
        EnemyIO = new EnemyIO(
            eventsMono.EnemyEvents,
            textMesh,
            // enemyOutline,
            kInactiveTextColor,
            kActiveTextColor,
            transform,
            cooldownScaleParent
        );
    }

    void Update()
    {
        EnemyIO.Update();
    }

    void OnMouseEnter()
    {
        EnemyIO.OnMouseEnter();
    }

    void OnMouseExit()
    {
        EnemyIO.OnMouseExit();
    }
}