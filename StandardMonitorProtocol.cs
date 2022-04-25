using System.Text.Json.Serialization;

namespace ByteTerraUtils
{
    internal class StandardMonitorProtocol
    {
        [JsonPropertyName("isotope_header")]
        [JsonInclude]
        public string IsotopeHeader { get; set; }

        [JsonPropertyName("seans")]
        [JsonInclude]
        public string SessionId { get; set; }

        [JsonPropertyName("test_stand")]
        [JsonInclude]
        public string TestStand { get; set; }

        [JsonPropertyName("organization")]
        [JsonInclude]
        public string Organization { get; set; }

        [JsonPropertyName("cipher")]
        [JsonInclude]
        public string Cipher { get; set; }

        [JsonPropertyName("irradiated_item")]
        [JsonInclude]
        public string IrradiatedItem { get; set; }

        [JsonPropertyName("begin_date")]
        [JsonInclude]
        public string BeginDate { get; set; }

        [JsonPropertyName("duration")]
        [JsonInclude]
        public string Duration { get; set; }

        [JsonPropertyName("angle")]
        [JsonInclude]
        public string Angle { get; set; }

        [JsonPropertyName("temperature")]
        [JsonInclude]
        public string Temperature { get; set; }

        [JsonPropertyName("degrader_material")]
        [JsonInclude]
        public string DegraderMaterial { get; set; }

        [JsonPropertyName("thickness")]
        [JsonInclude]
        public string Thickness { get; set; }

        [JsonPropertyName("element_name")]
        [JsonInclude]
        public string ElementName { get; set; }

        [JsonPropertyName("atomic_number")]
        [JsonInclude]
        public string AtomicNumber { get; set; }

        [JsonPropertyName("E")]
        [JsonInclude]
        public string Energy { get; set; }

        [JsonPropertyName("E_error")]
        [JsonInclude]
        public string EnergyError { get; set; }

        [JsonPropertyName("R")]
        [JsonInclude]
        public string Run { get; set; }

        [JsonPropertyName("R_error")]
        [JsonInclude]
        public string RunError { get; set; }

        [JsonPropertyName("energy_loss")]
        [JsonInclude]
        public string LES { get; set; }

        [JsonPropertyName("energy_loss_error")]
        [JsonInclude]
        public string LESError { get; set; }

        [JsonPropertyName("proportional_1")]
        [JsonInclude]
        public string Proportional1 { get; set; }

        [JsonPropertyName("proportional_2")]
        [JsonInclude]
        public string Proportional2 { get; set; }

        [JsonPropertyName("proportional_3")]
        [JsonInclude]
        public string Proportional3 { get; set; }

        [JsonPropertyName("proportional_4")]
        [JsonInclude]
        public string Proportional4 { get; set; }

        [JsonPropertyName("proportional_average")]
        [JsonInclude]
        public string ProportionalAverage { get; set; }

        [JsonPropertyName("K_theoretical")]
        [JsonInclude]
        public string KTheoretical { get; set; }

        [JsonPropertyName("K_error")]
        [JsonInclude]
        public string KError { get; set; }

        [JsonPropertyName("protocol_number")]
        [JsonInclude]
        public string ProtocolNumber { get; set; }

        [JsonPropertyName("K_measured")]
        [JsonInclude]
        public string KMeasured { get; set; }

        [JsonPropertyName("detector_1")]
        [JsonInclude]
        public string Detector1 { get; set; }

        [JsonPropertyName("detector_2")]
        [JsonInclude]
        public string Detector2 { get; set; }

        [JsonPropertyName("detector_3")]
        [JsonInclude]
        public string Detector3 { get; set; }

        [JsonPropertyName("detector_4")]
        [JsonInclude]
        public string Detector4 { get; set; }

        [JsonPropertyName("detector_5")]
        [JsonInclude]
        public string Detector5 { get; set; }

        [JsonPropertyName("detector_6")]
        [JsonInclude]
        public string Detector6 { get; set; }

        [JsonPropertyName("detector_7")]
        [JsonInclude]
        public string Detector7 { get; set; }

        [JsonPropertyName("detector_8")]
        [JsonInclude]
        public string Detector8 { get; set; }

        [JsonPropertyName("detector_9")]
        [JsonInclude]
        public string Detector9 { get; set; }

        [JsonPropertyName("heterogenity")]
        [JsonInclude]
        public string Heterogenity { get; set; }

        public StandardMonitorProtocol(Session session, Ion ion)
        {
            SessionId = session.SessionId.ToString();
            Organization = session.OrgName;
            Cipher = "XX-XXXX";
            IrradiatedItem = session.ObjTests[0];
            BeginDate = session.StartSession.ToString("G");
            Duration = session.IrradiationTime.ToString();
            Angle = session.IrradiationAngle.ToString();
            DegraderMaterial = ion.DegradorMaterial;
            Thickness = ion.DegradDepth <= 0 ? "-" : ion.DegradDepth.ToString();
            IsotopeHeader = $"ТЗЧ/{DateTime.Now.Date.Year}-{ion.Name}-{ion.NumSessionYear}/{ion.NumOutSession}-{session.SessionId}";
            ProtocolNumber = session.AdmProtocolCode;
            AtomicNumber = ion.Isotope.ToString();
            ElementName = ion.Name;
            Energy = ion.EnergySurface.ToString();
            EnergyError = ion.ErrorTestObj.ToString();
            Run = ion.Run.ToString();
            RunError = ion.ErrorRun.ToString();
            LES = ion.LES.ToString();
            LESError = ion.ErrorLES.ToString();
            Proportional1 = session.OnlineDetectors[0].ToString("0.00E00").Insert(5, "+");
            Proportional2 = session.OnlineDetectors[1].ToString("0.00E00").Insert(5, "+");
            Proportional3 = session.OnlineDetectors[2].ToString("0.00E00").Insert(5, "+");
            Proportional4 = session.OnlineDetectors[3].ToString("0.00E00").Insert(5, "+");
            ProportionalAverage = session.MedOnlineDetectors.ToString("0.00E00").Insert(5, "+");
            TestStand = "ИИК 10К-400";
            BeginDate = session.StartSession.ToString("G");
            Temperature = session.TemperatureSession.ToString();
            KTheoretical = session.Kcoeff.ToString();
            KMeasured = "";
            KError = session.Error.ToString();
            Heterogenity = session.Heterogeneity.ToString();
            Detector1 = session.TrackDetectors[0].ToString("0.00E00").Insert(5, "+");
            Detector2 = "";
            Detector3 = "";
            Detector4 = "";
            Detector5 = "";
            Detector6 = "";
            Detector7 = "";
            Detector8 = "";
            Detector9 = "";
            if (session.TrackDetectors.Count > 1)
            {
                Detector2 = session.TrackDetectors[1].ToString("0.00E00").Insert(5, "+");
                if (session.TrackDetectors.Count > 2)
                {
                    Detector3 = session.TrackDetectors[2].ToString("0.00E00").Insert(5, "+");
                    if (session.TrackDetectors.Count > 3)
                    {
                        Detector4 = session.TrackDetectors[3].ToString("0.00E00").Insert(5, "+");
                        if (session.TrackDetectors.Count > 4)
                        {
                            Detector5 = session.TrackDetectors[4].ToString("0.00E00").Insert(5, "+");
                            if (session.TrackDetectors.Count > 5)
                            {
                                Detector6 = session.TrackDetectors[5].ToString("0.00E00").Insert(5, "+");
                                if (session.TrackDetectors.Count > 6)
                                {
                                    Detector7 = session.TrackDetectors[6].ToString("0.00E00").Insert(5, "+");
                                    if (session.TrackDetectors.Count > 7)
                                    {
                                        Detector8 = session.TrackDetectors[7].ToString("0.00E00").Insert(5, "+");
                                        if (session.TrackDetectors.Count > 8)
                                        {
                                            Detector9 = session.TrackDetectors[8].ToString("0.00E00").Insert(5, "+");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
