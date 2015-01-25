#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System;

public class SubtitleMarkerDemo : MonoBehaviour {
	private const string m_BankName = "MarkerTest.bnk";

	private uint m_PlayingID = 0;
	private uint m_currentLineIndex = 0;
	private uint m_previousLineIndex = 999;

	private static string[] ms_subtitles = new string[]
	{
		"",
		"In this tutorial...",
		"...we will look at creating...",
		"...actor-mixers...",
		"...and control buses.",
		"We will also look at the...",
		"...actor-mixer and master-mixer structures...",
		"...and how to manage these structures efficiently."
	};

#if ! UNITY_METRO
	private AkLogger m_logger = new AkLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name);
#else
	private AkLogger m_logger = new AkLogger("SubtitleMarkerDemo");
#endif // #if ! UNITY_METRO

	private Rect m_screenInfoRect = new Rect(0, 0, 0, 0);
	
	void Awake()
	{
		InitGUI();
	}

	void InitGUI()
	{
		// Place info below the help text
		GameObject helpObj = GameObject.Find("DemoHelp2");
		Vector3 prevLinePos = Camera.main.ViewportToScreenPoint(helpObj.transform.position);

		int left = (int)prevLinePos.x;
		int top = (int)prevLinePos.y + 30;
		int width = 1000;
		int height = 100;

		m_screenInfoRect.Set(left, top, width, height);
	}

	// Use this for initialization
	void Start () {
		if ( ! AkSoundEngine.IsInitialized() )
		{
			m_logger.Error("Error_EngineNotInit");
			return;
		}

		uint bankID; // Not used
		AkSoundEngine.LoadBank( m_BankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID );

		m_PlayingID = AkSoundEngine.PostEvent( 
			AK.EVENTS.PLAY_MARKERS_TEST,
			gameObject,
			(uint)AkCallbackType.AK_EndOfEvent | (uint)AkCallbackType.AK_Marker | (uint)AkCallbackType.AK_EnableGetSourcePlayPosition,
			MarkersCallback,
			this
			);

	}

	bool IsPlaying() { return m_PlayingID != 0; }

	void OnDestroy()
	{
		if ( AkSoundEngine.IsInitialized() )
		{
			Term();
		}
	}

	private void Term()
	{
		if (IsPlaying())
		{
			AkSoundEngine.CancelEventCallback(m_PlayingID);
			AkSoundEngine.StopPlayingID(m_PlayingID);
		}

		IntPtr in_pInMemoryBankPtr = IntPtr.Zero;
		AkSoundEngine.UnloadBank(m_BankName, in_pInMemoryBankPtr);

		m_PlayingID = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if ( ! IsPlaying() )
			return;

		int narrationPosMs = 0;
		AkSoundEngine.GetSourcePlayPosition( m_PlayingID, out narrationPosMs );

		if(m_currentLineIndex != m_previousLineIndex && m_currentLineIndex > 0)
		{	
			string msg = string.Format("Time (ms): {0}, Subtitle: {1}", narrationPosMs, ms_subtitles[m_currentLineIndex]);
			Debug.Log(msg);

			m_previousLineIndex = m_currentLineIndex;
		}
	}

	static private void MarkersCallback(object in_cookie, AkCallbackType in_type, object in_callbackInfo)
	{
		SubtitleMarkerDemo demo = (SubtitleMarkerDemo)in_cookie;

		if( demo == null )
			return;

		if ( in_type == AkCallbackType.AK_Marker )
		{
			AkCallbackManager.AkMarkerCallbackInfo pMarkerCallbackInfo = (AkCallbackManager.AkMarkerCallbackInfo)in_callbackInfo;

			demo.m_currentLineIndex = pMarkerCallbackInfo.uIdentifier;
			if ( demo.m_currentLineIndex >= SubtitleMarkerDemo.ms_subtitles.Length )
			{	
				demo.m_currentLineIndex = 0;
			}
		}
		else if ( in_type == AkCallbackType.AK_EndOfEvent )
		{
			demo.m_currentLineIndex = 0;
		}
		else
		{
			Debug.Log( "Unsupported event occurred" );
		}
	}

	void OnGUI()
	{
		if( IsPlaying() )
		{
			string subtitle = string.Format("{0}", ms_subtitles[m_currentLineIndex]);

			GUI.Label(m_screenInfoRect, subtitle);
		}
	}

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms