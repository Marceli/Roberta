//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//

using System.Web.UI.WebControls;

namespace Roberta.WebControls
{
    public enum TypeColumn { Text, Bool, Image }

    public class ListColumn
    {
        public string Member;
        public string Label;
        public HorizontalAlign Align;
        public TypeColumn Type;
        public string Class;

        public ListColumn(string member, string label, HorizontalAlign align, TypeColumn type)
        {
            this.Member = member;
            this.Label = label;
            this.Align = align;
            this.Type = type;
        }

        public ListColumn(string member, string label, HorizontalAlign align, TypeColumn type, string Class)
            : this(member, label, align, type)
        {
            this.Class = Class;
        }

    }
}
