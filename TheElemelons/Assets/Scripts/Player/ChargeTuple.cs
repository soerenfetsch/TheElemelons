using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Diagnostics;
using PlasticPipe.PlasticProtocol.Messages;
using System.Collections;
using UnityEngine;

/// <summary>
/// Helper class to manage elemental charges.
/// Contains up to two charges (fire, water, earth, wind).
/// Order of charges does not matter when comparing two ChargeTuple objects.
/// </summary>
public class ChargeTuple
{
    private ChargeTypes charge1;
    private ChargeTypes charge2;

    /// <summary>
    /// Default constructor. Initializes charges to notype.
    /// </summary>
    public ChargeTuple()
    {
        charge1 = ChargeTypes.notype;
        charge2 = ChargeTypes.notype;
    }

    /// <summary>
    /// Initializes a ChargeTuple with two charges.
    /// </summary>
    /// <param name="chargeOne">The first charge.</param>
    /// <param name="chargeTwo">The second charge.</param>
    public ChargeTuple(ChargeTypes chargeOne, ChargeTypes chargeTwo)
    {
        charge1 = chargeOne;
        charge2 = chargeTwo;
    }

    /// <summary>
    /// Adds a charge to the tuple.
    /// </summary>
    /// <param name="charge">The charge to add.</param>
    public void Add(ChargeTypes charge)
    {
        if (charge1 == ChargeTypes.notype)
        {
            charge1 = charge;
        }
        else if (charge2 == ChargeTypes.notype)
        {
            charge2 = charge;
        }
    }

    /// <summary>
    /// Clears all charges from the tuple.
    /// </summary>
    public void Clear()
    {
        charge1 = ChargeTypes.notype;
        charge2 = ChargeTypes.notype;
    }

    /// <summary>
    /// Returns the charges as a list.
    /// </summary>
    /// <returns>A list of charges.</returns>
    public List<ChargeTypes> GetCharges()
    {
        return new List<ChargeTypes> { charge1, charge2 };
    }

    /// <summary>
    /// Counts the number of charges in the tuple.
    /// </summary>
    /// <returns>The number of charges (0, 1, or 2).</returns>
    public int Count()
    {
        int count = 0;
        if (charge1 != ChargeTypes.notype)
        {
            count++;
        }
        if (charge2 != ChargeTypes.notype)
        {
            count++;
        }
        return count;
    }

    /// <summary>
    /// Gets the first charge.
    /// </summary>
    /// <returns>The first charge.</returns>
    public ChargeTypes first()
    {
        return charge1;
    }

    /// <summary>
    /// Gets the second charge.
    /// </summary>
    /// <returns>The second charge.</returns>
    public ChargeTypes second()
    {
        return charge2;
    }

    /// <summary>
    /// Checks how many times a specific charge exists in the tuple.
    /// </summary>
    /// <param name="other">The charge to check.</param>
    /// <returns>The count of the specified charge.</returns>
    public float HasHowMany(ChargeTypes other)
    {
        float count = 0;
        if (charge1 == other)
        {
            count++;
        }
        if (charge2 == other)
        {
            count++;
        }
        return count;
    }

    /// <summary>
    /// Determines if two ChargeTuple objects are equal (order of charges does not matter).
    /// </summary>
    /// <param name="obj">The other ChargeTuple to compare.</param>
    /// <returns>True if the tuples are equal; otherwise false.</returns>
    public override bool Equals(object obj)
    {
        // 
        // See the full list of guidelines at 
        // http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator == at
        // http://go.microsoft.com/fwlink/?LinkId=85238
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        ChargeTuple chargeObj = (ChargeTuple)obj;
        // TODO: write your implementation of Equals() here
        if ((charge1 == chargeObj.first() && charge2 == chargeObj.second())
        || (charge2 == chargeObj.first() && charge1 == chargeObj.second()))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Generates a hash code for the ChargeTuple.
    /// </summary>
    /// <returns>The hash code.</returns>

    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        // throw new System.NotImplementedException();
        byte[] bytes = MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(((int)charge1 + (int)charge2).ToString()));
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return BitConverter.ToInt32(bytes);
    }
}
