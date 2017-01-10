using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.JScript;

namespace DodoNet.JScript
{
    /// <summary>
    /// motor para ejecutar macro-compilar código
    /// </summary>
    public class EvaluatorEngine : ActivationObject, IDisposable
    {
        Microsoft.JScript.Vsa.VsaEngine engine;
        StringBuilder output;
        TypeReflector typeReflector;

        Dictionary<string, object> vars;
        object sync = new object();

        List<string> imports;

        public EvaluatorEngine()
            : base(null)
        {
            vars = new Dictionary<string, object>();

            this.typeReflector = TypeReflector.GetTypeReflectorFor(Globals.TypeRefs.ToReferenceContext(this.GetType()));

            engine = new Microsoft.JScript.Vsa.VsaEngine();
            engine.doFast = false;
            output = new StringBuilder();

            StringWriter sw = new StringWriter(output);
            engine.InitVsaEngine("executerjscript.net", new VsaSite(sw));

            imports = new List<string>();

            List<string> asses = new List<string>();
            foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    VsaReference reference = new VsaReference(engine, Path.GetFileNameWithoutExtension(assem.Location));
                    engine.Items.CreateItem(reference.AssemblyName, Microsoft.Vsa.VsaItemType.Reference, Microsoft.Vsa.VsaItemFlag.None);

                    if (Path.GetExtension(assem.EscapedCodeBase).Equals(".dll", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string tmp = Path.GetFileNameWithoutExtension(assem.EscapedCodeBase);
                        if (!imports.Contains(tmp))
                        {
                            Import.JScriptImport(tmp, engine);
                        }
                    }

                    Import.JScriptImport("System.Net", engine);
                    Import.JScriptImport("System.IO", engine);
//using System;
//using System.IO;
//using System.Net;
//using System.Text;
                }
                catch { }
            }
        }

        public void AddNewField(string name, object value)
        {
            this.AddFieldOrUseExistingField(name, value, System.Reflection.FieldAttributes.Public);
        }

        public object Eval(string scriptCode, EvaluatorContext context)
        {
            // añadir propiedades del contexto
            context.AddFieldsAndProperties(this);
            return Eval(scriptCode);
        }

        public object Eval(string scriptCode)
        {
            object ret = null;
            try
            {
                this.SetParent((ScriptObject)engine.globalScope.GetObject());

                StackFrame sf = new StackFrame(this, null, new object[0], this);
                engine.Globals.ScopeStack.Push(sf);

                Context context = new Context(new DocumentContext("eval code", engine), ((IConvertible)scriptCode).ToString());
                JSParser p = new JSParser(context);

                Block b = p.ParseEvalBody();
                AST a = b.PartiallyEvaluate();

                Completion result = (Completion)a.Evaluate();

                ret = ((Completion)result).value;
            }
            finally
            {
                engine.Globals.ScopeStack.Pop();
            }

            return ret;
        }

        public override object GetDefaultThisObject()
        {
            return base.GetDefaultThisObject();
        }

        public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        {
            MemberInfo[] result = null;

            FieldInfo field = (FieldInfo)(this.name_table[name]);
            if (field == null)
            {
                //Look for global variables represented as static fields on subclass of GlobalScope. I.e. the script block case.
                result = this.typeReflector.GetMember(name, bindingAttr & ~BindingFlags.NonPublic | BindingFlags.Static);
                int n = result.Length;
                if (n > 0)
                {
                    int toBeHidden = 0;
                    MemberInfo[] newResult = new MemberInfo[n];
                    for (int i = 0; i < n; i++)
                    {
                        MemberInfo mem = newResult[i] = result[i];
                        if (mem.DeclaringType.IsAssignableFrom(Typeob.GlobalScope))
                        {
                            newResult[i] = null; toBeHidden++;
                        }
                        else if (mem is FieldInfo)
                        {
                            field = (FieldInfo)mem;
                            if (field.IsStatic && field.FieldType == Typeob.Type)
                            {
                                Type t = (Type)field.GetValue(null);
                                if (t != null)
                                    newResult[i] = t;
                            }
                        }
                    }
                    if (toBeHidden == 0) return result;
                    if (toBeHidden == n) return new MemberInfo[0];
                    MemberInfo[] remainingMembers = new MemberInfo[n - toBeHidden];
                    int j = 0;
                    foreach (MemberInfo mem in newResult)
                        if (mem != null)
                            remainingMembers[j++] = mem;
                    return remainingMembers;
                }
            }
            else
            {
                result = new MemberInfo[] { field };
            }
            return result;
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            MemberInfoList result = new MemberInfoList();

            MemberInfo[] mems = Globals.TypeRefs.ToReferenceContext(this.GetType()).GetMembers(bindingAttr | BindingFlags.DeclaredOnly);
            if (mems != null)
                foreach (MemberInfo mem in mems)
                    result.Add(mem);

            return result.ToArray();
        }

        internal override void SetMemberValue(string name, object value)
        {
            MemberInfo[] members = this.GetMember(name, BindingFlags.Instance | BindingFlags.Public);

            if (members.Length == 0)
            {
                lock (sync)
                {
                    if (vars.ContainsKey(name))
                        vars.Add(name, value);
                }
            }
            else
                LateBinding.SetMemberValue(this, name, value, LateBinding.SelectMember(members), members);
        }

        internal override object GetMemberValue(string name)
        {
            MemberInfo[] members = this.GetMember(name, BindingFlags.Instance | BindingFlags.Public);
            if (members.Length == 0)
            {
                lock (sync)
                {
                    if (vars.ContainsKey(name))
                    {
                        return vars[name];
                    }
                    else
                        return Microsoft.JScript.Missing.Value;
                }
            }
            return LateBinding.GetMemberValue(this, name, LateBinding.SelectMember(members), members);
        }

        public override FieldInfo GetField(string name, int lexLevel)
        {
            return this.GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {
            engine.Close();
        }

        #endregion
    }
}
