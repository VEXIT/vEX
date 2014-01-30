/*
 * Author:			Vex Tatarevic
 * Date Created:	2010-09-03
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 *
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

/*****
 * 
 *  Validation RegExp  - http://msdn.microsoft.com/en-us/library/ms998267.aspx
 *  ASP.Net Client API - http://msdn.microsoft.com/en-us/library/aa479045.aspx
 * */

namespace vEX.Text
{
    public class Validate
    {
        // MUST BE IN SYNC WITH : vEX.Validate.js
        public enum Type { NotEmpty, Range, Numeric, Alphabetic, AlphaNumeric, Email };
        /// <summary>
        ///     Only digits 0-9
        /// </summary>
        public const string ExpNumeric = "^[0-9]+$";
        /// <summary>
        ///     Only decimal numbers with maximum 2 decimal places
        /// </summary>
        public const string ExpDecimal2Places = @"^\d*\.\d{1,2}$"; //  ^\d+(\.\d{1,2})?$

        /// <summary>
        ///     Only alphabetic characters a-z , A-Z allowed
        /// </summary>
        public const string ExpAlphabetic = "^[a-zA-Z]+$";
        /// <summary>
        ///     Alphabetic characters a-z , A-Z , spaces and single quotes
        /// </summary>
        public const string ExpAlphabeticExtended = @"^[a-zA-Z'\s]+$";
        public const string ExpAlphaNumeric = "^[0-9a-zA-Z]+$";
        public const string ExpAlphaNumericWithSpaces = @"^[a-zA-Z0-9'.\s]*$";
        public static string ExpStringRange(int min, int max) { return @"^[\w\s]{" + min + "," + max + "}$"; }
        //public const string ExpEmail = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        public const string ExpEmail = @"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"; // from microsoft

        /// <summary>
        ///  Username validation patern
        ///  3 to 32 characters and start with a letter. You may use letters, numbers, underscores, and one dot
        /// </summary>
        public const string ExpUsername = "^[0-9a-zA-Z]{3,32}$"; // /^[a-z](?=[\w.]{2,30}$)\w*\.?\w*$/i

        // <summary>
        // BAAAAAAAAAAAAAAAD   Password (simple) validation patern
        // http://nilangshah.wordpress.com/2007/06/26/password-validation-via-regular-expression/
        //  At least 6 chars long , start and end with letter
        // </summary>
        //public const string ExpPasswordSimple = @"^[A-Za-z]\w{4,}[A-Za-z]$";

        /// <summary>
        ///  6 to 20 chars long AlphaNumeric and special characters @ # $ % & * ( ) - _ + ] [ ' ; : ? . , !
        /// </summary>
        public const string ExpPasswordSimple = @"^[a-zA-Z0-9@#$%&*+\-_(),+':;?.,!\[\]\\/]{6,20}$";

        /// <summary>
        ///  Password (medium) validation patern
        ///  Any length , at least 1 number, at least 1 lower case letter, and at least 1 upper case letter.
        /// </summary>
        public const string ExpPasswordMedium = @"^\w*(?=\w*\d)(?=\w*[a-z])(?=\w*[A-Z])\w*$";
        /// <summary>
        ///  Password (advanced) validation patern
        ///  At least 10 chars long, at least 1 lower case letter, 1 upper case letter, 1 digit and 1 special character. Valid special characters are - @#$%^&+=
        /// </summary>
        public const string ExpPasswordStrong = @"^.*(?=.{10,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$";

        /// <summary>
        ///  Validate Length In Range - ONLY LENGTH BETWEEN MIN & MAX
        /// </summary>
        public static bool IsLengthInRange(string text, int min, int max)
        {
            return (text.Length >= min && text.Length <= max);
        }
        /// <summary>
        ///  Validate Numeric - ONLY NUMBERS
        /// </summary>
        public static bool IsNumeric(string text)
        {
            return Regex.IsMatch(text, ExpNumeric);
        }
        /// <summary>
        ///  Validate Decimal with max 2 decimal places 
        /// </summary>
        public static bool IsDecimal2Places(string text)
        {
            return Regex.IsMatch(text, ExpDecimal2Places);
        }
        /// <summary>
        ///  Validate Alphabetic - ONLY Alphabet Letters
        /// </summary>
        public static bool IsAlphabetic(string text)
        {
            return Regex.IsMatch(text, ExpAlphabetic);
        }
        /// <summary>
        ///   Validate Alpha_Numeric - ONLY NUMBERS & LETTERS
        /// </summary>
        public static bool IsAlphaNumeric(string text)
        {
            return Regex.IsMatch(text, ExpAlphaNumeric);
        }
        /// <summary>
        ///  Function to allow only AlphaNumeric characters and Spaces.
        /// </summary>
        public static bool IsAlphaNumericWithSpaces(string text)
        {
            return Regex.IsMatch(text, ExpAlphaNumericWithSpaces);
        }
        /// <summary>
        ///	  Function to test for Positive Integers.
        /// </summary>
        public static bool IsNaturalNumber(String strNumber)
        {
            Regex objNotNaturalPattern = new Regex("[^0-9]");
            Regex objNaturalPattern = new Regex("0*[1-9][0-9]*");

            return !objNotNaturalPattern.IsMatch(strNumber) &&
                objNaturalPattern.IsMatch(strNumber);
        }
        /// <summary>
        ///	   Function to test for Positive Integers with zero inclusive
        /// </summary>
        public static bool IsWholeNumber(string strNumber)
        {
            Regex objNotWholePattern = new Regex("[^0-9]");

            return !objNotWholePattern.IsMatch(strNumber);
        }
        /// <summary>
        ///	  Function to Test for Integers both Positive & Negative
        /// </summary>
        public static bool IsInteger(string strNumber)
        {
            Regex objNotIntPattern = new Regex("[^0-9-]");
            Regex objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");

            return !objNotIntPattern.IsMatch(strNumber) &&
                objIntPattern.IsMatch(strNumber);
        }
        /// <summary>
        ///	 Function to Test for Positive Number both Integer & Real
        /// </summary>
        public static bool IsPositiveNumber(string strNumber)
        {
            Regex objNotPositivePattern = new Regex("[^0-9.]");
            Regex objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");

            return !objNotPositivePattern.IsMatch(strNumber) &&
                objPositivePattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber);
        }
        /// <summary>
        ///	  Function to test whether the string is valid number or not
        /// </summary>
        public static bool IsNumber(string strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber) &&
                !objTwoMinusPattern.IsMatch(strNumber) &&
                objNumberPattern.IsMatch(strNumber);
        }


        /// <summary>
        ///   Validate Email
        /// </summary>
        public static bool IsEmail(string text) { return Regex.IsMatch(text, ExpEmail); }
        /// <summary>
        ///   Validate Username
        /// </summary>
        public static bool IsUsername(string text) { return Regex.IsMatch(text, ExpUsername); }
        /// <summary>
        ///   Validate Password
        /// </summary>
        public static bool IsPasswordSimple(string text) { return Regex.IsMatch(text, ExpPasswordSimple); }



    } // End Class


} // End Namespace
