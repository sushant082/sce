using System.Text;

namespace EcommerceCrawler.Log
{

    /// <summary>
    /// Represents wrapper class of LogTraceToFile.
    /// </summary>
    public static class LogTrace
    {

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="functionName">The Function Name which generate the Trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>
        /// <param name="traceLevel">The Trace Log information category.</param>
        /// <param name="TraceID">The TraceID or File ID in which trace Info is to be logged.</param>
        public static void WriteLog(string className, string functionName, string Msg, eTRACELEVEL traceLevel, string TraceID)
        {
            StringBuilder tmpMsg = new StringBuilder();

            if (className.Trim().Length > 0)
                tmpMsg.AppendFormat("[{0}]::", className);
            if (functionName.Trim().Length > 0)
                tmpMsg.AppendFormat("[{0}]::", functionName);
            tmpMsg.Append(Msg);

            TraceID = TraceID.Trim();
            LogTraceToFile.WriteLog(new TraceLogInfo(tmpMsg.ToString(), traceLevel, TraceID.Length == 0 ? string.Empty : TraceID));
            //Trace Write Message Format.
            //[ClassName]  [FunctionName] Message

            //if (traceLevel == eTRACELEVEL.ERROR)
            //    Utility.MsgBox(Msg);                
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="functionName">The Function Name which generate the Trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>
        /// <param name="traceLevel">The Trace Log information category.</param>
        /// <param name="TraceID">The TraceID or File ID in which trace Info is to be logged.</param>
        //public static void WriteLog(string functionName, string Msg, eTRACELEVEL traceLevel, string TraceID)
        //{
        //    WriteLog(string.Empty, functionName, Msg, traceLevel, TraceID);
        //}

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>
        /// <param name="traceLevel">The Trace Log information category.</param>
        /// <param name="TraceID">The TraceID or File ID in which trace Info is to be logged.</param>
        public static void WriteLog(string className, string Msg, eTRACELEVEL traceLevel, string TraceID)
        {
            WriteLog(className, string.Empty, Msg, traceLevel, TraceID);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>
        /// <param name="Msg">The Trace Message to be logged.</param>
        /// <param name="traceLevel">The Trace Log information category.</param>
        /// <param name="TraceID">The TraceID or File ID in which trace Info is to be logged.</param>
        public static void WriteLog(string Msg, eTRACELEVEL traceLevel, string TraceID)
        {
            WriteLog(string.Empty, string.Empty, Msg, traceLevel, TraceID);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>
        /// <param name="Msg">The Trace Message to be logged.</param>
        /// <param name="traceLevel">The Trace Log information category.</param>        
        public static void WriteLog(string Msg, eTRACELEVEL traceLevel)
        {
            WriteLog(string.Empty, string.Empty, Msg, traceLevel, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>
        /// <param name="traceLevel">The Trace Log information category.</param>        
        public static void WriteLog(string className, string Msg, eTRACELEVEL traceLevel)
        {
            WriteLog(className, string.Empty, Msg, traceLevel, string.Empty);
        }


        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteInfoLog(string className, string Msg)
        {
            WriteLog(className, string.Empty, Msg, eTRACELEVEL.INFORMATION, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>                
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteInfoLog(string Msg)
        {
            WriteLog(string.Empty, string.Empty, Msg, eTRACELEVEL.INFORMATION, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="functionName">the function name.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteInfoLog(string className, string functionName, string Msg)
        {
            WriteLog(className, functionName, Msg, eTRACELEVEL.INFORMATION, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteDebugLog(string className, string Msg)
        {
            WriteLog(className, string.Empty, Msg, eTRACELEVEL.DEBUG, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="functionName">function name that generated the trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteDebugLog(string className, string functionName, string Msg)
        {
            WriteLog(className, functionName, Msg, eTRACELEVEL.DEBUG, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>                
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteDebugLog(string Msg)
        {
            WriteLog(string.Empty, string.Empty, Msg, eTRACELEVEL.DEBUG, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteWarningLog(string className, string Msg)
        {
            WriteLog(className, string.Empty, Msg, eTRACELEVEL.WARNING, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Trace.</param>
        /// <param name="functionName">function name that generated the trace.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteWarningLog(string className, string functionName, string Msg)
        {
            WriteLog(className, functionName, Msg, eTRACELEVEL.WARNING, string.Empty);
        }

        /// <summary>
        /// Writes Trace Log into the Log File.
        /// </summary>                
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteWarningLog(string Msg)
        {
            WriteLog(string.Empty, string.Empty, Msg, eTRACELEVEL.WARNING, string.Empty);
        }

        /// <summary>
        /// Writes Error Log into the Log File.
        /// </summary>
        /// <param name="className">The Class Name which generate the error.</param>
        /// <param name="functionName">function name that generated the error.</param>
        /// <param name="Msg">The Error Message to be logged.</param>
        public static void WriteErrorLog(string className, string functionName, string Msg)
        {
            WriteLog(className, functionName, Msg, eTRACELEVEL.ERROR, string.Empty);
        }

        /// <summary>
        /// Writes Error Log into the Log File.
        /// </summary>
        /// <param name="className">The Class Name which generate the error.</param>
        /// <param name="functionName">function name that generated the error.</param>
        /// <param name="Msg">The Error Message to be logged.</param>
        /// <param name="DisplayErrMsg">true if error message need to be displayed as a message box. Otherwise, false. Default is false</param>
        public static void WriteErrorLog(string className, string functionName, string Msg, bool DisplayErrMsg)
        {
            WriteLog(className, functionName, Msg, eTRACELEVEL.ERROR, string.Empty);
            if (DisplayErrMsg)
                DisplayErrorMessage(Msg);
        }

        /// <summary>
        /// Writes Error Log into the Log File.
        /// </summary>        
        /// <param name="className">The Class Name which generate the Error.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteErrorLog(string className, string Msg)
        {
            WriteLog(className, string.Empty, Msg, eTRACELEVEL.ERROR, string.Empty);
        }

        /// <summary>
        /// Writes Error Log into the Log File.
        /// </summary>
        /// <param name="className">The Class Name which generate the Error.</param>
        /// <param name="Msg">The Trace Message to be logged.</param>
        /// <param name="DisplayErrMsg">true if error message need to be displayed as a message box. Otherwise, false. Default is false</param>
        public static void WriteErrorLog(string className, string Msg, bool DisplayErrMsg)
        {
            WriteLog(className, string.Empty, Msg, eTRACELEVEL.ERROR, string.Empty);
            if (DisplayErrMsg)
                DisplayErrorMessage(Msg);
        }

        /// <summary>
        /// Writes Error Log into the Log File.
        /// </summary>                
        /// <param name="Msg">The Trace Message to be logged.</param>        
        public static void WriteErrorLog(string Msg)
        {
            WriteLog(string.Empty, string.Empty, Msg, eTRACELEVEL.ERROR, string.Empty);
        }

        /// <summary>
        /// Writes Error Log into the Log File.
        /// </summary>
        /// <param name="Msg">Message to be logged</param>
        /// <param name="DisplayErrMsg">true if error message need to be displayed as a message box. Otherwise, false. Default is false</param>
        public static void WriteErrorLog(string Msg, bool DisplayErrMsg)
        {
            WriteLog(string.Empty, string.Empty, Msg, eTRACELEVEL.ERROR, string.Empty);

            if (DisplayErrMsg)
                DisplayErrorMessage(Msg);
        }

        /// <summary>
        /// Display error Message.
        /// </summary>
        /// <param name="errorMessage">errorMessage: Error message that has to displayed.</param>
        private static void DisplayErrorMessage(string errorMessage)
        {
            //if (this._displayErrMsg)
            //{
            StringBuilder sb = new StringBuilder();
            sb.Append("Following Error Occurred:");
            sb.AppendLine(errorMessage);
            sb.AppendLine("The error was successfully logged into the error log file.");

            //errorMessage = "Following Error Occurred:" + Microsoft.VisualBasic.Constants.vbCrLf + errorMessage;

            //errorMessage += Microsoft.VisualBasic.Constants.vbCrLf + "The error was successfully logged into the error log file.";
            /// MessageBox.Show(sb.ToString());
            sb.Remove(0, sb.Length);
            errorMessage = string.Empty;
            //}
        }

    }
}
