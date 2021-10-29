using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BTSCodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            //if (args.Count() != 2)
            //{
            //    throw new Exception("Invalid parameters, expected Orchestration file name and cs file name");
            //}
            //else
            //{
            //    BTSCodeGen.CodeGen.GenCSFromODX(args[0], args[1]);
            //}

            //Gen("BizTalk Orchestration1");
            //Gen("CapitaOne");
            //Gen("CaseHandling");
            //Gen("DataQualityReporting");
            //Gen("DataQualityCheck");

            Gen("PersonProcess");
            Gen("ODMUpdateModule");
            Gen("RetrieveUPRN");
        }

        public static void Gen(string orchName)
        {
            BTSCodeGen.CodeGen.GenCSFromODX("..\\..\\..\\GeneratedCodeUnitTests\\Orchs\\" + orchName + ".odx", "..\\..\\..\\GeneratedCodeUnitTests\\GenCode\\" + orchName + ".cs");
        }
    }
}
