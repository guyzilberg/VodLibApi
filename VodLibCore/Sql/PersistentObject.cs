using System;
using System.Collections.Generic;
using System.Text;

namespace VodLibCore.Sql
{
    public abstract class PersistentObject
    {
        #region CRUD
        public virtual void Load()
        {
            
        }

        public virtual void Save()
        {

        }
        public virtual void Delete() 
        { 
        }
        #endregion
    }
}
