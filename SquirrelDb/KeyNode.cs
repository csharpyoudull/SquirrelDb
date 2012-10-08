using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquirrelDb
{
    public class KeyNode
    {
        public DataPointer Pointer { get; set; }

        public long Value { get; set; }

        public KeyNode Parent { get; set; }

        public KeyNode Left { get; set; }

        public KeyNode Right { get; set; }
    }
}
