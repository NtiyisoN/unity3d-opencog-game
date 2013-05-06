
/// Unity3D OpenCog World Embodiment Program
/// Copyright (C) 2013  Novamente			
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#region Usings, Namespaces, and Pragmas

using System.Collections;
using System.Collections.Generic;
using OpenCog.Attributes;
using OpenCog.Extensions;
using OpenCog.Network;
using GUISkin = UnityEngine.GUISkin;
using GUIStyle = UnityEngine.GUIStyle;
using ImplicitFields = ProtoBuf.ImplicitFields;
using ProtoContract = ProtoBuf.ProtoContractAttribute;
using Serializable = System.SerializableAttribute;

//The private field is assigned but its value is never used
#pragma warning disable 0414

#endregion

namespace OpenCog.Embodiment
{

/// <summary>
/// The OpenCog OCFeelingPanel.
/// </summary>
#region Class Attributes

[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
[OCExposePropertyFields]
[Serializable]
	
#endregion
public class OCFeelingPanel : OCMonoBehaviour
{

	//---------------------------------------------------------------------------

	#region Private Member Data

	//---------------------------------------------------------------------------

	private bool _isFeelingTextureMapInitialized = false;

	private bool _isShowingPanel = true;
	// the skin the console will use
	private GUISkin _panelSkin;
	// style for label
	private GUIStyle _boxStyle;
	// A map from feeling names to textures. The texture needs to be created dynamically
	// whenever a new feeling is added.
	private Dictionary<string, UnityEngine.Texture2D> _feelingTextureMap;
	// We need to initialize the feeling to texture map at the first time of obtaining the
	// feeling information.


	private OCConnector _connector;

	private UnityEngine.Rect _panel;

	private UnityEngine.Vector2 _scrollPosition;
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Accessors and Mutators

	//---------------------------------------------------------------------------
		
	/// <summary>
	/// Gets or sets the example variable.  Includes attribute examples.
	/// </summary>
	/// <value>
	/// The example variable.
	/// </value>
	[OCTooltip("I'm an example tooltip!")]
	//creates a tooltip popup in the editor
	[OCIntSlider(0, 100)]//creates a integer slider from 0 to 100 in the editor
	public int ExampleVar
	{
		get{ return _exampleVar; }

		set{ _exampleVar = value; }
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Public Member Functions

	//---------------------------------------------------------------------------

	/// <summary>
	/// Called when the script instance is being loaded.
	/// </summary>
	public void Awake()
	{
		Initialize();
		OCLogger.Fine(gameObject.name + " is awake.");
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	public void Start()
	{
		OCCon = GetComponent<OCConnector>() as OCConnector;
		feelingTextureMap = new Dictionary<string, Texture2D>();

		OCLogger.Fine(gameObject.name + " is started.");
	}

	/// <summary>
	/// Update is called once per frame.
	/// </summary>
	public void Update()
	{
		OCLogger.Fine(gameObject.name + " is updated.");	
	}
		
	/// <summary>
	/// Reset this instance to its default values.
	/// </summary>
	public void Reset()
	{
		Uninitialize();
		Initialize();
		OCLogger.Fine(gameObject.name + " is reset.");	
	}

	/// <summary>
	/// Raises the enable event when OCFeelingPanel is loaded.
	/// </summary>
	public void OnEnable()
	{
		OCLogger.Fine(gameObject.name + " is enabled.");
	}

	/// <summary>
	/// Raises the disable event when OCFeelingPanel goes out of scope.
	/// </summary>
	public void OnDisable()
	{
		OCLogger.Fine(gameObject.name + " is disabled.");
	}

	/// <summary>
	/// Raises the destroy event when OCFeelingPanel is about to be destroyed.
	/// </summary>
	public void OnDestroy()
	{
		Uninitialize();
		OCLogger.Fine(gameObject.name + " is about to be destroyed.");
	}

	public void ShowPanel()
	{
		showPanel = true;
	}

	public void HidePanel()
	{
		showPanel = false;
	}

	public void OnGUI()
	{
		if(panelSkin != null)
		{
			GUI.skin = panelSkin;
		}

		if(showPanel)
		{
			panel = new Rect(Screen.width * 0.65f, Screen.height * 0.7f, Screen.width * 0.35f, Screen.height * 0.3f);
			panel = GUI.Window(2, _panel, FeelingMonitorPanel, gameObject.name + " Feeling Panel");
		}
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Private Member Functions

	//---------------------------------------------------------------------------
	
	/// <summary>
	/// Initializes this instance.  Set default values here.
	/// </summary>
	private void Initialize()
	{
		ExampleVar = 0;
	}
	
	/// <summary>
	/// Uninitializes this instance.  Cleanup refernces here.
	/// </summary>
	private void Uninitialize()
	{
	}

	private void FeelingMonitorPanel(int id)
	{
		Dictionary<string, float> feelingValueMap = _connector.FeelingValueMap;
		if(feelingValueMap.Count == 0)
		{
			return;
		}

		_scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		float feelingBarWidth = Screen.width * 0.3f;

		_boxStyle = _panelSkin.box;
		lock(feelingValueMap)
		{
			int topOffset = 5;
			foreach(string feeling in feelingValueMap.Keys)
			{
				if(!isFeelingTextureMapInit)
				{
					float r = UnityEngine.Random.value;
					float g = UnityEngine.Random.value;
					float b = UnityEngine.Random.value;
					Color c = new Color(r, g, b, 0.6f);
					Texture2D t = new Texture2D(1, 1);
					t.SetPixel(0, 0, c);
					t.Apply();
					this.feelingTextureMap[feeling] = t;
				}
				float value = feelingValueMap[feeling];

				// Set the texture of background.
				_boxStyle.normal.background = feelingTextureMap[feeling];
				GUILayout.BeginHorizontal();
				GUILayout.Label(feeling + ": ", _panelSkin.label, GUILayout.MaxWidth(_panel.width * 0.3f));
				GUILayout.Box("", boxStyle, GUILayout.Width(feelingBarWidth * value), GUILayout.Height(16));
				GUILayout.EndHorizontal();
				topOffset += 15;
			}
			// We only need to initialize the map at the first time.
			if(!_isFeelingTextureMapInitialized)
			{
				_isFeelingTextureMapInitialized = true;
			}
		}

		GUILayout.EndScrollView();
	}
			
	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

	#region Other Members

	//---------------------------------------------------------------------------		

	/// <summary>
	/// Initializes a new instance of the <see cref="OpenCog.OCFeelingPanel"/> class.  
	/// Generally, intitialization should occur in the Start or Awake
	/// functions, not here.
	/// </summary>
	public OCFeelingPanel()
	{
	}

	//---------------------------------------------------------------------------

	#endregion

	//---------------------------------------------------------------------------

}// class OCFeelingPanel

}// namespace OpenCog




