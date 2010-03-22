//**************************************************************//
// Paul Wilson -- www.WilsonDotNet.com -- Paul@WilsonDotNet.com //
// Feel free to use and modify -- just leave these credit lines //
// I also always appreciate any other public credit you provide //
//**************************************************************//
using System.Collections;
using System.Collections.Generic;

namespace Roberta.WebControls
{
    public class ListSetup
    {
        
        private ListView listView;

        public ListSetup(ListView listView)
        {
            this.listView = listView;
        }

//      public string FiltrMember
//        {
//            get { return this.listView.FiltrMember; }
//            set { listView.FiltrMember = value; }
//        }
//        public string FiltrValue
//        {
//            get { return listView.FiltrValue; }
//            set { listView.FiltrValue = value; }
//        }
//
//        public bool SortAscending
//        {
//            get { return this.listView.SortAscending; }
//        }

        public IList Source
        {
            get { return listView.Source; }
            set { listView.Source = value; }
        }

        public string IdMember
        {
            get { return listView.IdMember; }
            set { listView.IdMember = value; }
        }

        public string TextMember
        {
            get { return listView.TextMember; }
            set { listView.TextMember = value; }
        }
       
        public IList<ListColumn> Columns
        {
            get { return listView.Columns; }
            set { listView.Columns = value; }
        }
    }
}