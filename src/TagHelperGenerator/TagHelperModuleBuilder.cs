using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.AspNet.Razor.Runtime.TagHelpers;
using Microsoft.Dnx.Runtime;

namespace TagHelperGenerator
{
    public class TagHelperModuleBuilder
    {
        private const string AssemblyName = "GeneratedTagHelpers";

        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;
        private List<TypeBuilder> _activeTypeBuilders;

        public TagHelperModuleBuilder(IApplicationEnvironment applicationEnvironment)
        {
            var assemblyName = new AssemblyName(AssemblyName) { Version = new Version(1, 0, 0, 0) };
#if !DNXCORE50
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.RunAndSave,
                applicationEnvironment.ApplicationBasePath + Path.DirectorySeparatorChar + "bin");
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName, AssemblyName + ".dll");
#else
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName);
#endif
            _activeTypeBuilders = new List<TypeBuilder>();
        }

        public TypeBuilder CreateTagHelperType<TTagHelper>(string fullTypeName) where TTagHelper : TagHelper
        {
            var accessibility = TypeAttributes.Public | TypeAttributes.Class;
            var typeBuilder = _moduleBuilder.DefineType(fullTypeName, accessibility, typeof(TTagHelper));

            _activeTypeBuilders.Add(typeBuilder);

            return typeBuilder;
        }

        public void Build()
        {
            foreach(var typeBuilder in _activeTypeBuilders)
            {
                typeBuilder.CreateTypeInfo();
            }

#if !DNXCORE50
            _assemblyBuilder.Save(AssemblyName + ".dll");
#endif
        }
    }
}
