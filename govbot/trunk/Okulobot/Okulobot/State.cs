using System;
using System.Collections.Generic;
using System.Linq;

namespace Okulobot
{
    public class State : ICloneable
    {
    	public InitData InitData { get; private set; }

    	public Fighter Fighter { get; set; }

    	public List<Unit> Enemies { get; set; }

		public List<State> History { get; private set; }

    	public State(InitData data)
    	{
    		InitData = data;
    		History = new List<State>();
    	}

    	public void Update(Reality reality)
    	{
    		Fighter = reality.ActiveFighter;
    		Enemies = reality.Enemies;
    	}

		public void AfterNewMove()
		{
			History.Add((State)Clone());
		}

    	public object Clone()
    	{
    		return new State(InitData)
    		       	{
    		       		Enemies = Enemies.ToList(),
    		       		Fighter = new Fighter(Fighter.Properties.TypeName, null, null)
    		       		          	{
    		       		          		Angle = Fighter.Angle,
    		       		          		Coordinates = Fighter.Coordinates,
    		       		          		Health = Fighter.Health,
    		       		          		IsAlive = Fighter.IsAlive,
    		       		          		Mana = Fighter.Mana
    		       		          	}
    					
    		       	};
    	}
    }
}