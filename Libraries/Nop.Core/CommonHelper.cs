using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Nop.Core.Infrastructure;

namespace Nop.Core
{
    public static class CommonExtention
    {
        #region Public extension methods...

        /// <summary>
        /// Chuyen dang datetime sang string co dang yyyy-MM-dd
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string toStringApi(this DateTime? input)
        {
            return input.HasValue ? input.Value.ToString("yyyy-MM-dd") : "";
        }

        public static int MonthDifference(this DateTime? lValue, DateTime? rValue)
        {
            if (lValue.HasValue && rValue.HasValue)
                return (lValue.Value.Month - rValue.Value.Month) + 12 * (lValue.Value.Year - rValue.Value.Year);
            return 0;
        }

        /// <summary>
        /// Chuyen dang datetime sang string co dang yyyy-MM-dd HH:mm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string toStringApiWithTime(this DateTime? input)
        {
            return input.HasValue ? input.Value.ToString("yyyy-MM-dd HH:mm:ss") : "";
            //return input.HasValue ? input.Value.ToString("dd-MM-yyyy HH:mm:ss") : "";
        }

        public static string toStringApiWithTime(this DateTime input)
        {
            return input != DateTime.MinValue ? input.ToString("yyyy-MM-dd HH:mm:ss") : "";
            //return input.HasValue ? input.Value.ToString("dd-MM-yyyy HH:mm:ss") : "";
        }

        /// <summary>
        /// Chuyen dang string yyyy-MM-dd sang datetime
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DateTime? toDateSys(this string input, string format = "")
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    DateTime ret;
                    if (string.IsNullOrEmpty(format))
                    {
                        ret = Convert.ToDateTime(input); //DateTime.ParseExact(input, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                        return ret;
                    }

                    if (DateTime.TryParseExact(input, format, new System.Globalization.CultureInfo("vi-VN"), System.Globalization.DateTimeStyles.None, out ret))
                    {
                        return ret;
                    }
                }
                catch { }
            }
            return null;
        }

        public static string toDateVNString(this string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                try
                {
                    DateTime ret = Convert.ToDateTime(input);
                    return ret.toDateVNString();
                }
                catch { }
            }
            return "";
        }

        public static string toDateVNString(this DateTime input, bool isShowTime = false)
        {
            if (isShowTime)
                return input.ToString("dd/MM/yyyy HH:mm");
            return input.ToString("dd/MM/yyyy");
        }

        public static string toDateVNString(this DateTime? input, bool isShowTime = false)
        {
            if (input.HasValue)
            {
                return input.Value.toDateVNString(isShowTime);
            }
            return "";
        }

        /// <summary>
        /// chuyen ngay thuong sang ngay nguoc
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToDistanceTime(this DateTime input)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - input.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "vừa xong" : ts.Seconds + " giây trước";

            if (delta < 2 * MINUTE)
                return "1 phút trước";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " phút trước";

            if (delta < 90 * MINUTE)
                return "1 giờ trước";

            if (delta < 24 * HOUR)
                return ts.Hours + " giờ trước";

            if (delta < 48 * HOUR)
                return "Hôm qua";

            if (delta < 30 * DAY)
                return ts.Days + " ngày trước";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "1 tháng trước" : months + " tháng trước";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "1 năm trước" : years + " năm trước";
            }
        }

        #endregion Public extension methods...

        #region Object to json

        public static string ToStringJson(this Object obj, bool isLowerCase = false)
        {
            if (obj == null)
                return "";
            try
            {
                if (!isLowerCase)
                    return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                else
                {
                    var serializerSettings = new Newtonsoft.Json.JsonSerializerSettings();
                    serializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                    serializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                    serializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                    return Newtonsoft.Json.JsonConvert.SerializeObject(obj, serializerSettings);
                }
            }
            catch (Exception ex)
            {
                string _ex = ex.Message;
            }
            return "";
        }

        /// <summary>
        /// Chuyen doi chuoi json thanh dang entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strjson"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this string strjson)
        {
            if (string.IsNullOrEmpty(strjson))
                return default(T);
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(strjson);
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// Chuyen doi chuoi json thanh dang list(entity)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strjson"></param>
        /// <returns></returns>
        public static List<T> ToEntities<T>(this string strjson)
        {
            if (string.IsNullOrEmpty(strjson))
                return default(List<T>);
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(strjson);
            }
            catch { }
            return default(List<T>);
        }

        #endregion Object to json

        #region Khac

        private static Random _random = new Random();
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string RandomString(int length)
        {
            return new string(Enumerable.Repeat(CHARS, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static string ToCode(this int Id, int len = 5)
        {
            return Id.ToString().PadLeft(len, '0');
        }

        private static Regex _convertToUnsign_rg = new Regex("\\p{IsCombiningDiacriticalMarks}+");

        /// <summary>
        /// Chuyen thanh tieng Viet khong dau
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public static string ToNoSign(this string strInput, bool isUpper = true)
        {
            var temp = strInput.Normalize(System.Text.NormalizationForm.FormD);
            string _input = _convertToUnsign_rg.Replace(temp, string.Empty).Replace("đ", "d").Replace("Đ", "D");
            if (isUpper)
                return _input.ToUpper();
            return _input;
        }

        /// <summary>
        /// Chuyen doi du lieu so sang dang format string co ngan cach theo so cua VN
        /// Chi ap dung cho du lieu dang so
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToVNStringNumber(this Object val, bool isFloat = false)
        {
            if (isFloat)
            {
                return (Math.Round(Convert.ToDecimal(val), 2)).ToString("###,###,##0.#0");
            }
            return Convert.ToDecimal(val).ToString("###,###,##0");
        }

        /// <summary>
        /// Chuyển đổi ký tự dạng số (có mask) sang số
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static decimal ToNumber(this String val)
        {
            if (val == null)
                return 0;
            return Convert.ToDecimal(val.Replace(".", ""));
        }

        /// <summary>
        /// Chuyển đổi ký tự dạng số (có mask) sang số
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int ToNumberInt32(this object val)
        {
            if (val == null)
                return 0;
            int rout = 0;
            Int32.TryParse(val.ToString().Replace(".", ""), out rout);
            return rout;
        }

        public static string ToPercentString(this Object val)
        {
            return Convert.ToDecimal(val).ToString("0.00%");
        }

        /// <summary>
        /// Tao duong dan cho viec luu tru file theo datetime yyyy\mm\dd
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string ToPathFolderStore(this DateTime val)
        {
            return string.Format("{0}\\{1}\\{2}", val.ToString("yyyy"), val.ToString("MM"), val.ToString("dd"));
        }

        /// <summary>
        /// Lay thong tin ten tu ten day du
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static string ToTen(this string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
                return "";
            return fullname.Split(' ').Last().Trim();
        }

        /// <summary>
        /// Lay thong tin ho va ten dem tu ten day du
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static string ToHo(this string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
                return "";
            int pos = fullname.LastIndexOf(' ');
            if (pos > 0)
                return fullname.Substring(0, pos).Trim();
            return fullname;
        }

        public static string RemoveVietnameseTone(this string text)
        {
            string result = text.Trim();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            result = Regex.Replace(result, "À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ẳ|Ặ|Ẳ|Ẵ|/g", "A");
            result = Regex.Replace(result, "È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ|/g", "E");
            result = Regex.Replace(result, "Ì|Í|Ị|Ỉ|Ĩ|/g", "I");
            result = Regex.Replace(result, "Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ|/g", "O");
            result = Regex.Replace(result, "Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ|/g", "U");
            result = Regex.Replace(result, "Ỳ|Ý|Ỵ|Ỷ|Ỹ|/g", "y");
            result = Regex.Replace(result, "Đ", "D");
            return result;
        }
        #endregion Khac
    }

    /// <summary>
    /// Represents a common helper
    /// </summary>
    public partial class CommonHelper
    {
        #region Fields

        //we use EmailValidator from FluentValidation. So let's keep them sync - https://github.com/JeremySkinner/FluentValidation/blob/master/src/FluentValidation/Validators/EmailValidator.cs
        private const string _emailExpression = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-||_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+([a-z]+|\d|-|\.{0,1}|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])?([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))$";

        private static readonly Regex _emailRegex;

        #endregion Fields

        #region Ctor

        static CommonHelper()
        {
            _emailRegex = new Regex(_emailExpression, RegexOptions.IgnoreCase);
        }

        #endregion Ctor

        #region Methods


        /// <summary>
        /// Ensures the subscriber email or throw.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public static string EnsureSubscriberEmailOrThrow(string email)
        {
            var output = EnsureNotNull(email);
            output = output.Trim();
            output = EnsureMaximumLength(output, 255);

            if (!IsValidEmail(output))
            {
                throw new NopException("Email is not valid.");
            }

            return output;
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            email = email.Trim();

            return _emailRegex.IsMatch(email);
        }

        /// <summary>
        /// Verifies that string is an valid IP-Address
        /// </summary>
        /// <param name="ipAddress">IPAddress to verify</param>
        /// <returns>true if the string is a valid IpAddress and false if it's not</returns>
        public static bool IsValidIpAddress(string ipAddress)
        {
            return IPAddress.TryParse(ipAddress, out IPAddress _);
        }

        /// <summary>
        /// Generate random digit code
        /// </summary>
        /// <param name="length">Length</param>
        /// <returns>Result string</returns>
        public static string GenerateRandomDigitCode(int length)
        {
            var random = new Random();
            var str = string.Empty;
            for (var i = 0; i < length; i++)
                str = string.Concat(str, random.Next(10).ToString());
            return str;
        }

        /// <summary>
        /// Returns an random integer number within a specified rage
        /// </summary>
        /// <param name="min">Minimum number</param>
        /// <param name="max">Maximum number</param>
        /// <returns>Result</returns>
        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

        /// <summary>
        /// Ensure that a string doesn't exceed maximum allowed length
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="maxLength">Maximum length</param>
        /// <param name="postfix">A string to add to the end if the original string was shorten</param>
        /// <returns>Input string if its length is OK; otherwise, truncated input string</returns>
        public static string EnsureMaximumLength(string str, int maxLength, string postfix = null)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            if (str.Length <= maxLength)
                return str;

            var pLen = postfix?.Length ?? 0;

            var result = str.Substring(0, maxLength - pLen);
            if (!string.IsNullOrEmpty(postfix))
            {
                result += postfix;
            }

            return result;
        }

        /// <summary>
        /// Ensures that a string only contains numeric values
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Input string with only numeric values, empty string if input is null/empty</returns>
        public static string EnsureNumericOnly(string str)
        {
            return string.IsNullOrEmpty(str) ? string.Empty : new string(str.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Ensure that a string is not null
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Result</returns>
        public static string EnsureNotNull(string str)
        {
            return str ?? string.Empty;
        }

        /// <summary>
        /// Indicates whether the specified strings are null or empty strings
        /// </summary>
        /// <param name="stringsToValidate">Array of strings to validate</param>
        /// <returns>Boolean</returns>
        public static bool AreNullOrEmpty(params string[] stringsToValidate)
        {
            return stringsToValidate.Any(string.IsNullOrEmpty);
        }

        /// <summary>
        /// Compare two arrays
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="a1">Array 1</param>
        /// <param name="a2">Array 2</param>
        /// <returns>Result</returns>
        public static bool ArraysEqual<T>(T[] a1, T[] a2)
        {
            //also see Enumerable.SequenceEqual(a1, a2);
            if (ReferenceEquals(a1, a2))
                return true;

            if (a1 == null || a2 == null)
                return false;

            if (a1.Length != a2.Length)
                return false;

            var comparer = EqualityComparer<T>.Default;
            return !a1.Where((t, i) => !comparer.Equals(t, a2[i])).Any();
        }

        /// <summary>
        /// Sets a property on an object to a value.
        /// </summary>
        /// <param name="instance">The object whose property to set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetProperty(object instance, string propertyName, object value)
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            var instanceType = instance.GetType();
            var pi = instanceType.GetProperty(propertyName);
            if (pi == null)
                throw new NopException("No property '{0}' found on the instance of type '{1}'.", propertyName, instanceType);
            if (!pi.CanWrite)
                throw new NopException("The property '{0}' on the instance of type '{1}' does not have a setter.", propertyName, instanceType);
            if (value != null && !value.GetType().IsAssignableFrom(pi.PropertyType))
                value = To(value, pi.PropertyType);
            pi.SetValue(instance, value, new object[0]);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType)
        {
            return To(value, destinationType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="destinationType">The type to convert the value to.</param>
        /// <param name="culture">Culture</param>
        /// <returns>The converted value.</returns>
        public static object To(object value, Type destinationType, CultureInfo culture)
        {
            if (value == null)
                return null;

            var sourceType = value.GetType();

            var destinationConverter = TypeDescriptor.GetConverter(destinationType);
            if (destinationConverter.CanConvertFrom(value.GetType()))
                return destinationConverter.ConvertFrom(null, culture, value);

            var sourceConverter = TypeDescriptor.GetConverter(sourceType);
            if (sourceConverter.CanConvertTo(destinationType))
                return sourceConverter.ConvertTo(null, culture, value, destinationType);

            if (destinationType.IsEnum && value is int)
                return Enum.ToObject(destinationType, (int)value);

            if (!destinationType.IsInstanceOfType(value))
                return Convert.ChangeType(value, destinationType, culture);

            return value;
        }

        /// <summary>
        /// Converts a value to a destination type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public static T To<T>(object value)
        {
            //return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            return (T)To(value, typeof(T));
        }

        /// <summary>
        /// Convert enum for front-end
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Converted string</returns>
        public static string ConvertEnum(string str)
        {
            if (string.IsNullOrEmpty(str)) return string.Empty;
            var result = string.Empty;
            foreach (var c in str)
                if (c.ToString() != c.ToString().ToLower())
                    result += " " + c.ToString();
                else
                    result += c.ToString();

            //ensure no spaces (e.g. when the first letter is upper case)
            result = result.TrimStart();
            return result;
        }

        /// <summary>
        /// Set Telerik (Kendo UI) culture
        /// </summary>
        public static void SetTelerikCulture()
        {
            //little hack here
            //always set culture to 'en-US' (Kendo UI has a bug related to editing decimal values in other cultures)
            var culture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        /// <summary>
        /// Get difference in years
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            //source: http://stackoverflow.com/questions/9/how-do-i-calculate-someones-age-in-c
            //this assumes you are looking for the western idea of age and not using East Asian reckoning.
            var age = endDate.Year - startDate.Year;
            if (startDate > endDate.AddYears(-age))
                age--;
            return age;
        }

        /// <summary>
        /// Get private fields property value
        /// </summary>
        /// <param name="target">Target object</param>
        /// <param name="fieldName">Field name</param>
        /// <returns>Value</returns>
        public static object GetPrivateFieldValue(object target, string fieldName)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target", "The assignment target cannot be null.");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("fieldName", "The field name cannot be null or empty.");
            }

            var t = target.GetType();
            FieldInfo fi = null;

            while (t != null)
            {
                fi = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

                if (fi != null) break;

                t = t.BaseType;
            }

            if (fi == null)
            {
                throw new Exception($"Field '{fieldName}' not found in type hierarchy.");
            }

            return fi.GetValue(target);
        }

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets or sets the default file provider
        /// </summary>
        public static INopFileProvider DefaultFileProvider { get; set; }

        public static string DecryptString(string strInput)
        {
            string strsrc = "";
            try
            {
                if ((strInput != null) && (strInput != ""))
                {
                    byte[] objInput = Convert.FromBase64String(strInput);
                    strsrc = System.Text.UTF32Encoding.UTF8.GetString(objInput);
                }
            }
            catch
            {
            }
            return strsrc;
        }

        #endregion Properties
    }
}