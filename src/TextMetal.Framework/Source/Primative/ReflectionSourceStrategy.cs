﻿/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using TextMetal.Framework.Associative;
using TextMetal.Middleware.Solder.Extensions;

namespace TextMetal.Framework.Source.Primative
{
	public class ReflectionSourceStrategy : SourceStrategy
	{
		#region Constructors/Destructors

		/// <summary>
		/// Initializes a new instance of the ReflectionSourceStrategy class.
		/// </summary>
		public ReflectionSourceStrategy()
		{
		}

		#endregion

		#region Methods/Operators

		private static bool IsRealMemberInfo(MethodInfo methodInfo)
		{
			PropertyInfo[] propertyInfos;
			EventInfo[] eventInfos;
			MethodInfo accessorMethodInfo = null;

			if ((object)methodInfo == null)
				throw new ArgumentNullException(nameof(methodInfo));

			propertyInfos = methodInfo.DeclaringType.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

			if ((object)propertyInfos != null)
			{
				foreach (PropertyInfo propertyInfo in propertyInfos)
				{
					accessorMethodInfo = propertyInfo.GetGetMethod(true);

					if ((object)accessorMethodInfo != null && accessorMethodInfo.Equals(methodInfo))
						return false;

					accessorMethodInfo = propertyInfo.GetSetMethod(true);
					if ((object)accessorMethodInfo != null && accessorMethodInfo.Equals(methodInfo))
						return false;
				}
			}

			eventInfos = methodInfo.DeclaringType.GetEvents(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

			if ((object)eventInfos != null)
			{
				foreach (EventInfo eventInfo in eventInfos)
				{
					accessorMethodInfo = eventInfo.GetAddMethod(true);

					if ((object)accessorMethodInfo != null && accessorMethodInfo.Equals(methodInfo))
						return false;

					accessorMethodInfo = eventInfo.GetRemoveMethod(true);
					if ((object)accessorMethodInfo != null && accessorMethodInfo.Equals(methodInfo))
						return false;
				}
			}

			return true;
		}

		private static void ModelAssemblies(Assembly[] assemblies, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			Type[] types;
			AssemblyName[] assemblyReferences;
			AssemblyName assemblyName;

			if ((object)assemblies == null)
				throw new ArgumentNullException(nameof(assemblies));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "Assemblies";
			parent.Items.Add(arrayConstruct00);

			foreach (Assembly assembly in assemblies)
			{
				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "AssemblyIsDynamic";
				propertyConstruct00.RawValue = assembly.IsDynamic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "AssemblyFullName";
				propertyConstruct00.RawValue = assembly.FullName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "AssemblyManifestModuleName";
				propertyConstruct00.RawValue = assembly.ManifestModule.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "AssemblyManifestModuleFullyQualifiedName";
				propertyConstruct00.RawValue = assembly.ManifestModule.FullyQualifiedName;
				objectConstruct00.Items.Add(propertyConstruct00);

				assemblyName = assembly.GetName();

				ModelAssemblyName(assemblyName, objectConstruct00);

				ModelCustomAttributes(assembly, objectConstruct00);

				types = assembly.GetExportedTypes();

				ModelTypes("Types", types, objectConstruct00);

				//ModelAssemblyReferences(assemblyReferences, objectConstruct00);
			}
		}

		private static void ModelAssemblyName(AssemblyName assemblyName, AssociativeXmlObject parent)
		{
			PropertyConstruct propertyConstruct00;

			if ((object)assemblyName == null)
				throw new ArgumentNullException(nameof(assemblyName));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "AssemblyVersion";
			propertyConstruct00.RawValue = assemblyName.Version;
			parent.Items.Add(propertyConstruct00);

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "AssemblyName";
			propertyConstruct00.RawValue = assemblyName.Name;
			parent.Items.Add(propertyConstruct00);

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "AssemblyContentType";
			propertyConstruct00.RawValue = assemblyName.ContentType;
			parent.Items.Add(propertyConstruct00);

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "AssemblyCultureName";
			propertyConstruct00.RawValue = assemblyName.CultureName;
			parent.Items.Add(propertyConstruct00);

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "AssemblyFullName";
			propertyConstruct00.RawValue = assemblyName.FullName;
			parent.Items.Add(propertyConstruct00);

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "AssemblyFlags";
			propertyConstruct00.RawValue = assemblyName.Flags;
			parent.Items.Add(propertyConstruct00);

			propertyConstruct00 = new PropertyConstruct();
			propertyConstruct00.Name = "AssemblyProcessorArchitecture";
			propertyConstruct00.RawValue = assemblyName.ProcessorArchitecture;
			parent.Items.Add(propertyConstruct00);
		}

		private static void ModelAssemblyReferences(AssemblyName[] assemblyReferences, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			ObjectConstruct objectConstruct00;

			if ((object)assemblyReferences == null)
				throw new ArgumentNullException(nameof(assemblyReferences));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "AssemblyReferences";
			parent.Items.Add(arrayConstruct00);

			foreach (AssemblyName assemblyReference in assemblyReferences)
			{
				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				ModelAssemblyName(assemblyReference, objectConstruct00);
			}
		}

		private static void ModelConstructors(ConstructorInfo[] constructorInfos, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			ParameterInfo[] parameterInfos;

			if ((object)constructorInfos == null)
				throw new ArgumentNullException(nameof(constructorInfos));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "Constructors";
			parent.Items.Add(arrayConstruct00);

			foreach (ConstructorInfo constructorInfo in constructorInfos)
			{
				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorName";
				propertyConstruct00.RawValue = constructorInfo.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorCallingConvention";
				propertyConstruct00.RawValue = constructorInfo.CallingConvention;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorContainsGenericParameters";
				propertyConstruct00.RawValue = constructorInfo.ContainsGenericParameters;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsGenericMethod";
				propertyConstruct00.RawValue = constructorInfo.IsGenericMethod;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsGenericMethodDefinition";
				propertyConstruct00.RawValue = constructorInfo.IsGenericMethodDefinition;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsAbstract";
				propertyConstruct00.RawValue = constructorInfo.IsAbstract;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsAssembly";
				propertyConstruct00.RawValue = constructorInfo.IsAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsFamily";
				propertyConstruct00.RawValue = constructorInfo.IsFamily;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsFamilyAndAssembly";
				propertyConstruct00.RawValue = constructorInfo.IsFamilyAndAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsFamilyOrAssembly";
				propertyConstruct00.RawValue = constructorInfo.IsFamilyOrAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsFinal";
				propertyConstruct00.RawValue = constructorInfo.IsFinal;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsHideBySig";
				propertyConstruct00.RawValue = constructorInfo.IsHideBySig;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsPrivate";
				propertyConstruct00.RawValue = constructorInfo.IsPrivate;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsPublic";
				propertyConstruct00.RawValue = constructorInfo.IsPublic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsSpecialName";
				propertyConstruct00.RawValue = constructorInfo.IsSpecialName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsStatic";
				propertyConstruct00.RawValue = constructorInfo.IsStatic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorIsVirtual";
				propertyConstruct00.RawValue = constructorInfo.IsVirtual;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ConstructorMethodImplementationFlags";
				propertyConstruct00.RawValue = constructorInfo.MethodImplementationFlags;
				objectConstruct00.Items.Add(propertyConstruct00);

				ModelCustomAttributes(constructorInfo, objectConstruct00);

				parameterInfos = constructorInfo.GetParameters();

				ModelParameters(parameterInfos, objectConstruct00);
			}
		}

		private static void ModelCustomAttributes(ICustomAttributeProvider customAttributeProvider, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;
			ArrayConstruct arrayConstruct01;
			ObjectConstruct objectConstruct01;
			PropertyConstruct propertyConstruct01;

			Attribute[] customAttributes;
			PropertyInfo[] publicPropertyInfos;
			object value;

			if ((object)customAttributeProvider == null)
				throw new ArgumentNullException(nameof(customAttributeProvider));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			customAttributes = SolderFascadeAccessor.ReflectionFascade.GetAllAttributes<Attribute>(customAttributeProvider);

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "CustomAttributes";
			parent.Items.Add(arrayConstruct00);

			if ((object)customAttributes != null)
			{
				foreach (Attribute customAttribute in customAttributes)
				{
					objectConstruct00 = new ObjectConstruct();
					arrayConstruct00.Items.Add(objectConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "CustomAttributeTypeFullName";
					propertyConstruct00.RawValue = customAttribute.GetType().FullName;
					objectConstruct00.Items.Add(propertyConstruct00);

					publicPropertyInfos = customAttribute.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

					if ((object)publicPropertyInfos != null)
					{
						arrayConstruct01 = new ArrayConstruct();
						arrayConstruct01.Name = "CustomAttributeProperties";
						objectConstruct00.Items.Add(arrayConstruct01);

						foreach (PropertyInfo publicPropertyInfo in publicPropertyInfos)
						{
							objectConstruct01 = new ObjectConstruct();
							arrayConstruct01.Items.Add(objectConstruct01);

							propertyConstruct01 = new PropertyConstruct();
							propertyConstruct01.Name = "CustomAttributePropertyName";
							propertyConstruct01.RawValue = publicPropertyInfo.Name;
							objectConstruct01.Items.Add(propertyConstruct01);

							if (SolderFascadeAccessor.ReflectionFascade.GetLogicalPropertyValue(customAttribute, publicPropertyInfo.Name, out value))
							{
								propertyConstruct01 = new PropertyConstruct();
								propertyConstruct01.Name = "CustomAttributePropertyValue";
								propertyConstruct01.RawValue = value.SafeToString();
								objectConstruct01.Items.Add(propertyConstruct01);
							}
						}
					}
				}
			}
		}

		private static void ModelEvents(EventInfo[] eventInfos, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			MethodInfo methodInfo;

			if ((object)eventInfos == null)
				throw new ArgumentNullException(nameof(eventInfos));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "Events";
			parent.Items.Add(arrayConstruct00);

			foreach (EventInfo eventInfo in eventInfos)
			{
				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "EventName";
				propertyConstruct00.RawValue = eventInfo.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "EventHandlerTypeName";
				propertyConstruct00.RawValue = eventInfo.EventHandlerType.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "EventHandlerTypeNamespace";
				propertyConstruct00.RawValue = eventInfo.EventHandlerType.Namespace;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "EventHandlerTypeFullName";
				propertyConstruct00.RawValue = eventInfo.EventHandlerType.FullName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "EventHandlerTypeAssemblyQualifiedName";
				propertyConstruct00.RawValue = eventInfo.EventHandlerType.AssemblyQualifiedName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "EventIsSpecialName";
				propertyConstruct00.RawValue = eventInfo.IsSpecialName;
				objectConstruct00.Items.Add(propertyConstruct00);

				methodInfo = eventInfo.GetAddMethod();

				if ((object)methodInfo != null)
				{
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "EventAddMethodIsStatic";
					propertyConstruct00.RawValue = methodInfo.IsStatic;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				methodInfo = eventInfo.GetRemoveMethod();

				if ((object)methodInfo != null)
				{
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "EventRemoveMethodIsStatic";
					propertyConstruct00.RawValue = methodInfo.IsStatic;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				methodInfo = eventInfo.GetRaiseMethod();

				if ((object)methodInfo != null)
				{
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "EventRaiseMethodIsStatic";
					propertyConstruct00.RawValue = methodInfo.IsStatic;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				ModelCustomAttributes(eventInfo, objectConstruct00);
			}
		}

		private static void ModelFields(FieldInfo[] fieldInfos, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			if ((object)fieldInfos == null)
				throw new ArgumentNullException(nameof(fieldInfos));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "Fields";
			parent.Items.Add(arrayConstruct00);

			foreach (FieldInfo fieldInfo in fieldInfos)
			{
				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldName";
				propertyConstruct00.RawValue = fieldInfo.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldTypeName";
				propertyConstruct00.RawValue = fieldInfo.FieldType.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldTypeNamespace";
				propertyConstruct00.RawValue = fieldInfo.FieldType.Namespace;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldTypeFullName";
				propertyConstruct00.RawValue = fieldInfo.FieldType.FullName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldTypeAssemblyQualifiedName";
				propertyConstruct00.RawValue = fieldInfo.FieldType.AssemblyQualifiedName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsAssembly";
				propertyConstruct00.RawValue = fieldInfo.IsAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsFamily";
				propertyConstruct00.RawValue = fieldInfo.IsFamily;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsFamilyAndAssembly";
				propertyConstruct00.RawValue = fieldInfo.IsFamilyAndAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsFamilyOrAssembly";
				propertyConstruct00.RawValue = fieldInfo.IsFamilyOrAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsInitOnly";
				propertyConstruct00.RawValue = fieldInfo.IsInitOnly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsLiteral";
				propertyConstruct00.RawValue = fieldInfo.IsLiteral;
				objectConstruct00.Items.Add(propertyConstruct00);

				if (fieldInfo.IsLiteral)
				{
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "FieldRawConstantValue";
					propertyConstruct00.RawValue = null;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsPrivate";
				propertyConstruct00.RawValue = fieldInfo.IsPrivate;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsPublic";
				propertyConstruct00.RawValue = fieldInfo.IsPublic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsSpecialName";
				propertyConstruct00.RawValue = fieldInfo.IsSpecialName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "FieldIsStatic";
				propertyConstruct00.RawValue = fieldInfo.IsStatic;
				objectConstruct00.Items.Add(propertyConstruct00);

				ModelCustomAttributes(fieldInfo, objectConstruct00);
			}
		}

		private static void ModelMethods(MethodInfo[] methodInfos, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			ParameterInfo[] parameterInfos;
			Type[] childTypes;

			if ((object)methodInfos == null)
				throw new ArgumentNullException(nameof(methodInfos));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "Methods";
			parent.Items.Add(arrayConstruct00);

			foreach (MethodInfo methodInfo in methodInfos)
			{
				if (!IsRealMemberInfo(methodInfo))
					continue;

				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodName";
				propertyConstruct00.RawValue = methodInfo.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodCallingConvention";
				propertyConstruct00.RawValue = methodInfo.CallingConvention;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodContainsGenericParameters";
				propertyConstruct00.RawValue = methodInfo.ContainsGenericParameters;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsAbstract";
				propertyConstruct00.RawValue = methodInfo.IsAbstract;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsAssembly";
				propertyConstruct00.RawValue = methodInfo.IsAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsConstructor";
				propertyConstruct00.RawValue = methodInfo.IsConstructor;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsFamily";
				propertyConstruct00.RawValue = methodInfo.IsFamily;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsFamilyAndAssembly";
				propertyConstruct00.RawValue = methodInfo.IsFamilyAndAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsFamilyOrAssembly";
				propertyConstruct00.RawValue = methodInfo.IsFamilyOrAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsFinal";
				propertyConstruct00.RawValue = methodInfo.IsFinal;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsGenericMethod";
				propertyConstruct00.RawValue = methodInfo.IsGenericMethod;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsGenericMethodDefinition";
				propertyConstruct00.RawValue = methodInfo.IsGenericMethodDefinition;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsHideBySig";
				propertyConstruct00.RawValue = methodInfo.IsHideBySig;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsPrivate";
				propertyConstruct00.RawValue = methodInfo.IsPrivate;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsPublic";
				propertyConstruct00.RawValue = methodInfo.IsPublic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsSpecialName";
				propertyConstruct00.RawValue = methodInfo.IsSpecialName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsStatic";
				propertyConstruct00.RawValue = methodInfo.IsStatic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodIsVirtual";
				propertyConstruct00.RawValue = methodInfo.IsVirtual;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodImplementationFlags";
				propertyConstruct00.RawValue = methodInfo.MethodImplementationFlags;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodReturnTypeName";
				propertyConstruct00.RawValue = methodInfo.ReturnType.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodReturnTypeNamespace";
				propertyConstruct00.RawValue = methodInfo.ReturnType.Namespace;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodReturnTypeFullName";
				propertyConstruct00.RawValue = methodInfo.ReturnType.FullName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "MethodReturnTypeAssemblyQualifiedName";
				propertyConstruct00.RawValue = methodInfo.ReturnType.AssemblyQualifiedName;
				objectConstruct00.Items.Add(propertyConstruct00);

				if (methodInfo.IsGenericMethod)
				{
					var _methodInfo = methodInfo.GetGenericMethodDefinition();

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "MethodGenericMethodDefinitionName";
					propertyConstruct00.RawValue = _methodInfo.DeclaringType.Name;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "MethodGenericMethodDefinitionNamespace";
					propertyConstruct00.RawValue = _methodInfo.DeclaringType.Namespace;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "MethodGenericMethodDefinitionFullName";
					propertyConstruct00.RawValue = _methodInfo.DeclaringType.FullName;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "MethodGenericMethodDefinitionAssemblyQualifiedName";
					propertyConstruct00.RawValue = _methodInfo.DeclaringType.AssemblyQualifiedName;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				ModelCustomAttributes(methodInfo, objectConstruct00);

				parameterInfos = methodInfo.GetParameters();
				Array.Resize(ref parameterInfos, parameterInfos.Length + 1);
				parameterInfos[parameterInfos.Length - 1] = methodInfo.ReturnParameter;

				ModelParameters(parameterInfos, objectConstruct00);

				childTypes = methodInfo.GetGenericArguments();

				ModelTypes("GenericArguments", childTypes, objectConstruct00);
			}
		}

		private static void ModelParameters(ParameterInfo[] parameterInfos, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			if ((object)parameterInfos == null)
				throw new ArgumentNullException(nameof(parameterInfos));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "Parameters";
			parent.Items.Add(arrayConstruct00);

			foreach (ParameterInfo parameterInfo in parameterInfos)
			{
				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterName";
				propertyConstruct00.RawValue = parameterInfo.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterDefaultValue";
				propertyConstruct00.RawValue = parameterInfo.DefaultValue;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterHasDefaultValue";
				propertyConstruct00.RawValue = parameterInfo.HasDefaultValue;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterIsIn";
				propertyConstruct00.RawValue = parameterInfo.IsIn;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterIsOptional";
				propertyConstruct00.RawValue = parameterInfo.IsOptional;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterIsOut";
				propertyConstruct00.RawValue = parameterInfo.IsOut;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterIsRetval";
				propertyConstruct00.RawValue = parameterInfo.IsRetval;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterTypeName";
				propertyConstruct00.RawValue = parameterInfo.ParameterType.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterTypeNamespace";
				propertyConstruct00.RawValue = parameterInfo.ParameterType.Namespace;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterTypeFullName";
				propertyConstruct00.RawValue = parameterInfo.ParameterType.FullName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterTypeAssemblyQualifiedName";
				propertyConstruct00.RawValue = parameterInfo.ParameterType.AssemblyQualifiedName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterIsByRef";
				propertyConstruct00.RawValue = parameterInfo.ParameterType.IsByRef;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "ParameterPosition";
				propertyConstruct00.RawValue = parameterInfo.Position;
				objectConstruct00.Items.Add(propertyConstruct00);

				ModelCustomAttributes(parameterInfo, objectConstruct00);
			}
		}

		private static void ModelProperties(PropertyInfo[] propertyInfos, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			ParameterInfo[] parameterInfos;
			MethodInfo methodInfo;

			if ((object)propertyInfos == null)
				throw new ArgumentNullException(nameof(propertyInfos));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = "Properties";
			parent.Items.Add(arrayConstruct00);

			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "PropertyName";
				propertyConstruct00.RawValue = propertyInfo.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "PropertyCanRead";
				propertyConstruct00.RawValue = propertyInfo.CanRead;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "PropertyCanWrite";
				propertyConstruct00.RawValue = propertyInfo.CanWrite;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "PropertyIsSpecialName";
				propertyConstruct00.RawValue = propertyInfo.IsSpecialName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "PropertyTypeName";
				propertyConstruct00.RawValue = propertyInfo.PropertyType.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "PropertyTypeNamespace";
				propertyConstruct00.RawValue = propertyInfo.PropertyType.Namespace;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "PropertyTypeFullName";
				propertyConstruct00.RawValue = propertyInfo.PropertyType.FullName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "PropertyTypeAssemblyQualifiedName";
				propertyConstruct00.RawValue = propertyInfo.PropertyType.AssemblyQualifiedName;
				objectConstruct00.Items.Add(propertyConstruct00);

				methodInfo = propertyInfo.GetGetMethod();

				if ((object)methodInfo != null)
				{
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "PropertyGetMethodIsStatic";
					propertyConstruct00.RawValue = methodInfo.IsStatic;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				methodInfo = propertyInfo.GetSetMethod();

				if ((object)methodInfo != null)
				{
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "PropertySetMethodIsStatic";
					propertyConstruct00.RawValue = methodInfo.IsStatic;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				ModelCustomAttributes(propertyInfo, objectConstruct00);

				parameterInfos = propertyInfo.GetIndexParameters();

				ModelParameters(parameterInfos, objectConstruct00);
			}
		}

		private static void ModelTypes(string arrayName, Type[] types, AssociativeXmlObject parent)
		{
			ArrayConstruct arrayConstruct00;
			PropertyConstruct propertyConstruct00;
			ObjectConstruct objectConstruct00;

			FieldInfo[] fieldInfos;
			PropertyInfo[] propertyInfos;
			MethodInfo[] methodInfos;
			EventInfo[] eventInfos;
			ConstructorInfo[] constructorInfos;

			Type[] childTypes;

			if ((object)types == null)
				throw new ArgumentNullException(nameof(types));

			if ((object)parent == null)
				throw new ArgumentNullException(nameof(parent));

			arrayConstruct00 = new ArrayConstruct();
			arrayConstruct00.Name = arrayName;
			parent.Items.Add(arrayConstruct00);

			foreach (Type type in types)
			{
				//Console.WriteLine("{0} ==> '{1}'", arrayName, type.FullName);

				objectConstruct00 = new ObjectConstruct();
				arrayConstruct00.Items.Add(objectConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeName";
				propertyConstruct00.RawValue = type.Name;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeNamespace";
				propertyConstruct00.RawValue = type.Namespace;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeFullName";
				propertyConstruct00.RawValue = type.FullName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeAssemblyQualifiedName";
				propertyConstruct00.RawValue = type.AssemblyQualifiedName;
				objectConstruct00.Items.Add(propertyConstruct00);

				var _typeInfo = type.GetTypeInfo();

				if ((object)_typeInfo.BaseType != null)
				{
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeBaseName";
					propertyConstruct00.RawValue = _typeInfo.BaseType.Name;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeBaseNamespace";
					propertyConstruct00.RawValue = _typeInfo.BaseType.Namespace;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeBaseFullName";
					propertyConstruct00.RawValue = _typeInfo.BaseType.FullName;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeBaseAssemblyQualifiedName";
					propertyConstruct00.RawValue = _typeInfo.BaseType.AssemblyQualifiedName;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeGuid";
				propertyConstruct00.RawValue = _typeInfo.GUID;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsAbstract";
				propertyConstruct00.RawValue = _typeInfo.IsAbstract;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsAnsiClass";
				propertyConstruct00.RawValue = _typeInfo.IsAnsiClass;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsArray";
				propertyConstruct00.RawValue = _typeInfo.IsArray;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsAutoClass";
				propertyConstruct00.RawValue = _typeInfo.IsAutoClass;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsAutoLayout";
				propertyConstruct00.RawValue = _typeInfo.IsAutoLayout;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsByRef";
				propertyConstruct00.RawValue = type.IsByRef;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsClass";
				propertyConstruct00.RawValue = _typeInfo.IsClass;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsComObject";
				propertyConstruct00.RawValue = _typeInfo.IsCOMObject;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsConstructedGenericType";
				propertyConstruct00.RawValue = _typeInfo.GetGenericTypeDefinition().IsConstructedGenericType;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsEnum";
				propertyConstruct00.RawValue = _typeInfo.IsEnum;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsExplicitLayout";
				propertyConstruct00.RawValue = _typeInfo.IsExplicitLayout;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsGenericParameter";
				propertyConstruct00.RawValue = _typeInfo.IsGenericParameter;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsGenericType";
				propertyConstruct00.RawValue = _typeInfo.IsGenericType;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsGenericTypeDefinition";
				propertyConstruct00.RawValue = _typeInfo.IsGenericTypeDefinition;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsImport";
				propertyConstruct00.RawValue = _typeInfo.IsImport;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsInterface";
				propertyConstruct00.RawValue = _typeInfo.IsInterface;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsLayoutSequential";
				propertyConstruct00.RawValue = _typeInfo.IsLayoutSequential;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsMarshalByRef";
				propertyConstruct00.RawValue = _typeInfo.IsMarshalByRef;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsNested";
				propertyConstruct00.RawValue = type.IsNested;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsNestedAssembly";
				propertyConstruct00.RawValue = _typeInfo.IsNestedAssembly;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsNestedFamANDAssem";
				propertyConstruct00.RawValue = _typeInfo.IsNestedFamANDAssem;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsNestedFamORAssem";
				propertyConstruct00.RawValue = _typeInfo.IsNestedFamORAssem;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsNestedFamily";
				propertyConstruct00.RawValue = _typeInfo.IsNestedFamily;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsNestedPrivate";
				propertyConstruct00.RawValue = _typeInfo.IsNestedPrivate;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsNestedPublic";
				propertyConstruct00.RawValue = _typeInfo.IsNestedPublic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsNotPublic";
				propertyConstruct00.RawValue = _typeInfo.IsNotPublic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsPointer";
				propertyConstruct00.RawValue = type.IsPointer;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsPrimitive";
				propertyConstruct00.RawValue = _typeInfo.IsPrimitive;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsPublic";
				propertyConstruct00.RawValue = _typeInfo.IsPublic;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsSealed";
				propertyConstruct00.RawValue = _typeInfo.IsSealed;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsSerializable";
				propertyConstruct00.RawValue = _typeInfo.IsSerializable;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsSpecialName";
				propertyConstruct00.RawValue = _typeInfo.IsSpecialName;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsUnicodeClass";
				propertyConstruct00.RawValue = _typeInfo.IsUnicodeClass;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsValueType";
				propertyConstruct00.RawValue = _typeInfo.IsValueType;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeIsVisible";
				propertyConstruct00.RawValue = _typeInfo.IsVisible;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeNamespace";
				propertyConstruct00.RawValue = _typeInfo.Namespace;
				objectConstruct00.Items.Add(propertyConstruct00);

				propertyConstruct00 = new PropertyConstruct();
				propertyConstruct00.Name = "TypeContainsGenericParameters";
				propertyConstruct00.RawValue = _typeInfo.ContainsGenericParameters;
				objectConstruct00.Items.Add(propertyConstruct00);

				if (_typeInfo.IsGenericParameter)
				{
					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeGenericParameterAttributes";
					propertyConstruct00.RawValue = _typeInfo.GenericParameterAttributes;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeGenericParameterPosition";
					propertyConstruct00.RawValue = type.GenericParameterPosition;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				if (_typeInfo.IsGenericType)
				{
					var _genericType = _typeInfo.GetGenericTypeDefinition();

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeGenericTypeDefinitionName";
					propertyConstruct00.RawValue = _genericType.Name;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeGenericTypeDefinitionNamespace";
					propertyConstruct00.RawValue = _genericType.Namespace;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeGenericTypeDefinitionFullName";
					propertyConstruct00.RawValue = _genericType.FullName;
					objectConstruct00.Items.Add(propertyConstruct00);

					propertyConstruct00 = new PropertyConstruct();
					propertyConstruct00.Name = "TypeGenericTypeDefinitionAssemblyQualifiedName";
					propertyConstruct00.RawValue = _genericType.AssemblyQualifiedName;
					objectConstruct00.Items.Add(propertyConstruct00);
				}

				ModelCustomAttributes(type, objectConstruct00);

				fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

				ModelFields(fieldInfos, objectConstruct00);

				propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

				ModelProperties(propertyInfos, objectConstruct00);

				methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

				ModelMethods(methodInfos, objectConstruct00);

				eventInfos = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

				ModelEvents(eventInfos, objectConstruct00);

				constructorInfos = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

				ModelConstructors(constructorInfos, objectConstruct00);

				childTypes = type.GenericTypeArguments;

				ModelTypes("GenericTypeArguments", childTypes, objectConstruct00);

				childTypes = type.GetGenericArguments();

				ModelTypes("GenericArguments", childTypes, objectConstruct00);

				if (_typeInfo.IsGenericParameter)
				{
					childTypes = _typeInfo.GetGenericParameterConstraints();

					ModelTypes("GenericParameterConstraints", childTypes, objectConstruct00);
				}

				childTypes = type.GetNestedTypes(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

				ModelTypes("NestedTypes", childTypes, objectConstruct00);
			}
		}

		protected override object CoreGetSourceObject(string sourceFilePath, IDictionary<string, IList<string>> properties)
		{
			ObjectConstruct objectConstruct00;

			List<Assembly> assemblies;
			Assembly assembly;
			IEnumerable<AssemblyName> assemblyNames;

			if ((object)sourceFilePath == null)
				throw new ArgumentNullException(nameof(sourceFilePath));

			if ((object)properties == null)
				throw new ArgumentNullException(nameof(properties));

			if (SolderFascadeAccessor.DataTypeFascade.IsWhiteSpace(sourceFilePath))
				throw new ArgumentOutOfRangeException(nameof(sourceFilePath));

			assemblies = new List<Assembly>();
			sourceFilePath = Path.GetFullPath(sourceFilePath);

			if (File.Exists(sourceFilePath))
				assemblyNames = new AssemblyName[] { new AssemblyName(sourceFilePath) };
			else if (Directory.Exists(sourceFilePath))
				assemblyNames = Directory.EnumerateFiles(sourceFilePath, "*.*", SearchOption.TopDirectoryOnly).Select(f => new AssemblyName(f)); // 2016-11-01 (dpbullington@gmail.com): changed this to support wildcard directory search
			else
				assemblyNames = null;

			if ((object)assemblyNames != null)
			{
				foreach (AssemblyName assemblyName in assemblyNames)
				{
					// 2016-11-01 (dpbullington@gmail.com): changed this to fail gracefully and support wildcard directory search
					try
					{
						assembly = Assembly.Load(assemblyName);
					}
					catch (ReflectionTypeLoadException)
					{
						assembly = null;
					}

					if ((object)assembly != null)
						assemblies.Add(assembly);

					//if ((object)assembly == null) throw new InvalidOperationException(string.Format("Failed to load the assembly file '{0}' via Assembly.LoadFile(..).", sourceFilePath));}
				}
			}

			objectConstruct00 = new ObjectConstruct();

			ModelAssemblies(assemblies.ToArray(), objectConstruct00);

			return objectConstruct00;
		}

		#endregion
	}
}