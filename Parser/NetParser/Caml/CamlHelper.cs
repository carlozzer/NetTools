using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using TC.Framework.Lib.Extensions;

namespace TC.Framework.SharePoint.Caml
{
    public class CamlHelper
    {
        #region CONSTANTES PÚBLICAS

        public class Order
        {
            public const bool Ascending = true;
            public const bool Descending = false;
        }

        public class Query
        {
            public const string Where = "Where";
        }

        public class Operation
        {
            public const string Contains                = "Contains";
            public const string Eq                      = "Eq";
            public const string Neq                     = "Neq";
            public const string Or                      = "Or";
            public const string And                     = "And";
            public const string BeginsWith              = "BeginsWith";
            public const string Geq                     = "Geq";
            public const string Leq                     = "Leq";
        }

        public class ValueType
        {
            public const string Text                    = "Text";
            public const string Choice                  = "Choice";
            public const string Boolean                 = "Boolean";
            public const string Counter                 = "Counter";
            public const string Number                  = "Number";
            public const string Lookup                  = "Lookup";
            public const string DateTime                = "DateTime";
            public const string DateOnly                = "DateOnly";
        }

        public class FieldRefName
        {
            public const string ID                      = "ID";
            public const string Author                  = "Author";
            public const string Title                   = "Title";
            public const string ContentType             = "ContentType";
            public const string CreadoPor               = "Creado por";
            public const string FileLeafRef             = "FileLeafRef";
        }

        #endregion

        #region ACENTOS EN CAML

        private static string Patron = "a|e|i|o|u|á|é|í|ó|ú|à|è|ì|ò|ù|ü";

        private static char[][] diacriticos = new char[5][] 
        { 
            new char[] { 'á', 'à' },            // familia a
            new char[] { 'é', 'è' },            // familia e
            new char[] { 'í', 'ì' },            // familia i
            new char[] { 'ó', 'ò' },            // familia o
            new char[] { 'ú', 'ù', 'ü' }        // familia u
        };

        private static bool EsVocal(char letra)
        {
            Regex regex = new Regex(Patron);
            return regex.IsMatch(letra.ToString());
        }

        private static char GetVocalFamilia(char vocal)
        {
            char ret = vocal; // por defecto se queda como está

            for (int i = 0; i < 5; i++)
            {
                if (diacriticos[i].Contains<char>(vocal)) ret = (char)(i + 'a');
            }

            return ret;
        }

        public static string QuitarDiacriticos(string palabra)
        {
            string ret = string.Empty;

            if (palabra != null && palabra != "")
            {
                StringBuilder sb = new StringBuilder();
                char letra = default(char);
                for (int i = 0; i < palabra.Length; i++)
                {
                    letra = EsVocal(palabra[i]) ? GetVocalFamilia(palabra[i]) : palabra[i];
                    sb.Append(letra);
                }
                ret = sb.ToString();
            }

            return ret;
        }


        private static string GenerarAlternativa(string palabra, int index, char alternativa)
        {

            string cruda = QuitarDiacriticos(palabra);
            StringBuilder sb = new StringBuilder(cruda);
            sb.Replace(palabra[index], alternativa, index, 1);
            return sb.ToString();

        }

        private static List<string> GenerarAlternativasAcentos(string inPalabra)
        {
            List<string> words = null;
            string palabra = string.Empty;

            // analizar palabra, buscamos las vocales y generamos una palabra 
            // acentuada por cada vocal y una palabra sin acentos
            words = new List<string>();
            if (!string.IsNullOrEmpty(inPalabra))
            {
                // bien peinadita
                palabra = QuitarDiacriticos(inPalabra);
                palabra = palabra.ToLower();

                words.Add(palabra); // la añadimos sin ningún acento

                for (int i = 0; i < palabra.Length; i++)
                {
                    if (EsVocal(palabra[i]))
                    {
                        // generamos las nuevas palabras
                        char vocal = GetVocalFamilia(palabra[i]);
                        int index = (int)Enum.Parse(typeof(vocales), vocal.ToString());
                        char[] familia = diacriticos[index];
                        for (int j = 0; j < familia.Length; j++)
                        {
                            string newWord = GenerarAlternativa(palabra, i, familia[j]);
                            words.Add(newWord);
                        }
                    }
                }
            }
            return words;
        }

        private static List<String> ExtractWords(string sentence)
        {
            List<string> ret = null;

            string stripped = sentence;
            string patron = ".,;:-_'";

            for (int i = 0; i < patron.Length; i++)
            {
                stripped = stripped.Replace(patron[i], ' ');
            }

            string[] palabras = sentence.Split(' ');

            if (palabras != null && palabras.Length > 0)
            {
                ret = new List<string>();
                foreach (string palabra in palabras)
                {
                    ret.Add(palabra.Trim());
                }
            }

            return ret;
        }

        public static string GenerarCamlAcentos(string comparator, string FieldRefName, string ValueType, string value)
        {
            string ret = string.Empty;
            List<string> criterios = new List<string>();

            // value puede taner varias palabras (varios criterios)
            List<string> palabras = ExtractWords(value);
            if (palabras != null && palabras.Count > 0)
            {
                foreach (string palabra in palabras)
                {
                    List<string> alternativas = GenerarAlternativasAcentos(palabra);
                    criterios.AddRange(alternativas);
                }
            }

            if (criterios != null && criterios.Count > 0)
            {
                string newBlock = string.Empty;
                string superblock = string.Empty;

                foreach (string criterio in criterios)
                {
                    newBlock = NewBlock(comparator, FieldRefName, ValueType, criterio);
                    if (!string.IsNullOrEmpty(superblock))
                    {
                        superblock = AddLogicOp(Operation.Or, superblock, newBlock);
                    }
                    else
                    {
                        superblock = newBlock;
                    }
                }

                ret = superblock;
            }

            return ret;
        }



        #endregion

        #region CAML BLOCK FACTORY

        public static string NewBlock(string comp, string FieldRefName, string ValueType, string value)
        {
            string ret = string.Empty;

            string includes = "";
            if (ValueType.ToLower() == "datetime") {
                includes = "IncludeTimeValue=\"TRUE\" ";
            }

            if (ValueType == "DateOnly") ValueType = "DateTime";
                            
            string pattern = "<{0}><FieldRef Name='{1}' /><Value {4}Type='{2}'>{3}</Value></{0}>";
            ret = string.Format(pattern, comp, FieldRefName, ValueType, value, includes);

            return ret;
        }    

        public static string AddLogicOp(string op, string block1, string block2)
        {
            string ret = string.Empty;

            ret = string.Format("{0}{1}", block1, block2);

            bool UseOperator = (!string.IsNullOrEmpty(block1)) && (!string.IsNullOrEmpty(block2));
            if (UseOperator) ret = AddTag(op, ret);

            return ret;
        }

        public static string AddTag(string Tag, string innerXml)
        {
            return string.Format("<{0}>{1}</{0}>", Tag, innerXml);
        }

        public static string AddWhere(string caml)
        {
            return AddTag(Query.Where, caml);
        }

        public static string NewOrder(string FieldRefName, bool ascending)
        {
            string ret = string.Empty;

            string asc = ascending ? "True" : "False";

            if (!string.IsNullOrEmpty(FieldRefName))
            {
                return string.Format("<OrderBy><FieldRef Name='{0}' Ascending='{1}' /></OrderBy>", FieldRefName, asc);
            }

            return ret;
        }

        #endregion

        #region CALM ORDER

        public static string BuildOrderBy(string order_text)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(order_text))
            {
                string[] segments = order_text.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (segments != null && segments.Length > 0)
                {
                    sb.Append("<OrderBy>");

                    foreach (string segment in segments)
                    {
                        string[] parts = segment.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (parts != null && parts.Length > 0)
                        {
                            string field = parts[0];
                            string direction = string.Empty;

                            if (parts.Length > 1)
                            {
                                if (parts[1].ToLower() == "asc") direction = " Ascending='True'";
                                if (parts[1].ToLower() == "desc") direction = " Ascending='False'";
                            }

                            sb.AppendFormat("<FieldRef Name='{0}'{1} />", field, direction);
                        }
                    }

                    sb.Append("</OrderBy>");
                }
            }

            return sb.ToString();
        }

        #endregion

        #region FIELDS

        public static string GetViewFields(string xsl)
        {
            // lo primero eliminamos comentarios
            Regex regex = new Regex("<!--(.*?)-->", RegexOptions.Singleline);
            string xslsincomentarios = regex.Replace(xsl, string.Empty);

            // obtenemos los atributos que requerimos de la lista
            regex = new Regex("@[0-9A-Za-z_]+");
            MatchCollection matches = regex.Matches(xslsincomentarios);

            List<string> Atributos = new List<string>();

            if (matches != null && matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    string atributo = matches[i].Value.Replace("@", string.Empty);
                    if (!Atributos.Contains(atributo)) Atributos.Add(atributo);
                }
            }

            string viewFields = string.Empty;
            if (Atributos.Count > 0)
            {
                foreach (string atributo in Atributos)
                {
                    viewFields += string.Format("<FieldRef Name='{0}' />", atributo);
                }
            }

            return viewFields;
        }

        #endregion

        #region FILTERS

        private static string 
        QueryContentType ( string ctypes ) {
            
            string ret = string.Empty;

            if (!string.IsNullOrWhiteSpace(ctypes))
            {
                string[] tipos = ctypes.ExtractSeparatedValues(";");
                if (tipos != null)
                {
                    for (int i = 0; i < tipos.Length; i++)
                    {
                        ret = CamlHelper.AddLogicOp(CamlHelper.Operation.Or, ret, CamlHelper.NewBlock(CamlHelper.Operation.Eq, CamlHelper.FieldRefName.ContentType, CamlHelper.ValueType.Text, tipos[i]));
                    }
                }
            }

            return ret;
        }

        private static string
        StripQuery ( string q) {

            return !string.IsNullOrWhiteSpace(q) ?
                                        q.Replace("<where>", string.Empty, true)
                                         .Replace("</where>", string.Empty, true)
                                         .Replace("<query>", string.Empty, true)
                                         .Replace("</query>", string.Empty, true)
                                        : string.Empty;
        }

        #endregion

        #region CAML BUILDER

        // ContentTypes separados por (;)
        public static string BuildCaml(string OriginalCamlQuery, string ContentTypes, string Order, string FilterColumnName, string FilterValue)
        {
            // CONTENT TYPE
            string queryContenType = QueryContentType(ContentTypes);

            // USER QUERY
            string queryCaml = StripQuery(OriginalCamlQuery);

            // WildCards en WebPartBase

            // FILTER
            if (!string.IsNullOrWhiteSpace(FilterColumnName) && !string.IsNullOrWhiteSpace(FilterValue))
            {
                string queryFilter = CamlHelper.NewBlock(CamlHelper.Operation.Contains, FilterColumnName, CamlHelper.ValueType.Text, FilterValue);
                queryCaml = CamlHelper.AddLogicOp(CamlHelper.Operation.And, queryFilter, queryCaml);
            }

            // <WHERE> WRAPPING
            string where = CamlHelper.AddWhere(CamlHelper.AddLogicOp(CamlHelper.Operation.And, queryContenType, queryCaml));

            // ORDER ascending by default
            string orderCaml = string.Empty;
            if (!string.IsNullOrWhiteSpace(Order))
            {
                orderCaml = BuildOrderBy(Order);
            }

            // Si no hay info en el where este no aprece
            where = where.Replace("<Where></Where>", string.Empty);

            return where + orderCaml;
        }

        public static string BuildCaml(string OriginalCamlQuery, string ContentTypes, string Order, string[] FilterColumnNames, string FilterValue)
        {
            // CONTENT TYPE
            string queryContenType = QueryContentType(ContentTypes);

            // USER QUERY
            string queryCaml = StripQuery(OriginalCamlQuery);

            // WildCards en WebPartBase

            // FILTER
            string queryFilter = string.Empty;
            if ( FilterColumnNames != null && FilterColumnNames.Length > 0 && FilterValue.NotEmpty()) {

                foreach (string fcn in FilterColumnNames)
                {
                    string filter_block = CamlHelper.NewBlock(CamlHelper.Operation.Contains, fcn, CamlHelper.ValueType.Text, FilterValue);
                    queryFilter = CamlHelper.AddLogicOp(CamlHelper.Operation.Or, queryFilter, filter_block);
                }
            }

            queryCaml = CamlHelper.AddLogicOp(CamlHelper.Operation.And, queryFilter, queryCaml);


            // <WHERE> WRAPPING
            string where = CamlHelper.AddWhere(CamlHelper.AddLogicOp(CamlHelper.Operation.And, queryContenType, queryCaml));

            // ORDER ascending by default
            string orderCaml = string.Empty;
            if (!string.IsNullOrWhiteSpace(Order))
            {
                orderCaml = BuildOrderBy(Order);
            }

            // Si no hay info en el where este no aprece
            where = where.Replace("<Where></Where>", string.Empty);

            return where + orderCaml;
        }

        #endregion

    }
}
