#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

public class InteractiveMusicSwitch : MonoBehaviour {
	private Dictionary<string, uint> m_eventDict = new Dictionary<string, uint>()
	{
		{"Explore", AK.EVENTS.IM_EXPLORE},
		{"BeginCommunication", AK.EVENTS.IM_COMMUNICATION_BEGIN},
		{"TheyAreHostile", AK.EVENTS.IM_THEYAREHOSTILE},
		{"FightOneEnemy", AK.EVENTS.IM_1_ONE_ENEMY_WANTS_TO_FIGHT},
		{"FightTwoEnemies", AK.EVENTS.IM_2_TWO_ENEMIES_WANT_TO_FIGHT},
		{"SurroundedByEnemies", AK.EVENTS.IM_3_SURRONDED_BY_ENEMIES},
		{"DeathIsComing", AK.EVENTS.IM_4_DEATH_IS_COMING},
		{"GameOver", AK.EVENTS.IM_GAMEOVER},
		{"WinTheFight", AK.EVENTS.IM_WINTHEFIGHT},
		{"Start", AK.EVENTS.IM_START},
	};

	private static string ms_nextMusicThemeName = "Start";

	static private Dictionary<string, string> ms_messageDict = new Dictionary<string, string>()
	{
		{"Prog_SwitchMusic", "Transitioning to next music: "}
	};

#if ! UNITY_METRO
	protected AkLogger m_logger = new AkLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, ms_messageDict);
#else
	protected AkLogger m_logger = new AkLogger("DynamicDialogueDemo", ms_messageDict);
#endif // #if ! UNITY_METRO

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown()
	{
		bool isTextHit = gameObject.guiText.HitTest(Input.mousePosition);
		if (isTextHit)
		{
			SwitchMusicTheme();
		}
	}
	
	public static string GetNextMusicThemeName() { return ms_nextMusicThemeName; }

	private void SwitchMusicTheme()
	{
		if( ! AkSoundEngine.IsInitialized() )
		{
			m_logger.Error("Error_EngineNotInit");
			return;
		}

		ms_nextMusicThemeName = gameObject.name;
		m_logger.Info("Prog_SwitchMusic", ms_nextMusicThemeName);
		uint musicEvent = m_eventDict[ms_nextMusicThemeName]; 
		AkSoundEngine.PostEvent( musicEvent, gameObject );
	}

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms