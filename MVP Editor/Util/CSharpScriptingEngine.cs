using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Xna.Framework;
using MVP_Core.Entities;
using MVP_Core.Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVP_Editor.Util
{
    public class CSharpScriptingEngine
    {
        private static ScriptState scriptState = null;
        private static Script script = null;
        private static Script up = null;

        public static Assembly Compile(out List<string> errors, params string[] sources)
        {
            var assemblyFilename = "gen" + Guid.NewGuid().ToString().Replace("-", "") + ".dll";
            var compilation = CSharpCompilation.Create(assemblyFilename,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                syntaxTrees: from source in sources
                             select CSharpSyntaxTree.ParseText(source),
                references: new[]
                {
                    MetadataReference.CreateFromFile((typeof(object).Assembly.Location)),
                    MetadataReference.CreateFromFile((typeof(RuntimeBinderException).Assembly.Location)),
                    MetadataReference.CreateFromFile((typeof(System.Runtime.CompilerServices.DynamicAttribute).Assembly.Location)),
                    MetadataReference.CreateFromFile((typeof(MVPScripts).Assembly.Location)),
                    //MetadataReference.CreateFromFile((typeof(Extensions).Assembly.Location)),
                    MetadataReference.CreateFromFile((typeof(Vector2).Assembly.Location)),
                    MetadataReference.CreateFromFile((typeof(Tile).Assembly.Location))
                });

            EmitResult emitResult;

            using (var ms = new MemoryStream())
            {
                emitResult = compilation.Emit(ms);

                if(emitResult.Success)
                {
                    var assembly = Assembly.Load(ms.GetBuffer());
                    errors = new List<string>();
                    return assembly;
                }
            }

            errors = emitResult.Diagnostics.Where(r => r.DefaultSeverity == DiagnosticSeverity.Error).Select(r => r.ToString()).ToList();
            var message = string.Join("\r\n", emitResult.Diagnostics);
            return null;
        }
    }
}
