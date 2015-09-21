using System;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;

namespace TagHelperGenerator
{
    public class TagHelperModuleBuilder
    {
        private const string AssemblyName = "GeneratedTagHelpers";
        private const string AssemblyFileName = AssemblyName + ".dll";

        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        //private List<TypeBuilder> _activeTypeBuilders;

        public TagHelperModuleBuilder()
        {
            var assemblyName = new AssemblyName(AssemblyName);
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName, AssemblyFileName);
        }

        public TypeBuilder CreateTagHelperType(string fullTypeName)
        {
            var accessibility = TypeAttributes.Public | TypeAttributes.Class;
            var typeBuilder = _moduleBuilder.DefineType(fullTypeName, accessibility, typeof(TagHelper));

            return typeBuilder;
        }

        public void Build()
        {
            _assemblyBuilder.Save(AssemblyFileName);
        }
    }
}
