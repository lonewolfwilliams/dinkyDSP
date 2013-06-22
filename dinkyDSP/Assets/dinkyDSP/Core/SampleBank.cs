using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * the sample bank stores buffers created at runtime by the AudioContextBehaviour from referenced
 * AudioClips in the UnityGameEngine
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class SampleBank
	{
		Dictionary<string, SampleBuffer> m_sampleBuffers = new Dictionary<string, SampleBuffer>();
		
		//http://docs.unity3d.com/Documentation/ScriptReference/AudioClip.GetData.html
		public void AddSampleBufferFromAudioClip(string name, UnityEngine.AudioClip clip)
		{
			SampleBuffer sample = new SampleBuffer();
			sample.channels = clip.channels;
			sample.sampleRate = clip.frequency;
			Common.noteToFrequency.TryGetValue("C", out sample.fundamentalPitch);
			sample.buffer = new float[(int)clip.samples * clip.channels];
			
			clip.GetData(sample.buffer, 0);
			m_sampleBuffers.Add(name, sample);
		}
		
		//TODO:other converters
		
		public void RemoveSampleBuffer(string name)
		{
			m_sampleBuffers.Remove(name);
		}
		
		public void Clear()
		{
			m_sampleBuffers.Clear();	
		}
		
		public SampleBuffer GetSampleBuffer(string name)
		{
			SampleBuffer sample;
			m_sampleBuffers.TryGetValue(name, out sample);
			
			return sample;
		}
		
		#region convenient factory methods
		public WavetableGenerator GetWaveTableForBuffer(string name)
		{
			WavetableGenerator voice = new WavetableGenerator();
			var sample = GetSampleBuffer(name);
			voice.channels = sample.channels;
			voice.Frequency = sample.fundamentalPitch;
			voice.buffer = sample.buffer;
			
			return voice;
		}
		
		public SamplePlayer GetSamplePlayerForBuffer(string name)
		{
			SamplePlayer voice = new SamplePlayer();
			var sample = GetSampleBuffer(name);
			voice.originalSampleRate = sample.sampleRate;
			//voice.channels = sample.channels;
			//voice.fundamentalFrequency = sample.fundamentalPitch;
			voice.buffer = sample.buffer;
			
			return voice;
		}
		#endregion
	}
}

