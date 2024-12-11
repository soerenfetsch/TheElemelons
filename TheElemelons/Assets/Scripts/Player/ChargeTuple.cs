using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System;
public class ChargeTuple
{
    private ChargeTypes charge1;
    private ChargeTypes charge2;
    public ChargeTuple()
    {
        charge1 = ChargeTypes.notype;
        charge2 = ChargeTypes.notype;
    }
    public ChargeTuple(ChargeTypes chargeOne, ChargeTypes chargeTwo)
    {
        charge1 = chargeOne;
        charge2 = chargeTwo;
    }
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
    public void Clear()
    {
        charge1 = ChargeTypes.notype;
        charge2 = ChargeTypes.notype;
    }
    public List<ChargeTypes> GetCharges()
    {
        return new List<ChargeTypes> { charge1, charge2 };
    }
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

    public ChargeTypes first()
    {
        return charge1;
    }
    public ChargeTypes second()
    {
        return charge2;
    }
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
    // override object.Equals
    public override bool Equals(object obj)
    {
        //
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

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

    //override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        //throw new System.NotImplementedException();
        byte[] bytes = MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(((int)charge1 + (int)charge2).ToString()));
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return BitConverter.ToInt32(bytes);
    }
}
