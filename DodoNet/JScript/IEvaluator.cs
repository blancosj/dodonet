using System;
using System.Collections.Generic;
using System.Text;

namespace DodoNet.JScript
{
    /// <summary>
    /// interface para clases Evaluator usado en los assemblies JScript.NET
    /// </summary>
    public interface IEvaluator
    {
        object Me { get; set; }
        object DoEval(string expr);
    }
}
