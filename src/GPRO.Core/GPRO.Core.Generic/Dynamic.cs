using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
namespace GPRO.Core.Generic
{
	public class Dynamic
	{
		private Dictionary<string, object> dictionary = new Dictionary<string, object>();
		public Dynamic Add<T>(string key, T value)
		{
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicAssembly"), AssemblyBuilderAccess.Run);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Dynamic.dll");
			TypeBuilder typeBuilder = moduleBuilder.DefineType(Guid.NewGuid().ToString());
			typeBuilder.SetParent(base.GetType());
			PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(key, PropertyAttributes.None, typeof(T), Type.EmptyTypes);
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("get_" + key, MethodAttributes.Public, CallingConventions.HasThis, typeof(T), Type.EmptyTypes);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldstr, key);
			iLGenerator.Emit(OpCodes.Callvirt, typeof(Dynamic).GetMethod("Get", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(new Type[]
			{
				typeof(T)
			}));
			iLGenerator.Emit(OpCodes.Ret);
			propertyBuilder.SetGetMethod(methodBuilder);
			Type type = typeBuilder.CreateType();
			Dynamic dynamic = (Dynamic)Activator.CreateInstance(type);
			dynamic.dictionary = this.dictionary;
			this.dictionary.Add(key, value);
			return dynamic;
		}
		protected T Get<T>(string key)
		{
			return (T)this.dictionary[key];
		}
	}
}
