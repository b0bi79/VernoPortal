using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Verno.Reports.Models;

namespace Verno.Reports.Executing
{
    public class ListValues: List<ListItem>
    {
        private readonly RepParameter _par;

        public ListValues(RepParameter par)
        {
            _par = par;
        }

        /// <inheritdoc />
        public ListValues(RepParameter par, IEnumerable<ListItem> collection) : base(collection)
        {
            _par = par;
        }

        public static ListValues Parse(RepParameter par, Report report, out bool lazy)
        {
            lazy = false;
            var str = par.ListValues;

            if (string.IsNullOrEmpty(par.ListValues))
                return null;

            if (str.StartsWith("LAZY{"))
            {
                lazy = true;
                return null;
            }
            return Parse(par, report);
        }

        public static ListValues Parse(RepParameter par, Report report)
        {
            var str = par.ListValues;

            if (string.IsNullOrEmpty(str))
                return null;

            if (str.StartsWith("LAZY{"))
                str = str.Substring(4);

            if (str[0] == '[' && str[str.Length - 1] == ']')
            {
                var quotes = new[] {'"', '\'', ' '};
                var items = from gr in str.Trim('[', ']').Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    select gr.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)
                    into vals
                    let id = vals[0].Trim(quotes)
                    let name = vals.Length == 1 ? vals[0].Trim(quotes) : vals[1].Trim(quotes)
                    select new ListItem(id, name);
                return new ListValues(par, items);
            }
            else if (str[0] == '{' && str[str.Length - 1] == '}')
                using (var conn = report.Connection.CreateConnection())
                {
                    conn.Open();

                    var user = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = str.Trim('{', '}');
                    AddParameter(cmd, "@_user", user.Name);

                    var pars = user.Claims
                        .Where(c => !c.Type.StartsWith("http:"))
                        .GroupBy(c => c.Type, c => c.Value)
                        .Select(claim => new KeyValuePair<string, string>("@_" + claim.Key.Replace(".", "_"), string.Join(", ", claim)))
                        .ToList();

                    pars.ForEach(x=>AddParameter(cmd, x.Key, x.Value));

                    var result = new ListValues(par);
                    var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        result.Add(new ListItem(r[0].ToString(), r.FieldCount == 1 ? r[0].ToString() : r[1].ToString()));
                    }
                    return result;
                }
            else
                throw new SyntaxErrorException("List value syntax error.");
        }

        public string GetName(object item)
        {
            var strItem = item as string;
            var value = strItem != null 
                ? this.FirstOrDefault(i => i.Id == strItem) 
                : this.FirstOrDefault(i => GetTypedValue(i.Id, _par.ValueType).Equals(item));
            return value != null ? value.Name : null;
        }

        public static object GetTypedValue(object value, string valueType)
        {
            return value.ChangeType(TypeExtensions.ParseTypeCode(valueType).ToType());
        }

        private static void AddParameter(IDbCommand cmd, string name, string value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value;
            cmd.Parameters.Add(param);
        }
    }
}