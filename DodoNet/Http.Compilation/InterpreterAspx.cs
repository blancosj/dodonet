using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.CodeDom;
using System.Security.Cryptography;
using System.Globalization;

using DodoNet.Overlay;
using DodoNet.JScript;
using DodoNet.Http.Caching;
using DodoNet.Http.Compilation;
using DodoNet.Http.Util;

using DodoNet.Tools;

using Microsoft.JScript;

namespace DodoNet.Http.Compilation
{
    public class InterpreterAspx : HttpInterpreter
    {
        private static WebCache cache = new WebCache();        

        public override  string Extension { get { return ".aspx"; } }

        public HttpRequest Request { get { return context.Request; } }
        public HttpReply Reply { get { return context.Reply; } }

        StringBuilder errors = new StringBuilder();

        AspParser parser;

        List<string> imports = new List<string>();

        CodeMemberMethod m;

        EvaluatorBuilder eb;

        public InterpreterAspx(HttpContext context)
            : base(context)
        {
        }

        public override void Interpret()
        {
            if (File.Exists(context.Request.PhysicalPathFile))
            {
                using (FileStream fs = new FileStream(context.Request.PhysicalPathFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    // calculamos el hashcode para 
                    string hashcode = Conversion.CalculateHashCode(fs);
                    string mainClassName = "CodeRender_" + Path.GetFileNameWithoutExtension(context.Request.PhysicalPathFile);

                    Assembly asam = cache.Get(hashcode) as Assembly;

                    if (asam == null)
                    {
                        StreamReader reader = new StreamReader(fs, Encoding.Default, true, 4096);
                        parser = new AspParser(context.Request.PhysicalPathFile, reader);
                        parser.TagParsed += new TagParsedHandler(parser_TagParsed);
                        parser.TextParsed += new TextParsedHandler(parser_TextParsed);
                        parser.Error += new ParseErrorHandler(parser_Error);

                        eb = new EvaluatorBuilder();

                        eb.BeginCompile("ASP");
                        eb.AppendNameSpace("System");
                        eb.AppendNameSpace("System.Web");

                        CodeTypeDeclaration c = eb.AppendClass(mainClassName, typeof(DinamicPage));

                        // eb.CreatePropertyGetForObject(c, context.App.GetType(), "App", "Context.App");
                        // eb.CreatePropertyGetForObject(c, context.SessionRequest == null ? typeof(Object) : context.SessionRequest.GetType(), "SessionRequest", "Context.SessionRequest");

                        m = eb.AppendMethod(c, "Render", MemberAttributes.Override);

                        parser.Parse();

                        asam = eb.EndCompile();

                        // añadimos al cache
                        cache.Add(hashcode, asam);
                    }

                    DinamicPage page = asam.CreateInstance("ASP." + mainClassName) 
                        as DinamicPage;

                    page.Me = this;
                    page.Context = context;
                    page.OutStream = new StringBuilder();
                    context.Reply.Render = page;

                    try
                    {
                        page.Render();
                    }
                    catch (Exception err)
                    {
                        page.OutStream.AppendFormat("Error: {0}\n{1}", err.Message, err.StackTrace);
                    }

                    // forzar descarga de archivo
                    if (!context.Reply.ForcedFileDownload && !context.Reply.LoadedStream)
                        context.Reply.LoadText(page.OutStream.ToString());
                }
            }
            else
            {
                context.Reply.Code = Codes.NOT_FOUND;
            }
        }

        void parser_TagParsed(ILocation location, TagType tagtype, string id, TagAttributes attributes)
        {
            string tmp = "";

            if (String.Compare(id, "script", true) == 0)
            {
                if (tagtype == TagType.Tag)
                {
                    parser.VerbatimID = "script";
                }
            }

            switch (tagtype)
            {
                case TagType.Tag:
                case TagType.Text:                
                case TagType.ServerComment:
                case TagType.SelfClosing:
                case TagType.Include:
                case TagType.DataBinding:
                case TagType.Close:
                case TagType.CodeRenderExpression:
                    // AppendText(location.PlainText, true);
                    AppendText(location.PlainText);
                    break;
                case TagType.Directive:
                    if (id.CompareTo("Import") == 0)
                        eb.AppendNameSpace(attributes["Namespace"].ToString());
                    else if (id.CompareTo("Assembly") == 0)
                        eb.AppendAssembly(attributes["Name"].ToString());
                    else if (id.CompareTo("SessionRequest") == 0)
                    {
                        if (attributes["Check"] != null)
                            eb.AppendCodeIfAndReturn(m, "!CheckSession(false)");
                        else if (attributes["CheckAndCreate"] != null)
                            eb.AppendCodeIfAndReturn(m, "!CheckSession(true)");
                        else if (attributes["CheckManual"] != null)
                            break;
                    }
                    break;
                case TagType.CodeRender:
                    AppendText(id, false);
                    break;
            }
        }

        void parser_TextParsed(ILocation location, string text)
        {
            AppendText(text);
        }

        void parser_Error(ILocation location, string message)
        {
            errors.AppendLine(string.Format("{0}-{1}", location.PlainText, message));
        }

        public void AppendText(string text)
        {
            int index = text.IndexOf("<%");
            if (index > 0)
            {
                AppendText(text.Substring(0, index), true);
                text = text.Substring(index);
                ParserInnerScript children = new ParserInnerScript(this, text);
                children.Parser();
            }
            else
            {
                AppendText(text, true);
            }
        }

        public void AppendText(string text, bool withPrint)
        {
            if (text.Length > 0)
            {
                string tmp = text;

                if (withPrint)
                {
                    eb.AppendCodeExpression(m, "print", tmp);
                }
                else
                {
                    // page.Append(tmp);
                    eb.AppendCodeSnippetExpression(m, tmp);
                }
            }
        }
   }

    internal class ParserInnerScript
    {
        AspParser parser;
        InterpreterAspx parent;

        public ParserInnerScript(InterpreterAspx parent, string text)
        {
            this.parent = parent;

            parser = new AspParser("@@inner_string@@", new StringReader(text));
            parser.TagParsed += new TagParsedHandler(parser_TagParsed);
            parser.TextParsed += new TextParsedHandler(parser_TextParsed);
            parser.Error += new ParseErrorHandler(parser_Error);            
        }

        public void Parser()
        {
            parser.Parse();
        }

        void parser_Error(ILocation location, string message)
        {
            
        }

        void parser_TextParsed(ILocation location, string text)
        {
            parent.AppendText(text, true);
        }

        void parser_TagParsed(ILocation location, TagType tagtype, string id, TagAttributes attributes)
        {
            switch (tagtype)
            {
                case TagType.CodeRender:
                    parent.AppendText(id, false);
                    break;
                default:
                    parent.AppendText(location.PlainText);
                    break;
            }
        }
    }
}
