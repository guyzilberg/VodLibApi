using System;
using System.Collections.Generic;
using System.Text;

namespace VodLibCore.Decorations
{
    public class DecorationPersistent : Attribute
    {
        public string FieldName { get; set; }
        public bool IsIdentity { get; set; }
        public DecorationPersistent()
        {

        }
        public DecorationPersistent(string fieldName)
        {
            FieldName = fieldName;
        }
        public DecorationPersistent(bool isIdentity)
        {
            IsIdentity = isIdentity;
        }
        public DecorationPersistent(string fieldName, bool isIdentity)
        {
            FieldName = fieldName;
            IsIdentity = isIdentity;
        }
    }
}
