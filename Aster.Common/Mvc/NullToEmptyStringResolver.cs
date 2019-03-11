using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aster.Common.Mvc
{
    public class NullToEmptyStringResolver : CamelCasePropertyNamesContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return type.GetProperties()
                    .Select(p => {
                        var jp = base.CreateProperty(p, memberSerialization);
                        jp.ValueProvider = new NullToEmptyStringValueProvider(p);
                        return jp;
                    }).ToList();
        }
    }

    public class NullToEmptyStringValueProvider : IValueProvider
    {
        PropertyInfo _memberInfo;
        public NullToEmptyStringValueProvider(PropertyInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }

        public object GetValue(object target)
        {
            object result = _memberInfo.GetValue(target);

            if (_memberInfo.PropertyType == typeof(string) && result == null)
                result = string.Empty;

            return result;
        }

        public void SetValue(object target, object value)
        {
            _memberInfo.SetValue(target, value);
        }
    }
}
