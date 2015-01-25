#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;

public class LocalizationLanguage : MonoBehaviour {
	private const string m_BankName = "Human.bnk";

	private Dictionary<string, string> m_languageDict = new Dictionary<string, string>()
	{
		{"SetLanguageEnglish", "English(US)"},
		{"SetLanguageFrench", "French(Canada)"}
	};

	static private Dictionary<string, string> ms_messageDict = new Dictionary<string, string>()
	{
		{"Prog_SwitchLanguage", "Switch language and reload SoundBank: "}
	};

#if ! UNITY_METRO
	private AkLogger m_logger = new AkLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, ms_messageDict);
#else
	private AkLogger m_logger = new AkLogger("LocalizationLanguage", ms_messageDict);
#endif // #if ! UNITY_METRO

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown() {
		bool isTextHit = gameObject.guiText.HitTest(Input.mousePosition);
		if (isTextHit)
		{
			SwitchLanguage();
		}
	}

	private void SwitchLanguage()
	{
		if ( ! AkSoundEngine.IsInitialized() )
		{
			m_logger.Error("Error_EngineNotInit");
			return;
		}

		string language = m_languageDict[gameObject.name];

		AkSoundEngine.SetCurrentLanguage(language);

		IntPtr in_pInMemoryBankPtr = IntPtr.Zero;
		AkSoundEngine.UnloadBank(m_BankName, in_pInMemoryBankPtr);
		uint bankID;
		AkSoundEngine.LoadBank( m_BankName, LocalizationDemo.BankCallback, (object) this, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID );
		m_logger.Info("Prog_SwitchLanguage", language);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms