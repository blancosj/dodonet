using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.CodeDom.Compiler;

namespace DodoNet.JScript
{
    public class EvaluatorBuilder
    {
        Microsoft.JScript.JScriptCodeProvider provider;
        CodeCompileUnit unit;
        CodeNamespace ns;
        List<string> _assemblies = new List<string>();

        public EvaluatorBuilder() { }

        public void BeginCompile(string nameSpace)
        {
            provider = new Microsoft.JScript.JScriptCodeProvider();

            unit = new CodeCompileUnit();
            ns = new CodeNamespace(nameSpace);
            unit.Namespaces.Add(ns);
        }

        public CodeTypeDeclaration AppendClass(string className, Type baseType)
        {
            CodeTypeDeclaration profileClass = new CodeTypeDeclaration(className);
            CodeTypeReference cref = new CodeTypeReference(baseType);
            profileClass.BaseTypes.Add(cref);
            profileClass.TypeAttributes = TypeAttributes.Public;
            ns.Types.Add(profileClass);
            return profileClass;
        }

        public CodeMemberMethod AppendMethod(CodeTypeDeclaration profileClass, string methodName, MemberAttributes atts)
        {
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = methodName;
            method.Attributes = MemberAttributes.Public | atts;
            method.ReturnType = new CodeTypeReference(typeof(int));
            profileClass.Members.Add(method);
            return method;
        }

        public string CreateFieldForObject(CodeTypeDeclaration profileClass, Type type, string fieldName)
        {
            CodeMemberField f = new CodeMemberField(type, fieldName);
            f.Attributes = MemberAttributes.Private;
            profileClass.Members.Add(f);
            return fieldName;
        }

        public void CreatePropertyGetForObject(CodeTypeDeclaration profileClass, Type type, string propName, string code)
        {
            CodeMemberProperty prop = new CodeMemberProperty();
            prop.Type = new CodeTypeReference(type);
            prop.Name = propName;
            prop.Attributes = MemberAttributes.Public;
            CodeCastExpression cast = new CodeCastExpression(prop.Type, new CodeSnippetExpression(code));
            prop.GetStatements.Add(new CodeMethodReturnStatement(cast));
            profileClass.Members.Add(prop);
        }

        public void AppendCodeExpression(CodeMemberMethod method, string functionName, params object[] args)
        {
            CodePrimitiveExpression[] exprs = new CodePrimitiveExpression[args.Length];
            for (int x = 0; x < args.Length; x++)
            {
                exprs[x] = new CodePrimitiveExpression();
                exprs[x].Value = args[x];
            }

            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression(
                new CodeThisReferenceExpression(),
                functionName, exprs);

            method.Statements.Add(methodInvoke);
        }

        public void AppendCodeIfAndReturn(CodeMemberMethod method, string condition)
        {
            CodeMethodReturnStatement r = new CodeMethodReturnStatement(new CodePrimitiveExpression(0));

            CodeConditionStatement c =
                new CodeConditionStatement(new CodeSnippetExpression(condition), r);

            method.Statements.Add(c);
        }

        public void AppendCodeSnippetExpression(CodeMemberMethod method, string code)
        {
            CodeSnippetExpression snippetExpresion = new CodeSnippetExpression(code);
            method.Statements.Add(snippetExpresion);
        }

        public void AppendNameSpace(string nameSpace)
        {
            ns.Imports.Add(new CodeNamespaceImport(nameSpace));
        }

        public void AppendAssembly(string assembly)
        {
            _assemblies.Add(assembly);
        }

        void CheckCompilerErrors(CompilerResults results)
        {
            CompilerErrorCollection errors = results.Errors;
            // results.NativeCompilerReturnValue == 0 && 
            if (errors.Count > 0)
            {
                string fileText = null;
                CompilerError ce = (errors != null && errors.Count > 0) ? errors[0] : null;
                string inFile = (ce != null) ? ce.FileName : null;

                if (inFile != null && File.Exists(inFile))
                {
                    using (StreamReader sr = File.OpenText(inFile))
                    {
                        fileText = sr.ReadToEnd();
                    }
                }
                else if (ce != null && ce.Line == 0)
                {
                    throw new Exception("Compilation error: " + ce);
                }
                else
                {
                    StringWriter writer = new StringWriter();
                    provider.GenerateCodeFromCompileUnit(unit, writer, null);
                    fileText = writer.ToString();
                }

                throw new CompilationException(ce.FileName, errors, fileText);
            }
        }

        protected string DynamicDir()
        {
            return Path.GetTempPath();
        }

        public Assembly EndCompile()
        {
            Assembly ret = null;

            CompilerParameters compilerparams = new CompilerParameters();
            compilerparams.GenerateInMemory = false;
            compilerparams.GenerateExecutable = false;
            compilerparams.IncludeDebugInformation = true;
            compilerparams.CompilerOptions = "/t:library";

            TempFileCollection tempcoll = new TempFileCollection(DynamicDir(), true);
            compilerparams.TempFiles = tempcoll;
            string dllfilename = Path.GetFileName(tempcoll.AddExtension("dll", true));
            compilerparams.OutputAssembly = Path.Combine(DynamicDir(), dllfilename);

            // cargamos los assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assem in assemblies)
            {
                try
                {
                    if (Path.GetExtension(assem.Location).EndsWith("dll") &&
                        !compilerparams.ReferencedAssemblies.Contains(assem.Location))
                    {
                        compilerparams.ReferencedAssemblies.Add(assem.Location);
                    }
                }
                catch { }
            }

            foreach (string assem in _assemblies)
            {
                try
                {
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assem);
                    if (!compilerparams.ReferencedAssemblies.Contains(filePath))
                        compilerparams.ReferencedAssemblies.Add(filePath);
                }
                catch { }
            }

            CompilerResults results = provider.CompileAssemblyFromDom(compilerparams, unit);
            CheckCompilerErrors(results);
            results.TempFiles.Delete();
            ret = results.CompiledAssembly;
            return ret;
        }
    }
}
