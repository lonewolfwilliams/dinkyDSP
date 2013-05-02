using UnityEngine;
using System.Collections;

namespace com.lonewolfwilliams.dinkyDSP
{
	public class SyncedSamplePlayer : SamplePlayer, IAudioNode
	{
		public int lengthInBeats = 4;
		int m_bpm;
		public int BPM
		{
			get
			{
				return m_bpm;	
			}
			set
			{
				m_bpm = value;
				int samplesPerBeat = buffer.Length / lengthInBeats;
				float beatsPerSecond = m_bpm / 60;
				float frequency = beatsPerSecond * samplesPerBeat;
				
				m_frequency = frequency;
				recalculate();
			}
		}
	}
}
