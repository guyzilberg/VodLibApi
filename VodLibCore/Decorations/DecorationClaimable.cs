using System;
using System.Collections.Generic;
using System.Text;

namespace VodLibCore.Decorations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DecorationClaimable : Attribute //todo: think of a better naming convention for decorations/attributes
    {
        public string ClaimType { get; set; }
        public DecorationClaimable(string claimType)
        {
            ClaimType = claimType;
        }
    }
}
