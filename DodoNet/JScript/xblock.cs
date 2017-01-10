using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Microsoft.JScript;

namespace xjscript
{
    [Serializable]
    public class xast
    {   
        public xast parser(AST ast)
        {
            xast ret = null;

            if (ast == null)
            {
            }
            else if (ast is VariableDeclaration)
            {
                ret = new xvardeclare((VariableDeclaration)ast);
            }
            else if (ast is Assign)
            {
                ret = new xassing((Assign)ast);
            }
            else if (ast is If)
            {
                ret = new xif((If)ast);
            }
            else if (ast is While)
            {
                ret = new xwhile((While)ast);
            }
            else if (ast is Expression)
            {
                ret = new xexpr((Expression)ast);
            }
            else if (ast is Block)
            {
                ret = new xblock((Block)ast);
            }
            else if (ast is ConstantWrapper)
            {
                ret = new xconstantwrapper((ConstantWrapper)ast);
            }
            else if (ast is For)
            {
                ret = new xfor((For)ast);
            }
            else if (ast is Call)
            {
                ret = new xcall((Call)ast);
            }
            else if (ast is Equality)
            {
                ret = new xequality((Equality)ast);
            }
            else if (ast is Relational)
            {
                ret = new xrelational((Relational)ast);
            }
            else if (ast is PostOrPrefixOperator)
            {
                ret = new xpostorprefixoperator((PostOrPrefixOperator)ast);
            }
            else if (ast is Lookup)
            {
                ret = new xlookup((Lookup)ast);
            }
            else if (ast is Call)
            {
                ret = new xcall((Call)ast);
            }
            else if (ast is ASTList)
            {
                ret = new xastlist((ASTList)ast);
            }
            return ret;
        }
    }

    [Serializable]
    public class xcompletion
    {
        bool cont;
        bool exit;
        bool ret;
        object value;
    }

    [Serializable]
    public class xastlist : xast
    {
        List<xast> list = new List<xast>();

        internal xastlist(ASTList al)
        {
            foreach (AST item in al.list)
            {
                list.Add(parser(item));
            }
        }
    }

    [Serializable]
    public class xblock : xast
    {
        public xcompletion completion;
        public List<xast> cmdlist = new List<xast>();        

        public static xblock GetCodeBlock(string code)
        {
            xblock ret = null;

            Microsoft.JScript.Vsa.VsaEngine engine = new Microsoft.JScript.Vsa.VsaEngine();
            StringWriter sw = new StringWriter();
            engine.InitVsaEngine("test", new VsaSite(sw));
            DocumentContext docContext = new DocumentContext("", engine);
            Context context = new Context(docContext, code);
            JSParser parser = new JSParser(context);
            Block block = parser.ParseEvalBody();
            ret = new xjscript.xblock(block);

            engine.Close();
             // MessageBox.Show(((Completion)block.Evaluate()).value.ToString());

            return ret;
        }

        public xblock(Block block)
        {
            foreach (AST item in block.List)
            {
                cmdlist.Add(parser(item));
            }
        }
    }

    [Serializable]
    public class xcall : xast
    {        
        xast func;
        xastlist args;

        internal xcall(Call c)
        {
            func = parser(c.func);
            args = parser(c.args) as xastlist;
        }
    }

    [Serializable]
    public class xwhile : xast
    {
        xast condition;
        xast body;

        internal xwhile(While w)
        {
            condition = parser(w.condition);
            body = parser(w.body);            
        }
    }

    [Serializable]
    public class xfor : xast
    {
        xast initializer;
        xast incrementer;
        xast condition;
        xast body;

        internal xfor(For f)
        {
            initializer = parser(f.initializer);
            condition = parser(f.condition);
            incrementer = parser(f.incrementer);            
            body = parser(f.body);            
        }
    }

    [Serializable]
    public class xvardeclare : xast
    {        
        xlookup identifier;
        xast initializer;
        xcompletion completion;

        internal xvardeclare(VariableDeclaration vd)
        {
            identifier = (xlookup)parser(vd.identifier);
            initializer = parser(vd.initializer);
        }
    }

    [Serializable]
    public class xconstantwrapper : xast
    {
        object value;

        internal xconstantwrapper(ConstantWrapper cw)
        {
            value = cw.value;
        }
    }

    [Serializable]
    public class xif : xast
    {
        xast condition;
        xast operand1;
        xast operand2;

        internal xif(If i)
        {
            condition = parser(i.condition);
            operand1 = parser(i.operand1);
            operand2 = parser(i.operand2);
        }
    }

    [Serializable]
    public class xexpr : xast
    {
        xast operand;

        internal xexpr(Expression e)
        {
            operand = parser(e.operand);
        }
    }

    [Serializable]
    public class xassing : xast
    {
        public xast lside;
        public xast rside;

        internal xassing(Assign a)
        {
            lside = parser(a.lhside);
            rside = parser(a.rhside);
        }
    }

    [Serializable]
    public class xequality : xbinaryop
    {
        internal xequality(Equality e)
            : base(e)
        { 
        }
    }

    [Serializable]
    public class xrelational : xbinaryop
    {
        internal xrelational(Relational r)
            : base(r)
        { 
        }
    }

    [Serializable]
    public class xlookup : xbinding
    {
        internal xlookup(Lookup l)
            : base(l.Name)
        {
        }
    }

    [Serializable]
    public class xbinding : xast
    {
        public string name;
        public object value;

        internal xbinding(string name)
        {
            this.name = name;
        }

        public void setvalue(object value)
        {
            this.value = value;
        }

        public object getvalue()
        {
            return value;
        }
    }

    [Serializable]
    public class xpostorprefixoperator : xunaryop
    {
        PostOrPrefix operatorTok;

        internal xpostorprefixoperator(PostOrPrefixOperator popo)
            : base(popo)
        {
            operatorTok = popo.operatorTok;
        }
    }

    [Serializable]
    public class xunaryop : xast
    {
        public xast operand;

        internal xunaryop(UnaryOp uo)
        {
            operand = parser(uo.operand);
        }
    }

    [Serializable]
    public class xbinaryop : xast
    {
        xast operand1;
        xast operand2;
        JSToken oper;

        internal xbinaryop(BinaryOp bo)
        {
            operand1 = parser(bo.operand1);
            operand2 = parser(bo.operand2);
            oper = bo.operatorTok;
        }
    }
}
