#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System;

public class MusicCallbackDemo : MonoBehaviour {
	private const string m_BankName = "MusicCallbacks.bnk";

	private uint m_PlayingID = 0;
	private uint m_beatCount = 0;
	private uint m_barCount = 0;
	private uint m_prevBeatCount = 999;
	private uint m_prevBarCount = 999;

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
		uint bankID; // Not used
			AkSoundEngine.LoadBank( m_BankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID );	
		
			m_PlayingID = AkSoundEngine.PostEvent( 
				AK.EVENTS.PLAYMUSICDEMO1,
				gameObject,
				(uint)AkCallbackType.AK_MusicSyncBeat | (uint)AkCallbackType.AK_MusicSyncBar | (uint)AkCallbackType.AK_MusicSyncEntry | (uint)AkCallbackType.AK_MusicSyncExit | (uint)AkCallbackType.AK_EndOfEvent,
				MusicCallback,
				this
				);
	}

	void OnDestroy()
	{
		if ( AkSoundEngine.IsInitialized() )
		{
			Term();
		}
	}

	bool IsPlaying() { return m_PlayingID != 0; }

	private void Term()
	{
		if ( IsPlaying() )
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
		if( IsPlaying() )
		{
			
			if(m_beatCount != m_prevBeatCount && m_barCount > 0)
			{
				string msg = string.Format("Beat: {0}, Bar: {1}", m_beatCount, m_barCount);
				Debug.Log(msg);
				m_prevBeatCount = m_beatCount;
			}
			if (m_barCount != m_prevBarCount)
				m_prevBarCount = m_barCount;
			
		}

	
	}

	private static void MusicCallback(object in_cookie, AkCallbackType in_type, object in_callbackInfo)
	{
		MusicCallbackDemo demo = (MusicCallbackDemo)in_cookie;

		if (demo == null)
			return;
	
		if ( in_type == AkCallbackType.AK_MusicSyncBar )
		{
				demo.m_beatCount = 0;
				demo.m_barCount++;	
			}

		else if ( in_type == AkCallbackType.AK_MusicSyncBeat )
		{
				demo.m_beatCount++;
			}
		/*
		else if ( in_type == AkCallbackType.AK_MusicSyncEntry )
		{

		}

		else if ( in_type == AkCallbackType.AK_MusicSyncExit )
		{

		}*/
		else if ( in_type == AkCallbackType.AK_EndOfEvent )
		{
			demo.m_beatCount = 0;
			demo.m_barCount = 0;
		}
		
	}

	void OnGUI()
	{
		if(IsPlaying())
		{
			string beatInfo = string.Format("Beat: {0}, Bar: {1}", m_beatCount, m_barCount);

			GUI.Label(m_screenInfoRect, beatInfo);
		}
	}
	
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms