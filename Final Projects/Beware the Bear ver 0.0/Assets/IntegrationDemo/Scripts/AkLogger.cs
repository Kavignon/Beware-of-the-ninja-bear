#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

class GlobalMessage
{
	public static Dictionary<string, string> ms_generalMessageDict = new Dictionary<string, string>()
	{
		{"Error_NullObj", "Found null object. Abort."},
		{"Error_EngineNotInit", "Sound engine is not initialized."},
		{"Prog_LoadScene", "Load Scene: "},
		{"Prog_Awake", "Awake."},
		{"Prog_OnDestroy", "OnDestroy."},
		{"Prog_OnAppQuit", "On Application Quit."},
		{"Prog_Term", "Termination."},
	};
}

public class AkLogger
{
	private static string ms_pluginName = "WwiseUnity";
	private string m_contextName = "UndefinedContext";

	private Dictionary<string, string> m_messageDict = new Dictionary<string, string>();

	public AkLogger(string contextName)
	{
		m_contextName = contextName;

		m_messageDict = GlobalMessage.ms_generalMessageDict;

	}

	public AkLogger(string contextName, Dictionary<string, string> contextMessageDict) 
		: this(contextName)
	{
		foreach(var pair in contextMessageDict)
		{
			if ( ! m_messageDict.ContainsKey(pair.Key) )
			{
				m_messageDict.Add(pair.Key, pair.Value);
			}
		}

	}

	public void Info(string key, string userMessage = "")
	{
		Debug.Log(FormatMessage(key) + userMessage);
	}

	public void Warning(string key, string userMessage = "")
	{
		Debug.LogWarning(FormatMessage(key) + userMessage);
	}

	public void Error(string key, string userMessage = "")
	{
		Debug.LogError(FormatMessage(key) + userMessage);
	}

	private string FormatMessage(string key)
	{
		if( ! m_messageDict.ContainsKey(key) )
		{
			return "AkLogger: Undefined Message for key: " + key;
		}

		return string.Format("{0}: {1}: {2}", ms_pluginName, m_contextName, m_messageDict[key]);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms