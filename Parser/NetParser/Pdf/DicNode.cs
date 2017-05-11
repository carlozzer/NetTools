using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetParser.Pdf
{
    public class DicNode
    {
        private DicEntry _item;
        private DicNode _parentNode;
        private List<DicNode> _children;

        public DicNode( DicEntry item )
        {
            _item = item;
        }

        public void SetParentNode( DicEntry parent)
        {
            _parentNode.Item = parent;
        }

        public DicEntry Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public void AddChild(DicEntry child)
        {
            _children.Add(new DicNode( child ));
        }

        public void RemoveChild(DicNode child)
        {
            var node = _children.FirstOrDefault(e => e.Item.Equals(child));
            if (node != null)
                _children.Remove(node);
        }
    }
}
