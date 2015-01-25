#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;

class RTPCDemo : MonoBehaviour {
	private const string m_BankName = "Car.bnk";
	// private const string m_GameObjName = "Car";
	private uint m_PlayingID = 0;
	private const uint m_DeltaRPM = 30;
	private uint m_rpm = 5000;
	private uint m_RpmMin = 1000;
	private uint m_RpmMax = 10000;

#if ! UNITY_METRO
	private AkLogger m_logger = new AkLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name);
#else
	private AkLogger m_logger = new AkLogger("RTPCDemo");
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
		int top = (int)prevLinePos.y + 50;
		int width = 300;
		int height = 100;

		m_screenInfoRect.Set(left, top, width, height);
	}

	void Start()
	{
		if ( ! AkSoundEngine.IsInitialized() )
		{
			m_logger.Error("Error_EngineNotInit");
			return;
		}

		uint bankID; // Not used
		AkSoundEngine.LoadBank( m_BankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID );

		m_PlayingID = AkSoundEngine.PostEvent( AK.EVENTS.PLAY_ENGINE, gameObject );
	}

	void OnDestroy()
	{
		if ( AkSoundEngine.IsInitialized() )
		{
			Term();
		}
	}

	private void Term()
	{
		AkSoundEngine.StopPlayingID(m_PlayingID);

		IntPtr in_pInMemoryBankPtr = IntPtr.Zero;
		AkSoundEngine.UnloadBank(m_BankName, in_pInMemoryBankPtr);
	}

	void Update()
	{
		bool isHoldingKeyUp = Input.GetKey("up");
		if (isHoldingKeyUp)
		{
			m_rpm += m_DeltaRPM;
			if (m_rpm > m_RpmMax)
				m_rpm = m_RpmMax;
		}

		bool isHoldingKeyDown = Input.GetKey("down");
		if (isHoldingKeyDown)
		{
			m_rpm -= m_DeltaRPM;
			if (m_rpm < m_RpmMin)
				m_rpm = m_RpmMin;
		}

		if ( isHoldingKeyUp || isHoldingKeyDown )
		{
			AkSoundEngine.SetRTPCValue( AK.GAME_PARAMETERS.RPM, (float)m_rpm, gameObject );
		}
	}

	void OnGUI()
	{
		string rpmInfo = string.Format("RPM: {0}", m_rpm);

		GUI.Label(m_screenInfoRect, rpmInfo);
	}

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms