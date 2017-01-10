using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Microsoft.JScript;

namespace DodoNet.JScript
{
    /// <summary>
    /// la clase EvaluatorEngine lee las propiedades de esta clase y se pueden ver al evaluar
    /// </summary>
    public class EvaluatorContext
    {
        internal void AddFieldsAndProperties(ActivationObject obj)
        {
            foreach (PropertyInfo pi in GetType().GetProperties())
            {
                obj.AddFieldOrUseExistingField(pi.Name, pi.GetValue(this, null), FieldAttributes.Public);
            }

            foreach (FieldInfo fi in GetType().GetFields())
            {
                obj.AddFieldOrUseExistingField(fi.Name, fi.GetValue(this), FieldAttributes.Public);
            }
        }
    }
}
