#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;


public class LocalizationDemo : MonoBehaviour {
	private const string m_BankName = "Human.bnk";

#if ! UNITY_METRO
	private AkLogger m_logger = new AkLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name);
#else
	private AkLogger m_logger = new AkLogger("LocalizationDemo");
#endif // #if ! UNITY_METRO
	
	void Start() {
		if ( ! AkSoundEngine.IsInitialized() )
		{
			m_logger.Error("Error_EngineNotInit");
			return;
		}

		AkSoundEngine.SetCurrentLanguage("English(US)");
		uint bankID;
		AkSoundEngine.LoadBank( m_BankName, BankCallback, (object) this, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID );
	}

	void OnDestroy()
	{
		IntPtr in_pInMemoryBankPtr = IntPtr.Zero;
		AkSoundEngine.UnloadBank(m_BankName, in_pInMemoryBankPtr);
	}

	void OnMouseDown() {
		bool isTextHit = gameObject.guiText.HitTest(Input.mousePosition);
		if (isTextHit)
		{
			PlayHello();
		}
	}

	private void PlayHello() {
		AkSoundEngine.PostEvent("Play_Hello", gameObject);
	}

	public static void BankCallback(uint in_bankID, IntPtr in_InMemoryBankPtr, AKRESULT in_eLoadResult, uint in_memPoolId, object in_pCookie)
	{
		Debug.Log (string.Format("Bank {0} loaded with result {1}.", in_bankID, in_eLoadResult));	
	}	

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms