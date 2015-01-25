#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
using UnityEngine;
using System.Collections.Generic;

public class PositioningDemo_Play : MonoBehaviour {
	
	private uint m_PlayingID = 0;
	private bool m_isHighlightPressedKey = false;
	
	private KeyCode m_button = KeyCode.Space;
	private Dictionary<string, KeyCode> m_buttonDict = new Dictionary<string, KeyCode>()
	{
#if (UNITY_XBOX360 || UNITY_PS3)  && ! UNITY_EDITOR
		{"FrontLeft", KeyCode.JoystickButton0},
		{"FrontRight", KeyCode.JoystickButton1},
		{"FrontCenter", KeyCode.JoystickButton2},
		{"RearLeft", KeyCode.JoystickButton3},
		{"RearRight", KeyCode.JoystickButton4},
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_METRO || UNITY_EDITOR
		{"FrontLeft", KeyCode.Q},
		{"FrontRight", KeyCode.E},
		{"FrontCenter", KeyCode.W},
		{"RearLeft", KeyCode.Z},
		{"RearRight", KeyCode.C},
#endif // #if UNITY_XBOX360 && ! UNITY_EDITOR
	};
	

	// Use this for initialization
	void Start () {
		m_button = m_buttonDict[gameObject.name];
		
		ObjectFacesListener();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(m_button))
		{
			if ( ! IsPlaying()) 
			{
				m_PlayingID = AkSoundEngine.PostEvent( AK.EVENTS.PLAY_POSITIONING_DEMO, gameObject);
				m_isHighlightPressedKey = true;
			}
			else
			{
				AkSoundEngine.StopPlayingID(m_PlayingID);
				m_PlayingID = 0;
				m_isHighlightPressedKey = false;
			}
			
		}
	}
	
	bool IsPlaying() { return m_PlayingID != 0; }
	
	void OnDestroy()
	{
		if (IsPlaying())
		{
			AkSoundEngine.CancelEventCallback(m_PlayingID);
			AkSoundEngine.StopPlayingID(m_PlayingID);
		}
	}
	
	void OnGUI() 
	{
		ShowKeyPressed();
		ShowPosition();
	}
	
	void ShowKeyPressed()
	{
		Rect nextLine = new Rect((uint)(gameObject.transform.position.x*Screen.width), (uint)((1-gameObject.transform.position.y)*Screen.height+20), 100, 50);
		if (m_isHighlightPressedKey)
		{
			GUI.contentColor = Color.red;
		}
		else
		{
			GUI.contentColor = Color.white;
		}
		GUI.Label(nextLine, string.Format("Press {0}", m_button.ToString()));
		
	}
	
	private void ShowPosition()
	{
		Rect nextLine = new Rect((uint)(gameObject.transform.position.x*Screen.width), (uint)((1-gameObject.transform.position.y)*Screen.height+30), 200, 50);
		GUI.Label(nextLine, string.Format("Pos: {0}\nFront: {1}", gameObject.transform.position.ToString(), gameObject.transform.forward.ToString()));
		
		Component listener = gameObject.GetComponent("AkListener");
		if (listener != null)
		{
			nextLine = new Rect(nextLine.x, nextLine.y+20, 200, 50);
			GUI.Label(nextLine, string.Format("Listener Pos: " + listener.transform.position.ToString()));
		}
	}
	
	// All sound objects stand perpendicular to the X-Y plane in the Wwise coordinate system, facing the listener front.
	private void ObjectFacesListener()
	{
		if (gameObject.name == "FrontLeft" || gameObject.name == "FrontCenter" || gameObject.name == "FrontRight")
		{
			gameObject.transform.forward = new Vector3(0.0f, -1.0f, 0.0f);
		}
		else if (gameObject.name == "RearLeft" || gameObject.name == "RearRight")
		{
			gameObject.transform.forward = new Vector3(0.0f, 1.0f, 0.0f);
		}
		else if (gameObject.name == "Main Camera")
		{
			gameObject.transform.forward = new Vector3(0.0f, 0.0f, 0.0f);
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms