using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Optomo
{
	public class ClassDetailer
	{
		public static ConsoleColor defaultColor = ConsoleColor.White;
		public static ConsoleColor keyword = ConsoleColor.Blue;
		public static ConsoleColor backgroundColor = ConsoleColor.Black;
		public static ConsoleColor identifier = ConsoleColor.White;
		public static ConsoleColor interfaceColor = ConsoleColor.DarkMagenta;
		public static ConsoleColor type = ConsoleColor.Cyan;
		public static ConsoleColor generic = ConsoleColor.Green;
		public static ConsoleColor divider = ConsoleColor.DarkGray;

		public static void GetDetails<T>(T obj, bool onlyPublic) where T : Type
		{
			Type type = obj;
			
			// change console colors
			Console.ForegroundColor = defaultColor;

			if (onlyPublic && !type.IsVisible)
			{
				Console.Write($"Private {type}");
			}
			if (type.IsClass || type.IsValueType)
			{
				//Console.WriteLine();
				GetFormattedClassInfo(type, onlyPublic);
			}
			if (type.IsEnum)
			{
				//Console.WriteLine();
				GetFormattedEnumInfo(type, onlyPublic);
			}
			if ( type.IsFunctionPointer )
			{
				//Console.WriteLine();
				GetFormattedMethodsInfo( type.GetMethod(type.Name), onlyPublic );
			}
		}
		public static void GetFormattedEnumInfo( Type type, bool onlyPublic )
		{
			if ( !type.IsVisible && onlyPublic )
				return;
			StringBuilder str = new StringBuilder();

			Console.WriteLine();
			if ( type.IsPublic )
				Console.Write( $"public" );

			if ( type.IsCollectible )
				Console.Write( " partial" );
			if ( type.IsSealed )
				Console.Write( " sealed" );
			if ( type.IsAbstract )
				Console.Write( " abstract" );

			Console.Write(" enum");
			Console.Write( $" {type.Name}" );

			if ( type.GetInterfaces().Length > 0 )
			{
				Console.Write( " :" );
				Type[] interfaces = type.GetInterfaces();
				for ( int i = 0; i < interfaces.Length; i++ )
				{
					GetFormattedInterfaceInfo( interfaces [ i ] );
				}
			}
		}
		public static void GetFormattedClassInfo( Type type, bool onlyPublic )
		{
			if ( !type.IsVisible && onlyPublic)
				return;

			Console.ForegroundColor = keyword;
			if ( type.IsPublic ) Console.Write( $"public" );
			else Console.Write( $"private" );

			if ( type.IsCollectible ) Console.Write( " partial" );
			if ( type.IsSealed ) Console.Write( " sealed" );
			if ( type.IsAbstract ) Console.Write( " abstract" );

			if ( type.IsClass ) Console.Write( " class" );
			if ( type.IsValueType ) Console.Write( " struct" );

			Console.ForegroundColor = identifier;
			Console.Write( $" {type.Name}" );

            if ( type.IsGenericType )
            {
				Console.ForegroundColor = defaultColor;
				Console.Write( "<" );
				Type[] gens = type.GetGenericArguments();
				for ( int i = 0; i < gens.Length; i++ )
				{
					GetFormattedGenericArgInfo( gens [ i ] );
					if ( i < gens.Length - 1 )
					{
						Console.ForegroundColor = defaultColor;
						Console.Write( ", " );
					}
				}
				Console.ForegroundColor = defaultColor;
				Console.Write( ">" );
				
			}
            if ( type.GetInterfaces().Length > 0 )
			{
				Console.Write( " :" );
				Type[] interfaces = type.GetInterfaces();
				for ( int i = 0; i < interfaces.Length; i++ )
				{
					GetFormattedInterfaceInfo( interfaces [ i ] );
					Console.ForegroundColor = defaultColor;
					if (i < interfaces.Length - 1 ) 
					{
						Console.Write(", ");
					}
				}
			}
			const int length = 100;
			Console.WriteLine();
			Console.ForegroundColor = divider;
			Console.WriteLine(Divider("-", "Nested", length ) );
			foreach ( Type t in type.GetNestedTypes() )
			{
				Console.WriteLine();
				GetFormattedClassBasicInfo( t, onlyPublic );
			}
			Console.WriteLine();

			Console.ForegroundColor = divider;
			Console.WriteLine( Divider( "-", "Properties", length ) );
			foreach ( PropertyInfo prop in type.GetProperties() )
			{
				Console.WriteLine();
				GetFormattedPropertiesInfo( prop );
			}
			Console.WriteLine(); 

			Console.ForegroundColor = divider;
			Console.WriteLine( Divider( "-", "Fields", length ) );
			foreach ( FieldInfo field in type.GetFields() )
			{
				Console.WriteLine();
				GetFormattedFieldsInfo( field, onlyPublic );
			}
			Console.WriteLine();

			Console.ForegroundColor = divider;
			Console.WriteLine(Divider( "-", "Methods", length ) );
			foreach ( MethodInfo method in type.GetMethods() )
			{
				Console.WriteLine();
				GetFormattedMethodsInfo( method, onlyPublic );
			}
			Console.WriteLine();
		}
		public static void GetFormattedClassBasicInfo ( Type type, bool onlyPublic )
		{
			if ( !type.IsVisible && onlyPublic )
				return;

			Console.ForegroundColor = keyword;
			if ( type.IsPublic )
				Console.Write( $"public" );
			else
				Console.Write( $"private" );	

			if ( type.IsCollectible )
				Console.Write( " partial" );
			if ( type.IsSealed )
				Console.Write( " sealed" );
			if ( type.IsAbstract )
				Console.Write( " abstract" );

			if ( type.IsClass )
				Console.Write( " class" );
			if ( type.IsValueType )
				Console.Write( " struct" );

			Console.ForegroundColor = defaultColor;
			Console.Write( $" {type.Name}" );
			if ( type.IsGenericType )
			{
				Console.ForegroundColor = defaultColor;
				Console.Write( "<" );
				Type[] gens = type.GetGenericArguments();
				for ( int i = 0; i < gens.Length; i++ )
				{
					GetFormattedGenericArgInfo( gens [ i ] );
					if ( i < gens.Length - 1 )
					{
						Console.ForegroundColor = defaultColor;
						Console.Write( ", " );
					}
				}
				Console.ForegroundColor = defaultColor;
				Console.Write( ">" );

			}
			if ( type.GetInterfaces().Length > 0 )
			{
				Console.Write(" :");
				Type[] interfaces = type.GetInterfaces();
				for (int i = 0; i < interfaces.Length; i++)
				{
					GetFormattedInterfaceInfo( interfaces[i] );
					if ( i < interfaces.Length - 1 )
					{
						Console.ForegroundColor = defaultColor;
						Console.Write( ", " );
					}
				}
			}
		}
		public static void GetFormattedInterfaceInfo( Type type )
		{
			StringBuilder str = new StringBuilder();
			Console.ForegroundColor = interfaceColor;
			Console.Write($" {type.Name}");
			if (type.IsGenericType)
			{
				Console.ForegroundColor = defaultColor;
				Console.Write("<");
				Type[] genericArgs = type.GetGenericArguments();
				for (int i = 0; i < genericArgs.Length; i++) 
				{
					Console.ForegroundColor = generic;
					GetFormattedGenericArgInfo( genericArgs[i] );
					if ( i < genericArgs.Length - 1 )
					{
						Console.ForegroundColor = defaultColor;
						Console.Write(", ");
					}
				}
				Console.ForegroundColor = defaultColor;
				Console.Write(">");
			}
		}
		public static void GetFormattedGenericArgInfo( Type type )
		{
			Console.ForegroundColor = generic;
			StringBuilder str = new StringBuilder();
			Console.Write($"{type.Name}");
		}
		public static void GetFormattedFieldsInfo( FieldInfo field, bool onlyPublic )
		{
			if ( field.IsPrivate && onlyPublic )
				return;

			Console.ForegroundColor = keyword;
			
			if (field.IsPublic)	Console.Write($"public");
			if (field.IsPrivate) Console.Write($"private");

			if (field.IsStatic) Console.Write(" static");
			if (field.IsInitOnly) Console.Write(" readonly");
			if (field.IsLiteral) Console.Write(" const");

			Console.ForegroundColor = type;
			Console.Write($" {field.FieldType.Name}");
			if ( field.FieldType.IsGenericType )
			{
				Console.ForegroundColor = defaultColor;
				Console.Write( "<" );
				Type[] gens = field.FieldType.GetGenericArguments();
				for ( int i = 0; i < gens.Length; i++ )
				{
					GetFormattedGenericArgInfo( gens [ i ] );
					if ( i < gens.Length - 1 )
					{
						Console.ForegroundColor = defaultColor;
						Console.Write( ", " );
					}
				}
				Console.ForegroundColor = defaultColor;
				Console.Write( ">" );

			}
			Console.ForegroundColor = identifier;
			Console.Write( $" {field.Name}" );
		}
		public static void GetFormattedPropertiesInfo( PropertyInfo prop )
		{
			Console.ForegroundColor = keyword;
			StringBuilder str = new StringBuilder();
			Console.Write( $"public" );
			Console.ForegroundColor = type;
			Console.Write( $" {prop.PropertyType.Name}" );
			Console.ForegroundColor = identifier;
			Console.Write( $" {prop.Name}" );
			Console.ForegroundColor = defaultColor;
			Console.Write( "{ " );
			Console.ForegroundColor = keyword;
			if ( prop.CanRead ) Console.Write( " get;" );
			
			if ( prop.CanWrite ) Console.Write( " set;" );
			Console.ForegroundColor = defaultColor;
			Console.Write(" }");
		}
		public static void GetFormattedMethodsInfo( MethodInfo method, bool onlyPublic )
		{
			Console.ForegroundColor = keyword;
			if ( method.IsPrivate && onlyPublic )
				return;
			
			if ( method.IsPublic )
				Console.Write( $"public" );

			if ( method.IsFamily)
				Console.Write( $"protected" );
			if ( method.IsFamilyAndAssembly )
				Console.Write( $"private protected" );
			if ( method.IsFamilyOrAssembly )
				Console.Write( $"protected internal" );
			if ( method.IsAssembly )
				Console.Write( $"internal" );
			
			if ( method.IsCollectible )
				Console.Write( " partial" );
			if ( method.IsStatic )
				Console.Write( " static" );
			if ( method.IsFinal )
				Console.Write( " sealed" );
			if ( method.IsVirtual )
				Console.Write( " virtual" );
			if ( method.IsAbstract )
				Console.Write( " abstract" );

			Console.ForegroundColor = type;
			Console.Write( $" {method.ReturnType.Name}" );
			if ( method.ReturnType.IsGenericType )
			{
				Console.ForegroundColor = defaultColor;
				Console.Write( "<" );
				Type[] gens = method.ReturnType.GetGenericArguments();
				for ( int i = 0; i < gens.Length; i++ )
				{
					GetFormattedGenericArgInfo( gens [ i ] );
					if ( i < gens.Length - 1 )
					{
						Console.ForegroundColor = defaultColor;
						Console.Write( ", " );
					}
				}
				Console.ForegroundColor = defaultColor;
				Console.Write( ">" );

			}
			Console.ForegroundColor = identifier;
			Console.Write( $" {method.Name}" );
			if (method.IsGenericMethod) 
			{
				Console.ForegroundColor = defaultColor;
				Console.Write("<");
				Type[] gens = method.GetGenericArguments();
				for (int i = 0; i < gens.Length; i++) 
				{
					GetFormattedGenericArgInfo(gens[i]);
					if (i < gens.Length - 1 )
					{
						Console.ForegroundColor = defaultColor;
						Console.Write( ", " );
					}
				}
				Console.ForegroundColor = defaultColor;
				Console.Write(">");
			}
			Console.ForegroundColor = defaultColor;
			Console.Write("(");
			ParameterInfo[] param = method.GetParameters();
			for (int i = 0; i < param.Length; i++)
			{
				GetFormattedParametersInfo( param[i] );
				Console.ForegroundColor = defaultColor;
				if ( i != param.Length - 1 ) Console.Write( ", " );
			}
			Console.ForegroundColor = defaultColor;
			Console.Write( " )" );
		}
		public static string GetFormattedParametersInfo(ParameterInfo param)
		{
			Console.ForegroundColor = defaultColor;
			StringBuilder str = new StringBuilder();
			if (param.IsOptional) Console.Write(" [Optional]");
			Console.ForegroundColor = keyword;
			if (param.IsIn) Console.Write(" in");
			if (param.IsOut) Console.Write(" out");
			if (param.ParameterType.IsByRef) Console.Write(" ref");
			Console.ForegroundColor = type;
			Console.Write($" {param.ParameterType.Name}");
			if ( param.ParameterType.IsGenericType )
			{
				Console.ForegroundColor = defaultColor;
				Console.Write( "<" );
				Type[] gens = param.ParameterType.GetGenericArguments();
				for ( int i = 0; i < gens.Length; i++ )
				{
					GetFormattedGenericArgInfo( gens [ i ] );
					if ( i < gens.Length - 1 )
					{
						Console.ForegroundColor = defaultColor;
						Console.Write( ", " );
					}
				}
				Console.ForegroundColor = defaultColor;
				Console.Write( ">" );

			}
			Console.ForegroundColor = identifier;
			Console.Write( $" {param.Name}" );

			return str.ToString();
		}

		private static string Divider(string str, int length)
		{
			StringBuilder strBuild = new StringBuilder();
			strBuild.Append(Joiner(str, length));
			return strBuild.ToString();
		}
		private static string Divider(string str, string label, int length)
		{
			StringBuilder strBuild = new StringBuilder();
			int displace = label.Length;
			strBuild.Append(Joiner (str, (int)( length * 0.5 - displace * 0.5 ) ) );
			strBuild.Append( label );
			strBuild.Append(Joiner( str, (int)( length * 0.5 - displace * 0.5 ) ) );
			return strBuild.ToString();
		}
		private static string Joiner(string str, int length)
		{
			StringBuilder strBuild = new StringBuilder();
			for (int i = 0; i < length; i++ )
			{
				strBuild.Append(str);
			}
			return strBuild.ToString();
		}
	}
}
