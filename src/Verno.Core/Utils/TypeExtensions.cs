using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if !WindowsCE
using System.Linq.Expressions;
#endif
using System.Reflection;

namespace Verno
{
    public static class TypeExtensions
    {
        public const int PeriodTypeCode = 112;
        public const int PeriodTimeTypeCode = 113;

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String) || type.IsEnum || type == typeof(DateTime) || type == typeof(Guid) || type == typeof(decimal)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Type GetItemType(this IEnumerable sourceList)
        {
            var type = sourceList.GetType();

            if (type.IsArray)
                return type.GetElementType();
            else
            {
                var genericType = (from intType in type.GetInterfaces()
                                   where intType.IsGenericType && intType.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                                   select intType.GetGenericArguments()[0]).FirstOrDefault();

                if (genericType != null)
                    return genericType;
                else
                {
                    var item = sourceList.Cast<object>().First();
                    if (item == null)
                        //throw new InvalidOperationException("Cannot define item type.");
                        return typeof(object);
                    return item.GetType();
                }
            }
        }

        public static bool IsNumeric(this Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        public static bool IsSignedIntegral(this Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        public static bool IsUnsignedIntegral(this Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        public static int GetNumericTypeKind(this Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum) return 0;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        public static bool IsEnum(this Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type GetNonNullableType(this Type type)
        {
            return IsNullable(type) ? type.GetGenericArguments()[0] : type;
        }

        public static Type ToType(this TypeCode code)
        {
            switch (code)
            {
                case TypeCode.Boolean:
                    return typeof(bool);

                case TypeCode.Byte:
                    return typeof(byte);

                case TypeCode.Char:
                    return typeof(char);

                case TypeCode.DateTime:
                    return typeof(DateTime);

                case TypeCode.DBNull:
                    return typeof(DBNull);

                case TypeCode.Decimal:
                    return typeof(decimal);

                case TypeCode.Double:
                    return typeof(double);

                case TypeCode.Empty:
                    return null;

                case TypeCode.Int16:
                    return typeof(short);

                case TypeCode.Int32:
                    return typeof(int);

                case TypeCode.Int64:
                    return typeof(long);

                case TypeCode.Object:
                    return typeof(object);

                case TypeCode.SByte:
                    return typeof(sbyte);

                case TypeCode.Single:
                    return typeof(Single);

                case TypeCode.String:
                    return typeof(string);

                case TypeCode.UInt16:
                    return typeof(UInt16);

                case TypeCode.UInt32:
                    return typeof(UInt32);

                case TypeCode.UInt64:
                    return typeof(UInt64);

                case (TypeCode)PeriodTypeCode:
                    return typeof(PeriodDate);

                case (TypeCode)PeriodTimeTypeCode:
                    return typeof(PeriodDateTime);
            }

            return null;
        }

        public static TypeCode ParseTypeCode(string type)
        {
            switch (type.ToLower())
            {
                case "integer": 
                case "int": return TypeCode.Int32;
                case "bigint": return TypeCode.Int64;
                case "money": 
                case "numeric": return TypeCode.Decimal;
                case "float": 
                case "real": return TypeCode.Double;
                case "date": return TypeCode.DateTime;
                case "varchar": return TypeCode.String;
                case "bit":
                case "bool": return TypeCode.Boolean;
                case "period": 
                case "perioddate": return (TypeCode) PeriodTypeCode;
                case "periodtime": return (TypeCode) PeriodTimeTypeCode;
                default: return type.AsEnum(TypeCode.Object);
            }
        }

        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        public static PropertyInfo GetPropertyInfo(this Type type, string fieldName)
        {
            var property = type.GetProperty(fieldName);
#if NET35
            if (property == null) throw new MissingMemberException(string.Format("Invalid property {0}.{1}", type.Name, fieldName));
#else
            if (property == null) throw new MissingMemberException(type.Name, fieldName);
#endif
            return property;
        }

        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
                return true;

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

#if !WindowsCE
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this Type type,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expresion '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }
#endif

#if NET20 || NET35 || WindowsCE 
        public static T GetCustomAttribute<T>(this MemberInfo member, bool inherit)
            where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(member, typeof (T), inherit);
        }

        public static T GetCustomAttribute<T>(this ParameterInfo member, bool inherit)
            where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(member, typeof (T), inherit);
        }
#endif
    }
}