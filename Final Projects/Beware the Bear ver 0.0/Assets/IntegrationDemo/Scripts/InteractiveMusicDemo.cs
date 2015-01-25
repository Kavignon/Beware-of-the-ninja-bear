#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;

public class InteractiveMusicDemo : MonoBehaviour {
	private const string m_BankName = "InteractiveMusic.bnk";

	private uint m_PlayingID = 0;
	AkSegmentInfo m_segmentInfo = null;

#if ! UNITY_METRO
	private AkLogger m_logger = new AkLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name);
#else
	private AkLogger m_logger = new AkLogger("InteractiveMusicDemo");
#endif // #if ! UNITY_METRO

	private Rect m_screenInfoRect = new Rect(0, 0, 0, 0);
	
	void Awake()
	{
		InitGUI();
	}

	void InitGUI()
	{
		// Place info below the help text
		GameObject helpObj = GameObject.Find("DemoHelp3");
		Vector3 prevLinePos = Camera.main.ViewportToScreenPoint(helpObj.transform.position);

		int left = (int)prevLinePos.x;
		int top = (int)prevLinePos.y + 20;
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

		StartMusic();

		m_segmentInfo = new AkSegmentInfo();

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
		if ( IsPlaying() )
		{
			AkSoundEngine.GetPlayingSegmentInfo(m_PlayingID, m_segmentInfo);	
		}
	}

	private void StartMusic()
	{
		m_PlayingID = AkSoundEngine.PostEvent( 
			AK.EVENTS.IM_START,
			gameObject,
			(uint) AkCallbackType.AK_EnableGetMusicPlayPosition
			);
	}

	void OnGUI()
	{
		if ( ! IsPlaying() || m_segmentInfo == null )
		{
			return;
		}

		string nextTheme = string.Format("Next Theme: {0}", InteractiveMusicSwitch.GetNextMusicThemeName());
		string playPos = string.Format("Playback Position (ms): {0}", m_segmentInfo.iCurrentPosition);
		string duration = "Durations(ms):";
		string segment = string.Format("    Segment: {0}", m_segmentInfo.iActiveDuration);
		string preEntry = string.Format("    Pre-Entry: {0}", m_segmentInfo.iPreEntryDuration);
		string postExit = string.Format("    Post-Exit: {0}", m_segmentInfo.iPostExitDuration);
		string[] messages = new string[] { nextTheme, playPos, duration, segment, preEntry, postExit };

		const int yOffset = 20;
		for(int m=0; m<messages.Length; ++m)
		{
			Rect rect = new Rect(m_screenInfoRect.x, m_screenInfoRect.y + m*yOffset, m_screenInfoRect.width, m_screenInfoRect.height);
			GUI.Label(rect, messages[m]);
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms