using System;

/*
 * Gareth Williams
 * 
 * an adsr envelope generates an envelope signal over time, 
 * the position needs resetting to zero to 'retrigger' the envelope.
 * 
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public class ADSREnvelope : IAudioNode, IHasPosition
	{
		#region accessors
		double m_attackMS;
		public double AttackMS
		{
			get
			{
				return m_attackMS;
			}
			set
			{
				m_attackMS = value;
				recalculateEnvelope();
			}
		}
		
		double m_decayMS;
		public double DecayMS
		{
			get
			{
				return m_decayMS;
			}
			set
			{
				m_decayMS = value;
				recalculateEnvelope();
			}
		}
		
		double m_sustainMS;
		public double SustainMS
		{
			get
			{
				return m_sustainMS;	
			}
			set
			{
				m_sustainMS = value;
				recalculateEnvelope();
			}
		}
		
		double m_releaseMS;
		public double ReleaseMS
		{
			get
			{
				return m_releaseMS;	
			}
			set
			{
				m_releaseMS = value;
				recalculateEnvelope();
			}
		}
		#endregion

		#region IPositionable
		double m_position;
		public double Position
		{
			get
			{
				return m_position;
			}
			set
			{
				m_position = value;
			}
		}
		#endregion
		
		public double lengthMS
		{
			get
			{
				return m_envelopeEnd;	
			}
		}
		
		public double sustainLevel = 0.5;
		
		double m_envelopeEnd;
		double m_releaseBegin;
		double m_sustainBegin;
		double m_decayBegin;
		
		#region IAudioNode
		public event SampleEventHandler SampleGenerated;
		
		public double GetSample()
		{
			//evaluate largest boundary to smallest...
			double amplitude = 0;
			if (m_position > m_envelopeEnd)//outside envelope
			{
				amplitude = 0;
			}
			else if (m_position > m_releaseBegin) //release
			{
				double offsetPosition = m_position - m_releaseBegin;
				amplitude = (1.0 / m_releaseMS) * offsetPosition;
				amplitude *= sustainLevel;
			}
			else if(m_position > m_sustainBegin) //sustain
			{
				amplitude = sustainLevel;
			}
			else if(m_position > m_decayBegin) //decay
			{
				double offsetPosition = m_position - m_decayBegin;
				amplitude = 1.0 - (1.0 / m_decayMS) * offsetPosition; 
				
				if(amplitude < sustainLevel)
				{
					amplitude = sustainLevel;	
				}
			}
			else if(m_attackMS > 0) //attack (prevent div by 0 if attack is zero
			{
				amplitude = (1.0 / m_attackMS) * m_position;
			}
			
			m_position++;
			
			if(SampleGenerated != null)
			{
				SampleGenerated(amplitude);	
			}
			
			return amplitude;
		}
		#endregion
		
		private void recalculateEnvelope()
		{
			m_envelopeEnd	= (m_attackMS + m_decayMS  + m_sustainMS  + m_releaseMS);
			m_releaseBegin	= (m_attackMS + m_decayMS  + m_sustainMS);
			m_sustainBegin	= (m_attackMS + m_decayMS);
			m_decayBegin 	= m_attackMS;
		}
	}
}

