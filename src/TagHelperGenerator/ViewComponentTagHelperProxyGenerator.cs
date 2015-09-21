using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ViewComponents;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace TagHelperGenerator
{
    public class ViewComponentTagHelperProxyGenerator
    {
        private const string TagHelperName = nameof(TagHelper);
        private static readonly ConstructorInfo BaseConstuctor = typeof(GeneratedViewComponentTagHelperProxy)
            .GetConstructor(new[] { typeof(IViewComponentHelper) });
        private static readonly MethodInfo BaseExecute = typeof(GeneratedViewComponentTagHelperProxy)
            .GetMethod(nameof(GeneratedViewComponentTagHelperProxy.InvokeViewComponentTagHelper));
        private readonly TagHelperModuleBuilder _moduleBuilder;

        public ViewComponentTagHelperProxyGenerator(TagHelperModuleBuilder moduleBuilder)
        {
            _moduleBuilder = moduleBuilder;
        }

        public void CreateTagHelper(ViewComponentDescriptor viewComponentDescriptor)
        {
            var tagHelperName = $"{viewComponentDescriptor.Type.Namespace}.{viewComponentDescriptor.ShortName}VC{TagHelperName}";
            var typeBuilder = _moduleBuilder.CreateTagHelperType<GeneratedViewComponentTagHelperProxy>(tagHelperName);

            var constructor = typeBuilder.DefineConstructor(
                MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName,
                CallingConventions.Standard,
                new[] { typeof(IViewComponentHelper) });
            var constructorILGenerator = constructor.GetILGenerator();
            constructorILGenerator.Emit(OpCodes.Ldarg_0);
            constructorILGenerator.Emit(OpCodes.Ldarg_1);
            constructorILGenerator.Emit(OpCodes.Call, BaseConstuctor);
            constructorILGenerator.Emit(OpCodes.Ret);

            var processBuilder = typeBuilder.DefineMethod(
                nameof(TagHelper.Process),
                MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                CallingConventions.HasThis,
                typeof(void),
                new[] { typeof(TagHelperContext), typeof(TagHelperOutput) });

            var methodInfo = viewComponentDescriptor.Type.GetMethod("Invoke");
            var methodParameters = methodInfo.GetParameters();
            var parameterCount = methodParameters.Length;
            var processILGenerator = processBuilder.GetILGenerator();
            processILGenerator.Emit(OpCodes.Ldc_I4, parameterCount);
            processILGenerator.Emit(OpCodes.Newarr, typeof(object));
            processILGenerator.Emit(OpCodes.Dup);

            for (var i = 0; i < parameterCount; i++)
            {
                var parameterInfo = methodParameters[i];
                var propertyBuilder = typeBuilder.DefineProperty(
                    parameterInfo.Name,
                    PropertyAttributes.HasDefault,
                    parameterInfo.ParameterType,
                    null);

                var getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                var backingFieldBuilder = typeBuilder.DefineField(
                    "m_" + parameterInfo.Name.ToLower(),
                    parameterInfo.ParameterType,
                    FieldAttributes.Private);

                var getMethodBuilder = typeBuilder.DefineMethod(
                    "get_" + parameterInfo.Name,
                    getSetAttr,
                    parameterInfo.ParameterType,
                    Type.EmptyTypes);

                var getMethodILGenerator = getMethodBuilder.GetILGenerator();
                getMethodILGenerator.Emit(OpCodes.Ldarg_0);
                getMethodILGenerator.Emit(OpCodes.Ldfld, backingFieldBuilder);
                getMethodILGenerator.Emit(OpCodes.Ret);

                // Define the "set" accessor method for Number, which has no return
                var setMethodBuilder = typeBuilder.DefineMethod(
                    "set_" + parameterInfo.Name,
                    getSetAttr,
                    null,
                    new Type[] { parameterInfo.ParameterType });

                ILGenerator setMethodILGenerator = setMethodBuilder.GetILGenerator();
                setMethodILGenerator.Emit(OpCodes.Ldarg_0);
                setMethodILGenerator.Emit(OpCodes.Ldarg_1);
                setMethodILGenerator.Emit(OpCodes.Stfld, backingFieldBuilder);
                setMethodILGenerator.Emit(OpCodes.Ret);

                // The property is now complete.
                propertyBuilder.SetGetMethod(getMethodBuilder);
                propertyBuilder.SetSetMethod(setMethodBuilder);

                // Update our params array that's used to execute the view component.
                processILGenerator.Emit(OpCodes.Ldc_I4, i);
                processILGenerator.Emit(OpCodes.Ldarg_0);
                processILGenerator.EmitCall(OpCodes.Call, propertyBuilder.GetMethod, null);
                processILGenerator.Emit(OpCodes.Box, parameterInfo.ParameterType);
                processILGenerator.Emit(OpCodes.Stelem_Ref);
                processILGenerator.Emit(OpCodes.Dup);
            }

            var arrayValue = processILGenerator.DeclareLocal(typeof(object[]));
            processILGenerator.Emit(OpCodes.Stloc, arrayValue);
            processILGenerator.Emit(OpCodes.Ldarg_0);
            processILGenerator.Emit(OpCodes.Ldarg_2);
            processILGenerator.Emit(OpCodes.Ldloc, arrayValue);
            var baseExecuteMethodInfo = BaseExecute.MakeGenericMethod(viewComponentDescriptor.Type);
            processILGenerator.EmitCall(OpCodes.Call, baseExecuteMethodInfo, null);
            processILGenerator.Emit(OpCodes.Ret);
        }
    }
}
