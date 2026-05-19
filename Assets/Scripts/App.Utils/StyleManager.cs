using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//namespace Assets.Scripts.Utils

public  class StyleManager
{
	static GUIStyle _normal = null;
	static GUIStyle _panel = null;
	static GUIStyle _bold = null;
	static GUIStyle _html = null;

	static int fontSize = 10;
	static Color fontColor = Color.white;

	public static GUIStyle html
	{
		get
		{
			if (_html == null)
			{
				_html = new GUIStyle("label");
				_html.richText = true;
			}
			return _html;
		}
	}

	public static GUIStyle normal
	{
		get
		{
			if (_normal == null)
			{
				_normal = new GUIStyle("label");
				_normal.normal.textColor = fontColor;
				_normal.fontSize = fontSize;
			}
			return _normal;
		}
	}
	public static GUIStyle bold
	{
		get
		{
			if (_bold == null)
			{
				_bold = new GUIStyle("label");
				_bold.fontStyle = FontStyle.Bold;
				_bold.normal.textColor = fontColor;
				_bold.fontSize = fontSize;
			}
			return _bold;
		}
	}
	public static GUIStyle panel
	{
		get
		{
			if (_panel == null)
			{
				_panel = new GUIStyle("box");
			}
			return _panel;
		}
	}

}

