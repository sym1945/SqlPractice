using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SqlPractice
{
    public class PropertyAccessor
    {
        protected delegate void SetValueHandler(object component, object value);
        protected delegate object GetValueHandler(object component);

        private SetValueHandler setValueHandler;
        private GetValueHandler getValueHandler;

        public PropertyAccessor(Type ownerType, string propertyName)
        {
            PropertyInfo propertyInfo = ownerType.GetProperty(propertyName);

            if (propertyInfo.CanRead)
            {
                this.getValueHandler = this.CreateGetValueHandler(propertyInfo);
            }

            if (propertyInfo.CanWrite)
            {
                this.setValueHandler = this.CreateSetValueHandler(propertyInfo);
            }
        }

        public object GetValue(object component)
        {
            if (this.getValueHandler == null)
            {
                throw new InvalidOperationException();
            }

            return this.getValueHandler(component);
        }

        public void SetValue(object component, object value)
        {
            if (this.setValueHandler == null)
            {
                throw new InvalidOperationException();
            }

            this.setValueHandler(component, value);
        }

        protected virtual SetValueHandler CreateSetValueHandler(PropertyInfo propertyInfo)
        {
            MethodInfo setMethodInfo = propertyInfo.GetSetMethod(false);
            DynamicMethod setPropertyValue = new DynamicMethod("SetValue", typeof(void), new Type[] { typeof(object), typeof(object) }, typeof(PropertyAccessor), true);
            ILGenerator ilGenerator = setPropertyValue.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);

            Type parameterType = setMethodInfo.GetParameters()[0].ParameterType;

            if (parameterType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Unbox_Any, parameterType);
            }

            ilGenerator.Emit(OpCodes.Call, setMethodInfo);
            ilGenerator.Emit(OpCodes.Ret);

            return setPropertyValue.CreateDelegate(typeof(SetValueHandler)) as SetValueHandler;
        }

        protected virtual GetValueHandler CreateGetValueHandler(PropertyInfo propertyInfo)
        {
            MethodInfo getMethodInfo = propertyInfo.GetGetMethod();
            DynamicMethod getMethod = new DynamicMethod("GetValue", typeof(object), new Type[] { typeof(object) }, typeof(PropertyAccessor), true);
            ILGenerator ilGenerator = getMethod.GetILGenerator();

            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Call, getMethodInfo);

            Type returnType = getMethodInfo.ReturnType;

            if (returnType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, returnType);
            }

            ilGenerator.Emit(OpCodes.Ret);

            return getMethod.CreateDelegate(typeof(GetValueHandler)) as GetValueHandler;

        }
    }
    
}
