using Dapper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using VodLibCore.Decorations;
using static Dapper.SqlMapper;

namespace VodLibCore.Sql
{
    public abstract class PersistentObj
    {
        public virtual DynamicParameters GetContextParams(bool addIdentity = true)
        {
            DynamicParameters parameters = new DynamicParameters();
            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                DecorationPersistent atr = propertyInfo.GetCustomAttribute<DecorationPersistent>();
                if (atr == null)
                    continue;

                if (addIdentity == false && atr.IsIdentity)
                    continue;

                string name = atr.FieldName;
                if(string.IsNullOrEmpty(name))
                    name = propertyInfo.Name;
                object val = propertyInfo.GetValue(this);

                if (propertyInfo.PropertyType.IsEnum)
                    val = (int)val;

                parameters.Add(name, val);
            }
            return parameters;
        } 

        public virtual void OnSave<T>(int identity) where T : PersistentObj
        {
            saveIdentity(identity);

        }

        public PropertyInfo GetIdentityProp()
        {
            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                DecorationPersistent atr = propertyInfo.GetCustomAttribute<DecorationPersistent>();
                if (atr.IsIdentity)
                    return propertyInfo;
            }
            return null;
        }

        private void saveIdentity(object identity)
        {
            PropertyInfo propertyInfo = GetIdentityProp();
            if(propertyInfo != null)
                propertyInfo.SetValue(this, identity);
        }

    }
}
