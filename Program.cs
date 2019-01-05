using System;
using System.Collections.Generic;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using dnlib;
using dnlib.DotNet.Writer;

namespace Anti_Debug_Remover
{
    static class Program
    {
        static int Run(this MethodDef method)
        {
            var i = 0;
            foreach (var instruction in method.Body.Instructions)
            {
                if (instruction.OpCode != OpCodes.Ldstr) continue;
                var value = instruction.Operand.ToString();
                if (value.Contains("GetEnvironmentVariable")) i++;
            }
            return i;
        }
        static void Main(string[] args)
        {
            try
            {
                ModuleDefMD module = ModuleDefMD.Load(args[0]);
                Console.WriteLine("Assembly Loaded !");
                var instruçoes = module.GlobalType.FindStaticConstructor().Body.Instructions;
                foreach (var instruction in instruçoes)
                {
                    if (instruction.OpCode != OpCodes.Call)
                        continue;
                    var Metodo = instruction.Operand as MethodDef;
                    if (Metodo == null)
                        continue;
                    if (!Metodo.DeclaringType.IsGlobalModuleType)
                        continue;
                    if (Metodo.Run() != 1)
                        continue;
                    instruction.OpCode = OpCodes.Nop;
                    instruction.Operand = null;
                    ModuleWriterOptions writerOptions = new ModuleWriterOptions(module);
                    writerOptions.MetaDataOptions.Flags |= MetaDataFlags.PreserveAll;
                    writerOptions.Logger = DummyLogger.NoThrowInstance;
                    module.Write(args[0].Replace(".exe", "_DeobF.exe"), writerOptions);
                    Console.WriteLine("Saved !");
                    Console.ReadLine();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error In load Assembly !");
                Console.ReadLine();
            }
        }

    }
}
