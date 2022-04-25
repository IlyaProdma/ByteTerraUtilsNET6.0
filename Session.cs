namespace ByteTerraUtils
{
    public class Session
    {
        private int _SessionId;
        private string _OrgName;
        private string[] _ObjTests = new string[4];
        private string _CodeEnv;
        private double _MedTrackDetectors;
        private List<double> _TrackDetectors;
        private double _MedOnlineDetectors;
        private double[] _OnlineDetectors;
        private double _StreamIntensity;
        private string _AdmProtocolCode;
        private double _IrradiationAngle;
        private double _Pressure;
        private double _Humidity;
        private double _TemperatureTest;
        private double _TemperatureSession;
        private double _Heterogeneity;
        private double _Kcoeff;
        private double _Error;
        private double _HeterogeneityLeft;
        private double _HeterogeneityRight;
        private DateTime _StartSession;
        private DateTime _EndSession;
        private TimeSpan _IrradiationTime;

        public Session(int sessionId, string orgName, string[] objTests, string codeEnv, string admProtCode,
                       double irradAngle, double pressure, double humid, double temperatureTest, double temperatureSession,
                       DateTime startSession, DateTime endSession, TimeSpan irradTime)
        {
            _SessionId = sessionId;
            _OrgName = orgName;
            _ObjTests = objTests;
            _CodeEnv = codeEnv;
            _AdmProtocolCode = admProtCode;
            _IrradiationAngle = irradAngle;
            _Pressure = pressure;
            _Humidity = humid;
            _TemperatureTest = temperatureTest;
            _TemperatureSession = temperatureSession;
            _StartSession = startSession;
            _EndSession = endSession;
            _MedTrackDetectors = 0;
            _MedOnlineDetectors = 0;
            _TrackDetectors = new List<double>();
            _OnlineDetectors = new double[4];
            _IrradiationTime = irradTime;
            _Heterogeneity = -1;
            _Kcoeff = -1;
            _Error = -1;
            _HeterogeneityLeft = -1;
            _HeterogeneityRight = -1;
        }

        public Session(int sessionId, string orgName, string[] objTests, string codeEnv,
                       List<double> trackDetectors, double[] onlineDetectors, double streamIntens,
                       string admProtCode, double irradAngle, double pressure, double humid, double temperatureTest,
                       double temperatureSession, DateTime startSession, DateTime endSession, TimeSpan irradTime)
        {
            _SessionId = sessionId;
            _OrgName = orgName;
            _ObjTests = objTests;
            _CodeEnv = codeEnv;
            _TrackDetectors = trackDetectors;
            _MedTrackDetectors = _TrackDetectors.Average();
            _OnlineDetectors = onlineDetectors;
            _MedOnlineDetectors = _OnlineDetectors.Average();
            _StreamIntensity = streamIntens;
            _AdmProtocolCode = admProtCode;
            _IrradiationAngle = irradAngle;
            _Pressure = pressure;
            _Humidity = humid;
            _TemperatureTest = temperatureTest;
            _TemperatureSession = temperatureSession;
            _StartSession = startSession;
            _EndSession = endSession;
            _Heterogeneity = -1;
            _Kcoeff = -1;
            _Error = -1;
            _HeterogeneityLeft = -1;
            _HeterogeneityRight = -1;
            _IrradiationTime = irradTime;
        }

        public Session(List<Object> dataRow, List<Object> timingRow)
        {
            _SessionId = Convert.ToInt32(dataRow[0]);
            _OrgName = Convert.ToString(dataRow[1])!;
            _ObjTests = new string[4] {
                                        Convert.ToString(dataRow[2])!,
                                        Convert.ToString(dataRow[3])!,
                                        Convert.ToString(dataRow[4])!,
                                        Convert.ToString(dataRow[5])!
                                      };
            _CodeEnv = Convert.ToString(dataRow[6])!;
            _TrackDetectors = new List<double>();
            for (int i = 8; i <= 16; ++i)
            {
                if (Convert.ToString(dataRow[i])!.Length > 0)
                {
                    _TrackDetectors.Add(Convert.ToDouble(dataRow[i]));
                }
            }
            _MedTrackDetectors = _TrackDetectors.Count > 0 ? _TrackDetectors.Average() : 0;
            _OnlineDetectors = new double[4];
            for (int i = 18, j = 0; i <= 21; ++i, ++j)
            {
                if (Convert.ToString(dataRow[i])!.Length > 0)
                    _OnlineDetectors[j] = Convert.ToDouble(dataRow[i]);
                else
                    _OnlineDetectors[j] = -1;
            }
            _MedOnlineDetectors = _OnlineDetectors.Average();
            _StreamIntensity = Double.TryParse(dataRow[22].ToString(), out double res) == true ? res : -1;
            _AdmProtocolCode = Convert.ToString(dataRow[23])!;
            _IrradiationAngle = Double.TryParse(dataRow[24].ToString(), out res) == true ? res : -1;
            _Pressure = Double.TryParse(dataRow[25].ToString(), out res) == true ? res : -1;
            _Humidity = Double.TryParse(dataRow[26].ToString(), out res) == true ? res : -1;
            _TemperatureTest = Double.TryParse(dataRow[27].ToString(), out res) == true ? res : -1;
            _TemperatureSession = Double.TryParse(dataRow[28].ToString(), out res) == true ? res : -1;
            _Heterogeneity = Double.TryParse(dataRow[29].ToString(), out res) == true ? res : -1;
            _Kcoeff = Double.TryParse(dataRow[30].ToString(), out res) == true ? res : -1;
            _Error = Double.TryParse(dataRow[31].ToString(), out res) == true ? res : -1;
            if (dataRow.Count == 34)
            {
                _HeterogeneityLeft = Double.TryParse(dataRow[32].ToString(), out res) == true ? res : -1;
                _HeterogeneityRight = Double.TryParse(dataRow[33].ToString(), out res) == true ? res : -1;
            }
            _StartSession = DateTime.Parse(timingRow[9].ToString()!);
            _EndSession = DateTime.Parse(timingRow[10].ToString()!);
            _IrradiationTime = TimeSpan.Parse(timingRow[11].ToString()!);
        }

        public int SessionId { get { return _SessionId; } }
        public string OrgName { get { return _OrgName; } }
        public string[] ObjTests { get { return _ObjTests; } }
        public string CodeEnv { get { return _CodeEnv; } }
        public double MedTrackDetectors { get { return _MedTrackDetectors; } }
        public List<double> TrackDetectors { get { return _TrackDetectors; } }
        public double MedOnlineDetectors { get { return _MedOnlineDetectors; } }
        public double[] OnlineDetectors { get { return _OnlineDetectors; } }
        public double StreamIntensity { get { return _StreamIntensity; } set { _StreamIntensity = value; } }
        public string AdmProtocolCode { get { return _AdmProtocolCode; } }
        public double IrradiationAngle { get { return _IrradiationAngle; } }
        public double Pressure { get { return _Pressure; } }
        public double Humidity { get { return _Humidity; } }
        public double TemperatureTest { get { return _TemperatureTest; } }
        public double TemperatureSession { get { return _TemperatureSession; } }
        public double Heterogeneity { get { return _Heterogeneity; } }
        public double Kcoeff { get { return _Kcoeff; } }
        public double Error { get { return _Error; } }
        public double HeterogeneityLeft { get { return _HeterogeneityLeft; } }
        public double HeterogeneityRight { get { return _HeterogeneityRight; } }
        public DateTime StartSession { get { return _StartSession; } }
        public DateTime EndSession { get { return _EndSession; } }
        public TimeSpan IrradiationTime { get { return _IrradiationTime; } }
    }
}
