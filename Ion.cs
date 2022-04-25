namespace ByteTerraUtils
{
    public class Ion
    {
        private string _Name;
        private int _Isotope;
        private int _NumOutputSession;
        private string _DegradorMaterial;
        private double _DegradorDepth;
        private string _Environment;
        private double _EnergySurface;
        private double _ErrorTestObj;
        private double _Run;
        private double _ErrorRun;
        private double _LES;
        private double _ErrorLES;
        private double _EnergyIonPipe;
        private int _NumSessionYear;
        private string _CodeEnv;

        public Ion(string name, int isotope, int numOutSession,
                   string degradMat, double degradDepth, string environment,
                   double energySurface, double errTestObj, double run,
                   double errRun, double les, double errLes, double energyIonPipe,
                   int numSessionYear, string codeEnv)
        {
            _Name = name;
            _Isotope = isotope;
            _NumOutputSession = numOutSession;
            _DegradorMaterial = degradMat;
            _DegradorDepth = degradDepth;
            _Environment = environment;
            _EnergySurface = energySurface;
            _ErrorTestObj = errTestObj;
            _Run = run;
            _ErrorRun = errRun;
            _LES = les;
            _ErrorLES = les;
            _ErrorLES = errLes;
            _EnergyIonPipe = energyIonPipe;
            _NumSessionYear = numSessionYear;
            _CodeEnv = codeEnv;
        }

        public Ion(List<Object> tableInfo)
        {
            _Name = Convert.ToString(tableInfo[0])!;
            _Isotope = Convert.ToInt32(tableInfo[1]);
            _NumOutputSession = Convert.ToInt32(tableInfo[2]);
            _DegradorMaterial = Convert.ToString(tableInfo[3])!;
            _DegradorDepth = Convert.ToDouble(Double.TryParse(tableInfo[4].ToString(), out double res) == true ? res : 0.0);
            _Environment = Convert.ToString(tableInfo[5])!;
            _EnergySurface = Convert.ToDouble(tableInfo[6]);
            _ErrorTestObj = Convert.ToDouble(tableInfo[7]);
            _Run = Convert.ToDouble(tableInfo[8]);
            _ErrorRun = Convert.ToDouble(tableInfo[9]);
            _LES = Convert.ToDouble(tableInfo[10]);
            _ErrorLES = Convert.ToDouble(tableInfo[11]);
            _EnergyIonPipe = Convert.ToDouble(tableInfo[12]);
            _NumSessionYear = Convert.ToInt32(tableInfo[13]);
            _CodeEnv = Convert.ToString(tableInfo[14])!;
        }

        public string Name { get { return _Name; } }
        public int Isotope { get { return _Isotope; } }
        public int NumOutSession { get { return _NumOutputSession; } }
        public string DegradorMaterial { get { return _DegradorMaterial; } }
        public double DegradDepth { get { return _DegradorDepth; } }
        public string Environment { get { return _Environment; } }
        public double EnergySurface { get { return _EnergySurface; } }
        public double ErrorTestObj { get { return _ErrorTestObj; } }
        public double Run { get { return _Run; } }
        public double ErrorRun { get { return _ErrorRun; } }
        public double LES { get { return _LES; } }
        public double ErrorLES { get { return _ErrorLES; } }
        public double EnergyIonPipe { get { return _EnergyIonPipe; } }
        public double NumSessionYear { get { return _NumSessionYear; } }
        public string CodeEnv { get { return _CodeEnv; } }
    }
}
