using UnityEngine;
using System.Collections.Generic;
using com.lonewolfwilliams.dinkyDSP;

/*
 * Gareth Williams
 *
 * Welcome to DinkyDSP
 * 
 * This is a framework that I wrote primarily for my own audio games, I can think of many possibilities for it
 * in the spirit of homebrew I am open-sourcing it so that anyone who is, or fancies becoming a DSP hacker 
 * can benefit from it. Or even contribute to it :O
 * 
 * !!!warning----------------------------------------------------
 * 
 *	this library (and playing with synthesis in general) can produce 
 *	some frequencies that may damage your hearing, especially if you 
 *	have your headphones on and the volume turned up!
 *	
 *	be careful - the first few times you run your graphs, have the 
 *	master volume on your OS TURNED DOWN LOW!
 *	
 * !!!warning----------------------------------------------------
 * 
 * This behaviour component binds the com.lonewolfwilliams.dinkyDSP to the native audioBuffer
 * 
 * The guiding principles behind dinkyDSP are:
 * 	  to provide an audio graph implementation (inspiration from most dsp frameworks - coreaudio / naudio / tonfall / msp)
 *    to provide a simple and light implementation for synthesis in Unity (inspiration from Tonfall)
 * 	  to use Inversion of control principle to provide the audio data without dependency on the unity update loop (promoting modularity)
 *    to provide an open ended approach to the audio graph promoting experimentation - 
 * 	     most things should connect to most things (inspiration from msp)
 * 	  to provide a platform primarily for DSP hackers and the DSP curious (who don't mind reading around the subject)
 * 
 * In general Naudio (an excellent library for .net which I believe there is a unity / mono port of) and Tonfall
 * (a very nicely implemented AS3 library) have been my inspiration.
 * 
 * TODO: 
 *    Optimisation
 * 	  Add some higher level api calls for casual users
 *    Add some analysis mechanisms for visualisation
 *	  Create lots more crazee synths ^_^
 */

public class AudioContextBehaviour : MonoBehaviour 
{
	public List<AudioClip> samples = new List<AudioClip>();
	Driver m_driver;
	DinkyDemo m_welcomeToDinky;
	
	//expose for input control
	public float Cutoff
	{
		set
		{
			m_welcomeToDinky.FilterCutoffHZ = value;	
		}
	}
	
	void Awake()
	{
		//initialisation
		SampleBank bank = new SampleBank();
		
		foreach(AudioClip clip in samples)
		{
			bank.AddSampleBufferFromAudioClip(clip.name, clip);
		}
		
		//context
		Driver.sampleRate = 48000;
		Driver.channels = 2;
		
		m_driver = new Driver();	
		m_welcomeToDinky = new DinkyDemo(m_driver);
		
		m_welcomeToDinky.BPM = 120;
	}
	
	void OnAudioFilterRead(float[] data, int channels)
	{
		com.lonewolfwilliams.dinkyDSP.Driver.channels = channels;
		m_driver.GenerateSamples(ref data);
	}
	
	public void Transpose(int interval)
	{
		m_welcomeToDinky.Transpose(interval);
	}
}
