/*
 * Author:			Vex Tatarevic
 * Date Created:	2013-02-12
 * Copyright:       VEX IT Pty Ltd 2013 - www.vexit.com
 * 
 * Description:     Object used to store data for grid paging
 * 
 */

using System;
using System.Collections.Generic;

namespace vEX.DataAccess
{
    /// <summary>
    ///  This is a class used to store data in a format for grid paging.
    ///  It is implemented in VEXIT ModMVC Web development framework
    /// </summary>
    public class DataPage<T>
    {
        public DataPage()
        {
            PageNumber = 1;
            PageSize = 20;
            RangeLength = 10;
            Result = new List<T>();
        }
        /// <summary>
        ///  Current page number
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        ///  Number of records per page
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        ///  Total number of pages. Derived property
        /// </summary>
        public int PageCount
        {
            get
            {
                if (_PageCount == -1)
                {
                    if (PageSize == 0) PageSize = 20;
                    decimal count = (TotalCount / PageSize);
                    decimal roundCount = Math.Ceiling(count);
                    _PageCount = (int)roundCount;
                }
                return _PageCount;
            }
        }
        private int _PageCount = -1;
        /// <summary>
        ///  Range of clickable page links that are visible to the user
        /// </summary>
        public int RangeLength { get; set; }
        /// <summary>
        ///  Number of first page in the visible range
        /// </summary>
        public int RangeFirst
        {
            get
            {
                if (_RangeFirst == -1 && RangeLength > 0)
                {
                    var m = (PageNumber % RangeLength);
                    if (m == 0)
                        _RangeFirst = (PageNumber - RangeLength) + 1;
                    else
                        _RangeFirst = (PageNumber - m) + 1;
                }
                return _RangeFirst;
            }
        }
        private int _RangeFirst = -1;
        /// <summary>
        ///  Number of last page in the visible range
        /// </summary>
        public int RangeLast
        {
            get
            {
                if (_RangeLast == -1 && RangeLength > 0)
                {
                    var m = (PageNumber % RangeLength);
                    if (m == 0)
                        _RangeLast = PageNumber;
                    else
                    {
                        _RangeLast = (PageNumber - m) + RangeLength;
                        if (_RangeLast > PageCount)
                            _RangeLast = PageCount;
                    }
                }
                return _RangeLast;
            }
        }
        private int _RangeLast = -1;
        /// <summary>
        ///  Total number of records for all pages returned by a given paged search.
        ///  MUST BE A DECIMAL ! - otherwise the code will break
        /// </summary>
        public decimal TotalCount { get; set; }
        /// <summary>
        /// Field by which the records are ordered
        /// </summary>
        public string SortField { get; set; }
        /// <summary>
        /// If false sort descending on Sort field
        /// </summary>
        public bool SortAscending { get; set; }
        /// <summary>
        ///  List of records for the current page
        /// </summary>
        public List<T> Result { get; set; }

    }
}