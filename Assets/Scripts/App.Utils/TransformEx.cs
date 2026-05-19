using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//namespace Assets.Scripts.Utils

public class RectPlane
{
    public Vector3 Origin;
    public Vector3 LocalOrigin;
    public Vector3 X;
    public Vector3 Y;
    public Vector3 Z;
    public Vector3[] Bound;
}

static public class RectTransformExt
{
    /// <summary>
    /// Converts RectTransform.rect's local coordinates to world space
    /// Usage example RectTransformExt.GetWorldRect(myRect, Vector2.one);
    /// </summary>
    /// <returns>The world rect.</returns>
    /// <param name="rt">RectangleTransform we want to convert to world coordinates.</param>
    /// <param name="scale">Optional scale pulled from the CanvasScaler. Default to using Vector2.one.</param>
    static public Rect GetWorldRect(this RectTransform rt, Vector2 scale)
    {
        // Convert the rectangle to world corners and grab the top left
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        Vector3 topLeft = corners[0];

        // Rescale the size appropriately based on the current Canvas scale
        Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);

        return new Rect(topLeft, scaledSize);
    }

    public static Vector2 WorldToCanvas(this RectTransform canv, Vector3 pos)
    {

        //Please note that this will only work if the element you're positioning is either a direct child of the main canvas or the child of a transform that stretches to the full extents of the canvas. Otherwise you have to take the offsets of all parent RectTransforms into account.
        if (Camera.main == null)
            return Vector2.zero;
        //This part is a bit complicated, but you have to make sure that you adjust your coordinates by the size of the canvas.
        Vector2 cPos = new Vector2(((Camera.main.WorldToViewportPoint(pos).x * canv.sizeDelta.x) - (canv.sizeDelta.x * .5f)), ((Camera.main.WorldToViewportPoint(pos).y * canv.sizeDelta.y) - (canv.sizeDelta.y * .5f)));
        return cPos;
    }

    public static RectPlane GetPlaneWorld(this RectTransform rect)
    {
        RectPlane plane = new RectPlane();
        plane.Bound = new Vector3[4];
        rect.GetWorldCorners(plane.Bound);
        Vector3 size = plane.Bound[2] - plane.Bound[0];
        plane.Origin = plane.Bound[0];
        plane.LocalOrigin = -size / 2;
        plane.Y = (plane.Bound[1] - plane.Bound[0]).normalized;
        plane.X = (plane.Bound[3] - plane.Bound[0]).normalized;
        plane.Z = Vector3.Cross(plane.Y, plane.X).normalized;

        return plane;
    }

    public static Rect ToScreenSpace(this RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
        rect.x -= (transform.pivot.x * size.x);
        rect.y -= ((1.0f - transform.pivot.y) * size.y);
        return rect;
    }
}

public static class MatrixExtensions
{
    public static Quaternion ExtractRotation(this Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    public static Vector3 ExtractPosition(this Matrix4x4 matrix)
    {
        Vector3 position;
        position.x = matrix.m03;
        position.y = matrix.m13;
        position.z = matrix.m23;
        return position;
    }

    public static Vector3 ExtractScale(this Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }
}


public static class TransformEx
	{
		public static void FromMatrix(this Transform transform, Matrix4x4 matrix)
		{
			transform.localScale = matrix.ExtractScale();
			transform.rotation = matrix.ExtractRotation();
			transform.position = matrix.ExtractPosition();
		}

	public static Transform Clear(this Transform transform)
		{
			int childs = transform.childCount;
			for (int i = childs - 1; i >= 0; i--)
			{
				GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
			}
			return transform;
		}

		public static void ZeroLocal(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}

		public static Transform FindInChildren(this Transform transform,string name)
		{
			var f = transform.Find(name);
			if (f != null) return f;
			else
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					var t = transform.GetChild(i).FindInChildren(name);
					if (t != null) return t;
				}
			}
			return null;
		}
	}

public static class GameObjectEx
{
    public static bool IsOrChildOf(this GameObject go, GameObject who)
    {
        GameObject g = go;
        while (g != who && g != null)
            g = g.transform.parent.gameObject;
        return g != null;
    }
    public static bool IsChildOf(this GameObject go, GameObject who)
    {
        GameObject g = go.transform.parent.gameObject;
        while (g != who && g != null)
            g = g.transform.parent.gameObject;
        return g != null;
    }

}

