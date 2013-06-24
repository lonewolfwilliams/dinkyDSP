using System;
using System.Collections.Generic;

/*
 * Gareth Williams
 * 
 * can either be used to reduce processing down the line 
 * or to create a neate decimation effect
 * 
 * stereo
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class StereoSampleConverter : IAudioNode, IHasInput
	{
		#region accessors
		int m_convertFrom = 44100;
		public int ConvertFrom
		{
			get
			{
				return m_convertFrom;	
			}
			set
			{
				m_convertFrom = value;
				recalculate();
			}
		}
		
		int m_convertTo = Driver.sampleRate / 32;
		public int ConvertTo
		{
			get
			{
				return m_convertTo;	
			}
			set
			{
				m_convertTo = value;
				recalculate();
			}
		}
		
		int m_channels = 1;
		public int Channels
		{
			get
			{
				return m_channels;
			}
			set
			{
				m_channels = value;
				recalculate();
			}
		}
		#endregion accessors
		
		double m_inScale;
		double m_outScale;
		
		double m_readPosition;
		
		List<double[]> m_buffer = new List<double[]>();
		int m_channel;
		
		public StereoSampleConverter()
		{
			recalculate();
		}

		#region IHasInput implementation
		IAudioNode m_inputNode;
		public IAudioNode InputNode 
		{
			get 
			{
				return m_inputNode;
			}
			set 
			{
				m_inputNode = value;
			}
		}
		#endregion	
		
		#region IAudioNode implementation
		public event SampleEventHandler SampleGenerated;
		public double GetSample ()
		{
			if(m_buffer == null)
			{
				return 0;	
			}
			
			//resample n samples into buffer--------------------------
			
			//read n samples upfront to resample
			//TODO: is there a way to do this without a for loop ?
			int samplesRequired = (int)Math.Ceiling(m_inScale);
			double[] channel = new double[m_channels];
			for (int i = 0; i < samplesRequired; i++)
			{
				for (int j = 0; j < m_channels; j++)
				{
					channel[j] += m_inputNode.GetSample() / samplesRequired;
				}
			}
			m_buffer.Add(channel);
			
			//----------------------------------------------------------
			
			//read from buffer compensating for sample rate-------------
			
			if(m_channel >= m_channels)
			{
				m_channel = 0;	
			}
			
			//get 'block' position within higher samplerate
			int prevIndex = (int)Math.Floor(m_readPosition);// * channels + m_channel;
			int nextIndex = (int)Math.Ceiling(m_readPosition);// * channels + m_channel;
			
			if(nextIndex >= m_buffer.Count)
			{
				nextIndex = m_buffer.Count - 1;	
			}
			
			//smooth and downsample for current 'sub-position' in 'block'
			double sampleDelta = m_buffer[nextIndex][m_channel] - m_buffer[prevIndex][m_channel];
			double interpolationAmount = m_readPosition - prevIndex;
			double lerp = m_buffer[prevIndex][m_channel] + sampleDelta * interpolationAmount;
			m_readPosition += m_outScale;
				
			m_channel++;
			
			//-----------------------------------------------------------
			
			/*
			//free up a little memory...this is causing some problems with threading :/
			//I suspect that either it's to do with the fact that audio is on it's own thread
			//seperate to unityengine and this is not a threadsafe resource, or alternatively
			//calling removerange is making the audio thread lose it's synchronisation by taking too
			//long to complete...
			if(m_readPosition > Driver.sampleRate * 4)
			{
				m_readPosition -= Driver.sampleRate * 4;
				m_buffer.RemoveRange(0, Driver.sampleRate * 4 - 1);
			}
			*/
			
			if(SampleGenerated != null)
			{
				SampleGenerated(lerp);	
			}
			return lerp;
		}
		#endregion
		
		void recalculate()
		{
			m_inScale = (double)m_convertFrom / m_convertTo;
			m_outScale = (double)m_convertTo / Driver.sampleRate;
		}
	}
}

