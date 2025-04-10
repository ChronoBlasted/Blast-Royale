using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlastInWorld : MonoBehaviour
{
    [SerializeField] bool _isPlayer;
    public SpriteRenderer BlastRender;
    public PlatformLayout PlatformLayout;


    public void Init(Sprite sprite)
    {
        BlastRender.sprite = sprite;

        string targetLayerName = _isPlayer ? "Player" : "Opponent";

        SortingLayer targetLayer = SortingLayer.layers.FirstOrDefault(l => l.name == targetLayerName);

        BlastRender.sortingLayerID = targetLayer.id;
    }

    void Update()
    {
        var target = _isPlayer ? UIManager.Instance.GameView.PlayerHUD.BlastTransformInUI : UIManager.Instance.GameView.OpponentHUD.BlastTransformInUI;

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane ));

        worldPos.z = 0;

        transform.position = worldPos;
    }
}
