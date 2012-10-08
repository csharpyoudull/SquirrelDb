using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquirrelDb
{
    public struct DataPointer
    {
        public long Pointer { get; set; }

        public long FileId { get; set; }

        public override int GetHashCode()
        {
            return (new [] {Pointer, FileId}).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var pointer = (DataPointer) obj;
            return pointer.FileId.Equals(FileId) && pointer.Pointer.Equals(Pointer);
        }
    }
}
