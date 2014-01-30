/*
 * Author:			vEX - Vedran Tatarevic
 * Date Created:	2007-08
 *                  2013-08 - ExportListToExcel
 * Copyright:       VEXIT Pty Ltd - www.vexit.com
 */

using System.Web.UI.WebControls;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
namespace vEX.Web
{
    public class Exporter
    {
        public static void ExportListToExcel<T>(List<T> list, string exportFileName, HttpResponse response)
        {

            Type t = list[0].GetType();

            //make a new instance of the class name we figured out to get its props
            object o = Activator.CreateInstance(t);
            //gets all properties
            PropertyInfo[] props = o.GetType().GetProperties();
            //foreach of the properties in class above, write out properties
            //this is the header row

            System.Text.StringBuilder resultXML = new System.Text.StringBuilder();
            resultXML.Append("<TABLE>");
            resultXML.Append("<TR>");
            foreach (PropertyInfo pi in props)
            {
                resultXML.Append("<TD>");
                resultXML.Append(pi.Name.ToUpper());
                resultXML.Append("</TD>");
            } resultXML.Append("</TR>");

            //this acts as datarow
            foreach (T item in list)
            {
                resultXML.Append("<TR>");
                //this acts as datacolumn
                foreach (PropertyInfo pi in props)
                {
                    //this is the row+col intersection (the value)
                    string whatToWrite =
                        Convert.ToString(item.GetType()
                                             .GetProperty(pi.Name)
                                             .GetValue(item, null)).Replace(',', ' ');

                    resultXML.Append("<TD>" + whatToWrite + "</TD>"); 

                } resultXML.Append("</TR>");
            }
            resultXML.Append("</TABLE>");
            response.Clear();
            response.AddHeader("content-disposition", string.Format("attachment;filename={0}", exportFileName));
            response.ContentType = "application/ms-excel";
            response.Write(resultXML.ToString());
            response.End();
        }

        // Grid TO Excel
        public static void ExportGridViewToExcel(GridView grd, string exportFileName, HttpResponse response)
        {

            System.Text.StringBuilder resultXML = new System.Text.StringBuilder();
            DataColumn objColumn = new DataColumn();
            string ColumnName = "";
            string ColumnValue = "";

            resultXML.Append("<HTML><BODY>");
            resultXML.Append("<TABLE>");


            //------------- HEADER
            resultXML.Append("<TR>");
            foreach (TableCell HeaderCell in grd.HeaderRow.Cells)
            {
                ColumnName = HeaderCell.Text;
                resultXML.Append("<TD>");
                resultXML.Append(ColumnName);
                resultXML.Append("</TD>");
            }
            resultXML.Append("</TR>");


            //------------- ROWS
            foreach (GridViewRow ReportRow in grd.Rows)
            {

                resultXML.Append("<TR>");

                foreach (DataControlFieldCell ReportCell in ReportRow.Cells)
                {
                    if (ReportCell.HasControls())
                    {
                        // Has Control
                        //this must be our template field for the address
                        foreach (Control CellControl in ReportCell.Controls)
                        {
                            if (CellControl.GetType().ToString().Contains("Label"))
                            {
                                Label CellLabel = (Label)CellControl;
                                if (!string.IsNullOrEmpty(CellLabel.Text))
                                {
                                    ColumnValue = CellLabel.Text;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Doesnt have Control
                        ColumnValue = ReportCell.Text;
                    }

                    resultXML.Append("<TD>" + ColumnValue + "</TD>");
                }

                resultXML.Append("</TR>");
            }


            //------------- FOOTER
            resultXML.Append("<TR>");
            foreach (TableCell FooterCell in grd.FooterRow.Cells)
            {

                ColumnValue = FooterCell.Text;

                resultXML.Append("<TD>" + ColumnValue + "</TD>");
            }
            resultXML.Append("</TR>");



            resultXML.Append("</TABLE>");
            resultXML.Append("</BODY></HTML>");

            response.Clear();
            //Response.Buffer = True
            response.AddHeader("content-disposition", string.Format("attachment;filename={0}", exportFileName));
            response.ContentType = "application/vnd.ms-excel";
            response.Charset = "";
            response.ContentEncoding = System.Text.Encoding.UTF7;

            response.Write(resultXML.ToString());

            response.End();
        }

    }
}
