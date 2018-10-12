using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anti_Debug_Remover
{
    internal static class F1nder
    {
        internal static int Run(this MethodDef method, OpCode opCode, object operand)
        {
            var num = 0;
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode != opCode)
                    continue;
                if (operand is int)
                {
                    var value = instruction.GetLdcI4Value();
                    if (value == (int)operand)
                        num++;
                }
                else if (operand is string)
                {
                    var value = instruction.Operand.ToString();
                    if (value.Contains(operand.ToString()))
                        num++;
                }
            }
            return num;
        }

    }
}
