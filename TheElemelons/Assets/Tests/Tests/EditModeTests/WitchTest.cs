using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WitchTest
{
    Witch witchScript;
    [SetUp]
    public void SetUp()
    {
        witchScript = new Witch();
        witchScript.InitializeDice();
        for (int i = 0; i < witchScript.dice.Count; i++)
        {
            witchScript.dice[i] = UnityEngine.Random.Range(0, 1);
        }
        witchScript.InitilializeMoveStats();
    }
    [Test]
    public void TestChose()
    {
        List<WitchMoves> chosens = new List<WitchMoves>();
        for (int i = 0; i < 30; i++)
        {
            List<WitchMoves> choices = new List<WitchMoves> { WitchMoves.GreenPoof, WitchMoves.RedPoof, WitchMoves.Dash, WitchMoves.Medusa, WitchMoves.Boulder, WitchMoves.Vortex, WitchMoves.Offense };
            WitchMoves choice = witchScript.Chose(choices);
            chosens.Add(choice);
        }
        bool allIsSame = true;
        for (int i = 1; i < chosens.Count; i++)
        {
            if (chosens[i] != chosens[0])
            {
                allIsSame = false;
            }
        }
        Assert.False(allIsSame, " always picking the same is highly unlikely, please check");
    }
}
