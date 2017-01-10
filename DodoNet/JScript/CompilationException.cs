using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace DodoNet.JScript
{
    public class CompilationException : Exception
    {
        string fileName;
        public string FileName { get { return fileName; } }

        CompilerErrorCollection errors;
        public CompilerErrorCollection Errors { get { return errors; } }

        string fileText;
        public string FileText { get { return fileText; } }

        public CompilationException(string fileName, CompilerErrorCollection errors, string fileText)
        {
            this.fileName = fileName;
            this.errors = errors;
            this.fileText = fileText;
        }
    }
}
