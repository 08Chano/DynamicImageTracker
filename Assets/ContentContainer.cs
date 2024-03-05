using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContentContainer : MonoBehaviour
{
    public int ImageCount;
    public List<ContentContainer> ChildContainers = new();
    public TextMeshProUGUI TitleText;
    public RectTransform RectTransform;
    public RectTransform GridView;
    public RectTransform GroupView;
    public GridLayoutGroup GridLayout;

    public int UpdateRectAndImageSizes(int newRectSize, float screenWidth)
    {
        GridLayout.cellSize = Vector2.one * newRectSize;

        int newGridHeight = 300;
        
        if (ChildContainers.Count > 0)
        {
            for (var i = 0; i < ChildContainers.Count; i++)
            {
                newGridHeight += ChildContainers[i].UpdateRectAndImageSizes(newRectSize, screenWidth);
            }
        }

        newGridHeight += UpdateRectSize(newRectSize, screenWidth);

        RectTransform.sizeDelta = new Vector2(UiGenerator.HorizontalRes, newGridHeight);
        return newGridHeight;
    }

    public int UpdateRectSize(int imageSize, float screenWidth)
    {
        var maxColumn = Mathf.CeilToInt(screenWidth / imageSize);
        var ceilToInt = ImageCount % maxColumn == 0
            ? Mathf.CeilToInt(ImageCount / imageSize)
            : Mathf.CeilToInt(ImageCount / imageSize) + 1;
        var gridHeight = 40 + ((imageSize + (int)GridLayout.spacing.y) * ceilToInt);
        GridView.sizeDelta = new Vector2(UiGenerator.HorizontalRes, gridHeight);
        return gridHeight;
    }
}