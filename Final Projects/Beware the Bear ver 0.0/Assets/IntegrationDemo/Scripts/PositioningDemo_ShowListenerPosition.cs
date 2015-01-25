#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
using UnityEngine;
using System.Collections;

public class PositioningDemo_ShowListenerPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI() 
	{
		Show();
	}
	
	private void Show()
	{
		Rect nextLine = new Rect((uint)(gameObject.transform.position.x*Screen.width), (uint)((1-gameObject.transform.position.y)*Screen.height+30), 200, 50);
		Component listener = gameObject.GetComponent("AkListener");
		if (listener != null)
		{
			nextLine = new Rect(nextLine.x, nextLine.y, 200, 100);
			GUI.Label(nextLine, string.Format("Listener\nPos: {0}\nFront: {1}\nTop: {2}", listener.transform.position.ToString(), listener.transform.forward.ToString(), listener.transform.up.ToString()));
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms