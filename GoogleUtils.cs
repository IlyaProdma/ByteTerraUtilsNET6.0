using System.Text.RegularExpressions;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2;

namespace ByteTerraUtils
{
    public class GoogleUtils
    {
        private SheetsService _SheetsService;

        public GoogleUtils(string apiKey, string applicationName)
        {
            _SheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                ApplicationName = applicationName,
                ApiKey = apiKey
            });
        }

        public GoogleUtils(string serviceAccountEmail, string jsonfile, string applicationName)
        {
            ServiceAccountCredential credential;
            string[] Scopes = { SheetsService.Scope.Spreadsheets };
            using (Stream stream = new FileStream(@jsonfile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                credential = (ServiceAccountCredential)
                    GoogleCredential.FromStream(stream).UnderlyingCredential;

                var initializer = new ServiceAccountCredential.Initializer(credential.Id)
                {
                    User = serviceAccountEmail,
                    Key = credential.Key,
                    Scopes = Scopes
                };
                credential = new ServiceAccountCredential(initializer);
            }
            _SheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });
        }

        public IList<IList<Object>> GetValuesFromRange(string spreadSheetId, string range)
        {
            SpreadsheetsResource.ValuesResource.GetRequest request =
                _SheetsService!.Spreadsheets.Values.Get(spreadSheetId, range);
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            return values;
        }

        public List<string> GetSheetsNames(string spreadSheetId)
        {
            var sheets = _SheetsService.Spreadsheets.Get(spreadSheetId).Execute().Sheets;
            List<string> sheetNames = sheets.Select(sheet => sheet.Properties.Title).ToList();
            return sheetNames;
        }

        public string GetLastColumn(string spreadSheetId, string sheetName)
        {
            var response = _SheetsService.Spreadsheets.Values.Get(spreadSheetId, sheetName).Execute();
            string range = response.Range;
            string lastCol = Regex.Match(range, ".+!(\\D+)\\d*:(\\D+)\\d*").Groups[2].Value;
            return lastCol;
        }

        public List<Tuple<string, string>> GetColumns(string spreadSheetId, string sheetName)
        {
            var response = _SheetsService.Spreadsheets.Values.Get(spreadSheetId, sheetName).Execute();
            string range = response.Range;
            string lastCol = Regex.Match(range, ".+!(\\D+)\\d*:(\\D+)\\d*").Groups[2].Value;
            range = $"{sheetName}!A1:{lastCol}1";
            List<string> columnHeaders = response.Values[0].Select(value => value.ToString()).ToList()!;
            List<char> col = new List<char>() { 'A' };
            List<Tuple<string, string>> columns = new List<Tuple<string, string>>();
            string colNow = new string(col.ToArray());
            int index = 0;
            while (!(colNow.Equals(lastCol)))
            {
                columns.Add(new Tuple<string, string>($"{colNow}", columnHeaders[index]));
                if (col[col.Count - 1] == 'Z')
                {
                    col[col.Count - 1] = 'A';
                    col.Add('A');
                }
                else
                {
                    col[col.Count - 1]++;
                }
                index++;
                colNow = new string(col.ToArray());
            }
            columns.Add(new Tuple<string, string>($"{lastCol}", columnHeaders[index]));
            return columns;
        }

        private int GetIndexBySessionNumber(string spreadSheetId, string sheet, int sessionNumber)
        {
            var sessionNums = _SheetsService.Spreadsheets.Values.Get(spreadSheetId, $"{sheet}!A1:A").Execute().Values;
            int index = 1;
            foreach (var sessionNum in sessionNums)
            {
                if (Int32.TryParse(Convert.ToString(sessionNum[0]), out int res) && res == sessionNumber)
                {
                    break;
                }
                index++;
            }
            if (index > sessionNums.Count)
            {
                throw new InvalidOperationException("Wrong session");
            }
            return index;
        }

        public int GetLastRowIndex(string spreadSheetId, string sheet)
        {
            return _SheetsService.Spreadsheets.Values.Get(spreadSheetId, $"{sheet}!A1:A").Execute().Values.Count();
        }

        public List<Object> GetRowBySessionNumber(string spreadSheetId, string sheet, int sessionNumber)
        {
            int index = GetIndexBySessionNumber(spreadSheetId, sheet, sessionNumber);
            string lastCol = GetLastColumn(spreadSheetId, sheet);
            List<Object> row = new List<Object>();
            var response = _SheetsService.Spreadsheets.Values.Get(spreadSheetId, $"{sheet}!A{index}:{lastCol}{index}").Execute();
            if (response.Values != null)
                row = response.Values[0].ToList();
            return row;
        }

        public List<Object>? GetIonInfoByEnvCode(string spreadSheetId, string envCode)
        {
            var ions = _SheetsService.Spreadsheets.Values.Get(spreadSheetId, $"Информация по иону!A2:O").Execute().Values;
            foreach (var ion in ions)
            {
                if (ion[ion.Count - 1].ToString()!.StartsWith(envCode) || envCode.StartsWith(ion[ion.Count - 1].ToString()!))
                {
                    return ion.ToList();
                }
            }
            return null;
        }

        public void SetTrackDetectorsInfo(string spreadSheetId, int sessionNumber, List<double> trackDetectorsInfo)
        {
            int index = GetIndexBySessionNumber(spreadSheetId, "Data", sessionNumber);
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "ROWS";
            double avg = trackDetectorsInfo.Average();
            trackDetectorsInfo.Add(trackDetectorsInfo.Last());
            for (int i = trackDetectorsInfo.Count; i > 0; --i)
            {
                trackDetectorsInfo[i] = trackDetectorsInfo[i - 1];
            }
            trackDetectorsInfo[0] = avg;
            valueRange.Values = new List<IList<Object>> { trackDetectorsInfo.Select(dt => (Object)dt.ToString("0.00E00").Insert(5, "+")).ToList() };
            SpreadsheetsResource.ValuesResource.UpdateRequest request = _SheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetId, $"Data!H{index}:Q{index}");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            request.Execute();
        }

        public void SetOnlineDetectorsInfo(string spreadSheetId, int sessionNumber, List<double> onlineDetectorsInfo)
        {
            int index = GetIndexBySessionNumber(spreadSheetId, "Data", sessionNumber);
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "ROWS";
            double avg = onlineDetectorsInfo.Average();
            onlineDetectorsInfo.Add(onlineDetectorsInfo.Last());
            for (int i = onlineDetectorsInfo.Count - 1; i > 0; --i)
            {
                onlineDetectorsInfo[i] = onlineDetectorsInfo[i - 1];
            }
            onlineDetectorsInfo[0] = avg;
            valueRange.Values = new List<IList<Object>> { onlineDetectorsInfo.Select(dt => (Object)dt.ToString("0.00E00").Insert(5, "+")).ToList() };
            SpreadsheetsResource.ValuesResource.UpdateRequest request = _SheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetId, $"Data!R{index}:V{index}");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            request.Execute();
        }

        public void SetMeasureResults(string spreadSheetId, int sessionNumber, double K, double error, double heterogeneity = -1,
            double heterogeneityLeft = -1, double heterogeneityRight = -1)
        {
            int index = GetIndexBySessionNumber(spreadSheetId, "Data", sessionNumber);
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "ROWS";
            if (heterogeneityLeft == -1 && heterogeneityRight == -1)
            {
                valueRange.Values = new List<IList<Object>> { new List<Object> { heterogeneity, K, error, "", "" } };
            }
            else
            {
                valueRange.Values = new List<IList<Object>> { new List<Object> { heterogeneity, K, error, heterogeneityLeft, heterogeneityRight } };
            }
            SpreadsheetsResource.ValuesResource.UpdateRequest request = _SheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetId, $"Data!AD{index}:AH{index}");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            request.Execute();
        }

        public void SetNewSessionInfo(string spreadSheetId, Session session)
        {
            List<Object> dataValues = new List<Object>();
            dataValues.Add(session.SessionId);
            dataValues.Add(session.OrgName);
            dataValues.AddRange(session.ObjTests);
            dataValues.AddRange(Enumerable.Repeat("", (session.ObjTests.Select(t => t != "").Count() - 4)));
            dataValues.Add(session.CodeEnv);
            dataValues.AddRange(Enumerable.Repeat("", 15));
            dataValues.Add(session.StreamIntensity);
            dataValues.Add(session.AdmProtocolCode);
            dataValues.Add(session.IrradiationAngle == -1 ? "-" : session.IrradiationAngle);
            dataValues.Add(session.Pressure == -1 ? "-" : session.Pressure);
            dataValues.Add(session.Humidity == -1 ? "-" : session.Humidity);
            dataValues.Add(session.TemperatureTest == -1 ? "-" : session.TemperatureTest);
            dataValues.Add(session.TemperatureSession == -1 ? "-" : session.TemperatureSession);
            List<Object> timingValues = new List<Object>();
            timingValues.Add(session.SessionId);
            timingValues.Add(session.OrgName);
            timingValues.Add(GetIonInfoByEnvCode(spreadSheetId, session.CodeEnv)![0]);
            timingValues.AddRange(Enumerable.Repeat("", 6));
            timingValues.Add(session.StartSession.ToString("G"));
            timingValues.Add(session.EndSession.ToString("G"));
            timingValues.Add(session.IrradiationTime.ToString());
            timingValues.Add((session.EndSession - session.StartSession).ToString("T"));
            int index = GetIndexBySessionNumber(spreadSheetId, "Data", session.SessionId - 1) + 1;
            ValueRange valueRange = new ValueRange();
            valueRange.MajorDimension = "ROWS";
            valueRange.Values = new List<IList<Object>> { dataValues };
            SpreadsheetsResource.ValuesResource.UpdateRequest request = _SheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetId, $"Data!A{index}:AH{index}");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            request.Execute();
            valueRange.Values = new List<IList<Object>> { timingValues };
            request = _SheetsService.Spreadsheets.Values.Update(valueRange, spreadSheetId, $"Timing!A{index}:R{index}");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            request.Execute();
        }
    }
}