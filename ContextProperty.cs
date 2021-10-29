using BTSCodeGen.BizTalkBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTSCodeGen
{
    public class ContextProperty
    {
        public string Name;
        public object Value;
        public Type Type;

        public static MessageBase operator |(ContextProperty p, MessageBase msg)
        {
            if (msg.ContextProperties.Contains(p))
            {
                return msg;
            }
            else
            {
                return null;
            }
        }

        // Boolean, Byte, Char, Decimal, Double, Int16, Int32, Int64, SByte, Single,String, UInt16, UInt32, and UInt64.​ 

        public static implicit operator bool(ContextProperty p)
        {
            return (bool)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator byte(ContextProperty p)
        {
            return (byte)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator char(ContextProperty p)
        {
            return (char)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator decimal(ContextProperty p)
        {
            return (decimal)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator double(ContextProperty p)
        {
            return (double)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator Int16(ContextProperty p)
        {
            return (Int16)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator Int32(ContextProperty p)
        {
            return (Int32)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator Int64(ContextProperty p)
        {
            return (Int64)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator SByte(ContextProperty p)
        {
            return (SByte)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator Single(ContextProperty p)
        {
            return (Single)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator string(ContextProperty p)
        {
            return (string)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator UInt16(ContextProperty p)
        {
            return (UInt16)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator UInt32(ContextProperty p)
        {
            return (UInt32)System.Convert.ChangeType(p.Value, p.Type);
        }

        public static implicit operator UInt64(ContextProperty p)
        {
            return (UInt64)System.Convert.ChangeType(p.Value, p.Type);
        }        

    }​
}
