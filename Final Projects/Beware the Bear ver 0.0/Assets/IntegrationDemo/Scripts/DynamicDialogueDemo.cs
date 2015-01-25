#if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;

public class DynamicDialogueDemo : MonoBehaviour {
	private const string m_BankName = "DynamicDialogue.bnk";

	private bool m_isTestStuiteStarted = false;
	private int m_currentTestIndex = -1;
	private DynamicDialogueTest[] m_testSuite = null;

	static private Dictionary<string, string> ms_messageDict = new Dictionary<string, string>()
	{
		{"Prog_NextTest", "Run Next Test..."},
		{"Prog_AllDone", "All tests are done."}
	};

#if ! UNITY_METRO
	protected AkLogger m_logger = new AkLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, ms_messageDict);
#else
	protected AkLogger m_logger = new AkLogger("DynamicDialogueDemo", ms_messageDict);
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

		CreateTestSuite(gameObject);
		Application.targetFrameRate = 60;
		StartTestSuite();
	}

	void Update() {
		if ( m_isTestStuiteStarted && ! IsCurrentTestInProgressOrWaiting()  )
		{
			// m_logger.Info("Prog_NextTest");
			RunNextTest();
		}
	}

	void OnDestroy()
	{
		// Do nothing. As of Unity3.5.2, OnDestroy() may not to be called in time to close a dynamic sequence when trying to stop the Player, even if the monoBehavior script is set at higher priority than AkGlobalSoundEngineTerminator. To avoid crashes due to this, Term() is called in OnApplicationQuit() instead.
	}

	void OnApplicationQuit()
	{
		if ( AkSoundEngine.IsInitialized() )
		{
			Term();
		}
	}

	private void Term()
	{
		StopAndResetAll();

		IntPtr in_pInMemoryBankPtr = IntPtr.Zero;
		AkSoundEngine.UnloadBank(m_BankName, in_pInMemoryBankPtr);
	}

	private void StopAndResetAll()
	{
		if ( m_testSuite[m_currentTestIndex] != null )
			m_testSuite[m_currentTestIndex].StopAndRemoveAll();
		
		ResetCurrentTest();
		SetTestSuiteDone();
	}

	void OnGUI()
	{
		if(m_isTestStuiteStarted && IsCurrentTestInProgress())
		{
			string testName = string.Format("{0}", m_testSuite[m_currentTestIndex].GetTestName());
			GUI.Label(m_screenInfoRect, testName);
		}
	}
	
	private void CreateTestSuite(GameObject gameObject)
	{
		m_testSuite = new DynamicDialogueTest[] {
			new DynamicDialogueTest_Set1_1_SimpleSequenceUsingID(gameObject),
			new DynamicDialogueTest_Set3_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set3_2_AddItemToPlaylist(gameObject),
			new DynamicDialogueTest_Set4_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set4_2_InsertItemsToPlaylist(gameObject),
			new DynamicDialogueTest_Set5_1_StartEmptyPlaylist(gameObject),
			new DynamicDialogueTest_Set5_2_AddItemsToPlaylist(gameObject),
			new DynamicDialogueTest_Set5_3_WaitForEmptyListThenAdd(gameObject),
			new DynamicDialogueTest_Set6_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set6_2_CallingStop(gameObject),
			new DynamicDialogueTest_Set6_3_ResumePlayingAfterStop(gameObject),
			new DynamicDialogueTest_Set7_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set7_2_CallingBreak(gameObject),
			new DynamicDialogueTest_Set7_3_ResumePlayingAfterBreak(gameObject),
			new DynamicDialogueTest_Set8_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set8_2_CallingPause(gameObject),
			new DynamicDialogueTest_Set8_3_CallingResumeAfterPause(gameObject),
			new DynamicDialogueTest_Set9_1_UsingDelay(gameObject),
			new DynamicDialogueTest_Set10_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set10_2_ClearPlaylist(gameObject),
			new DynamicDialogueTest_Set11_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set11_2_StopAndClearPlaylist(gameObject),
			new DynamicDialogueTest_Set12_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set12_2_BreakAndClearPlaylist(gameObject),
			new DynamicDialogueTest_Set13_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set13_2_PausePlaylist(gameObject),
			new DynamicDialogueTest_Set13_3_ClearAndResumePlaylist(gameObject),
			new DynamicDialogueTest_Set14_1_StartPlaybackWithCallback(gameObject),
			new DynamicDialogueTest_Set15_1_StartPlaybackWithCallback(gameObject),
			new DynamicDialogueTest_Set16_1_StartPlaybackWithCallback(gameObject),
			new DynamicDialogueTest_Set17_1_StartPlaybackWithCallback(gameObject),
			new DynamicDialogueTest_Set18_1_StartPlayback(gameObject),
			new DynamicDialogueTest_Set18_2_PostPauseEvent(gameObject),
			new DynamicDialogueTest_Set18_3_PostResumeEvent(gameObject),
			new DynamicDialogueTest_Set18_4_PostStopEvent(gameObject),
			new DynamicDialogueTest_Set18_5_PlayRestOfSequence(gameObject)
			
		};
	}
	
	private void StartTestSuite()
	{
		uint bankID;
		AkSoundEngine.LoadBank( m_BankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out bankID );

		SetTestSuiteReady();
		ResetCurrentTest();
		RunCurrentTest();
	}
	
	private void RunNextTest()
	{
		UpdateCurrentTest();
		if ( IsCurrentTestValid() )
			RunCurrentTest();
		else
		{
			ResetCurrentTest();
			SetTestSuiteDone();
			m_logger.Info("Prog_AllDone");
		}
		
	}
	
	private bool IsCurrentTestValid()
	{
		return m_currentTestIndex >= 0  && m_currentTestIndex < m_testSuite.Length;
	}
	
	private bool IsCurrentTestSetInProgress()
	{
		return IsCurrentTestValid() && m_testSuite[m_currentTestIndex].IsTestSetInProgress();
	}

	private bool IsCurrentTestInProgressOrWaiting()
	{
		return IsCurrentTestValid() 
			&& ( m_testSuite[m_currentTestIndex].IsTestInProgress() 
				|| m_testSuite[m_currentTestIndex].CountdownAndCheckIsWaitingAfterTest() );
	}
	
	private bool IsCurrentTestInProgress()
	{
		return IsCurrentTestValid() 
			&& ( m_testSuite[m_currentTestIndex].IsTestInProgress() 
				|| m_testSuite[m_currentTestIndex].IsWaitingAfterTest() );
	}

	private void UpdateCurrentTest()
	{
		++m_currentTestIndex;
	}
	
	private int RunCurrentTest()
	{
		return m_testSuite[m_currentTestIndex].Run();
	}
	
	private void ResetCurrentTest()
	{
		m_currentTestIndex = 0;
	}
	
	private void SetTestSuiteReady()
	{
		m_isTestStuiteStarted = true;
	}

	private void SetTestSuiteDone()
	{
		m_isTestStuiteStarted = false;
	}
}

// The testsuite contains a few sets of tests.
// A set is in progress if any test in the set is in progress.
// The tail test in a set releases the set and allows next set in the testsuite to start.
public abstract class DynamicDialogueTest
{
	protected GameObject m_gameObject;
	protected static uint ms_playingID = 0;
	protected string m_progKey = "UndefinedTest";
	protected bool m_isTestInProgress = false;
	protected bool m_isTestSetInProgress = false;
	protected bool m_isTailOfTestSet = false;
	protected int m_postTestWaitMs = 0;
	protected int m_postTestWaitFrame = 0;
	private const int m_PostTestWaitScaler = 1000;

	static private string[] ms_testSetNames = new string[] 
	{
		"Test 1 - Playing a simple dynamic sequence (using IDs).",
		"Test 2 - Playing a simple dynamic sequence (using strings).",
		"Test 3 - Add an item during playback.",
		"Test 4 - Insert an item into the list during playback.",
		"Test 5 - Add an item to an empty list during playback.",
		"Test 6 - Using the Stop call.",
		"Test 7 - Using the Break call.",
		"Test 8 - Using the Pause and Resume calls.",
		"Test 9 - Using a Delay when queueing to a playlist.",
		"Test 10 - Clearing the playlist during playback.",
		"Test 11 - Stopping the playlist and clearing it.",
		"Test 12 - Breaking the playlist and clearing it.",
		"Test 13 - Pausing the playlist and clearing it.",
		"Test 14 - Using a callback with custom parameters.",
		"Test 15 - Using a callback to cancel after 3 items play.",
		"Test 16 - Using a callback to play 2 items in sequence.",
		"Test 17 - Checking playlist content during playback.",
		"Test 18 - Using events with Dynamic Dialogue."
	};

	static private Dictionary<string, string> ms_messageDict = new Dictionary<string, string>()
	{
		{"Prog_Undefined", "UndefinedTest"},
		{"Prog_Set1_1", ms_testSetNames[0]},
		{"Prog_Set2_1", ms_testSetNames[1]},
		{"Prog_Set3_1", ms_testSetNames[2] + "..Start play back."},
		{"Prog_Set3_2", ms_testSetNames[2] + "..Add item to playlist."},
		{"Prog_Set4_1", ms_testSetNames[3] + "..Start play back."},
		{"Prog_Set4_2", ms_testSetNames[3] + "..Insert items to playlist."},
		{"Prog_Set5_1", ms_testSetNames[4] + "..Start empty playlist."},
		{"Prog_Set5_2", ms_testSetNames[4] + "..Add items to playlist."},
		{"Prog_Set5_3", ms_testSetNames[4] + "..Wait for empty list then add."},
		{"Prog_Set6_1", ms_testSetNames[5] + "..Start play back."},
		{"Prog_Set6_2", ms_testSetNames[5] + "..Calling stop."},
		{"Prog_Set6_3", ms_testSetNames[5] + "..Resume playing after stop."},
		{"Prog_Set7_1", ms_testSetNames[6] + "..Start play back."},
		{"Prog_Set7_2", ms_testSetNames[6] + "..Calling break."},
		{"Prog_Set7_3", ms_testSetNames[6] + "..Resume playing after break."},
		{"Prog_Set8_1", ms_testSetNames[7] + "..Start play back."},
		{"Prog_Set8_2", ms_testSetNames[7] + "..Calling pause."},
		{"Prog_Set8_3", ms_testSetNames[7] + "..Resume playing after pause."},
		{"Prog_Set9_1", ms_testSetNames[8] + "..Using delay."},
		{"Prog_Set10_1", ms_testSetNames[9] + "..Start play back."},
		{"Prog_Set10_2", ms_testSetNames[9] + "..Clear playlist."},
		{"Prog_Set11_1", ms_testSetNames[10] + "..Start play back."},
		{"Prog_Set11_2", ms_testSetNames[10] + "..Stop and clear playlist."},
		{"Prog_Set12_1", ms_testSetNames[11] + "..Start play back."},
		{"Prog_Set12_2", ms_testSetNames[11] + "..Break and clear playlist."},
		{"Prog_Set13_1", ms_testSetNames[12] + "..Start play back."},
		{"Prog_Set13_2", ms_testSetNames[12] + "..Pause playlist."},
		{"Prog_Set13_3", ms_testSetNames[12] + "..Clear and resume playlist."},
		{"Prog_Set14_1", ms_testSetNames[13] + "..Start play back with callback."},
		{"Prog_Set14_2", ms_testSetNames[13] + "..Callback."},
		{"Prog_Set15_1", ms_testSetNames[14] + "..Start play back with callback."},
		{"Prog_Set15_2", ms_testSetNames[14] + "..Callback."},
		{"Prog_Set16_1", ms_testSetNames[15] + "..Start play back with callback."},
		{"Prog_Set16_2", ms_testSetNames[15] + "..Callback."},
		{"Prog_Set17_1", ms_testSetNames[16] + "..Start play back with callback."},
		{"Prog_Set17_2", ms_testSetNames[16] + "..Callback."},
		{"Prog_Set18_1", ms_testSetNames[17] + "..Start play back."},
		{"Prog_Set18_2", ms_testSetNames[17] + "..Post pause event."},
		{"Prog_Set18_3", ms_testSetNames[17] + "..Post resume event."},
		{"Prog_Set18_4", ms_testSetNames[17] + "..Post stop event."},
		{"Prog_Set18_5", ms_testSetNames[17] + "..Play rest of sequence."},

		{"Error_ParamMismatch", "Params didn't match up"},
		{"Prog_TestDone", "Current test done: "},
		{"Prog_TestSetDone", "Current test set done: "},
	};

#if ! UNITY_METRO
	protected AkLogger m_logger = new AkLogger(System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.Name, ms_messageDict);
#else
	protected AkLogger m_logger = new AkLogger("DynamicDialogueTest", ms_messageDict);
#endif // #if ! UNITY_METRO
	
	public DynamicDialogueTest()
	{
	}
	
	public DynamicDialogueTest(GameObject obj)
	{
		m_gameObject = obj;
	}
	
	public string GetTestName()
	{
		return ms_messageDict[m_progKey];
	}

	public int Run()
	{
		Init();
		
		Test();
		
		return Term();
	}

	public virtual void StopAndRemoveAll()
	{
		if (ms_playingID != 0)
		{
			Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
			if( pPlaylist != null )
			{
				AkSoundEngine.DynamicSequenceStop( ms_playingID );
				pPlaylist.RemoveAll();
				AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
				AkSoundEngine.DynamicSequenceClose( ms_playingID );
			}

			ms_playingID = 0;
		}

	}
	
	abstract protected void Test();
	
	protected void Init()
	{
		m_logger.Info(m_progKey);
		SetTestInProgress();
		SetTestSetInProgress();
	}
	
	protected virtual int Term()
	{
		// m_logger.Info("Prog_TestDone", m_progKey);
		SetPostWait();

		SetTestDone();
		if ( IsTailOfTestSet() )
		{
			// m_logger.Info("Prog_TestSetDone", m_progKey);
			SetTestSetDone();
		}

		return m_postTestWaitFrame;
	}
	
	public string GetProgressKey() { return m_progKey; }
	public bool IsTestInProgress() { return m_isTestInProgress; }
	private void SetTestInProgress() { m_isTestInProgress = true; }
	private void SetTestDone() { m_isTestInProgress = false; }

	public bool IsTestSetInProgress() { return m_isTestSetInProgress; }
	private void SetTestSetInProgress() { m_isTestSetInProgress = true; }
	private void SetTestSetDone() { m_isTestSetInProgress = false; }
	private bool IsTailOfTestSet() { return m_isTailOfTestSet; }

	protected void SetPostWait() 
	{
		m_postTestWaitFrame = Application.targetFrameRate * m_postTestWaitMs / m_PostTestWaitScaler;
		// Debug.Log("Test: " + m_progKey + " SetPostWait: " + m_postTestWaitFrame.ToString());
	}

	public bool CountdownAndCheckIsWaitingAfterTest()
	{
		if(m_postTestWaitFrame > 0)
		{
			// Debug.Log("waiting " + m_postTestWaitFrame.ToString());
			--m_postTestWaitFrame;
		}

		return m_postTestWaitFrame > 0;
	}

	public bool IsWaitingAfterTest()
	{
		return m_postTestWaitFrame > 0;
	}

}

class DynamicDialogueTest_Set1_1_SimpleSequenceUsingID : DynamicDialogueTest
{
	public DynamicDialogueTest_Set1_1_SimpleSequenceUsingID(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set1_1";
		m_postTestWaitMs = 5000;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// Starting Test Set 1

		// Open a dynamic sequence on the GAME_OBJECT_ID_DYNAMICDIALOGUE game object
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

		// Locking the playlist for edition
		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			// In this section we will be adding 3 items in the playlist: Comm_In, Speech, Comm_Out

			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			// Resolve the dialogue Event
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			// Enqueue the Audio node in the playlist
			pPlaylist.Enqueue( audioNodeID );

			// Set up the arguments array
			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_B, 
									   AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID, 
									   AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED };

			// Resolve the dialogue Event using the arguments array. 
			// Note: It's very important to have the same argument order in Wwise and in the code.
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}
		
		// Play the dynamic sequence
		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		// Close the dynamic sequence
		// NOTE: It will continue to play until the end, but you won't be able to modify it anymore.
		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
		
	}

	
}

///////////////////////////////////////////////////////////////////////////////////////////////////
// Temporarily disabled to avoid crashes:
// - string version of ResolveDialogueEvent() introduces crashes when switching between play and stop.

// public class DynamicDialogueTest_Set2_1_SimpleSequenceUsingString : DynamicDialogueTest
// {
// 	public DynamicDialogueTest_Set2_1_SimpleSequenceUsingString(GameObject obj)
// 	{
// 		m_gameObject = obj;
		
// 		m_progKey = "Prog_Set2_1";
// 		m_postTestWaitMs = 5000;
// 		m_isTailOfTestSet = true;
// 	}

// 	protected override void Test()
// 	{
// 		TextWriter tw = new StreamWriter("d:\\Unity\\Logs\\crash_dyndemo_test2-1.txt", true);
//         tw.WriteLine(DateTime.Now.ToString("HH:mm:ss.fffffff: ") + "dyndemo_test2-1::Test: enter");

// 		// Starting Test Set 2

// 		// Open a dynamic sequence on the m_gameObject game object
// 		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

// 		// Locking the playlist for editting
// 		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
// 		if( pPlaylist != null )
// 		{
// 			// Adding 3 items in the playlist: Comm_In, Speech, Comm_Out
// 			string[]  szWalkieTalkieInArgument = new string[] { "Comm_In" };
// 			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( "WalkieTalkie", szWalkieTalkieInArgument, (uint)szWalkieTalkieInArgument.Length );
// 			pPlaylist.Enqueue( audioNodeID );

// 			string[]  underAttackStringArguments = new string[] { "Unit_A", "Gang", "Hangar" };

// 			// Resolve the dialogue Event using the arguments strings array.
// 			audioNodeID = AkSoundEngine.ResolveDialogueEvent( "Unit_Under_Attack", underAttackStringArguments, (uint)underAttackStringArguments.Length );
// 			pPlaylist.Enqueue( audioNodeID );

// 			string[] szWalkieTalkieOutArgument = new string[] { "Comm_Out" };
// 			audioNodeID = AkSoundEngine.ResolveDialogueEvent( "WalkieTalkie", szWalkieTalkieOutArgument, (uint)szWalkieTalkieOutArgument.Length );
// 			pPlaylist.Enqueue( audioNodeID );

// 			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
// 		}
		
// 		AkSoundEngine.DynamicSequencePlay( ms_playingID );
// 		AkSoundEngine.DynamicSequenceClose( ms_playingID );
// 		ms_playingID = 0;

// 		tw.WriteLine(DateTime.Now.ToString("HH:mm:ss.fffffff: ") + "dyndemo_test2-1::Test: exit");

// 		tw.Close();

// 	}
// }

///////////////////////////////////////////////////////////////////////////////////////////////////

public class DynamicDialogueTest_Set3_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set3_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set3_1";
		m_postTestWaitMs = 500;
	}

	protected override void Test()
	{
		// Starting Test Set 3
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] underAttackArguments = { AK.STATES.UNIT.STATE.UNIT_A, 
											AK.STATES.HOSTILE.STATE.BUM, 
											AK.STATES.LOCATION.STATE.STREET };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, underAttackArguments, (uint)underAttackArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );
	}

}

public class DynamicDialogueTest_Set3_2_AddItemToPlaylist : DynamicDialogueTest
{
	public DynamicDialogueTest_Set3_2_AddItemToPlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set3_2";
		m_postTestWaitMs = 5000;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{

		// Lock the playlist to add item during playback
		// NOTE: You should not keep the playlist locked for too long when the sequence is playing. 
		//       It could result in a stall in the Sound Engine.
		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			// Add new item in the playlist during the playback of the dynamic sequence
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_B, 
													AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE, 
													AK.STATES.OBJECTIVESTATUS.STATE.FAILED };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			// Unlock the playlist as soon as possible to prevent problems
			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}
}

public class DynamicDialogueTest_Set4_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set4_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set4_1";
		m_postTestWaitMs = 500;
	}
	
	override protected void Test()
	{
		ms_playingID = AkSoundEngine.DynamicSequenceOpen(m_gameObject);

		Playlist playlist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );

		if (playlist != null)
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE,  walkieTalkieArgument, (uint)walkieTalkieArgument.Length );

			playlist.Enqueue( audioNodeID );

			uint[] underAttackArguments = new uint[] { AK.STATES.UNIT.STATE.UNIT_A, 
													   AK.STATES.HOSTILE.STATE.BUM, 
													   AK.STATES.LOCATION.STATE.STREET 
													 };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, underAttackArguments, (uint)underAttackArguments.Length );
			playlist.Enqueue( audioNodeID );

			underAttackArguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			underAttackArguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			underAttackArguments[2] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, underAttackArguments, (uint)underAttackArguments.Length );
			playlist.Enqueue( audioNodeID );

			underAttackArguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			underAttackArguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			underAttackArguments[2] = AK.STATES.LOCATION.STATE.HANGAR;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, underAttackArguments, (uint)underAttackArguments.Length );
			playlist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			playlist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}
		
		AkSoundEngine.DynamicSequencePlay( ms_playingID );

	}
}


public class DynamicDialogueTest_Set4_2_InsertItemsToPlaylist : DynamicDialogueTest
{	
	public DynamicDialogueTest_Set4_2_InsertItemsToPlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set4_2";
		m_postTestWaitMs = 10000;
		m_isTailOfTestSet = true;
	}
	
	override protected void Test()
	{
		Playlist playlist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		
		if (playlist != null)
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			
			// Insert a new item in front of the playlist
			// Make sure to initialize all members of the PlaylistItem struct
			PlaylistItem playlistItem = playlist.Insert( 0 );
			playlistItem.audioNodeID = audioNodeID;
			playlistItem.msDelay = 0;
			playlistItem.pCustomInfo = IntPtr.Zero;

			uint[] statusArguments = {  AK.STATES.UNIT.STATE.UNIT_B, 
										AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE, 
										AK.STATES.OBJECTIVESTATUS.STATE.FAILED 
									 };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			
			// Insertion in the middle of the playlist
			playlistItem = playlist.Insert( 1 );
			playlistItem.audioNodeID = audioNodeID;
			playlistItem.msDelay = 0;
			playlistItem.pCustomInfo = IntPtr.Zero;

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			
			// Insertion in the middle of the playlist
			playlistItem = playlist.Insert( 2 );
			playlistItem.audioNodeID = audioNodeID;
			playlistItem.msDelay = 0;
			playlistItem.pCustomInfo = IntPtr.Zero;

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}



		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;

	}

}

public class DynamicDialogueTest_Set5_1_StartEmptyPlaylist : DynamicDialogueTest
{	
	public DynamicDialogueTest_Set5_1_StartEmptyPlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set5_1";
		m_postTestWaitMs = 500;
	}
	
	override protected void Test()
	{
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );
		// Start the playback right away.. we will add new items directly in the already playing
		// Dynamic sequence
		AkSoundEngine.DynamicSequencePlay( ms_playingID );
	}
}

public class DynamicDialogueTest_Set5_2_AddItemsToPlaylist : DynamicDialogueTest
{	
	public DynamicDialogueTest_Set5_2_AddItemsToPlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set5_2";
		m_postTestWaitMs = 1000;
	}
	
	override protected void Test()
	{
		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );

		if (pPlaylist != null)
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] underAttackArguments = { AK.STATES.UNIT.STATE.UNIT_B, 
											AK.STATES.HOSTILE.STATE.GANG, 
											AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, underAttackArguments, (uint)underAttackArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			// As soon as you unlock the playlist the new items will start to play
			// NOTE: Do not keep the playlist locked after the insertion! To trigger the playback at the right time,
			//       stop the dynamic sequence before adding the item and play it at the right time.
			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

	}
}

class DynamicDialogueTest_Set5_3_WaitForEmptyListThenAdd : DynamicDialogueTest
{
	public DynamicDialogueTest_Set5_3_WaitForEmptyListThenAdd(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set5_3";
		m_postTestWaitMs = 5000;
		m_isTailOfTestSet = true;
	}

	override protected void Test()
	{
		// Wait until the playlist is empty (at which point the last item will be playing),
		// then add new items that will start to play right away
		// NOTE: The newly added items are played with sample accuracy if the last item is still playing.

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );

		if( pPlaylist != null && pPlaylist.IsEmpty() )
		{
			// The playlist is empty we can add new items and they will play right away.
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_A, 
									   AK.STATES.OBJECTIVE.STATE.DEFUSEBOMB, 
									   AK.STATES.OBJECTIVESTATUS.STATE.FAILED };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
			
			AkSoundEngine.DynamicSequenceClose( ms_playingID );
			ms_playingID = 0;

		}
		else
		{
			if( pPlaylist != null )
			{
				// Don't keep the playlist locked.
				AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
			}

			// Keep waiting until the playlist is empty.
			m_postTestWaitMs = 200;
		}
	}
};

class DynamicDialogueTest_Set6_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set6_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set6_1";
		m_postTestWaitMs = 1000;
	}

	protected override void Test()
	{
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] arguments = { AK.STATES.UNIT.STATE.UNIT_A, 
								 AK.STATES.HOSTILE.STATE.BUM, 
								 AK.STATES.LOCATION.STATE.STREET };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.LOCATION.STATE.HANGAR;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );
	}

};

class DynamicDialogueTest_Set6_2_CallingStop : DynamicDialogueTest
{
	public DynamicDialogueTest_Set6_2_CallingStop(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set6_2";
		m_postTestWaitMs = 2000;
	}

	protected override void Test()
	{
		// Stop the playback immediatly
		AkSoundEngine.DynamicSequenceStop( ms_playingID );
	}
};

class DynamicDialogueTest_Set6_3_ResumePlayingAfterStop : DynamicDialogueTest
{
	public DynamicDialogueTest_Set6_3_ResumePlayingAfterStop(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set6_3";
		m_postTestWaitMs = 4000;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// Play the rest of the sequence
		// NOTE: The sequence will restart with the item that was in the playlist
		//       after the item that was stopped.
		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}
};

///////////////////////////////////////////////////////////////////////////////////////////////////

class DynamicDialogueTest_Set7_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set7_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set7_1";
		m_postTestWaitMs = 1000;
	}

	protected override void Test()
	{
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] arguments = { AK.STATES.UNIT.STATE.UNIT_A, 
								 AK.STATES.HOSTILE.STATE.BUM, 
								 AK.STATES.LOCATION.STATE.STREET };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.LOCATION.STATE.HANGAR;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );

	}
	
};

class DynamicDialogueTest_Set7_2_CallingBreak : DynamicDialogueTest
{
	public DynamicDialogueTest_Set7_2_CallingBreak(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set7_2";
		m_postTestWaitMs = 4000;
	}

	protected override void Test()
	{
		// Break the sequence, it will stop after the current item.
		AkSoundEngine.DynamicSequenceBreak( ms_playingID );
	}
	
};

class DynamicDialogueTest_Set7_3_ResumePlayingAfterBreak : DynamicDialogueTest
{
	public DynamicDialogueTest_Set7_3_ResumePlayingAfterBreak(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set7_3";
		m_postTestWaitMs = 5000;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// Play the rest of the sequence
		// NOTE: The sequence will play the item that was right after the one breaked.
		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}
};

///////////////////////////////////////////////////////////////////////////////////////////////////

class DynamicDialogueTest_Set8_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set8_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set8_1";
		m_postTestWaitMs = 1000;
	}

	protected override void Test()
	{
		// Starting Test Set 8
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] arguments = { AK.STATES.UNIT.STATE.UNIT_A, 
											  AK.STATES.HOSTILE.STATE.BUM, 
											  AK.STATES.LOCATION.STATE.STREET };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.LOCATION.STATE.HANGAR;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );
	}
	
};

class DynamicDialogueTest_Set8_2_CallingPause : DynamicDialogueTest
{
	public DynamicDialogueTest_Set8_2_CallingPause(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set8_2";
		m_postTestWaitMs = 2000;
	}

	protected override void Test()
	{
		// Pause the sequence.
		AkSoundEngine.DynamicSequencePause( ms_playingID );
	}
};

class DynamicDialogueTest_Set8_3_CallingResumeAfterPause : DynamicDialogueTest
{
	public DynamicDialogueTest_Set8_3_CallingResumeAfterPause(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set8_3";
		m_postTestWaitMs = 8000;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// Resume the sequence.
		// NOTE: The paused item will resume playing, followed by the rest of the sequence.
		AkSoundEngine.DynamicSequenceResume( ms_playingID );

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}
};

///////////////////////////////////////////////////////////////////////////////////////////////////

class DynamicDialogueTest_Set9_1_UsingDelay : DynamicDialogueTest
{
	public DynamicDialogueTest_Set9_1_UsingDelay(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set9_1";
		m_postTestWaitMs = 7000;
	}

	protected override void Test()
	{
		// Starting Test Set 9
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_A, 
													AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID, 
													AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			// Add a delay between the Comm_In click and the start of the speech
			// NOTE: Delays are in milliseconds.
			pPlaylist.Enqueue( audioNodeID, 300 );

			uint[] underAttackArguments = { AK.STATES.UNIT.STATE.UNIT_A, 
											AK.STATES.HOSTILE.STATE.GANG, 
											AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, underAttackArguments, (uint)underAttackArguments.Length );
			// Add a delay between the 2 speeches
			pPlaylist.Enqueue( audioNodeID, 1500 );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			// Add a small delay before the Comm_Out
			pPlaylist.Enqueue( audioNodeID, 400 );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;

	}
	
};

///////////////////////////////////////////////////////////////////////////////////////////////////

class DynamicDialogueTest_Set10_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set10_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set10_1";
		m_postTestWaitMs = 1000;
	}	

	// Starting Test Set 10
	protected override void Test()
	{
		// Specifying DynamicSequenceType_NormalTransition prevents the next sound in the playlist from 
		// being prepared in advance and removed from the playlist before the current sound finishes playing.
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( 
													m_gameObject,
													0, 
													null, 
													null, 
													DynamicSequenceType.DynamicSequenceType_NormalTransition );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] arguments = { AK.STATES.UNIT.STATE.UNIT_B, 
											  AK.STATES.HOSTILE.STATE.GANG, 
											  AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.LOCATION.STATE.ALLEY;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			arguments[1] = AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE;
			arguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );
	}


};

class DynamicDialogueTest_Set10_2_ClearPlaylist : DynamicDialogueTest
{
	public DynamicDialogueTest_Set10_2_ClearPlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set10_2";
		m_postTestWaitMs = 3500;
		m_isTailOfTestSet = true;
	}	

	protected override void Test()
	{
		// Clear the playlist
		// NOTE: The item currently playing will continue to play until it finishes.
		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			pPlaylist.RemoveAll();
			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}

	
};

///////////////////////////////////////////////////////////////////////////////////////////////////

class DynamicDialogueTest_Set11_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set11_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set11_1";
		m_postTestWaitMs = 1000;
	}

	protected override void Test()
	{
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] arguments = { AK.STATES.UNIT.STATE.UNIT_B, 
											  AK.STATES.HOSTILE.STATE.GANG, 
											  AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.LOCATION.STATE.ALLEY;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			arguments[1] = AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE;
			arguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );
	}


};

class DynamicDialogueTest_Set11_2_StopAndClearPlaylist : DynamicDialogueTest
{
	public DynamicDialogueTest_Set11_2_StopAndClearPlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set11_2";
		m_postTestWaitMs = 3500;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// Stop the sequence.
		AkSoundEngine.DynamicSequenceStop( ms_playingID );

		// Clear the playlist and play. 
		// NOTE: Nothing should play since the playlist has been cleared.
		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			pPlaylist.RemoveAll();
			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}
		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}

};

///////////////////////////////////////////////////////////////////////////////////////////////////

class DynamicDialogueTest_Set12_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set12_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set12_1";
		m_postTestWaitMs = 500;
	}

	protected override void Test()
	{
		// Starting Test Set 12
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] arguments = { AK.STATES.UNIT.STATE.UNIT_B, 
											  AK.STATES.HOSTILE.STATE.GANG, 
											  AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.LOCATION.STATE.ALLEY;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			arguments[1] = AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE;
			arguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );

	}


};

class DynamicDialogueTest_Set12_2_BreakAndClearPlaylist : DynamicDialogueTest
{
	public DynamicDialogueTest_Set12_2_BreakAndClearPlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set12_2";
		m_postTestWaitMs = 5500;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// Break the sequence, the currently playing item will finish playing.
		AkSoundEngine.DynamicSequenceBreak( ms_playingID );

		// Clear the playlist and play.
		// NOTE: Nothing should play since the playlist has been cleared.
		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			pPlaylist.RemoveAll();
			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}
		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}

};

///////////////////////////////////////////////////////////////////////////////////////////////////

class DynamicDialogueTest_Set13_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set13_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set13_1";
		m_postTestWaitMs = 1000;
	}

	protected override void Test()
	{
		// Starting Test Set 13
		// Specifying DynamicSequenceType.DynamicSequenceType_NormalTransition prevents the next sound in the playlist from 
		// being prepared in advance and removed from the playlist before the current sound finishes playing.
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( 
								m_gameObject, 
								0, 
								null, 
								null,
								DynamicSequenceType.DynamicSequenceType_NormalTransition  );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] arguments = { AK.STATES.UNIT.STATE.UNIT_B, 
											  AK.STATES.HOSTILE.STATE.GANG, 
											  AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			arguments[1] = AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID;
			arguments[2] = AK.STATES.LOCATION.STATE.ALLEY;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			arguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			arguments[1] = AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE;
			arguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, arguments, (uint)arguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );

	}

};

class DynamicDialogueTest_Set13_2_PausePlaylist : DynamicDialogueTest
{
	public DynamicDialogueTest_Set13_2_PausePlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set13_2";
		m_postTestWaitMs = 2000;
	}

	protected override void Test()
	{
		// Pause the sequence.
		AkSoundEngine.DynamicSequencePause( ms_playingID );
	}
	
};

class DynamicDialogueTest_Set13_3_ClearAndResumePlaylist : DynamicDialogueTest
{
	public DynamicDialogueTest_Set13_3_ClearAndResumePlaylist(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set13_3";
		m_postTestWaitMs = 4000;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// Clear the playlist and resume.
		// NOTE: Only the item that was paused will resume playback - the rest
		//       have been cleared.
		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			pPlaylist.RemoveAll();
			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequenceResume( ms_playingID );

		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}
};

///////////////////////////////////////////////////////////////////////////////////////////////////

// Initialize the static member variables.
class DynamicDialogueTest_Set14_1_StartPlaybackWithCallback : DynamicDialogueTest
{
	private int mSet14_iParamIndex = 0;
	private IntPtr[] mSet14_CustomParams = { (IntPtr)123, (IntPtr)456, (IntPtr)789 };

	public DynamicDialogueTest_Set14_1_StartPlaybackWithCallback()
	{

	}

	public DynamicDialogueTest_Set14_1_StartPlaybackWithCallback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set14_1";
		m_postTestWaitMs = 2000;
		m_isTailOfTestSet = true;
	}

	protected override int Term()
	{
		Debug.Log("Current test done. But wait for callback to terminate the test.");
		return m_postTestWaitFrame;
	}

	protected int TermCallback()
	{
		// m_logger.Info("Prog_TestDone", m_progKey);
		SetPostWait();

		m_isTestInProgress = false;
		if ( m_isTailOfTestSet )
		{
			// m_logger.Info("Prog_TestSetDone", m_progKey);
			m_isTestSetInProgress = false;
		}

		return m_postTestWaitFrame;
	}
	
	protected override void Test()
	{
		mSet14_iParamIndex = 0;


		// Open a dynamic sequence specifying that we want ot be notified when:
		//   - each item ends (AK_EndOfDynamicSequenceItem) 
		//   - the sequence is finished (AK_EndOfEvent)
		// Specify the callback and the "this" pointer as a cookie.
		// The callback will ensure that the information received matches what is expected.
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject,
														  (uint)AkCallbackType.AK_EndOfDynamicSequenceItem | (uint)AkCallbackType.AK_EndOfEvent,
														  DynamicDialogueTest_Set14_Callback,
														  this );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet14_CustomParams[0] );

			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_B, 
									   AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID, 
									   AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet14_CustomParams[1] );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet14_CustomParams[2] );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		// NOTE: Make sure to close the dynamic sequence or you won't receive the end of sequence callback notification.
		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
		// Started playback - let the callback handle the rest.
	}

	public static void DynamicDialogueTest_Set14_Callback( object in_cookie, AkCallbackType in_eType, object in_pCallbackInfo )
	{
		DynamicDialogueTest_Set14_1_StartPlaybackWithCallback pDemoInstance = (DynamicDialogueTest_Set14_1_StartPlaybackWithCallback)in_cookie;

		if (pDemoInstance == null)
		{
			Debug.Log("null demo instance");
			return;
		}

		pDemoInstance.m_logger.Info("Prog_Set14_2");

		// End of Item callback
		if( in_eType == AkCallbackType.AK_EndOfDynamicSequenceItem ) 
		{
			// prepare for the next item
			++pDemoInstance.mSet14_iParamIndex;			
		}
		// End of sequence callback (Only called if the sequence is closed)
		else if( in_eType == AkCallbackType.AK_EndOfEvent )
		{
			// Done with Test Set 14 - wait a bit and move on to Set 15.
			pDemoInstance.TermCallback();
		}
	}
};


///////////////////////////////////////////////////////////////////////////////////////////////////


class DynamicDialogueTest_Set15_1_StartPlaybackWithCallback : DynamicDialogueTest_Set14_1_StartPlaybackWithCallback
{
	// Initialize static member variables.
	private static int mSet15_iItemsPlayed = 0;

	public DynamicDialogueTest_Set15_1_StartPlaybackWithCallback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set15_1";
		m_postTestWaitMs = 2000;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// Starting Test Set 15
		mSet15_iItemsPlayed = 0;

		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject,
															  (uint)AkCallbackType.AK_EndOfDynamicSequenceItem | (uint)AkCallbackType.AK_EndOfEvent,
															  DynamicDialogueTest_Set15_Callback,
															  this );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			// Item 1 (Comm_in)
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			// Item 2 (Mission Completed)
			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_B, 
									   AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID, 
									   AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED };
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			// Item 3 (Neutralize Hostile Failed)
			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			// Item 4 (Rescue Hostage Failed)
			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.RESCUEHOSTAGE;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			// Item 5 (Defuse Bomb Failed)
			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.DEFUSEBOMB;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			// Item 6 (Comm_out)
			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		// Started playback with the callback. Let the callback handle the rest.
	}
	
	static private void DynamicDialogueTest_Set15_Callback( object in_cookie, AkCallbackType in_eType, object in_pCallbackInfo )
	{
		DynamicDialogueTest_Set15_1_StartPlaybackWithCallback pDemoInstance = (DynamicDialogueTest_Set15_1_StartPlaybackWithCallback)in_cookie;

		if (pDemoInstance == null)
		{
			Debug.Log("null demo instance");
			return;
		}

		pDemoInstance.m_logger.Info("Prog_Set15_2");

		if( in_eType == AkCallbackType.AK_EndOfDynamicSequenceItem )
		{
			++mSet15_iItemsPlayed;

			if ( mSet15_iItemsPlayed == 2 )
			{
				// The second item has finished playing - the third item is now playing.
				// Clear out the rest of the items on the playlist and readd the Comm_out.
				// NOTE: Items 4 and 5 should not be heard!
				Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( DynamicDialogueTest.ms_playingID );
				if( pPlaylist != null )
				{
					uint lastAudioNode = pPlaylist.Last().audioNodeID;
					pPlaylist.RemoveAll();

					pPlaylist.Enqueue( lastAudioNode );

					AkSoundEngine.DynamicSequenceUnlockPlaylist( DynamicDialogueTest.ms_playingID );
				}

				AkSoundEngine.DynamicSequenceClose( DynamicDialogueTest.ms_playingID );

				// Done with Test Set 15 - wait a bit and move on to Set 16.
				pDemoInstance.TermCallback();
			}
		}
	}

};


	
///////////////////////////////////////////////////////////////////////////////////////////////////


class DynamicDialogueTest_Set16_1_StartPlaybackWithCallback : DynamicDialogueTest_Set14_1_StartPlaybackWithCallback
{
	static uint mSet16_Seq1PlayingID = 0;
	static uint mSet16_Seq2PlayingID = 0;

	public DynamicDialogueTest_Set16_1_StartPlaybackWithCallback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set16_1";
		m_postTestWaitMs = 5000;
		m_isTailOfTestSet = true;
	}

	public override void StopAndRemoveAll()
	{
		// Set 16 requires a special clear as it uses different Sequence IDs.
		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( mSet16_Seq1PlayingID );
		if( pPlaylist != null )
		{
			AkSoundEngine.DynamicSequenceStop( mSet16_Seq1PlayingID );
			pPlaylist.RemoveAll();
			AkSoundEngine.DynamicSequenceUnlockPlaylist( mSet16_Seq1PlayingID );
			AkSoundEngine.DynamicSequenceClose( mSet16_Seq1PlayingID );
		}
		pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( mSet16_Seq2PlayingID );
		if( pPlaylist != null )
		{
			AkSoundEngine.DynamicSequenceStop( mSet16_Seq2PlayingID );
			pPlaylist.RemoveAll();
			AkSoundEngine.DynamicSequenceUnlockPlaylist( mSet16_Seq2PlayingID );
			AkSoundEngine.DynamicSequenceClose( mSet16_Seq2PlayingID );
		}

	}
	
	protected override void Test()
	{
		// Starting Test Set 16
		// Open Sequence 1
		mSet16_Seq1PlayingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject, (uint)AkCallbackType.AK_EndOfEvent, DynamicDialogueTest_Set16_Callback, this );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( mSet16_Seq1PlayingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_B, 
									   AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID, 
									   AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( mSet16_Seq1PlayingID );
		}

		// Open Sequence 2
		mSet16_Seq2PlayingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject, 
			(uint) AkCallbackType.AK_EndOfEvent, 
			DynamicDialogueTest_Set16_Callback, this);

		pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( mSet16_Seq2PlayingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] underAttackArguments = { AK.STATES.UNIT.STATE.UNIT_A, 
											AK.STATES.HOSTILE.STATE.BUM, 
											AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.UNIT_UNDER_ATTACK, underAttackArguments, (uint)underAttackArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( mSet16_Seq2PlayingID );
		}

		// Start playing Sequence 1.
		AkSoundEngine.DynamicSequencePlay( mSet16_Seq1PlayingID );
		AkSoundEngine.DynamicSequenceClose( mSet16_Seq1PlayingID );

		// Playback has started with callback. Let the Callback handle the rest.		
	}

	private static void DynamicDialogueTest_Set16_Callback( object in_cookie, AkCallbackType in_eType, object in_pCallbackInfo )
	{
		DynamicDialogueTest_Set16_1_StartPlaybackWithCallback pDemoInstance = (DynamicDialogueTest_Set16_1_StartPlaybackWithCallback)in_cookie;

		if (pDemoInstance == null)
		{
			Debug.Log("null demo instance");
			return;
		}

		pDemoInstance.m_logger.Info("Prog_Set16_2");

		AkCallbackManager.AkEventCallbackInfo pEventInfo = (AkCallbackManager.AkEventCallbackInfo) in_pCallbackInfo;

		if ( in_eType == AkCallbackType.AK_EndOfEvent && pEventInfo.playingID == mSet16_Seq1PlayingID )
		{
			// The first sequence is done - play the second sequence.
			// NOTE: The transition between the two sequences won't be sample accurate.
			AkSoundEngine.DynamicSequencePlay( mSet16_Seq2PlayingID );
			AkSoundEngine.DynamicSequenceClose( mSet16_Seq2PlayingID );

			// End of Test Set 16. Wait a while and move on to Set 17.
			pDemoInstance.TermCallback();
		}
	}

};



///////////////////////////////////////////////////////////////////////////////////////////////////


class DynamicDialogueTest_Set17_1_StartPlaybackWithCallback : DynamicDialogueTest_Set14_1_StartPlaybackWithCallback
{
	static IntPtr[] mSet17_CustomParams = { (IntPtr)123, (IntPtr)456, (IntPtr)789, (IntPtr)321, (IntPtr)654, (IntPtr)987 };

	private bool m_isSequenceClosed = false;

	public DynamicDialogueTest_Set17_1_StartPlaybackWithCallback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set17_1";
		m_postTestWaitMs = 5000;
		m_isTailOfTestSet = true;
	}


	protected override void Test()
	{
		// Starting Test Set 17

		ms_playingID = AkSoundEngine.DynamicSequenceOpen( m_gameObject,
													     (uint) (AkCallbackType.AK_EndOfDynamicSequenceItem),
														 DynamicDialogueTest_Set17_Callback, 
														 this );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet17_CustomParams[0] );

			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_B, 
													AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID, 
													AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet17_CustomParams[1] );

			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet17_CustomParams[2] );

			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.RESCUEHOSTAGE;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet17_CustomParams[3] );

			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.DEFUSEBOMB;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet17_CustomParams[4] );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID, 0, mSet17_CustomParams[5] );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );

		// Started playback with callback. Let the callback handle the rest.	
	}

	public static void DynamicDialogueTest_Set17_Callback( object in_cookie, AkCallbackType in_eType, object in_pCallbackInfo )
	{
		DynamicDialogueTest_Set17_1_StartPlaybackWithCallback pDemoInstance = (DynamicDialogueTest_Set17_1_StartPlaybackWithCallback)in_cookie;

		if (pDemoInstance.m_isSequenceClosed)
			return;

		pDemoInstance.m_logger.Info("Prog_Set17_2");

		if( in_eType == AkCallbackType.AK_EndOfDynamicSequenceItem )
		{
			// An item has finish playing so let's check the playlist content..
			Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( DynamicDialogueTest.ms_playingID );
			if( pPlaylist != null )
			{
				// Check if the playlist is empty
				if( pPlaylist.IsEmpty() )
				{
					// End of Test Set 17. Wait a while and move on to Set 18.
					AkSoundEngine.DynamicSequenceUnlockPlaylist( DynamicDialogueTest.ms_playingID );
					AkSoundEngine.DynamicSequenceClose( DynamicDialogueTest.ms_playingID );

					pDemoInstance.m_isSequenceClosed = true;

					pDemoInstance.TermCallback();
					return;
				}
				else
				{
					// The items remaining in the list should be the last item added
					// Let's validate using mSet17_CustomParams array.
					uint playlistLength = pPlaylist.Length();
					uint customParamsIndex = 6 - playlistLength;
					for( uint i = 0; i < playlistLength; ++i, ++customParamsIndex )
					{
						if ( mSet17_CustomParams[customParamsIndex] != pPlaylist.ItemAtIndex(i).pCustomInfo )
						{
							Debug.LogWarning( "Error: Params didn't match up!" );
						}
					}

					AkSoundEngine.DynamicSequenceUnlockPlaylist( DynamicDialogueTest.ms_playingID );
				}
			}
		}
	}
	
};



///////////////////////////////////////////////////////////////////////////////////////////////////
	
class DynamicDialogueTest_Set18_1_StartPlayback : DynamicDialogueTest
{
	public DynamicDialogueTest_Set18_1_StartPlayback(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set18_1";
		m_postTestWaitMs = 1000;
	}

	protected override void Test()
	{
		// Starting Test Set 18
		// Specifying DynamicSequenceType_NormalTransition prevents the next playlist item to be stopped with
		// the currently playing sound. If not specified, the sample accurate transition would have already prepared 
		// the next item to play and this item would be stopped as well.
		ms_playingID = AkSoundEngine.DynamicSequenceOpen( 
							m_gameObject, 
							0, 
							null, 
							null, 
							DynamicSequenceType.DynamicSequenceType_NormalTransition );

		Playlist pPlaylist = AkSoundEngine.DynamicSequenceLockPlaylist( ms_playingID );
		if( pPlaylist != null )
		{
			uint[] walkieTalkieArgument = {AK.STATES.WALKIETALKIE.STATE.COMM_IN};
			uint audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			uint[] statusArguments = { AK.STATES.UNIT.STATE.UNIT_B, 
													AkSoundEngine.AK_FALLBACK_ARGUMENTVALUE_ID, 
													AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED };

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.NEUTRALIZEHOSTILE;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.RESCUEHOSTAGE;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_A;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.DEFUSEBOMB;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.FAILED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			statusArguments[0] = AK.STATES.UNIT.STATE.UNIT_B;
			statusArguments[1] = AK.STATES.OBJECTIVE.STATE.DEFUSEBOMB;
			statusArguments[2] = AK.STATES.OBJECTIVESTATUS.STATE.COMPLETED;

			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.OBJECTIVE_STATUS, statusArguments, (uint)statusArguments.Length );
			pPlaylist.Enqueue( audioNodeID );

			walkieTalkieArgument[0] = AK.STATES.WALKIETALKIE.STATE.COMM_OUT;
			audioNodeID = AkSoundEngine.ResolveDialogueEvent( AK.DIALOGUE_EVENTS.WALKIETALKIE, walkieTalkieArgument, (uint)walkieTalkieArgument.Length );
			pPlaylist.Enqueue( audioNodeID );

			AkSoundEngine.DynamicSequenceUnlockPlaylist( ms_playingID );
		}

		AkSoundEngine.DynamicSequencePlay( ms_playingID );
	}

};

class DynamicDialogueTest_Set18_2_PostPauseEvent : DynamicDialogueTest
{
	public DynamicDialogueTest_Set18_2_PostPauseEvent(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set18_2";
		m_postTestWaitMs = 3000;
	}

	protected override void Test()
	{
		// Pause the dynamic dialogue using a Pause_All event
		AkSoundEngine.PostEvent( AK.EVENTS.PAUSE_ALL, m_gameObject );
	}

};

class DynamicDialogueTest_Set18_3_PostResumeEvent : DynamicDialogueTest
{
	public DynamicDialogueTest_Set18_3_PostResumeEvent(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set18_3";
		m_postTestWaitMs = 2000;
	}

	protected override void Test()
	{
		// Resume the dynamic dialogue playback using a Resume_All event
		AkSoundEngine.PostEvent( AK.EVENTS.RESUME_ALL, m_gameObject );
	}
};

class DynamicDialogueTest_Set18_4_PostStopEvent : DynamicDialogueTest
{
	public DynamicDialogueTest_Set18_4_PostStopEvent(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set18_4";
		m_postTestWaitMs = 2000;
	}

	protected override void Test()
	{
		// Stop the dynamic dialogue playback using a Stop_All event
		AkSoundEngine.PostEvent( AK.EVENTS.STOP_ALL, m_gameObject );
	}

};

class DynamicDialogueTest_Set18_5_PlayRestOfSequence : DynamicDialogueTest
{
	public DynamicDialogueTest_Set18_5_PlayRestOfSequence(GameObject obj)
	{
		m_gameObject = obj;
		
		m_progKey = "Prog_Set18_5";
		m_postTestWaitMs = 6500;
		m_isTailOfTestSet = true;
	}

	protected override void Test()
	{
		// The rest of the sequence will play.
		AkSoundEngine.DynamicSequencePlay( ms_playingID );
		AkSoundEngine.DynamicSequenceClose( ms_playingID );
		ms_playingID = 0;
	}
};
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY || UNITY_WP8) // Disable under unsupported platforms