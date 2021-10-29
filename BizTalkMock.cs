using BTSCodeGen.BizTalkBase;
using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTSCodeGen
{
    public class ConvertibleObject
    {
        public ConvertibleObject()
        { 
        }

        public ConvertibleObject(ConvertibleObject obj)
        {
            Val = obj;
        }

        public object Val;

        // Boolean, Byte, Char, Decimal, Double, Int16, Int32, Int64, SByte, Single,String, UInt16, UInt32, and UInt64.​ 

        public static implicit operator bool(ConvertibleObject p)
        {
            return (bool)System.Convert.ChangeType(p.Val, typeof(bool));
        }

        public static implicit operator ConvertibleObject(bool p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator byte(ConvertibleObject p)
        {
            return (byte)System.Convert.ChangeType(p.Val, typeof(byte));
        }

        public static implicit operator ConvertibleObject(byte p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator char(ConvertibleObject p)
        {
            return (char)System.Convert.ChangeType(p.Val, typeof(char));
        }
        
        public static implicit operator ConvertibleObject(char p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator decimal(ConvertibleObject p)
        {
            return (decimal)System.Convert.ChangeType(p.Val, typeof(decimal));
        }

        public static implicit operator ConvertibleObject(decimal p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator double(ConvertibleObject p)
        {
            return (double)System.Convert.ChangeType(p.Val, typeof(double));
        }

        public static implicit operator ConvertibleObject(double p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator Int16(ConvertibleObject p)
        {
            return (Int16)System.Convert.ChangeType(p.Val, typeof(Int16));
        }

        public static implicit operator ConvertibleObject(Int16 p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator Int32(ConvertibleObject p)
        {
            return (Int32)System.Convert.ChangeType(p.Val, typeof(Int32));
        }

        public static implicit operator ConvertibleObject(Int32 p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator Int64(ConvertibleObject p)
        {
            return (Int64)System.Convert.ChangeType(p.Val, typeof(Int64));
        }

        public static implicit operator ConvertibleObject(Int64 p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator SByte(ConvertibleObject p)
        {
            return (SByte)System.Convert.ChangeType(p.Val, typeof(SByte));
        }

        public static implicit operator ConvertibleObject(SByte p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator Single(ConvertibleObject p)
        {
            return (Single)System.Convert.ChangeType(p.Val, typeof(Single));
        }

        public static implicit operator ConvertibleObject(Single p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator string(ConvertibleObject p)
        {
            return (string)System.Convert.ChangeType(p.Val, typeof(string));
        }

        public static implicit operator ConvertibleObject(string p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator UInt16(ConvertibleObject p)
        {
            return (UInt16)System.Convert.ChangeType(p.Val, typeof(UInt16));
        }

        public static implicit operator ConvertibleObject(UInt16 p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator UInt32(ConvertibleObject p)
        {
            return (UInt32)System.Convert.ChangeType(p.Val, typeof(UInt32));
        }

        public static implicit operator ConvertibleObject(UInt32 p)
        {
            return new ConvertibleObject(p);
        }

        public static implicit operator UInt64(ConvertibleObject p)
        {
            return (UInt64)System.Convert.ChangeType(p.Val, typeof(UInt64));
        }

        public static implicit operator ConvertibleObject(UInt64 p)
        {
            return new ConvertibleObject(p);
        }

    }

    public class MockXLANGPart<T> : XLANGPart
    {
        T m_obj;

        public MockXLANGPart(T obj)
        {
            m_obj = obj;
        }

        public override void Dispose()
        {
        }

        public override object GetPartProperty(Type propType)
        {
            throw new NotImplementedException();
        }

        public override Type GetPartType()
        {
            throw new NotImplementedException();
        }

        public override string GetXPathValue(string xpath)
        {
            throw new NotImplementedException();
        }

        public override void LoadFrom(object source)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return "MockXLANGPart"; }
        }

        public override void PrefetchXPathValue(string xpath)
        {
            throw new NotImplementedException();
        }

        public override object RetrieveAs(Type t)
        {
            if (t == typeof(T))
            {
                return m_obj;
            }

            return null;
        }

        public override void SetPartProperty(Type propType, object value)
        {
            throw new NotImplementedException();
        }

        public override System.Xml.Schema.XmlSchema XmlSchema
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Xml.Schema.XmlSchemaCollection XmlSchemaCollection
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class MockXLANGMessage<T> : XLANGMessage
    {
        List<MockXLANGPart<T>> m_parts = new List<MockXLANGPart<T>>();

        public MockXLANGMessage(T obj)
        {
            m_parts.Add(new MockXLANGPart<T>(obj));
        }

        public override void AddPart(object part, string partName)
        {
            throw new NotImplementedException();
        }

        public override void AddPart(XLANGPart part, string partName)
        {
            throw new NotImplementedException();
        }

        public override void AddPart(XLANGPart part)
        {
            throw new NotImplementedException();
        }

        public override int Count
        {
            get { return m_parts.Count; }
        }

        public override void Dispose()
        {
        }

        public override System.Collections.IEnumerator GetEnumerator()
        {
            return m_parts.GetEnumerator();
        }

        public override object GetPropertyValue(Type propType)
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return "MockXLANGMessage"; }
        }

        public override void SetPropertyValue(Type propType, object value)
        {
            throw new NotImplementedException();
        }

        public override XLANGPart this[int partIndex]
        {
            get { return m_parts[partIndex]; }
        }

        public override XLANGPart this[string partName]
        {
            get { return m_parts[0]; }
        }
    }
}

namespace Microsoft.XLANGs.BaseTypes
{
    public class XLANGPart : IDisposable
    {
        virtual public void Dispose()
        {
            throw new NotImplementedException();
        }

        public virtual object GetPartProperty(Type propType)
        {
            throw new NotImplementedException();
        }

        public virtual Type GetPartType()
        {
            throw new NotImplementedException();
        }

        public virtual string GetXPathValue(string xpath)
        {
            throw new NotImplementedException();
        }

        public virtual void LoadFrom(object source)
        {
            throw new NotImplementedException();
        }

        public virtual string Name
        {
            get { return "MockXLANGPart"; }
        }

        public virtual void PrefetchXPathValue(string xpath)
        {
            throw new NotImplementedException();
        }

        public virtual object RetrieveAs(Type t)
        {
            return null;
        }

        public virtual void SetPartProperty(Type propType, object value)
        {
            throw new NotImplementedException();
        }

        public virtual System.Xml.Schema.XmlSchema XmlSchema
        {
            get { throw new NotImplementedException(); }
        }

        public virtual System.Xml.Schema.XmlSchemaCollection XmlSchemaCollection
        {
            get { throw new NotImplementedException(); }
        }

    }

    public class XLANGMessage
    {
        public virtual void AddPart(object part, string partName)
        {
            throw new NotImplementedException();
        }

        public virtual void AddPart(XLANGPart part, string partName)
        {
            throw new NotImplementedException();
        }

        public virtual void AddPart(XLANGPart part)
        {
            throw new NotImplementedException();
        }

        public virtual int Count
        {
            get { return 0; }
        }

        public virtual void Dispose()
        {
        }

        public virtual System.Collections.IEnumerator GetEnumerator()
        {
            return null;
        }

        public virtual object GetPropertyValue(Type propType)
        {
            throw new NotImplementedException();
        }

        public virtual string Name
        {
            get { return "MockXLANGMessage"; }
        }

        public virtual void SetPropertyValue(Type propType, object value)
        {
            throw new NotImplementedException();
        }

        public virtual XLANGPart this[int partIndex]
        {
            get { return null; }
        }

        public virtual XLANGPart this[string partName]
        {
            get { return null; }
        }    
    }

    public class MissingPropertyException : Exception
    { }

    public class DeliveryFailureException : Exception
    { }


}

namespace Microsoft.RuleEngine
{
    public class Policy
    {
        private string _policyName;
        private PolicyManager _manager;

        public Policy(string policyName, PolicyManager manager)
        {
            _policyName = policyName;
            _manager = manager;
        }

        public void Execute(params object[] interchange)
        {
            _manager.Eval(_policyName, interchange);
        }

        public void Dispose()
        { }
    }
}