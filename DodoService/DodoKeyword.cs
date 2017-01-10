using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DodoService
{
    public class DodoKeyword
    {
        public string name;
        public string Name { get { return name; } set { name = value; } }

        public string dump = "dumb";

        // public Relation brothers;
        /*
        public Dictionary<string, DodoKeyword> brothers = new Dictionary<string,DodoKeyword>();
        */

        // List<DodoKeyword> brothers = new List<DodoKeyword>();
        // public RedBlack brothers = new RedBlack();
        // public LinkedList<DodoKeyword> brothers = new LinkedList<DodoKeyword>();
        public List<DodoKeyword> brothers = new List<DodoKeyword>();

        // public DodoKeyword[] brothers;

        // public Hashtable brothers = new Hashtable();

        public DodoKeyword() { }

        public DodoKeyword(string name) { this.name = name; }

        /*
        public void AddBrothers(string keywords)
        {
            DodoApplication app = DodoApplication.CurrentContext.App as DodoApplication;

            keywords = keywords.ToLowerInvariant();
            keywords = keywords.Replace(" ", "");
            foreach (string brotherName in keywords.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (name.CompareTo(brotherName) != 0)
                {
                    DodoKeyword tmp = null;
                    // if (tmp != null)
                    if (!brothers.TryGetValue(brotherName, out tmp))                    
                    {
                        DodoKeyword found = app.CurrentDb.GetKeyword(brotherName);
                        if (found == null)
                        {
                            found = new DodoKeyword(brotherName);
                            app.CurrentDb.SetKeyword(found);
                        }
                        brothers.Add(brotherName, found);
                    }
                }
            }
            app.CurrentDb.SetKeyword(this);
        }
        */

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
