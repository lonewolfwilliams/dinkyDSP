using System;

/*
 * Gareth Williams
 * 
 * some AudioNodes can have their position set
 */

namespace com.lonewolfwilliams.dinkyDSP
{
	public interface IHasPosition
	{
		double Position {get;set;}
	}
}

