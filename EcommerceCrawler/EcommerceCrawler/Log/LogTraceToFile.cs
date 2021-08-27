using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Threading;

namespace EcommerceCrawler.Log
{    //Example:

    public enum eTRACELEVEL
    {
        DISABLED = -1,
        ERROR = 0,
        WARNING = 1,
        INFORMATION = 2,
        DEBUG = 3
    }

    /// <summary>
    /// Represents a log writer class.
    /// </summary>
    public static class LogTraceToFile
    {
        //Holds information whether Writing to a log file or not,
        private static bool m_bWrittingToLogFile = false;
        //Holds the no of message to be logged store in the queue.
        private static int _logInfoCnt = 0;
        //Holds the trace file information in which log will written.
        private static Hashtable m_htTraceFileInfo = Hashtable.Synchronized(new Hashtable());
        //Queue that holds the Message or Log entered through WriteLog()
        private static Queue _logQueue = Queue.Synchronized(new Queue());
        //The default encoding is set to UTF8. The text ecoding that will be used for writing in file
        private static Encoding m_encoding = Encoding.UTF8;

        private static TimerCallback dlgThreadProc = new TimerCallback(OnTimerChange);
        private static Timer m_tmrPool = new Timer(dlgThreadProc, null, 50, 50);
        public static bool ShowInConsole = false;
        public static eTRACELEVEL TraceLogLevel = eTRACELEVEL.DEBUG;
        public static string defTraceID = "ECC_" + "_log";
        private static string _defaultLogFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Replace("file:\\", "");
        //System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        //Maximum allow for a log file.
        private static int _logSizeLimit = 5;


        public static string DefaultLogFolder
        {
            get { return LogTraceToFile._defaultLogFolder; }
            set { LogTraceToFile._defaultLogFolder = value; }
        }

        /// <summary>
        /// Gets or sets the Limit of log file size in MB. Default is 5 mb. Set it to 0 for removing the limit.
        /// </summary>
        public static int LogSizeLimit
        {
            get { return LogTraceToFile._logSizeLimit; }
            set { LogTraceToFile._logSizeLimit = value; }
        }

        /// <summary>
        /// Sets encoding of the file in which the file will be opened and written.
        /// </summary>
        /// <param name="pEncoding">Encoding type of file</param>
        public static void SetEncoding(Encoding pEncoding)
        {
            m_encoding = pEncoding;
        }

        /// <summary>
        /// Adds a traceFile in which the Log will be written.
        /// If the traceID already exists then it updates the fileName with the New FileName.
        /// logFileName fomat = [TraceLogPath]\[TraceID]_[YYYYMMDD].log
        /// </summary>
        /// <param name="TraceID">The trace Id which represents file name in which the message will be logged. Default is productname + "_log"</param>
        /// <param name="TraceLogPath">The traceLog path in which traceLog file will be saved.</param>
        public static void AddTraceFile(string TraceID, string TraceLogPath)
        {
            //if fileName is not set the set the TraceID in the fileName
            //If fileNameInitial.Trim() = "" Then
            // fileNameInitial = TraceID
            //End If
            lock ((m_htTraceFileInfo.SyncRoot))
            {
                if (m_htTraceFileInfo.ContainsKey(TraceID))
                {
                    m_htTraceFileInfo[TraceID] = TraceLogPath;
                }
                else
                {
                    m_htTraceFileInfo.Add(TraceID, TraceLogPath);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="TraceID">The trace Id which represents file name in which the message will be logged. Default is productname + "_log"</param>
        public static void AddTraceFile(string TraceID)
        {
            AddTraceFile(TraceID, "");
        }

        /// <summary>
        /// Adds a traceFile in which the Log will be written.
        /// If the traceID already exists then it updates the fileName with the New FileName.
        /// logFileName fomat = [TraceLogPath]\[TraceID]_[YYYYMMDD].log
        /// </summary>
        public static void AddTraceFile()
        {
            AddTraceFile(defTraceID, "");
        }

        /// <summary>
        /// Removes a traceFile information
        /// </summary>
        /// <param name="TraceID">The trace Id which represents file name to be removed from trace queue</param>
        public static void RemoveTraceFile(string TraceID)
        {
            lock ((m_htTraceFileInfo.SyncRoot))
            {
                if (m_htTraceFileInfo.ContainsKey(TraceID))
                {
                    m_htTraceFileInfo.Remove(TraceID);
                }
            }
        }

        /// <summary>
        /// Adds LogInformation into the queue.
        /// </summary>
        /// <param name="logInfo">Trace Log Information</param>
        private static void EnqueueLogInfo(TraceLogInfo logInfo)
        {
            //Lock the queue
            lock ((_logQueue.SyncRoot))
            {
                if (IsLogFileSizeLimitCrossed(logInfo))
                {
                    //delete log file
                    deleteTraceLogFile(logInfo);
                }

                //Add Log Info into the queue.
                _logQueue.Enqueue(logInfo);
                //increment the log information counter.
                System.Threading.Interlocked.Increment(ref _logInfoCnt);
            }
        }

        private static bool IsLogFileSizeLimitCrossed(TraceLogInfo logInfo)
        {
            //if the limit is set to 0 then no need to perform the checking.
            if (_logSizeLimit <= 0)
                return false;

            string fileName = GetTraceFileName(logInfo.TraceID);

            if (fileName == null || fileName.Trim().Length == 0)
                return false;

            FileInfo info = new FileInfo(fileName);
            if (info != null)
            {
                if (info.Exists)
                {
                    if ((info.Length / 1024 / 1024) >= _logSizeLimit)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// deletes log file
        /// </summary>
        /// <param name="logInfo">logInfo</param>
        /// <returns>true=if success,else guess?</returns>
        private static bool deleteTraceLogFile(TraceLogInfo logInfo)
        {
            string fileName = GetTraceFileName(logInfo.TraceID);
            if (fileName == null || fileName.Trim().Length == 0)
                return false;
            FileInfo info = new FileInfo(fileName);
            if (info != null)
            {
                if (info.Exists)
                {
                    try
                    {
                        info.Delete();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Dequeues LogInformation from the queue
        /// </summary>
        /// <returns>TraceLogInfo object.</returns>
        private static TraceLogInfo DequeueLogInfo()
        {
            TraceLogInfo logInfo = null;
            if ((_logInfoCnt > 0))
            {
                //Lock the queue
                lock ((_logQueue.SyncRoot))
                {
                    //Add Log Info into the queue.
                    logInfo = (TraceLogInfo)_logQueue.Dequeue();
                    //decrement the log information counter.
                    System.Threading.Interlocked.Decrement(ref _logInfoCnt);
                }
            }
            return logInfo;
        }

        /// <summary>
        /// Enqueues the TaceLogInfo in the queue to be written in the log file
        /// Writing is done by ThreadProcWriteLog() proc
        /// </summary>
        /// <param name="logInfo">Trace Log Information</param>
        public static void WriteLog(TraceLogInfo logInfo)
        {
            if ((logInfo.TraceLevel == eTRACELEVEL.DISABLED))
            {
                return; // TODO: might not be correct. Was : Exit Sub
            }
            if ((logInfo.TraceLevel > TraceLogLevel))
            {
                return; // TODO: might not be correct. Was : Exit Sub
            }

            logInfo.ThreadID = Thread.CurrentThread.ManagedThreadId.ToString();

            //Add the Log trace info to the queue.
            EnqueueLogInfo(logInfo);

            //2) Check if a process is already running that is writing to log file or not.
            // if already running then do noting.
            // else start thread that writes into the traceFiles.
            //If Not m_bWrittingToLogFile Then
            // m_bWrittingToLogFile = True
            // Dim thrdStrt As ThreadStart = New ThreadStart(AddressOf ThreadProcWriteLog)
            // Dim thrd As Thread = New Thread(thrdStrt)
            // thrd.IsBackground = True
            // thrd.Name = "ThreadWriteLogTraceToFile"
            // thrd.Start()
            //End If
        }

        /// <summary>
        /// when the local timer tick this proc is called
        /// </summary>
        /// <param name="state"></param>
        private static void OnTimerChange(object state)
        {
            if (!m_bWrittingToLogFile)
            {
                m_bWrittingToLogFile = true;
                try
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadProcWriteLog));
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.ToString());
                }

            }
        }

        private static void ThreadProcWriteLog(object state)
        {
            ThreadProcWriteLog();
        }

        /// <summary>
        /// Writes log in to the file.
        /// Deques each TraceLogInfo from the queue and writes into the log file 
        /// according to the information provided in TraceLogInfo.
        /// </summary>
        private static void ThreadProcWriteLog()
        {
            try
            {
                while ((true))
                {
                    //1) Dequeues the data from the queue.
                    TraceLogInfo logInfo;
                    logInfo = DequeueLogInfo();

                    //checking for logSize Limit
                    if (IsLogFileSizeLimitCrossed(logInfo))
                    {
                        logInfo = null;
                    }

                    if ((logInfo != null))
                    {
                        if (logInfo.TraceID.Trim().Length == 0)
                            logInfo.TraceID = LogTraceToFile.defTraceID;

                        //2)opens the corresponding log file of the TraceID for writing the log.
                        string fileName = GetTraceFileName(logInfo.TraceID);
                        if ((fileName != string.Empty))
                        {
                            StreamWriter wr = null;
                            try
                            {
                                //if the file does not exists and being created then update the program version information into the file.
                                if (!File.Exists(fileName))
                                {
                                    WriteAppDetail(logInfo);
                                }

                                wr = new StreamWriter(fileName, true, m_encoding);

                                //3) Writes into the logfile
                                //Format for writing the trace log
                                // yyyy/MM/dd HH:mm:ss [intLevel] [strLog]
                                string strMsg = string.Format("{0:yyyy/MM/dd HH:mm:ss} {1} {2:000} {3}", DateTime.Now, (int)logInfo.TraceLevel, Convert.ToInt32(logInfo.ThreadID), logInfo.Message);

                                //display the log message into console.
                                if ((ShowInConsole))
                                {
                                    System.Console.WriteLine("[{0}] :: {1}", logInfo.TraceID, strMsg);
                                }

                                //Added By Krishna. To find out the Memory Leak Problem..the used memory is appended 
                                double memoryUsed = ((System.Diagnostics.Process.GetCurrentProcess().WorkingSet64) / 1024) / 1024;

                                wr.WriteLine(strMsg + " Memory Used:" + memoryUsed.ToString("#0.00") + "MB");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            //Do nothing
                            finally
                            {
                                if ((wr != null))
                                {
                                    wr.Flush();
                                    wr.Close();
                                }
                            }
                        }
                    }

                    if ((_logInfoCnt <= 0))
                    {
                        break; // TODO: might not be correct. Was : Exit While
                    }
                }
            }
            catch
            {
            }
            finally
            {
                m_bWrittingToLogFile = false;
            }
        }

        /// <summary>
        /// Writes Application detail in log file.
        /// If the file is creating for the first time then put some file information on the top of the file.
        /// Each file should contain exe information at the top.
        /// Message format is Application Name = [xxxxx] Version No = [0.0.0.0] Last Build Date Time = [yyyy/MM/dd hh:mm:ss]
        /// </summary>
        /// <param name="logInfo">Log Trace Information</param>
        private static void WriteAppDetail(TraceLogInfo logInfo)
        {
            string fileName = GetTraceFileName(logInfo.TraceID);
            if ((fileName != string.Empty))
            {
                StreamWriter wr = null;
                try
                {
                    //if the file does not exists and being created then update the program version information into the file.
                    if (!File.Exists(fileName))
                    {
                        if (wr == null)
                            wr = new StreamWriter(fileName, true, m_encoding);


                    }
                    if (wr == null)
                        wr = new StreamWriter(fileName, true, m_encoding);

                    //FileInfo inf = new FileInfo("");

                    //3) Writes into the logfile
                    //Format for writing the trace log
                    // yyyy/MM/dd HH:mm:ss [intLevel] [strLog]
                    //Exe information should be in the format
                    //Application Name = [xxxxx] Version No = [0.0.0.0] Last Build Date Time = [yyyy/MM/dd hh:mm:ss]
                    string strMsg = string.Format("{0:yyyy/MM/dd HH:mm:ss} {1} {2:000} Application Name [] Version No [] Last Build Date Time [{yyyy/MM/dd HH:mm:ss}]", DateTime.Now, (int)logInfo.TraceLevel, Convert.ToInt32(logInfo.ThreadID));
                    //display the log message into console.
                    if ((ShowInConsole))
                    {
                        System.Console.WriteLine("[{0}] :: {1}", logInfo.TraceID, strMsg);
                    }

                    wr.WriteLine();
                    wr.WriteLine(strMsg);
                    //Write 2 blank lines
                    wr.WriteLine();
                    wr.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                //Do nothing
                finally
                {
                    if ((wr != null))
                    {
                        wr.Flush();
                        wr.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Gets valid file name from the specified TraceID
        /// </summary>
        /// <param name="TraceID">Trace Id string which represents file name</param>
        /// <returns>File name with full path</returns>
        private static string GetTraceFileName(string TraceID)
        {
            if (TraceID.Trim().Length == 0)
                TraceID = LogTraceToFile.defTraceID;
            string fileName = null;
            try
            {
                if (m_htTraceFileInfo.ContainsKey(TraceID))
                {
                    fileName = (string)m_htTraceFileInfo[TraceID];
                    //FileName Format.
                    //logFileName fomat = [TraceLogPath]\[TraceID]_[YYYYMMDD].log
                    string logTracefolder = string.Empty;
                    if (_defaultLogFolder.Trim().Length == 0)
                        _defaultLogFolder = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                    logTracefolder = _defaultLogFolder + "\\LogTrace";

                    if (fileName.Trim() == string.Empty)
                    {
                        if (!System.IO.Directory.Exists(logTracefolder))
                        {
                            System.IO.Directory.CreateDirectory(logTracefolder);
                        }
                        logTracefolder += "\\" + TraceID;

                        if (!System.IO.Directory.Exists(logTracefolder))
                        {
                            System.IO.Directory.CreateDirectory(logTracefolder);
                        }

                        fileName = string.Format("{0}\\{1}_{2:yyyyMMdd}.log", logTracefolder, TraceID, DateTime.Now);
                    }
                    else
                    {
                        if (!System.IO.Directory.Exists(fileName))
                        {
                            System.IO.Directory.CreateDirectory(fileName);
                        }

                        fileName = string.Format("{0}\\{1}_{2:yyyyMMdd}.log", fileName, TraceID, DateTime.Now);
                    }

                }
                else
                {
                    //If the TraceID not found then add and again fetch the filename
                    AddTraceFile(TraceID);
                    GetTraceFileName(TraceID);
                }
            }
            catch (Exception)
            {
                //Do nothing
            }
            return fileName;
        }

    }

    /// <summary>
    /// Represents TraceLogInfo class which holds Information to write.
    /// </summary>
    public class TraceLogInfo
    {
        ////the trace id of the file in which the trace will be written
        public string TraceID;
        public eTRACELEVEL TraceLevel;
        public string Message;
        public string ThreadID = string.Empty;

        /// <summary>
        /// Initializes a new instance of the TraceLogInfo class.
        /// When given message string.
        /// </summary>
        /// <param name="pMessage">The message string to write.</param>
        public TraceLogInfo(string pMessage)
        {
            this.TraceID = LogTraceToFile.defTraceID;
            this.TraceLevel = eTRACELEVEL.INFORMATION;
            this.Message = pMessage;
        }

        /// <summary>
        /// Initializes a new instance of the TraceLogInfo class.
        /// When given message and tracelevel.
        /// </summary>
        /// <param name="pMessage">The message string to write.</param>
        /// <param name="pTraceLevel">The trace level enumaration. Default is eTRACELEVEL.INFORMATION</param>
        public TraceLogInfo(string pMessage, eTRACELEVEL pTraceLevel)
        {
            this.TraceID = LogTraceToFile.defTraceID;
            this.TraceLevel = pTraceLevel;
            this.Message = pMessage;
        }

        /// <summary>
        /// Initializes a new instance of the TraceLogInfo class.
        /// When given message, tracelevel and traceid.
        /// </summary>
        /// <param name="pMessage">The message string to write.</param>
        /// <param name="pTraceLevel">The trace level enumaration.</param>
        /// <param name="pTraceID">The trace Id which represents file name in which the message will be logged. Default is productname + "_log"</param>
        public TraceLogInfo(string pMessage, eTRACELEVEL pTraceLevel, string pTraceID)
        {
            this.TraceID = pTraceID;
            this.TraceLevel = pTraceLevel;
            this.Message = pMessage;
        }

    }
    //TraceLogInfo
}
