using Microsoft.XLANGs.BaseTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using BTSCodeGen;

namespace BTSCodeGen.BizTalkBase
{
    public partial class MessageBase
    {
        public List<ContextProperty> ContextProperties;
        public XmlDocument Xml;

        public MessageBase()
        {
            ContextProperties = new List<ContextProperty>();
            BTS.InterchangeID.Value = Guid.Empty.ToString();
            ContextProperties.Add(BTS.InterchangeID);

            Xml = new XmlDocument();
        }

        public MessageBase(XmlDocument doc)
        {
            ContextProperties = new List<ContextProperty>();
            BTS.InterchangeID.Value = Guid.Empty.ToString();
            ContextProperties.Add(BTS.InterchangeID);

            Xml = doc;
        }

        public MessageBase LoadFromRes(string resourceName)
        {
            string xml = null;
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    xml = reader.ReadToEnd();
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return new MessageBase(doc);
        }​

        //public string this[string propName]
        //{
        //    get { return ReadProperty(propName); }
        //    set { WriteProperty(propName, value); }
        //}

        public void WriteProperty(ContextProperty prop, object propVal)
        {
            if (ContextProperties.Contains(prop))
            {
                ContextProperties[prop].Value = propVal;
            }
            else
            {
                prop.Value = propVal;
                ContextProperties.Add(prop);
            }
        }

        public ContextProperty ReadProperty(ContextProperty prop)
        {
            if (ContextProperties.Contains(prop))
            {
                return ContextProperties[prop];
            }
            else
            {
                throw new MissingPropertyException();
            }
        }

        public ContextProperty ReadProperty(string propName)
        {
            ContextProperty prop = ContextProperties.FirstOrDefault(p => p.Name == propName);

            if (prop == null)
            {
                throw new MissingPropertyException();
            }
            else
            {
                return prop;
            }
        }

        public void CopyAllProperties(MessageBase source)
        {
            this.ContextProperties = source.ContextProperties;
        }

        public static implicit operator XmlDocument(MessageBase msg)
        {
            return msg.Xml; ;
        }

        public static implicit operator MessageBase(XmlDocument doc)
        {
            MessageBase msg = new MessageBase(doc);
            return msg;
        }


        public static implicit operator XLANGMessage(MessageBase msg)
        {
            MockXLANGMessage<MessageBase> xmsg = new MockXLANGMessage<MessageBase>(msg);
            return xmsg;
        }

        public static implicit operator MessageBase(XLANGMessage msg)
        {
            XmlDocument doc = (XmlDocument)msg[0].RetrieveAs(typeof(XmlDocument));

            // TODO Copy the properties

            MessageBase msgOut = new MessageBase(doc);
            return msgOut;
        }

        public static implicit operator MessageBase(string text)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<string></string>");
            xml.DocumentElement.InnerText = text;

            MessageBase msg = new MessageBase(xml);
            
            return msg;
        }



    }

    public class Operation
    {
        public delegate MessageBase ReceiveDelegate();
        public ReceiveDelegate Receive;

        public delegate void SendDelegate(MessageBase msg);
        public SendDelegate Send;

        //public C.Message[] Fault;
    }

    public class Transaction
    {
        public delegate bool TxSucceededDelegate();
        public TxSucceededDelegate TxSucceeded;
    }

    public class LongRunningTransaction
    {
        public Transaction Transaction;

        public LongRunningTransaction(Transaction tx)
        {
            Transaction = tx;
        }
    }

    public class AtomicTransaction
    {
        public Transaction Transaction;

        public AtomicTransaction(Transaction tx)
        {
            Transaction = tx;
        }
    }

    public class PolicyManager
    {
        public delegate void EvalDelegate(string policyName, object[] interchange);
        public EvalDelegate Eval;
    }

    public class OrchestrationBase
    {
        public static ConvertibleObject xpath(MessageBase msg, string p)
        {
            return (ConvertibleObject)msg.Xml.SelectSingleNode(p).InnerText;
        }

        public static ConvertibleObject xpath(Exception ex, string p)
        {
            return (ConvertibleObject)ex.Message;
        }


        public static void xpath(MessageBase msg, string p, ConvertibleObject setValue)
        {
            XmlNode n = msg.Xml.SelectSingleNode(p);
            if (n != null)
            {
                n.InnerText = setValue;
            }
        }

        public static void suspend(string text)
        {
            // Wait for a Resume event
        }

        public static void Resume()
        {
            // Send a resume event
        }

        public static void receive(Operation op, out MessageBase msg, CorrelationBase corr = null)
        {
            msg = op.Receive();
        }

        public static void send(Operation op, MessageBase msg, CorrelationBase corr = null)
        {
            op.Send(msg);
        }

        public static void MessageBase(Operation op, out MessageBase msg)
        {
            msg = op.Receive();
        }

        public static void send(Operation op, MessageBase msg, out CorrelationBase corr)
        {
            corr = new CorrelationBase(); // TODO Correlation ??
            op.Send(msg);
        }

        public static bool succeeded(LongRunningTransaction tx)
        {
            return tx.Transaction.TxSucceeded();
        }

        public static bool succeeded(AtomicTransaction tx)
        {
            if (tx.Transaction.TxSucceeded == null)
            {
                return true;
            }

            return tx.Transaction.TxSucceeded();
        }

        public static void delay(TimeSpan time)
        {
            System.Threading.Thread.Sleep(time);
        }

        public static void delay(DateTime dateTime)
        { 
            TimeSpan time = dateTime.Subtract(DateTime.Now);

            System.Threading.Thread.Sleep(time);
        }

        public static void transform(out MessageBase msgOut, string type, params MessageBase[] msgIn)
        {
            if (msgIn.Count() != 1)
            {
                // TODO merge all the xml in into as single (Biztalk schema)
                throw new NotSupportedException();
            }

            // Get the type
            object map = null;
            Type ty = null;
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if (t.Namespace + "." + t.Name == type)
                    {
                        ty = t;
                        map = Activator.CreateInstance(t);
                        break;
                    }
                }

                if (map != null)
                {
                    break;
                }
            }

            if (map == null)
            {
                msgOut = new MessageBase();
                return;
            }

            // Get the XSL from _strMap  (_strArgList)
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = ty.GetField("_strMap", bindFlags);
            object xsl = field.GetValue(map);


            // Transform

            XsltSettings settings = new XsltSettings();
            settings.EnableScript = true;

            string output = String.Empty;
            using (StringReader srt = new StringReader(xsl.ToString()))
            using (StringReader sri = new StringReader(msgIn[0].Xml.OuterXml))
            {
                using (XmlReader xrt = XmlReader.Create(srt))
                using (XmlReader xri = XmlReader.Create(sri))
                {
                    XslCompiledTransform xslt = new XslCompiledTransform();

                    xslt.Load(xrt, settings, null);
                    using (StringWriter sw = new StringWriter())
                    using (XmlWriter xwo = XmlWriter.Create(sw, xslt.OutputSettings))
                    {
                        xslt.Transform(xri, xwo);
                        output = sw.ToString();
                    }
                }
            }

            XmlDocument doc = new XmlDocument();

            if (!string.IsNullOrEmpty(output))
            {
                doc.LoadXml(output);
            }

            msgOut = new MessageBase(doc);
        }

        public delegate void FuncCallDelegate();

        public static void receiveuntil(Operation op, out MessageBase msg, FuncCallDelegate receiveCall, TimeSpan timeout, FuncCallDelegate timeoutCall)
        { 
            // Start two trheads one to call receive and another to sleep
            // The first trhead to complete cancel the other and executes the delegate

            msg = new MessageBase();
        }


        //public static void test()
        //{
        //    MessageBase msg;

        //    receiveuntil(new Operation(), out msg, () =>
        //        {}
        //    , new System.TimeSpan(1), () => 
        //        {}
        //    );

        //}
    }

    //public class a
    //{ 
    //    public static a a;

    //    public void Call(string s)
    //    {
    //    }
    //}

    //public class b
    //{ 
    //    a.Call("pippo");
    //}

    public class Fault : Exception
    {
        public Fault()
        { }

        public Fault(string msg)
            : base(msg)
        { }
    }

    public class TerminateException : Exception
    {
        public TerminateException()
        { }

        public TerminateException(string msg)
            : base(msg)
        { }
    }

    public class CorrelationBase
    {
        public List<string> Properties = new List<string>();
    }

}
