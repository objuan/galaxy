using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public  static class VisualElementEx
{
    public static void SetWidth(this VisualElement ele, float width)
    {
        ele.style.minWidth = ele.style.maxWidth = width;  
    }
    public static void SetHeight(this VisualElement ele, float height)
    {
        ele.style.minHeight = ele.style.maxHeight = height;
    }
    public static void SetHeightPec(this VisualElement ele, float height)
    {
        ele.style.minHeight = ele.style.maxHeight = new StyleLength(new Length(height, LengthUnit.Percent));
    }
    public static void SetSize(this VisualElement ele,  float width,float height)
    {
        ele.SetWidth(width);
        ele.SetHeight(height);
    }
    public static void SetVisible(this VisualElement ele, bool visible)
    {
        ele.style.visibility = visible?  Visibility.Visible : Visibility.Hidden;
        ele.style.display =  visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
    public static void SetBorder(this VisualElement ele, Color color, float width, float ray)
    {
        /*
         *   border-left-color: rgb(255, 250, 250);
      border-right-color: rgb(255, 250, 250);
      border-top-color: rgb(255, 250, 250);
      border-bottom-color: rgb(255, 250, 250);
      border-top-width: 1px;
      border-right-width: 1px;
      border-bottom-width: 1px;
      border-left-width: 1px;
      border-top-left-radius: 5px;
      border-top-right-radius: 5px;
      border-bottom-right-radius: 5px;
      border-bottom-left-radius: 5px;
        */
        ele.style.borderBottomColor = color;
        ele.style.borderTopColor = color;
        ele.style.borderLeftColor = color;
        ele.style.borderRightColor = color;
        ele.style.borderBottomLeftRadius = ray;
        ele.style.borderBottomRightRadius = ray;
        ele.style.borderTopLeftRadius = ray;
        ele.style.borderTopRightRadius = ray;
        ele.style.borderRightWidth = width;
        ele.style.borderLeftWidth = width;
        ele.style.borderTopWidth = width;
        ele.style.borderBottomWidth = width;
    }
}
