using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoSingleton<ColorManager>
{
    [SerializeField] Color _normalColor, _fireColor, _waterColor, _grassColor, _groundColor, _flyColor, _electricColor, _lightColor, _darkColor;
    [SerializeField] Color _healColor, _manaColor, _catchColor, _statusColor;

    public Color GetTypeColor(TYPE type)
    {
        Color colorToReturn = new Color();

        switch (type)
        {
            case TYPE.NORMAL:
                colorToReturn = _normalColor;
                break;
            case TYPE.FIRE:
                colorToReturn = _fireColor;
                break;
            case TYPE.WATER:
                colorToReturn = _waterColor;
                break;
            case TYPE.GRASS:
                colorToReturn = _grassColor;
                break;
            case TYPE.GROUND:
                colorToReturn = _groundColor;
                break;
            case TYPE.FLY:
                colorToReturn = _flyColor;
                break;
            case TYPE.ELECTRIC:
                colorToReturn = _electricColor;
                break;
            case TYPE.LIGHT:
                colorToReturn = _lightColor;
                break;
            case TYPE.DARK:
                colorToReturn = _darkColor;
                break;
        }

        return colorToReturn;
    }

    public Color GetItemColor(ItemBehaviour behaviour)
    {
        Color colorToReturn = new Color();

        switch (behaviour)
        {
            case ItemBehaviour.HEAL:
                colorToReturn = _healColor;
                break;
            case ItemBehaviour.MANA:
                colorToReturn = _manaColor;
                break;
            case ItemBehaviour.STATUS:
                colorToReturn = _statusColor;
                break;
            case ItemBehaviour.CATCH:
                colorToReturn = _catchColor;
                break;
        }

        return colorToReturn;
    }
}
