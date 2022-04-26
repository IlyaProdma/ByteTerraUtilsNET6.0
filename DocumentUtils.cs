using System.Diagnostics;
using System.Text.Json;

namespace ByteTerraUtils
{
    public class DocumentUtils
    {
        private GoogleUtils? _GoogleUtils;

        private enum ProtocolType
        {
            AdmissionProtocol,
            StandardMonitorProtocol,
            NonStandardMonitorProtocol
        }

        public DocumentUtils()
        {
            _GoogleUtils = null;
        }

        public DocumentUtils(string apiKey, string applicationName)
        {
            _GoogleUtils = new GoogleUtils(apiKey, applicationName);
        }

        public DocumentUtils(string serviceAccountEmail, string jsonfile, string applicationName)
        {
            _GoogleUtils = new GoogleUtils(serviceAccountEmail, jsonfile, applicationName);
        }

        private void CreatePDF(string jsonName, string template, string outputName, string dirPath)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $"/C cd pyt && python createdoc.py {jsonName} {template} {dirPath}/{outputName}";
            startInfo.UseShellExecute = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            process = new Process();
            startInfo.Arguments = $"/C latexmk -cd {dirPath}/{outputName}.tex -xelatex -halt-on-error -synctex=0 -interaction=nonstopmode";// {outputName}.tex";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            process = new Process();
            startInfo.Arguments = $"/C latexmk -c -cd {dirPath}/{outputName}.tex && cd {dirPath} && erase {outputName}.tex";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public void CreateAdmissionReport(string spreadSheetId, int sessionId, string dirPath)
        {
            if (_GoogleUtils == null)
            {
                throw new InvalidOperationException("No google account");
            }
            List<Object> dataRow = _GoogleUtils.GetRowBySessionNumber(spreadSheetId, "Data", sessionId);
            List<Object> timingRow = _GoogleUtils.GetRowBySessionNumber(spreadSheetId, "Timing", sessionId);
            if (dataRow.Count == 0 || timingRow.Count == 0)
            {
                throw new InvalidOperationException("No such sessionId");
            }
            List<Object> ionInfo = _GoogleUtils.GetIonInfoByEnvCode(spreadSheetId, dataRow[6].ToString()!)!;
            CreateAdmissionReport(dataRow, timingRow, ionInfo, dirPath);
        }

        public void CreateAdmissionReport(List<Object> dataRow, List<Object> timingRow, List<Object> ionInfo, string dirPath)
        {
            if (dataRow.Count == 0 || timingRow.Count == 0)
            {
                throw new InvalidOperationException("No such sessionId");
            }
            Session session;
            Ion ion;
            try
            {
                session = new Session(dataRow, timingRow);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException("Not enough info");
            }
            try
            {
                ion = new Ion(ionInfo);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException("Not enough info");
            }
            if (!CanCreateProtocol(session, ProtocolType.AdmissionProtocol))
            {
                throw new InvalidOperationException("Not enough info");
            }
            AdmProtocolInfo protocolInfo = new AdmProtocolInfo(session, ion);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(protocolInfo, options);
            File.WriteAllText("pyt/temp.json", jsonString);
            CreatePDF("temp.json", "dopusk.template", $"admission_{session.SessionId}", dirPath);
        }

        public void CreateMonitorReport(string spreadSheetId, int sessionId, string dirPath)
        {
            if (_GoogleUtils == null)
            {
                throw new InvalidOperationException("No google account");
            }
            List<Object> dataRow = _GoogleUtils.GetRowBySessionNumber(spreadSheetId, "Data", sessionId);
            List<Object> timingRow = _GoogleUtils.GetRowBySessionNumber(spreadSheetId, "Timing", sessionId);
            if (dataRow.Count == 0 || timingRow.Count == 0)
            {
                throw new InvalidOperationException("No such sessionId");
            }
            List<Object> ionInfo = _GoogleUtils.GetIonInfoByEnvCode(spreadSheetId, dataRow[6].ToString()!)!;
            CreateMonitorReport(dataRow, timingRow, ionInfo, dirPath);
        }

        public void CreateMonitorReport(List<Object> dataRow, List<Object> timingRow, List<Object> ionInfo, string dirPath)
        {
            if (dataRow.Count == 0 || timingRow.Count == 0)
            {
                throw new InvalidOperationException("No such sessionId");
            }
            Session session;
            Ion ion;
            try
            {
                session = new Session(dataRow, timingRow);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException("Not enough info");
            }
            try
            {
                ion = new Ion(ionInfo);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidOperationException("Not enough info");
            }
            if (!CanCreateProtocol(session, ProtocolType.StandardMonitorProtocol) &&
                !CanCreateProtocol(session, ProtocolType.NonStandardMonitorProtocol))
            {
                throw new InvalidOperationException("Not enough info");
            }
            if (session.TrackDetectors.Count <= 2)
            {
                CreateStandardMonitorReport(session, ion, dirPath);
            }
            else
            {
                CreateNonStandardMonitorReport(session, ion, dirPath);
            }
        }

        private void CreateStandardMonitorReport(Session session, Ion ion, string dirPath)
        {
            StandardMonitorProtocol protocolInfo = new StandardMonitorProtocol(session, ion);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(protocolInfo, options);
            File.WriteAllText("pyt/temp.json", jsonString);
            CreatePDF("temp.json", "monitoring.template", $"monitoring_{session.SessionId}", dirPath);
        }

        private void CreateNonStandardMonitorReport(Session session, Ion ion, string dirPath)
        {
            NonStandardMonitorProtocol protocolInfo = new NonStandardMonitorProtocol(session, ion);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(protocolInfo, options);
            File.WriteAllText("pyt/temp.json", jsonString);
            CreatePDF("temp.json", "monitoring_nonstandard.template", $"monitoring_{session.SessionId}", dirPath);
        }

        private bool CanCreateProtocol(Session session, ProtocolType protocolType)
        {
            if (session.OnlineDetectors.Contains(-1) || session.TrackDetectors.Count == 0 ||
                session.Kcoeff == -1 || session.Error == -1)
            {
                return false;
            }
            if ((protocolType == ProtocolType.StandardMonitorProtocol || protocolType == ProtocolType.AdmissionProtocol) &&
                session.Heterogeneity == -1)
            {
                return false;
            }
            else if (protocolType == ProtocolType.NonStandardMonitorProtocol &&
              (session.HeterogeneityLeft == -1 || session.HeterogeneityRight == -1))
            {
                return false;
            }
            return true;
        }
    }
}
