using BTSCodeGen.BizTalkBase;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BTSCodeGen
{
    public class CodeGen
    {
        public static void GenCSFromODX(string odxFile, string csFileName)
        {
            string[] lines = File.ReadAllLines(odxFile);            

            // Check if there are errors
            bool isInError = false;
            List<string> errors = new List<string>();
            for (int j = 0; j < lines.Count(); j++)
            {
                if (lines[j].Trim().StartsWith("#error \""))
                {
                    errors.Add(lines[j]);
                    isInError = true;
                }
            }

            if (isInError)
            {
                StringBuilder errorcode = new StringBuilder();

                errorcode.AppendLine("using System;");
                errorcode.AppendLine("");
                errorcode.AppendLine("namespace ReviewTheFollowingErrors");
                errorcode.AppendLine("{");

                foreach (string s in errors)
                {
                    errorcode.AppendLine("   // " + s);    
                }

                errorcode.AppendLine("}");

                File.WriteAllText(csFileName, errorcode.ToString());

                return;
            }


            // body {} => it may have an exceptions block or not  (end with finally {})
            string openingBracketBody = "";
            string closingBracketBody = "";
            bool inBodyBlock = false;
            for (int j = 0; j < lines.Count(); j++)
            {
                if (lines[j].Trim().StartsWith("body"))
                {
                    openingBracketBody = lines[j + 1];
                    closingBracketBody = lines[j + 1].Replace("{", "}");
                    inBodyBlock = true;
                    //lines[j] = "";
                    //lines[j + 1] = "";
                }

                if (inBodyBlock)
                {
                    if (lines[j] == closingBracketBody)
                    {
                        if (!lines[j + 1].Trim().StartsWith("exceptions"))
                        {
                            lines[j] += " finally {}";
                        }
                        inBodyBlock = false;
                    }
                }
            }

            // Remove exceptions { }
            string openingBracket = "";
            string closingBracket = "";
            bool inExceptionsBlock = false;
            for (int j = 0; j < lines.Count(); j++)
            {
                if (lines[j].Trim().StartsWith("exceptions"))
                {
                    openingBracket = lines[j + 1];
                    closingBracket = lines[j + 1].Replace("{", "}");
                    inExceptionsBlock = true;
                    lines[j] = "";
                    lines[j + 1] = "";
                }

                if (inExceptionsBlock)
                {
                    if (lines[j] == closingBracket)
                    {
                        lines[j] = "";
                        inExceptionsBlock = false;
                    }
                }
            }

            // Normalize (trim and ;)
            int i = 0;
            foreach (string line in lines)
            {
                lines[i] = line.Trim();                

                if (lines[i].EndsWith(";"))
                {
                    while (lines[i].EndsWith(";"))
                    {
                        lines[i] = lines[i].Remove(lines[i].Length - 1);
                    }

                    lines[i] += ";";
                }

                i++;
            }

            // Parse namespace, orchestration name, ports, code
            Parsed parsed = new Parsed();

            int codeStartAt = 0;
            i = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith("module "))
                {
                    codeStartAt = i;
                }
                else
                {
                    i++;
                }
            }

            int currentPos = codeStartAt;

            parsed.Namespace = lines[currentPos].Replace("module ", "");
            currentPos += 2;

            while (lines[currentPos].Contains(" porttype "))
            {
                PortType portType = new PortType();
                portType.TypeName = lines[currentPos].Replace("internal porttype ", "").Replace("public porttype ", "").Replace("private porttype ", "");
                currentPos += 2;

                while (lines[currentPos].StartsWith("requestresponse ") || lines[currentPos].StartsWith("oneway "))
                {
                    PortOperation op = new PortOperation();
                    op.OperationName = lines[currentPos].Replace("requestresponse ", "").Replace("oneway ", "");
                    currentPos += 2;
                    op.Schemas = lines[currentPos].Split(',');

                    portType.PortOperations.Add(op);

                    currentPos += 1;
                }

                currentPos += 2;

                parsed.PortTypes.Add(portType);
            }

            if (lines[currentPos].Contains("internal correlationtype "))
            {
                // TODO
                // internal correlationtype CorrelationType_1
                // {
                //          prop1, prop2
                // };
                string correlationTypeName = lines[currentPos].Replace("internal correlationtype ", "").Replace("public correlationtype ", "").Replace("private correlationtype ", "");
                string correlationProperties = lines[currentPos + 2];
                string[] props = correlationProperties.Split(',');

                parsed.CorrelationTypes.Add(correlationTypeName, props);

                currentPos += 3;
            }

            while (!lines[currentPos].Contains(" service "))
            {
                currentPos++;
            }

            // internal class longrunning transaction
            if (lines[currentPos].Contains("internal service longrunning transaction "))
            {
                parsed.OrchestrationName = lines[currentPos].Replace("internal service longrunning transaction ", "");
            }
            else if (lines[currentPos].Contains("internal service atomic transaction "))
            {
                parsed.OrchestrationName = lines[currentPos].Replace("internal service atomic transaction ", "");
            }
            else if (lines[currentPos].Contains("public service longrunning transaction "))
            {
                parsed.OrchestrationName = lines[currentPos].Replace("public service longrunning transaction ", "");
            }
            else if (lines[currentPos].Contains("public service atomic transaction "))
            {
                parsed.OrchestrationName = lines[currentPos].Replace("public service atomic transaction ", "");
            }
            else
            {
                parsed.OrchestrationName = lines[currentPos].Replace("internal service ", "").Replace("public service ", "");
            }
            currentPos += 2;

            for (int j = currentPos; j < lines.Count() - 3; j++)
            {
                if (!lines[j].StartsWith("["))
                {
                    parsed.OrchestrationCode.Add(lines[j]);
                }
            }

            // Normalize ports
            foreach (PortType p in parsed.PortTypes)
            {
                foreach (PortOperation o in p.PortOperations)
                {
                    int x = 0;
                    foreach (string s in o.Schemas)
                    {
                        if (s.Contains("="))
                        {
                            string[] parts = s.Split('=');

                            SchemaOperation op = new SchemaOperation();
                            op.Direction = parts[0].Trim();
                            op.MessageType = parts[1].Trim();
                            o.Operations.Add(op);
                        }
                        else
                        {
                            SchemaOperation op = new SchemaOperation();
                            op.MessageType = s;

                            if (x == 0) op.Direction = "Request";
                            if (x == 1) op.Direction = "Response";

                            o.Operations.Add(op);
                        }

                        x++;
                    }
                }
            }


            // Transform xlang to code

            for (int w = 0; w < parsed.OrchestrationCode.Count(); w++)
            {                                
                if (parsed.OrchestrationCode[w].StartsWith("correlation ")) parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("correlation ", "");
                if (parsed.OrchestrationCode[w].StartsWith("suspend ")) { parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("suspend ", "suspend("); parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Remove(parsed.OrchestrationCode[w].Count() - 1); parsed.OrchestrationCode[w] += ");"; }
                if (parsed.OrchestrationCode[w].StartsWith("construct ")) parsed.OrchestrationCode[w] = "";
                if (parsed.OrchestrationCode[w].StartsWith("scope longrunning transaction ")) { parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("scope longrunning transaction ", "LongRunningTransaction "); parsed.OrchestrationCode[w] += " = new LongRunningTransaction(Transaction);"; }
                if (parsed.OrchestrationCode[w].StartsWith("scope atomic transaction ")) { parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("scope longrunning transaction ", "AtomicTransaction "); parsed.OrchestrationCode[w] += " = new AtomicTransaction(Transaction);"; }
                if (parsed.OrchestrationCode[w] == "scope") parsed.OrchestrationCode[w] = "";
                if (parsed.OrchestrationCode[w] == "parallel") parsed.OrchestrationCode[w] = "";
                if (parsed.OrchestrationCode[w] == "task") parsed.OrchestrationCode[w] = "";
                if (parsed.OrchestrationCode[w] == "body") parsed.OrchestrationCode[w] = "try";
                if (parsed.OrchestrationCode[w].Contains(", initialize ")) { parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(", initialize ", ", out "); }
                if (parsed.OrchestrationCode[w].Contains(" = new Microsoft.RuleEngine.Policy(\"")) parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("\");", "\", PolicyManager);");

                if (parsed.OrchestrationCode[w].StartsWith("call ") || parsed.OrchestrationCode[w].StartsWith("exec "))
                { 
                    // call func(parameters);
                    //  Add a static 
                    // code ........
                    // func(parameter);
                }

                if (parsed.OrchestrationCode[w] == "listen")
                {
                    // TODO listen
                    // listen
                    // {
                    //      until activate receive (op, msg)
                    //      {
                    //      }
                    //      timeout new System.TimeSpan(1)
                    //      {
                    //      }
                    // }
                    //  code ....
                    // {
                    //      receiveuntil (op, out msg, () =>
                    //      {
                    //      }
                    //      , new System.TimeStamp(1), () =>
                    //      {
                    //      }
                    //      );
                    // }
                }

                if (parsed.OrchestrationCode[w].StartsWith("xpath"))
                { 
                    // TODO xpath assignment
                    // xpath (msg, xpath) = value;
                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(") = ", ", ");
                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Remove(parsed.OrchestrationCode[w].Length - 1) + ");";
                    // xpath (msg, xpath, value);
                }

                if (parsed.OrchestrationCode[w].StartsWith("activate "))
                {
                    int pos = parsed.OrchestrationCode[w].IndexOf("receive (");
                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Substring(pos);
                }

                if (parsed.OrchestrationCode[w].StartsWith("body ("))
                {
                    if (parsed.OrchestrationCode[w] == "body ()")
                    {
                        parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("body ()", "public void Call()");
                    }
                    else
                    {
                        string[] tokens = parsed.OrchestrationCode[w].Replace("body (", "").Replace(")", "").Split(',');
                        
                        string parameters = "";

                        foreach (string token in tokens)
                        {
                            string[] parts = token.Trim().Split(' ');

                            string refOut = null;
                            string type = null;
                            int typeIndex = 0;

                            if (parts[0] == "ref" || parts[0] == "out")
                            {
                                refOut = parts[0];
                                typeIndex = 1;
                            }
                            else
                            {
                                typeIndex = 0;
                            }

                            type = parts[typeIndex];

                            if (type == "message")
                            {
                                // [out|ref] 'message' msgtype msgname
                                string msgtype = parts[typeIndex + 1].Split('.')[parts[typeIndex + 1].Split('.').Count() - 1];
                                if (msgtype == "System.Xml.XmlDocument") msgtype = "MessageBase";
                                if (msgtype == "XmlDocument") msgtype = "MessageBase";

                                parameters += refOut + " " + msgtype + " " + parts[typeIndex + 2] + ",";
                            }
                            else if (type == "port")
                            {
                                // [out|ref] 'port implements' porttype portname
                                // [out|ref] 'port uses' porttype portname
                                parameters += refOut + " " + parts[typeIndex + 2] + " " + parts[typeIndex + 3] + ",";
                            }
                            else if (type == "correlation")
                            {
                                // [out|ref] 'correlation' correlationtype correlationame
                                parameters += refOut + " " + parts[typeIndex + 1] + " " + parts[typeIndex + 2] + ",";
                            }
                            else if (type == "rolelink")
                            {
                                // TODO
                                parameters += refOut + " object role,";
                            }
                            else
                            {
                                // [out|ref] type varname    
                                parameters += token + ",";
                            }

                            parsed.OrchestrationCode[w] = "public void Call(" + parameters.Remove(parameters.Length-1) + ")";
                        }
                    }
                }

                if (parsed.OrchestrationCode[w].StartsWith("terminate "))
                {
                    string terminateDesc = parsed.OrchestrationCode[w].Replace("terminate ", "");
                    terminateDesc = terminateDesc.Remove(terminateDesc.Length - 1);

                    parsed.OrchestrationCode[w] = "throw new TerminateException(" + terminateDesc + ");";
                }

                if (parsed.OrchestrationCode[w].StartsWith("delay "))
                {
                    string delay = parsed.OrchestrationCode[w].Replace("delay ", "");
                    delay = delay.Remove(delay.Length - 1);

                    parsed.OrchestrationCode[w] = "delay(" + delay + ");";
                }

                // transform (MediaIntegrationResponseMsg) = Aegis.Integration.MarketServices.DefaultEndpoint.Maps.MediaIntegrationRequestToMediaIntegrationResponse (MediaIntegrationRequestMsg);
                // transform (Message_1, Message_2) = BizTalk_Server_Project1.Transform_2 (Message_1);
                if (parsed.OrchestrationCode[w].StartsWith("transform ("))
                {
                    //parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("transform (", ""); parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(") =", " ="); 

                    string parameters = parsed.OrchestrationCode[w].Replace("transform", "");
                    parameters = parameters.Replace("(", "").Replace(")", "");
                    parameters = parameters.Replace(";", "");
                    parameters = parameters.Replace(" = ", " ");
                    parameters = parameters.Replace(",", "");

                    string[] parts = parameters.Trim().Split(' ');
                    int count = parts.Count();

                    parsed.OrchestrationCode[w] = "transform (out " + parts[0] + ", \"" + parts[1] + "\", ";

                    for (int ii = 2; ii < count; ii++)
                    {
                        parsed.OrchestrationCode[w] += parts[ii] + ", ";
                    }

                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Remove(parsed.OrchestrationCode[w].Count() - 2);
                    parsed.OrchestrationCode[w] += ");";
                }

                if (parsed.OrchestrationCode[w].StartsWith("port implements "))
                {
                    string[] parts = parsed.OrchestrationCode[w].Split(' ');
                    string portType = parts[2];

                    foreach (var p in parsed.PortTypes)
                    {
                        if (p.TypeName == portType)
                        {
                            p.PortName = parts[3].Remove(parts[3].Length - 1);
                            p.Direction = PortDirection.ReceiveSend;
                        }
                    }

                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("port implements ", "public ");
                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Remove(parsed.OrchestrationCode[w].Count()-1);
                    parsed.OrchestrationCode[w] += " = new " + portType + "();";
                }

                if (parsed.OrchestrationCode[w].StartsWith("port uses "))
                {
                    string[] parts = parsed.OrchestrationCode[w].Split(' ');
                    string portType = parts[2];

                    foreach (var p in parsed.PortTypes)
                    {
                        if (p.TypeName == portType)
                        {
                            p.PortName = parts[3].Remove(parts[3].Length - 1);
                            p.Direction = PortDirection.SendReceive;
                        }
                    }

                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("port uses ", "public ");
                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Remove(parsed.OrchestrationCode[w].Count() - 1);
                    parsed.OrchestrationCode[w] += " = new " + portType + "();";
                }

                if (parsed.OrchestrationCode[w].StartsWith("receive "))
                {
                    string operationName = parsed.OrchestrationCode[w].Replace("receive (", "");
                    operationName = operationName.Split(',')[0];
                    string portName = operationName.Split('.')[0];

                    if (parsed.OrchestrationCode[w].Split('.').Count() == 2)
                    {
                        foreach (var p in parsed.PortTypes)
                        {
                            if (p.PortName == portName)
                            {
                                if (p.Direction == PortDirection.ReceiveSend)
                                {
                                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(operationName + ", ", operationName + ".Request, out ");
                                }
                                else
                                {
                                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(operationName + ", ", operationName + ".Response, out ");
                                }
                            }
                        }
                    }
                }

                if (parsed.OrchestrationCode[w].Contains("receive "))
                {
                    int pos = parsed.OrchestrationCode[w].IndexOf("receive");

                    if (pos != 0)
                    {
                        parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Substring(pos);
                    }
                }

                if (parsed.OrchestrationCode[w].StartsWith("send "))
                {
                    string operationName = parsed.OrchestrationCode[w].Replace("send (", "");
                    operationName = operationName.Split(',')[0];
                    string portName = operationName.Split('.')[0];

                    if (parsed.OrchestrationCode[w].Split('.').Count() == 2)
                    {
                        foreach (var p in parsed.PortTypes)
                        {
                            if (p.TypeName.StartsWith(portName))
                            {
                                if (p.Direction == PortDirection.ReceiveSend)
                                {
                                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(operationName + ", ", operationName + ".Response, ");
                                }
                                else
                                {
                                    parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(operationName + ", ", operationName + ".Request, ");
                                }
                            }
                        }
                    }
                }

                if (parsed.OrchestrationCode[w].StartsWith("message "))
                {
                    if (parsed.OrchestrationCode[w].StartsWith("message System.Xml.XmlDocument "))
                    {
                        parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace("message System.Xml.XmlDocument ", "MessageBase ");
                        parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Remove(parsed.OrchestrationCode[w].Count() -1);
                        parsed.OrchestrationCode[w] += " = null;";

                        string[] parts = parsed.OrchestrationCode[w].Split(' ');

                        parsed.Messages.Add(new Message() { Name = parts[1], Type = "MessageBase" });
                    }
                    else
                    {
                        string[] parts = parsed.OrchestrationCode[w].Split(' ');

                        string name = parts[2].Remove(parts[2].Length-1);

                        string type = parts[1].Split('.')[parts[1].Split('.').Count() - 1];

                        parsed.Messages.Add(new Message() { Name = name, Type = type });

                        parsed.OrchestrationCode[w] = type + " " + name + " = null;";
                    }
                }

                if (parsed.OrchestrationCode[w].StartsWith("catch ("))
                {
                    if (!parsed.OrchestrationCode[w].Contains("System."))
                    {
                        string[] parts = parsed.OrchestrationCode[w].Split(' ');

                        parsed.OrchestrationCode[w] = "catch (Fault " + parts[2];
                    }
                }
            }

            //for (int ww = 0; ww < 3; ww++)
            {
                for (int w = 0; w < parsed.OrchestrationCode.Count(); w++)
                {
                    foreach (Message msg in parsed.Messages)
                    {
                        if (parsed.OrchestrationCode[w].Contains(" exists " + msg.Name))
                        {
                            // ContextPropertyName exists msg_1
                            parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(" exists " + msg.Name, " | " + msg.Name);
                            // ContextPropertyName | msg_1
                        }

                        if (parsed.OrchestrationCode[w].Contains(msg.Name + "("))
                        {
                            if (parsed.OrchestrationCode[w].Contains(msg.Name + "(*) ="))
                            {
                                // msg_1(*) = msg_2(*);
                                parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(msg.Name + "(*) = ", msg.Name + ".CopyAllProperties(").Replace("(*);", ");");
                                // msg_1.CopyAllProperties(msg_2(*);
                            }
                            else if (parsed.OrchestrationCode[w].Contains(msg.Name + "(*);"))
                            {
                                // msg_1.CopyAllProperties(msg_2(*);
                                parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(msg.Name + "(*);", msg.Name + ");");
                                // msg_1.CopyAllProperties(msg_2);
                            }
                            else if (parsed.OrchestrationCode[w].Contains(") = "))
                            {
                                // msg_1(BTS.InterchangeID) = "aaaa";
                                parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(msg.Name + "(", msg.Name + ".WriteProperty(").Replace(") = ", ", ").Replace(";", ");");
                                // msg_1.WriteProperty(BTS.InterchangeID, "aaaa");
                            }
                            else
                            {
                                // var_1 = msg_1(BTS.InterchangeID);
                                parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(msg.Name + "(", msg.Name + ".ReadProperty(");
                                // var_1 = msg_1.ReadProperty(BTS.InterchangeID);
                            }

                        }

                        if (parsed.OrchestrationCode[w].StartsWith(msg.Name + " = "))
                        {
                            // msg_1 = msg_2;
                            parsed.OrchestrationCode[w] = parsed.OrchestrationCode[w].Replace(msg.Name + " = ", msg.Name + " = (" + msg.Type + ")");
                            // msg_1 = (cast)msg_2;
                        }
                    }
                }
            }



            // Generate code

            StringBuilder code = new StringBuilder();

            code.AppendLine("using System;");
            code.AppendLine("using System.Collections.Generic;");
            code.AppendLine("using System.Linq;");
            code.AppendLine("using System.Text;");
            code.AppendLine("using System.Threading.Tasks;");
            code.AppendLine("using System.Web.Services.Protocols;");
            code.AppendLine("using System.Xml;");
            code.AppendLine("using Microsoft.XLANGs.BaseTypes;");
            code.AppendLine("using BTSCodeGen.BizTalkBase;");
            code.AppendLine();
            code.AppendLine("namespace " + parsed.Namespace);
            code.AppendLine("{");

            // Ports
            foreach (PortType port in parsed.PortTypes)
            {
                foreach (PortOperation op in port.PortOperations)
                {
                    code.AppendLine("   public class Operation" + op.OperationName);
                    code.AppendLine("   {");
                    foreach (var o in op.Operations)
                    {
                        code.AppendLine("       public Operation " + o.Direction + " = new Operation();");
                    }
                    code.AppendLine("   }");
                }

                code.AppendLine("   public class " + port.TypeName);
                code.AppendLine("   {");
                foreach (PortOperation op in port.PortOperations)
                {
                    code.AppendLine("       public Operation" + op.OperationName + " " + op.OperationName + " = new Operation" + op.OperationName + "();");
                }
                code.AppendLine("   }");
            }

            List<string> done = new List<string>();
            foreach (Message message in parsed.Messages)
            {
                if (done.Contains(message.Type))
                {
                }
                else
                {

                    if (message.Type == "MessageBase")
                    {
                        done.Add("MessageBase");
                    }
                    else
                    {
                        code.AppendLine("   public partial class " + message.Type + " : MessageBase");
                        code.AppendLine("   {");

                        code.AppendLine("   }");

                        done.Add(message.Type);
                    }
                }

            }

            foreach (string key in parsed.CorrelationTypes.Keys)
            {
                code.AppendLine("   public class " + key + " : CorrelationBase");
                code.AppendLine("   {");
                code.AppendLine("       public " + key + "()");
                code.AppendLine("       {");
                foreach (string p in parsed.CorrelationTypes[key])
                {
                    code.AppendLine("           Properties.Add(\"" + p.Trim() + "\");");
                }
                code.AppendLine("       }");
                code.AppendLine("   }");
            }

            code.AppendLine("");
            
            // Code

            code.AppendLine("   internal class " + parsed.OrchestrationName + " : OrchestrationBase"); // TODO derive by Orch
            code.AppendLine("   {");

            code.AppendLine("      public Transaction Transaction = new Transaction();");
            code.AppendLine("      public PolicyManager PolicyManager = new PolicyManager();");

            foreach (string line in parsed.OrchestrationCode)
            {
                code.AppendLine("      " + line);
            }

            code.AppendLine("   }");

            code.AppendLine("}");

            // TODO indent

            File.WriteAllText(csFileName, code.ToString());            
        }
    }

    public class Parsed
    {
        public string Namespace;
        public List<PortType> PortTypes = new List<PortType>();
        public string OrchestrationName;
        public List<string> OrchestrationCode = new List<string>();
        public List<Message> Messages = new List<Message>();
        public Dictionary<string, string[]> CorrelationTypes = new Dictionary<string, string[]>();
    }

    public class Message
    {
        public string Name;
        public string Type;
    }

    public class PortType
    {
        public string TypeName;
        public string PortName;
        public List<PortOperation> PortOperations = new List<PortOperation>();
        public PortDirection Direction;
    }

    public class PortOperation
    {
        public string OperationName;
        public string[] Schemas;
        public List<SchemaOperation> Operations = new List<SchemaOperation>();
    }

    public class SchemaOperation
    {
        public string Direction;
        public string MessageType;

    }

    public enum PortDirection
    {
        ReceiveSend,
        SendReceive
    }

}
