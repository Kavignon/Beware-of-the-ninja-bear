#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
using UnityEngine;
using System.Collections;
using System;

public class PositioningDemo : MonoBehaviour {
	static public string m_BankName = "Positioning_Demo.bnk";
	
	
	// Use this for initialization
	void Awake () {
		uint bankID; // Not used
		AkSoundEngine.LoadBank( m_BankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID );
		
		ListenerFacesFront();
	}
	
	void OnDestroy()
	{
		IntPtr in_pInMemoryBankPtr = IntPtr.Zero;
		AkSoundEngine.UnloadBank(m_BankName, in_pInMemoryBankPtr);
	}	
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// Listener stands perpendicular to the X-Y plane in the Wwise coordinate system, facing along positive Y.
	private void ListenerFacesFront()
	{
		if (gameObject.name == "Main Camera")
		{
			gameObject.transform.forward = new Vector3(0.0f, 1.0f, 0.0f);
			gameObject.transform.up = new Vector3(0.0f, 0.0f, -1.0f);
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms